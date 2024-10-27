using System.Collections.Generic;
using MSCLoader;
using System.Linq;
using System.Reflection;
using MscModApi.Parts;
using UnityEngine;

namespace MscModApi.Tools
{
	public static class ScrewPlacementAssist
	{
		internal static Keybind keySelectPart;

		internal static Part selectedPart;
		internal static Screw[] screws;
		internal static int selectedScrew;
		private static Rect windowRect;

		private static Dictionary<string, bool> screwPlacementEnabledMods;

		internal static void ModSettings(Mod mod)
		{
			keySelectPart = Keybind.Add(mod, "screw-placement-select-part", "Select part", KeyCode.RightArrow);
		}

		private static void CreateWindow(int windowID)
		{
			var screwStyle = new GUIStyle(GUI.skin.label)
			{
				alignment = TextAnchor.MiddleCenter
			};
			var selectedScrewStyle = new GUIStyle(GUI.skin.label)
			{
				alignment = TextAnchor.MiddleCenter,
				fontStyle = FontStyle.Bold
			};

			GUILayout.Label($"Part: {selectedPart.gameObject.name.Replace("(Clone)", "")}");
			GUILayout.Box("Screws");

			var valueX = "";
			var valueY = "";
			var valueZ = "";

			for (var i = 0; i < screws.Length; i++) {
				var screw = screws[i];
				GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
				GUILayout.Label(i.ToString(), selectedScrew == i ? selectedScrewStyle : screwStyle);
				GUILayout.EndVertical();
				if (selectedScrew != i) continue;

				var vector = CalculateVector(screw.gameObject.transform);

				valueX = PrintAxis("X", vector.x);
				valueY = PrintAxis("Y", vector.y);
				valueZ = PrintAxis("Z", vector.z);
			}

			GUILayout.BeginHorizontal(GUILayout.Width(100));
			if (GUILayout.Button("Previous", GUILayout.Width(100))) {
				selectedScrew--;
				if (selectedScrew <= 0) {
					selectedScrew = 0;
				}
			}

			if (GUILayout.Button("Next", GUILayout.Width(100))) {
				selectedScrew++;
				if (selectedScrew >= screws.Length - 1) {
					selectedScrew = screws.Length - 1;
				}
			}

			GUILayout.EndHorizontal();

			if (valueX != "" && valueY != "" && valueZ != "") {
				if (GUILayout.Button("Copy to clipboard", GUILayout.ExpandWidth(true))) {
					$"new Vector3({valueX}f, {valueY}f, {valueZ}f)".CopyToClipBoard();
				}
			}

			GUI.DragWindow(new Rect(0, 0, 10000, 10000));
		}

		internal static void OnGui()
		{
			if (selectedPart == null) return;
			windowRect = GUILayout.Window(0, windowRect, ScrewPlacementAssist.CreateWindow, "Screw placement mode",
				GUILayout.ExpandWidth(true));
		}

		internal static void HandlePartInteraction(Part part)
		{
			UserInteraction.GuiInteraction(
				$"Press [{keySelectPart.Key}] to {(selectedPart == null ? "select" : "deselect")} part"
			);

			if (keySelectPart.GetKeybindDown()) {
				if (selectedPart == null) {
					ShowPartInteraction(part);
				}
				else {
					HidePartInteraction();
				}
			}
			else {
				windowRect = new Rect(windowRect.xMin, windowRect.yMin, 200, 50);
			}
		}

		internal static void ShowPartInteraction(Part part)
		{
			selectedPart = part;
			screws = selectedPart.partSave.screws.OrderBy(screw => screw.gameObject.name).ToArray();
		}

		internal static void HidePartInteraction()
		{
			selectedPart = null;
			screws = new Screw[0];
		}

		private static void CopyToClipBoard(this string value)
		{
			typeof(GUIUtility).GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic)
				?.SetValue(null, value, null);
		}

		private static string PrintAxis(string label, float val)
		{
			GUILayout.BeginHorizontal("box", GUILayout.Width(100));
			GUILayout.Label(label);
			var value = val.ToString("0.0000");
			GUILayout.Label(value, GUILayout.Width(100));
			if (GUILayout.Button("Copy")) {
				value.CopyToClipBoard();
			}

			GUILayout.EndHorizontal();
			return value;
		}

		private static Vector3 CalculateVector(Transform transform)
		{
			var vector = transform.localPosition;
			var quaternion = Quaternion.Euler(transform.localRotation.eulerAngles);
			vector += (quaternion * Vector3.forward) * (Screw.maxTightness * Screw.transformStep);
			return vector;
		}

		public static void EnableScrewPlacementMode(Mod mod, bool enabled)
		{
			screwPlacementEnabledMods[mod.ID] = enabled;
		}

		public static bool IsScrewPlacementModeEnabled(string modId)
		{
			return screwPlacementEnabledMods.TryGetValue(modId, out var enabled) && enabled;
		}

		public static bool IsScrewPlacementModeEnabled(Mod mod)
		{
			return IsScrewPlacementModeEnabled(mod.ID);
		}

		internal static void LoadCleanup()
		{
			keySelectPart = null;

			selectedPart = null;
			screws = null;
			selectedScrew = 0;
			windowRect = new Rect(20, 20, 200, 50);
			screwPlacementEnabledMods = new Dictionary<string, bool>();
		}
	}
}