using System;
using HutongGames.PlayMaker;
using MSCLoader;
using System.Linq;
using MSCLoader.Helper;
using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.Parts.ReplacePart
{
	public class OldPart : BasicPart
	{
		protected PlayMakerFSM fsm;
		protected GameObject gameObject;
		protected GameObject trigger;
		protected FsmBool installedFsm;
		protected FsmBool bolted;
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
			bolted = fsm.FsmVariables.FindFsmBool("Bolted");

			assembleFsm = GetFsmByName(trigger, "Assembly");
			removalFsm = GetFsmByName(gameObject, "Removal");

			if (!assembleFsm.Fsm.Initialized)
			{
				assembleFsm.InitializeFSM();
			}

			if (!removalFsm.Fsm.Initialized)
			{
				removalFsm.InitializeFSM();
			}
		}

		/// <inheritdoc />
		public override string name => gameObject.name;

		public bool installBlocked
		{
			get => !assembleFsm.enabled;
			set => assembleFsm.enabled = !value;
		}

		public bool installed
		{
			get
			{
				if (justUninstalled)
				{
					justUninstalled = false;
					return false;
				}

				return installedFsm.Value;
			}
			set
			{
				if (!allowSettingFakedStatus)
				{
					return;
				}

				installedFsm.Value = value;
				bolted.Value = value;
			}
		}

		public bool IsFixed() => installed && bolted.Value;

		public void Uninstall() => removalFsm.SendEvent("REMOVE");

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
			get => true;
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

		[Obsolete("Use 'installed' property instead")]
		internal void SetFakedInstallStatus(bool status)
		{
			installed = status;
		}

		[Obsolete("Use 'installBlocked' property instead")]
		public bool IsInstallBlocked()
		{
			return installBlocked;
		}

		[Obsolete("Use 'installBlocked' property instead")]
		public void BlockInstall(bool blocked)
		{
			installBlocked = blocked;
		}

		[Obsolete("Use 'installed' property instead")]
		public bool IsInstalled()
		{
			return installed;
		}
	}
}