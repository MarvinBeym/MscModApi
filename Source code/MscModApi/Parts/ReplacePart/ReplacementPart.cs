﻿using MscModApi.Parts.ReplacementPart;
using System;
using System.Collections.Generic;
using System.Linq;
using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.Parts.ReplacePart
{
	internal class Actions
	{
		internal List<Action> allInstalled = new List<Action>();
		internal List<Action> allUninstalled = new List<Action>();
		internal List<Action> allFixed = new List<Action>();
		internal List<Action> allUnfixed = new List<Action>();

		internal List<Action> anyInstalled = new List<Action>();
		internal List<Action> anyUninstalled = new List<Action>();
		internal List<Action> anyFixed = new List<Action>();
		internal List<Action> anyUnfixed = new List<Action>();
	}
	public class ReplacementPart
	{
		public enum ActionType
		{
			AnyInstalled,
			AnyFixed,
			AnyUninstalled,
			AnyUnfixed,

			AllInstalled,
			AllFixed,
			AllUninstalled,
			AllUnfixed
		}
		public enum PartType
		{
			NewPart,
			OldPart
		}


		public List<Part> newParts = new List<Part>();
		public List<OldPart> oldParts = new List<OldPart>();

		internal Actions newPartActions = new Actions();
		internal Actions oldPartActions = new Actions();

		public ReplacementPart(OldPart oldPart, Part newPart) : this(new[] { oldPart }, new[] { newPart })
		{
		}

		public ReplacementPart(OldPart[] oldParts, Part newPart) : this(oldParts, new[] { newPart })
		{
		}

		public ReplacementPart(OldPart oldPart, Part[] newParts) : this(new[] { oldPart }, newParts)
		{
		}

		public ReplacementPart(OldPart[] oldParts, Part[] newParts)
		{
			foreach (var newPart in newParts) {
				this.newParts.Add(newPart);
			}

			foreach (var oldPart in oldParts)
			{
				oldPart.SetInstallAction(OldPartInstalled);
				oldPart.SetUninstallAction(OldPartUninstalled);
				this.oldParts.Add(oldPart);
			}
			foreach (var newPart in newParts) {
				newPart.AddPostInstallAction(NewPartInstalled);
				newPart.AddPostUninstallAction(NewPartUninstalled);

				newPart.AddPostFixedAction(NewPartFixed);
				newPart.AddPostUnfixedActions(NewPartUnfixed);
			}
		}

		public bool AreAllNewInstalled()
		{
			return newParts.All(part => part.IsInstalled());
		}

		public bool AreAllNewUninstalled()
		{
			return newParts.All(part => !part.IsInstalled());
		}

		public bool AreAnyNewFixed(bool ignoreUnsetScrews = true)
		{
			return newParts.Any(part => part.IsFixed(ignoreUnsetScrews));
		}

		public void SetFakedInstallStatus(bool status)
		{
			foreach (var oldPart in oldParts)
			{
				oldPart.SetFakedInstallStatus(status);
			}
		}

		public bool AreAllNewFixed(bool ignoreUnsetScrews =  true)
		{
			return newParts.All(part => part.IsFixed(ignoreUnsetScrews));
		}

		public bool AreAnyOldFixed()
		{
			return oldParts.Any(part => part.IsFixed());
		}

		public bool AreAllOldFixed()
		{
			return oldParts.All(part => part.IsFixed());
		}

		public bool AreAnyNewInstalled()
		{
			return newParts.Any(part => part.IsInstalled());
		}

		public bool AreAnyNewUninstalled()
		{
			return newParts.Any(part => !part.IsInstalled());
		}

		public bool AreAllOldInstalled()
		{
			return oldParts.All(part => part.IsInstalled());
		}

		public bool AreAllOldUninstalled()
		{
			return oldParts.All(part => !part.IsInstalled());
		}

		public bool AreAnyOldInstalled()
		{
			return oldParts.Any(part => part.IsInstalled());
		}

		public bool AreAnyOldUninstalled()
		{
			return oldParts.Any(part => !part.IsInstalled());
		}

		public void AddAction(ActionType actionType, PartType partType, Action action)
		{
			var actions = new Actions();
			var actionList = new List<Action>();
			switch (partType)
			{
				case PartType.NewPart:
					actions = newPartActions;
					break;
				case PartType.OldPart:
					actions = oldPartActions;
					break;
			}

			switch (actionType)
			{
				case ActionType.AllInstalled:
					actionList = actions.allInstalled;
					break;
				case ActionType.AllUninstalled:
					actionList = actions.allUninstalled;
					break;
				case ActionType.AllFixed:
					actionList = actions.allFixed;
					break;
				case ActionType.AllUnfixed:
					actionList = actions.allUnfixed;
					break;
				case ActionType.AnyInstalled:
					actionList = actions.anyInstalled;
					break;
				case ActionType.AnyUninstalled:
					actionList = actions.anyUninstalled;
					break;
				case ActionType.AnyFixed:
					actionList = actions.anyFixed;
					break;
				case ActionType.AnyUnfixed:
					actionList = actions.anyUnfixed;
					break;
			}

			actionList.Add(action);
		}

		internal void NewPartInstalled()
		{
			foreach (var oldPart in oldParts) {
				oldPart.BlockInstall(true);
			}

			if(newPartActions.anyInstalled.Count > 0) newPartActions.anyInstalled.InvokeAll();
			if (AreAllNewInstalled())
			{
				if (newPartActions.allInstalled.Count > 0)
				{
					newPartActions.allInstalled.InvokeAll();
				}
			}

			// => wont work => if(newPartActions.anyFixed.Count > 0 && AreAnyNewFixed()) newPartActions.anyFixed.InvokeAll();
			// => wont work => if (newPartActions.allFixed.Count > 0 && AreAllNewFixed()) newPartActions.allFixed.InvokeAll();
		}

		internal void NewPartFixed()
		{
			if(newPartActions.anyFixed.Count > 0) newPartActions.anyFixed.InvokeAll();
			if (AreAllNewFixed())
			{
				SetFakedInstallStatus(true);

				if (newPartActions.allFixed.Count > 0) {
					newPartActions.allFixed.InvokeAll();
				}
			}
		}

		internal void NewPartUnfixed()
		{
			if (AreAnyNewFixed())
			{
				SetFakedInstallStatus(false);
				if (newPartActions.anyUnfixed.Count > 0) {
					newPartActions.anyUnfixed.InvokeAll();
				}
			}
		}

		internal void NewPartUninstalled()
		{
			var allNewUninstalled = AreAllNewUninstalled();
			foreach (var oldPart in oldParts) {
				oldPart.BlockInstall(!allNewUninstalled);
			}

			if (AreAnyNewUninstalled())
			{
				
				if (newPartActions.anyUninstalled.Count > 0) {
					newPartActions.anyUninstalled.InvokeAll();
				}
			}


			if (newPartActions.allUninstalled.Count > 0 && allNewUninstalled) newPartActions.allUninstalled.InvokeAll();
		}

		internal void OldPartInstalled()
		{
			foreach (var newPart in newParts) {
				newPart.BlockInstall(true);
			}

			if (oldPartActions.anyInstalled.Count > 0) oldPartActions.anyInstalled.InvokeAll();
			if (oldPartActions.allInstalled.Count > 0 && AreAllOldInstalled()) oldPartActions.allInstalled.InvokeAll();

			// => wont work => if (oldPartActions.anyFixed.Count > 0 && AreAnyOldFixed()) oldPartActions.anyFixed.InvokeAll();
			// => wont work => if (oldPartActions.allFixed.Count > 0 && AreAllOldFixed()) oldPartActions.allFixed.InvokeAll();
		}

		internal void OldPartUninstalled()
		{
			var allOldUninstalled = AreAllOldUninstalled();
			foreach (var newPart in newParts) {
				newPart.BlockInstall(!allOldUninstalled);
			}

			if (oldPartActions.anyUninstalled.Count > 0 && AreAnyOldUninstalled()) oldPartActions.anyUninstalled.InvokeAll();
			if (oldPartActions.allUninstalled.Count > 0 && allOldUninstalled) oldPartActions.allUninstalled.InvokeAll();
		}
	}
}