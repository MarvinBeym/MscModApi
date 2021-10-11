using MSCLoader;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MscPartApi.Tools
{
	internal static class ScrewPlacementAssist
	{
		internal static Keybind keySelectPart;

		internal static Part selectedPart;
		internal static Screw[] screws;
		internal static int selectedScrew;
		private static Rect windowRect = new Rect(20, 20, 200, 50);


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
			windowRect = GUILayout.Window(0, windowRect, ScrewPlacementAssist.CreateWindow, "Screw placement mode", GUILayout.ExpandWidth(true));
		}

		internal static void ShowPartInteraction(Part part)
		{
			UserInteraction.ShowGuiInteraction(
				UserInteraction.Type.None,
				$"Press [{keySelectPart.Key}] to {(selectedPart == null ? "select" : "deselect")} part"
			);

			if (keySelectPart.GetKeybindDown()) {
				if (selectedPart == null) {
					selectedPart = part;
					screws = selectedPart.partSave.screws.OrderBy(screw => screw.gameObject.name).ToArray();
				} else {
					selectedPart = null;
					screws = new Screw[0];
				}


			} else {
				windowRect = new Rect(windowRect.xMin, windowRect.yMin, 200, 50);
			}
		}

		private static void CopyToClipBoard(this string value)
		{
			typeof(GUIUtility).GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, value, null);
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
			var vector = Helper.CopyVector3(transform.localPosition);
			var quaternion = Quaternion.Euler(Helper.CopyVector3(transform.localRotation.eulerAngles));
			vector += (quaternion * Vector3.forward) * (Screw.maxTightness * Screw.transformStep);
			return vector;
		}
	}
}