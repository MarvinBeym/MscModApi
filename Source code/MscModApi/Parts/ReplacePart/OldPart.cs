﻿using System;
using HutongGames.PlayMaker;
using MSCLoader;
using System.Linq;
using MSCLoader.Helper;
using MscModApi.Caching;
using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.Parts.ReplacePart
{
	[Obsolete(
		"Soon to be made obsolete, will be replaced with a new implementation using the new 'GamePart' wrapper class")]
	public class OldPart : BasicPart
	{
		protected PlayMakerFSM fsm;
		public override GameObject gameObject { get; protected set; }
		protected GameObject trigger;
		protected FsmBool installedFsm;
		protected FsmBool boltedFsm;
		protected FsmBool purchased;
		protected PlayMakerFSM assembleFsm;
		protected PlayMakerFSM removalFsm;
		protected GameObject oldFsmGameObject;
		protected bool allowSettingFakedStatus;
		protected bool justUninstalled = false;

		public OldPart(GameObject oldFsmGameObject, bool allowSettingFakedStatus = true)
		{
			this.oldFsmGameObject = oldFsmGameObject;
			this.allowSettingFakedStatus = allowSettingFakedStatus;
			fsm = oldFsmGameObject.FindFsm("Data");
			gameObject = fsm.FsmVariables.FindFsmGameObject("ThisPart").Value;
			trigger = fsm.FsmVariables.FindFsmGameObject("Trigger").Value;
			installedFsm = fsm.FsmVariables.FindFsmBool("Installed");
			boltedFsm = fsm.FsmVariables.FindFsmBool("Bolted");
			purchased = fsm.FsmVariables.FindFsmBool("Purchased");
			assembleFsm = GetFsmByName(trigger, "Assembly");
			removalFsm = GetFsmByName(gameObject, "Removal");

			if (!assembleFsm.Fsm.Initialized) {
				assembleFsm.InitializeFSM();
			}

			if (!removalFsm.Fsm.Initialized) {
				removalFsm.InitializeFSM();
			}
		}

		/// <inheritdoc />
		public override bool isHolding => gameObject.IsHolding();

		/// <inheritdoc />
		public override bool isLookingAt => gameObject.IsLookingAt();

		/// <inheritdoc />
		public override string name => gameObject.name;

		public override bool installBlocked
		{
			get => !assembleFsm.enabled;
			set => assembleFsm.enabled = !value;
		}

		public override bool installed
		{
			get
			{
				if (justUninstalled) {
					justUninstalled = false;
					return false;
				}

				return installedFsm.Value;
			}
		}

		public override bool hasBolts => true;
		public override bool installedOnCar => gameObject.transform.root == CarH.satsuma;

		public void Install(bool install)
		{
			if (!allowSettingFakedStatus) {
				return;
			}

			installedFsm.Value = install;
			boltedFsm.Value = install;
		}

		public override bool bolted => boltedFsm.Value;

		public override void Uninstall() => removalFsm.SendEvent("REMOVE");

		internal void Setup(ReplacePart.ReplacementPart replacementPart)
		{
			FsmHook.FsmInject(oldFsmGameObject, "Save game", replacementPart.OnOldSave);
		}

		internal void SetInstallAction(Action installAction)
		{
			FsmHook.FsmInject(trigger, "Assemble", installAction);
		}

		internal void SetUninstallAction(Action uninstallAction)
		{
			FsmHook.FsmInject(gameObject, "Remove part", delegate
			{
				justUninstalled = true;
				uninstallAction.Invoke();
			});
		}

		public override bool bought
		{
			get => purchased != null && purchased.Value;
			set => throw new NotImplementedException();
		}

		public override Vector3 position
		{
			get => gameObject.transform.position;
			set => gameObject.transform.position = value;
		}

		public override Vector3 rotation
		{
			get => gameObject.transform.rotation.eulerAngles;
			set => gameObject.transform.rotation = Quaternion.Euler(value);
		}

		public override bool active
		{
			get => gameObject.activeSelf;
			set => gameObject.SetActive(value);
		}

		private static PlayMakerFSM GetFsmByName(GameObject gameObject, string fsmName)
		{
			return gameObject.GetComponents<PlayMakerFSM>().FirstOrDefault(comp => comp.FsmName == fsmName);
		}

		public override void ResetToDefault(bool uninstall = false)
		{
			//Don't implement
		}
	}
}