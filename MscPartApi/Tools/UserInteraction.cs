using System;
using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

namespace MscPartApi.Tools
{
	public static class UserInteraction
	{
		public enum Type
		{
			Assemble,
			Disassemble,
			Use,
			None
		}

		private static AudioSource assembleAudio;
		private static AudioSource disassembleAudio;

		private static FsmString guiInteraction;
		private static FsmBool guiAssemble;
		private static FsmBool guiDisassemble;
		private static FsmBool guiUse;

		private static GameObject itemPivot;

		public static void ShowGuiInteraction(Type type, string text = "")
		{
			if (guiInteraction != null && text != guiInteraction.Value) {
				guiInteraction.Value = text.Replace("(Clone)", "");
			}

			switch (type) {
				case Type.Assemble:
					ShowAssembleIcon();
					break;
				case Type.Disassemble:
					ShowDisassembleIcon();
					break;
				case Type.Use:
					ShowAssembleIcon();
					break;
				default:
					ShowAssembleIcon(false);
					ShowDisassembleIcon(false);
					ShowUseIcon(false);
					break;
			}

			guiInteraction = PlayMakerGlobals.Instance.Variables.FindFsmString("GUIinteraction");
		}

		private static void ShowAssembleIcon(bool show = true)
		{
			if (guiAssemble != null) {
				guiAssemble.Value = show;
			}

			guiAssemble = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIassemble");
		}

		private static void ShowDisassembleIcon(bool show = true)
		{
			if (guiDisassemble != null) {
				guiDisassemble.Value = show;
			}

			guiDisassemble = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIdisassemble");
		}

		private static void ShowUseIcon(bool show = true)
		{
			if (guiUse != null) {
				guiUse.Value = show;
			}

			guiUse = PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse");
		}

		public static void PlayAssemble(this GameObject gameObject)
		{
			if (assembleAudio == null) {
				var audioGameObject = GameObject.Find("MasterAudio/CarBuilding/assemble");
				if (!audioGameObject) return;
				var audioSource = audioGameObject.GetComponent<AudioSource>();
				if (audioSource) {
					assembleAudio = audioSource;
				}
			}

			if (assembleAudio != null) {
				AudioSource.PlayClipAtPoint(assembleAudio.clip, gameObject.transform.position);
			}
		}

		public static void PlayDisassemble(this GameObject gameObject)
		{
			if (disassembleAudio == null) {
				var audioGameObject = GameObject.Find("MasterAudio/CarBuilding/disassemble");
				if (!audioGameObject) return;
				var audioSource = audioGameObject.GetComponent<AudioSource>();
				if (audioSource) {
					disassembleAudio = audioSource;
				}
			}

			if (disassembleAudio != null)
			{
				AudioSource.PlayClipAtPoint(disassembleAudio.clip, gameObject.transform.position);
			}
		}

		public static bool IsLookingAt(this GameObject gameObject)
		{
			return (
				Camera.main != null
				&& Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1f,
					1 << gameObject.layer)
				&& hit.collider.gameObject == gameObject
			);
		}

		public static bool IsHolding(this GameObject gameObject)
		{
			return gameObject.layer == LayerMask.NameToLayer("Wheel");
		}

		public static bool EmptyHand()
		{
			try
			{
				if (itemPivot == null)
				{
					itemPivot = PlayMakerGlobals.Instance.Variables.FindFsmGameObject("ItemPivot").Value;
				}
			}
			catch (Exception)
			{
				return false;
			}

			if (!itemPivot)
			{
				return false;
			}

			return itemPivot.transform.childCount == 0;

		}

		public static bool LeftMouseDown => Input.GetMouseButtonDown(0);

		public static bool LeftMouseDownContinuous => Input.GetMouseButton(0);

		public static bool RightMouseDown => Input.GetMouseButtonDown(1);

		public static bool UseButtonDown => cInput.GetKeyDown("Use");


		public static class MouseScrollWheel
		{
			public static bool Up => Input.GetAxis("Mouse ScrollWheel") > 0f;
			public static bool Down => Input.GetAxis("Mouse ScrollWheel") < 0f;
		}
	}
}