using MSCLoader;
using MscPartApi.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace MscPartApi
{
	internal class MscPartApi : Mod
	{
		public override string ID => "MscPartApi";
		public override string Name => "MscPartApi";
		public override string Author => "DonnerPlays";
		public override string Version => "1.0";

		public override string Description =>
			"This allows developers to make their parts installable on the car. Also adds screws";

		public override bool UseAssetsFolder => true;
		private static Settings showBoltSizeSetting = new Settings("showBoltSizeSetting", "Show screw size", false);

		private const string AssetsFile = "mscPartApi.unity3d";
		internal static GameObject clampModel;
		private Tool tool;

		internal static Dictionary<string, string> modSaveFileMapping = new Dictionary<string, string>();
		internal static Dictionary<string, Dictionary<string, Part>> modsParts = new Dictionary<string, Dictionary<string, Part>>();
		internal static Dictionary<string, Screw> screws = new Dictionary<string, Screw>();
		private Screw previousScrew;

		public static bool ShowScrewSize => (bool) showBoltSizeSetting.Value;

		public static bool loadedAssets = false;

		public override void ModSetup()
		{
			SetupFunction(Setup.OnGUI, Mod_OnGui);
			SetupFunction(Setup.OnNewGame, Mod_OnNewGame);
			SetupFunction(Setup.OnLoad, Mod_OnLoad);
			SetupFunction(Setup.OnSave, Mod_OnSave);
			SetupFunction(Setup.Update, Mod_Update);

			if (!loadedAssets)
			{
				LoadAssets();
				loadedAssets = true;
			}
		}

		public override void ModSettings()
		{
			Settings.AddCheckBox(this, showBoltSizeSetting);

			Keybind.AddHeader(this, "Developer area - Screw placement mode");
			ScrewPlacementAssist.ModSettings(this);
		}

		public void Mod_OnGui()
		{
			ScrewPlacementAssist.OnGui();
		}

		private void Mod_OnNewGame()
		{
		}

		private void Mod_OnLoad()
		{
			tool = new Tool();
		}

		private void Mod_OnSave()
		{
			foreach (var modParts in modsParts) {
				var mod = ModLoader.GetMod(modParts.Key);

				if (!modSaveFileMapping.TryGetValue(mod.ID, out var saveFileName)) {
					//save file for mod can't be found, skip the whole mod.
					continue;
				}

				var modPartSaves = new Dictionary<string, PartSave>();

				foreach (var partData in modParts.Value) {
					var id = partData.Key;
					var part = partData.Value;

					var partSave = part.partSave;
					partSave.position = part.gameObject.transform.position;
					partSave.rotation = part.gameObject.transform.rotation;

					modPartSaves.Add(id, partSave);
				}

				SaveLoad.SerializeSaveFile<Dictionary<string, PartSave>>(mod, modPartSaves, saveFileName);
			}
		}

		private void Mod_Update()
		{
			var toolInHand = tool.GetToolInHand();
			if (toolInHand == Tool.ToolType.None) {
				if (previousScrew != null) {
					previousScrew.Highlight(false);
					previousScrew = null;
				}

				return;
			}

			Screw screw = DetectScrew();

			if (screw == null) return;

			if (screw.part.screwPlacementMode) {
				return;
			}

			if (ShowScrewSize && screw.showSize) {
				UserInteraction.ShowGuiInteraction(UserInteraction.Type.None,
					$"Screw size: {screw.size.ToString("#.#").Replace(".00", "")}mm");
			}

			if (!tool.CheckScrewFits(screw)) return;

			screw.Highlight(true);

			if (!tool.CheckBoltingSpeed()) return;

			if (UserInteraction.MouseScrollWheel.Up) {
				switch (toolInHand) {
					case Tool.ToolType.RatchetTighten:
						screw.In();
						break;
					case Tool.ToolType.RatchetLoosen:
						screw.Out();
						break;
					default:
						screw.In();
						break;
				}
			} else if (UserInteraction.MouseScrollWheel.Down) {
				switch (toolInHand) {
					case Tool.ToolType.RatchetTighten:
						screw.In();
						break;
					case Tool.ToolType.RatchetLoosen:
						screw.Out();
						break;
					default:
						screw.Out();
						break;
				}
			}


			//ModConsole.Print();
		}

		private Screw DetectScrew()
		{
			if (previousScrew != null) {
				previousScrew.Highlight(false);
				previousScrew = null;
			}

			if (Camera.main == null) {
				return null;
			}

			if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 1f,
				1 << LayerMask.NameToLayer("DontCollide"))) {
				return null;
			}

			if (!hit.collider) {
				return null;
			}

			var hitObject = hit.collider.gameObject;
			if (hitObject == null || !hitObject.name.Contains("_screw_")) return null;

			if (!screws.TryGetValue(hitObject.name, out var screw)) return null;
			if (!screw.part.IsInstalled()) return null;

			previousScrew = screw;

			return screw;
		}

		private void LoadAssets()
		{
			var assetBundle = Helper.LoadAssetBundle(this, AssetsFile);
			Screw.material = assetBundle.LoadAsset<Material>("Screw-Material.mat");
			Screw.soundClip = assetBundle.LoadAsset<AudioClip>("screwable_sound.wav");
			clampModel = assetBundle.LoadAsset<GameObject>("Tube_Clamp.prefab");

			Screw.nutModel = assetBundle.LoadAsset<GameObject>("screwable_nut.prefab");
			Screw.screwModel = assetBundle.LoadAsset<GameObject>("screwable_screw1.prefab");
			Screw.normalModel = assetBundle.LoadAsset<GameObject>("screwable_screw2.prefab");
			Screw.longModel = assetBundle.LoadAsset<GameObject>("screwable_screw3.prefab");
			assetBundle.Unload(false);
		}
	}
}