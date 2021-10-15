using HutongGames.PlayMaker;
using System;
using MscModApi.Caching;
using UnityEngine;

namespace MscModApi.Tools
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
		private static AudioSource touchAudio;
		private static AudioSource buyAudio;
		private static AudioSource checkoutAudio;

		private static FsmString guiInteraction;
		private static FsmBool guiAssemble;
		private static FsmBool guiDisassemble;
		private static FsmBool guiUse;

		private static GameObject itemPivot;

		public static void GuiInteraction(string text = "")
		{
			GuiInteraction(Type.None, text);
		}

		public static void GuiInteraction(Type type)
		{
			GuiInteraction(type, "");
		}

		public static void GuiInteraction(Type type, string text)
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
				var audioGameObject = Cache.Find("MasterAudio/CarBuilding/assemble");
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
				var audioGameObject = Cache.Find("MasterAudio/CarBuilding/disassemble");
				if (!audioGameObject) return;
				var audioSource = audioGameObject.GetComponent<AudioSource>();
				if (audioSource) {
					disassembleAudio = audioSource;
				}
			}

			if (disassembleAudio != null) {
				AudioSource.PlayClipAtPoint(disassembleAudio.clip, gameObject.transform.position);
			}
		}

		public static void PlayTouch(this GameObject gameObject)
		{
			if (touchAudio == null)
			{
				var audioGameObject = Cache.Find("MasterAudio/CarFoley/dash_button");
				if (!audioGameObject) return;
				var audioSource = audioGameObject.GetComponent<AudioSource>();
				if (audioSource)
				{
					touchAudio = audioSource;
				}
			}

			if (touchAudio != null) {
				AudioSource.PlayClipAtPoint(touchAudio.clip, gameObject.transform.position);
			}
		}

		public static void PlayBuy(this GameObject gameObject)
		{
			if (buyAudio == null)
			{
				var audioGameObject = Cache.Find("MasterAudio/Store/cash_register_1");
				if (!audioGameObject) return;
				var audioSource = audioGameObject.GetComponent<AudioSource>();
				if (audioSource) {
					buyAudio = audioSource;
				}
			}

			if (buyAudio != null) {
				AudioSource.PlayClipAtPoint(buyAudio.clip, gameObject.transform.position);
			}
		}

		public static void PlayCheckout(this GameObject gameObject)
		{
			if (checkoutAudio == null) {
				var audioGameObject = Cache.Find("MasterAudio/Store/cash_register_2");
				if (!audioGameObject) return;
				var audioSource = audioGameObject.GetComponent<AudioSource>();
				if (audioSource) {
					checkoutAudio = audioSource;
				}
			}

			if (checkoutAudio != null) {
				AudioSource.PlayClipAtPoint(checkoutAudio.clip, gameObject.transform.position);
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
			try {
				if (itemPivot == null) {
					itemPivot = PlayMakerGlobals.Instance.Variables.FindFsmGameObject("ItemPivot").Value;
				}
			} catch (Exception) {
				return false;
			}

			if (!itemPivot) {
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