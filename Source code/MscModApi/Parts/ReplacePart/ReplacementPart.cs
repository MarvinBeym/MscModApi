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

	/// <summary>The ReplacementPart class</summary>
	public class ReplacementPart
	{
		/// <summary>All available action types</summary>
		public enum ActionType
		{
			/// <summary>Any installed</summary>
			AnyInstalled,

			/// <summary>Any fixed</summary>
			AnyFixed,

			/// <summary>Any uninstalled</summary>
			AnyUninstalled,

			/// <summary>Any unfixed</summary>
			AnyUnfixed,

			/// <summary>All installed</summary>
			AllInstalled,

			/// <summary>All fixed</summary>
			AllFixed,

			/// <summary>All uninstalled</summary>
			AllUninstalled,

			/// <summary>All unfixed</summary>
			AllUnfixed
		}

		/// <summary>The part the ActionType should be added to</summary>
		public enum PartType
		{
			/// <summary>Add action to NewPart</summary>
			NewPart,

			/// <summary>Add action to OldPart</summary>
			OldPart
		}

		/// <summary>All NewPart's</summary>
		public List<NewPart> newParts = new List<NewPart>();

		/// <summary>All OldPart's</summary>
		public List<OldPart> oldParts = new List<OldPart>();

		internal Actions newPartActions = new Actions();
		internal Actions oldPartActions = new Actions();

		/// <summary>
		/// Initializes a new instance of the <see cref="ReplacementPart"/> class.
		/// A single NewPart replaces a single OldPart
		/// </summary>
		/// <param name="oldPart">Single OldPart</param>
		/// <param name="newPart">Single NewPart</param>
		public ReplacementPart(OldPart oldPart, NewPart newPart) : this(new[] { oldPart }, new[] { newPart })
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReplacementPart"/> class.
		/// A single NewPart replaces multiple OldPart's
		/// </summary>
		/// <param name="oldParts">Multiple OldPart's</param>
		/// <param name="newPart">Single NewPart</param>
		public ReplacementPart(OldPart[] oldParts, NewPart newPart) : this(oldParts, new[] { newPart })
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReplacementPart"/> class.
		/// Multiple NewPart's replace a single OldPart
		/// </summary>
		/// <param name="oldPart">Single OldPart</param>
		/// <param name="newParts">Multiple NewPart's</param>
		public ReplacementPart(OldPart oldPart, NewPart[] newParts) : this(new[] { oldPart }, newParts)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReplacementPart"/> class.
		/// Multiple NewPart's replace multiple OldPart's
		/// </summary>
		/// <param name="oldParts">Multiple OldPart's</param>
		/// <param name="newParts">Multiple NewPart's</param>
		public ReplacementPart(OldPart[] oldParts, NewPart[] newParts)
		{
			foreach (var newPart in newParts)
			{
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

			foreach (var newPart in newParts)
			{
				newPart.part.AddPostInstallAction(delegate { NewPartInstalled(newPart); });
				newPart.part.AddPostUninstallAction(delegate { NewPartUninstalled(newPart); });

				newPart.part.AddPostFixedAction(NewPartFixed);
				newPart.part.AddPostUnfixedActions(NewPartUnfixed);

				if (newPart.installed)
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

		/// <summary>Are all new installed.</summary>
		/// <returns>bool</returns>
		public bool AreAllNewInstalled()
		{
			return newParts.All(part => part.installed);
		}

		/// <summary>Are all new uninstalled.</summary>
		/// <returns></returns>
		public bool AreAllNewUninstalled()
		{
			return newParts.All(part => !part.installed);
		}

		/// <summary>Are any new fixed.</summary>
		/// <param name="ignoreUnsetScrews">if set to <c>true</c> [ignore unset/undefined screws].</param>
		/// <returns>b</returns>
		public bool AreAnyNewFixed(bool ignoreUnsetScrews = true)
		{
			return newParts.Any(part => part.IsFixed(ignoreUnsetScrews));
		}

		/// <summary>Sets the faked install status.</summary>
		/// <param name="status">if set to <c>true</c> [status].</param>
		public void SetFakedInstallStatus(bool status)
		{
			foreach (var oldPart in oldParts)
			{
				oldPart.installed = status;
			}
		}

		/// <summary>Are all new fixed.</summary>
		/// <param name="ignoreUnsetScrews">if set to <c>true</c> [ignore unset/undefined screws].</param>
		/// <returns></returns>
		public bool AreAllNewFixed(bool ignoreUnsetScrews = true)
		{
			return newParts.All(part => part.IsFixed(ignoreUnsetScrews));
		}

		/// <summary>Are any old fixed.</summary>
		/// <returns></returns>
		public bool AreAnyOldFixed()
		{
			return oldParts.Any(part => part.IsFixed());
		}

		/// <summary>Are all old fixed.</summary>
		public bool AreAllOldFixed()
		{
			return oldParts.All(part => part.IsFixed());
		}

		/// <summary>Are any new installed.</summary>
		public bool AreAnyNewInstalled()
		{
			return newParts.Any(part => part.installed);
		}

		/// <summary>Are any new uninstalled.</summary>
		/// <returns></returns>
		public bool AreAnyNewUninstalled()
		{
			return newParts.Any(part => !part.installed);
		}

		/// <summary>Are all old installed.</summary>
		/// <returns></returns>
		public bool AreAllOldInstalled()
		{
			return oldParts.All(part => part.installed);
		}

		/// <summary>Are all old uninstalled.</summary>
		/// <returns></returns>
		public bool AreAllOldUninstalled()
		{
			return oldParts.All(part => !part.installed);
		}

		/// <summary>Are any old installed.</summary>
		/// <returns></returns>
		public bool AreAnyOldInstalled()
		{
			return oldParts.Any(part => part.installed);
		}

		/// <summary>Are any old uninstalled.</summary>
		/// <returns></returns>
		public bool AreAnyOldUninstalled()
		{
			return oldParts.Any(part => !part.installed);
		}

		/// <summary>Adds the action.</summary>
		/// <param name="actionType">Type of the action.</param>
		/// <param name="partType">Part to add the action to.</param>
		/// <param name="action">The action.</param>
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
					}
					else
					{
						invokeActionsNow = AreAllOldUninstalled();
					}

					break;
				case ActionType.AllFixed:
					actionList = actions.allFixed;
					if (partType == PartType.NewPart)
					{
						invokeActionsNow = AreAllNewFixed();
					}
					else
					{
						invokeActionsNow = AreAllOldFixed();
					}

					break;
				case ActionType.AllUnfixed:
					actionList = actions.allUnfixed;
					break;
				case ActionType.AnyInstalled:
					actionList = actions.anyInstalled;
					if (partType == PartType.NewPart)
					{
						invokeActionsNow = AreAnyNewInstalled();
					}
					else
					{
						invokeActionsNow = AreAnyOldInstalled();
					}

					break;
				case ActionType.AnyUninstalled:
					actionList = actions.anyUninstalled;
					if (partType == PartType.NewPart)
					{
						invokeActionsNow = AreAnyNewUninstalled();
					}
					else
					{
						invokeActionsNow = AreAnyOldUninstalled();
					}

					break;
				case ActionType.AnyFixed:
					actionList = actions.anyFixed;
					if (partType == PartType.NewPart)
					{
						invokeActionsNow = AreAnyNewFixed();
					}
					else
					{
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
			if (!installedNewPart.canBeInstalledWithoutReplacing)
			{
				foreach (var oldPart in oldParts)
				{
					oldPart.installBlocked = true;
				}
			}

			if (newPartActions.anyInstalled.Count > 0) newPartActions.anyInstalled.InvokeAll();
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
			if (newPartActions.anyFixed.Count > 0) newPartActions.anyFixed.InvokeAll();
			if (AreAllNewFixed())
			{
				SetFakedInstallStatus(true);

				if (newPartActions.allFixed.Count > 0)
				{
					newPartActions.allFixed.InvokeAll();
				}
			}
		}

		internal void NewPartUnfixed()
		{
			if (AreAnyNewFixed())
			{
				SetFakedInstallStatus(false);
				if (newPartActions.anyUnfixed.Count > 0)
				{
					newPartActions.anyUnfixed.InvokeAll();
				}
			}
		}

		internal void NewPartUninstalled(NewPart uninstalledNewPart)
		{
			var allNewUninstalled = AreAllNewUninstalled();
			if (!uninstalledNewPart.canBeInstalledWithoutReplacing)
			{
				foreach (var oldPart in oldParts)
				{
					oldPart.installBlocked = !allNewUninstalled;
				}
			}

			if (AreAnyNewUninstalled())
			{
				if (newPartActions.anyUninstalled.Count > 0)
				{
					newPartActions.anyUninstalled.InvokeAll();
				}
			}


			if (newPartActions.allUninstalled.Count > 0 && allNewUninstalled) newPartActions.allUninstalled.InvokeAll();
		}

		internal void OldPartInstalled()
		{
			foreach (var newPart in newParts)
			{
				newPart.installBlocked = true;
			}

			if (oldPartActions.anyInstalled.Count > 0) oldPartActions.anyInstalled.InvokeAll();
			if (oldPartActions.allInstalled.Count > 0 && AreAllOldInstalled()) oldPartActions.allInstalled.InvokeAll();
		}

		internal void OldPartUninstalled()
		{
			var allOldUninstalled = AreAllOldUninstalled();
			foreach (var newPart in newParts)
			{
				newPart.installBlocked = !allOldUninstalled;
			}

			if (oldPartActions.anyUninstalled.Count > 0 && AreAnyOldUninstalled())
				oldPartActions.anyUninstalled.InvokeAll();
			if (oldPartActions.allUninstalled.Count > 0 && allOldUninstalled) oldPartActions.allUninstalled.InvokeAll();
		}
	}
}