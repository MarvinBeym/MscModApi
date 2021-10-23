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


		public List<NewPart> newParts = new List<NewPart>();
		public List<OldPart> oldParts = new List<OldPart>();

		internal Actions newPartActions = new Actions();
		internal Actions oldPartActions = new Actions();

		public ReplacementPart(OldPart oldPart, NewPart newPart) : this(new[] { oldPart }, new[] { newPart })
		{
		}

		public ReplacementPart(OldPart[] oldParts, NewPart newPart) : this(oldParts, new[] { newPart })
		{
		}

		public ReplacementPart(OldPart oldPart, NewPart[] newParts) : this(new[] { oldPart }, newParts)
		{
		}

		public ReplacementPart(OldPart[] oldParts, NewPart[] newParts)
		{
			foreach (var newPart in newParts) {
				this.newParts.Add(newPart);
			}

			foreach (var oldPart in oldParts)
			{
				oldPart.SetInstallAction(OldPartInstalled);
				oldPart.SetUninstallAction(OldPartUninstalled);
				oldPart.Setup(this);
				this.oldParts.Add(oldPart);
			}

			if (AreAnyOldInstalled())
			{
				OldPartInstalled();
			}

			foreach (var newPart in newParts) {
				newPart.part.AddPostInstallAction(delegate { NewPartInstalled(newPart); });
				newPart.part.AddPostUninstallAction(delegate { NewPartUninstalled(newPart); });

				newPart.part.AddPostFixedAction(NewPartFixed);
				newPart.part.AddPostUnfixedActions(NewPartUnfixed);

				if (newPart.IsInstalled())
				{
					NewPartInstalled(newPart);
				}
			}

			if (AreAnyNewFixed())
			{
				NewPartFixed();
			}
		}

		internal void OnOldSave()
		{
			if (AreAnyOldInstalled())
			{
				SetFakedInstallStatus(false);
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
			bool invokeActionsNow = false;
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
					if (partType == PartType.NewPart)
					{
						invokeActionsNow = AreAllNewInstalled();
					}
					else
					{
						invokeActionsNow = AreAllOldInstalled();
					}
					break;
				case ActionType.AllUninstalled:
					actionList = actions.allUninstalled;
					if (partType == PartType.NewPart)
					{
						invokeActionsNow = AreAllNewUninstalled();
					} else
					{
						invokeActionsNow = AreAllOldUninstalled();
					}
					break;
				case ActionType.AllFixed:
					actionList = actions.allFixed;
					if (partType == PartType.NewPart) {
						invokeActionsNow = AreAllNewFixed();
					} else {
						invokeActionsNow = AreAllOldFixed();
					}
					break;
				case ActionType.AllUnfixed:
					actionList = actions.allUnfixed;
					break;
				case ActionType.AnyInstalled:
					actionList = actions.anyInstalled;
					if (partType == PartType.NewPart) {
						invokeActionsNow = AreAnyNewInstalled();
					} else {
						invokeActionsNow = AreAnyOldInstalled();
					}
					break;
				case ActionType.AnyUninstalled:
					actionList = actions.anyUninstalled;
					if (partType == PartType.NewPart)
					{
						invokeActionsNow = AreAnyNewUninstalled();
					} else {
						invokeActionsNow = AreAnyOldUninstalled();
					}
					break;
				case ActionType.AnyFixed:
					actionList = actions.anyFixed;
					if (partType == PartType.NewPart) {
						invokeActionsNow = AreAnyNewFixed();
					} else {
						invokeActionsNow = AreAnyOldFixed();
					}
					break;
				case ActionType.AnyUnfixed:
					actionList = actions.anyUnfixed;
					break;
			}

			actionList.Add(action);
			if (invokeActionsNow)
			{
				actionList.InvokeAll();
			}
		}

		internal void NewPartInstalled(NewPart installedNewPart)
		{
			if (!installedNewPart.CanBeInstalledWithoutReplacing())
			{
				foreach (var oldPart in oldParts) {
					oldPart.BlockInstall(true);
				}
			}

			if(newPartActions.anyInstalled.Count > 0) newPartActions.anyInstalled.InvokeAll();
			if (AreAllNewInstalled())
			{
				if (newPartActions.allInstalled.Count > 0)
				{
					newPartActions.allInstalled.InvokeAll();
				}
			}
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

		internal void NewPartUninstalled(NewPart uninstalledNewPart)
		{
			var allNewUninstalled = AreAllNewUninstalled();
			if (!uninstalledNewPart.CanBeInstalledWithoutReplacing())
			{
				foreach (var oldPart in oldParts) {
					oldPart.BlockInstall(!allNewUninstalled);
				}
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