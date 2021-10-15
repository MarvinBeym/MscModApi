using System;
using HutongGames.PlayMaker;
using MSCLoader;
using System.Linq;
using UnityEngine;

namespace MscModApi.Parts.ReplacementPart
{
	public class OldPart
	{
		private PlayMakerFSM fsm;
		private GameObject gameObject;
		private GameObject trigger;
		private FsmBool installed;
		private FsmBool bolted;
		private PlayMakerFSM assembleFsm;
		private PlayMakerFSM removalFsm;


		public OldPart(GameObject oldFsmGameObject, Action installedAction, Action uninstalledAction)
		{
			fsm = oldFsmGameObject.GetComponent<PlayMakerFSM>();
			gameObject = fsm.FsmVariables.FindFsmGameObject("ThisPart").Value;
			trigger = fsm.FsmVariables.FindFsmGameObject("Trigger").Value;
			installed = fsm.FsmVariables.FindFsmBool("Installed");
			bolted = fsm.FsmVariables.FindFsmBool("Bolted");

			assembleFsm = GetFsmByName(trigger, "Assembly");
			removalFsm = GetFsmByName(gameObject, "Removal");

			FsmHook.FsmInject(trigger, "Assemble", installedAction);
			FsmHook.FsmInject(gameObject, "Remove part", uninstalledAction);
		}

		private static PlayMakerFSM GetFsmByName(GameObject gameObject, string fsmName)
		{
			return gameObject.GetComponents<PlayMakerFSM>().FirstOrDefault(comp => comp.FsmName == fsmName);
		}

		public bool IsInstallBlocked()
		{
			return !assembleFsm.enabled;
		}

		public void BlockInstall(bool blocked)
		{
			assembleFsm.enabled = !blocked;
		}

		public bool IsInstalled() => installed.Value;

		public bool IsFixed() => installed.Value && bolted.Value;

		public void Uninstall() => removalFsm.SendEvent("REMOVE");

		internal void SetFakedInstallStatus(bool status)
		{
			installed.Value = status;
			bolted.Value = status;
		}
	}
}