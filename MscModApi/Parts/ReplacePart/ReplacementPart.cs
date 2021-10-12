using MscModApi.Parts.ReplacementPart;
using System;
using System.Collections.Generic;
using System.Linq;
using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.Parts.ReplacePart
{
	internal class Actions
	{
		internal struct NewPartActions
		{
			internal List<Action> any;
			internal List<Action> all;
		}

		internal struct OldPartActions
		{
			internal List<Action> any;
			internal List<Action> all;
		}

		internal NewPartActions newPart;
		internal OldPartActions oldPart;

		internal Actions()
		{
			newPart.any = new List<Action>();
			newPart.all = new List<Action>();

			oldPart.any = new List<Action>();
			oldPart.all = new List<Action>();
		}
	}
	public class ReplacementPart
	{
		public enum ActionType
		{
			AnyInstalled,
			AllInstalled
		}
		public enum PartType
		{
			NewPart,
			OldPart
		}


		public List<Part> newParts = new List<Part>();
		public List<OldPart> oldParts = new List<OldPart>();

		internal Actions actions = new Actions();

		public ReplacementPart(GameObject oldFsmGameObject, Part newPart) : this(new[] { oldFsmGameObject }, new[] { newPart })
		{
		}

		public ReplacementPart(GameObject[] oldFsmGameObjects, Part newPart) : this(oldFsmGameObjects, new[] { newPart })
		{
		}

		public ReplacementPart(GameObject oldFsmGameObject, Part[] newParts) : this(new[] { oldFsmGameObject }, newParts)
		{
		}

		public ReplacementPart(GameObject[] oldFsmGameObjects, Part[] newParts)
		{
			foreach (var newPart in newParts) {
				this.newParts.Add(newPart);
			}

			foreach (var oldFsmGameObject in oldFsmGameObjects) {
				oldParts.Add(new OldPart(oldFsmGameObject, new Action(OldPartInstalled), new Action(OldPartUninstalled)));
			}
			foreach (var newPart in newParts) {
				newPart.AddPostInstallAction(NewPartInstalled);
				newPart.AddPostUninstallAction(NewPartUninstalled);
			}
		}

		public bool IsNewFixed()
		{
			return newParts.Any(part => !part.IsFixed());
		}
		public bool IsOldFixed()
		{
			return oldParts.Any(part => !part.IsFixed());
		}

		public bool AreAllNewInstalled()
		{
			return newParts.All(part => part.IsInstalled());
		}

		public bool AreAllNewUninstalled()
		{
			return !AreAllNewInstalled();
		}

		public bool AreAnyNewInstalled()
		{
			return newParts.Any(part => part.IsInstalled());
		}

		public bool AreAnyNewUninstalled()
		{
			return !AreAnyNewInstalled();
		}

		public bool AreAllOldInstalled()
		{
			return oldParts.All(part => part.IsInstalled());
		}

		public bool AreAllOldUninstalled()
		{
			return !AreAllOldInstalled();
		}

		public bool AreAnyOldInstalled()
		{
			return oldParts.Any(part => part.IsInstalled());
		}

		public bool AreAnyOldUninstalled()
		{
			return !AreAnyOldInstalled();
		}

		public void AddInstalledAction(ActionType actionType, PartType partType, Action action)
		{
			switch (actionType)
			{
				case ActionType.AllInstalled:
					switch (partType)
					{
						case PartType.NewPart:
							actions.newPart.all.Add(action);
							break;
						case PartType.OldPart:
							actions.oldPart.all.Add(action);
							break;
					}
					break;
				case ActionType.AnyInstalled:
					switch (partType) {
						case PartType.NewPart:
							actions.newPart.any.Add(action);
							break;
						case PartType.OldPart:
							actions.oldPart.any.Add(action);
							break;
					}
					break;
			}
		}

		internal void NewPartInstalled()
		{
			var anyNewInstalled = AreAnyNewInstalled();
			var allNewInstalled = AreAllNewInstalled();
			foreach (var oldPart in oldParts) {
				oldPart.BlockInstall(anyNewInstalled);
			}

			if(anyNewInstalled) actions.newPart.any.InvokeAll();
			if(allNewInstalled) actions.newPart.all.InvokeAll();
		}

		internal void NewPartUninstalled()
		{
			var allNewUninstalled = AreAllNewUninstalled();
			foreach (var oldPart in oldParts) {
				oldPart.BlockInstall(!allNewUninstalled);
			}
		}

		internal void OldPartInstalled()
		{
			var anyOldInstalled = AreAnyOldInstalled();
			var allOldInstalled = AreAllOldInstalled();
			foreach (var oldPart in oldParts) {
				oldPart.BlockInstall(anyOldInstalled);
			}

			if (anyOldInstalled) actions.oldPart.any.InvokeAll();
			if (allOldInstalled) actions.oldPart.all.InvokeAll();
		}

		internal void OldPartUninstalled()
		{
			var allOldUninstalled = AreAllOldUninstalled();
			foreach (var newPart in newParts) {
				newPart.BlockInstall(!allOldUninstalled);
			}
		}
	}
}