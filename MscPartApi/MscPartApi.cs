using MSCLoader;
using MscPartApi.Tools;
using System.Collections.Generic;
using System.IO;
using MscPartApi.Parts;
using UnityEngine;

namespace MscPartApi
{
	public class MscPartApi : Mod
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

		internal static Dictionary<string, string> modSaveFileMapping;
		internal static Dictionary<string, Dictionary<string, Part>> modsParts;
		internal static Dictionary<string, Screw> screws;
		internal static List<string> modsToIgnoreWhenSaving;
		private Screw previousScrew;
		
#if DEBUG
		private Keybind instantInstallKeybind;
#endif

		internal static bool ShowScrewSize => (bool) showBoltSizeSetting.Value;

		private static bool loadedAssets = false;

		public override void ModSetup()
		{
			SetupFunction(Setup.OnGUI, OnGui);
			SetupFunction(Setup.OnLoad, Load);
			SetupFunction(Setup.OnSave, Save);
			SetupFunction(Setup.Update, Update);

			modSaveFileMapping = new Dictionary<string, string>();
			modsParts = new Dictionary<string, Dictionary<string, Part>>();
			screws = new Dictionary<string, Screw>();
			modsToIgnoreWhenSaving = new List<string>();

			if (!loadedAssets)
			{
				LoadAssets();
				loadedAssets = true;
			}
		}

		public static void DontSaveParts(Mod mod)
		{
			if (!modsToIgnoreWhenSaving.Contains(mod.ID))
			{
				modsToIgnoreWhenSaving.Add(mod.ID);
			}
		}

		public override void ModSettings()
		{
			Settings.AddCheckBox(this, showBoltSizeSetting);
		
			Keybind.AddHeader(this, "Developer area - Screw placement mode");
#if DEBUG
			instantInstallKeybind = Keybind.Add(this, "instant-install", "Instant install part looking at", KeyCode.UpArrow);
#endif
			ScrewPlacementAssist.ModSettings(this);
		}

		public void OnGui()
		{
			ScrewPlacementAssist.OnGui();
		}

		public static void NewGameCleanUp(Mod mod, string saveFileName)
		{
			SaveLoad.SerializeSaveFile(mod, new Dictionary<string, PartSave>(), saveFileName);
		}

		private void Load()
		{
			tool = new Tool();
		}

		private void Save()
		{
			foreach (var modParts in modsParts) {
				var mod = ModLoader.GetMod(modParts.Key);

				if (modsToIgnoreWhenSaving.Contains(mod.ID))
				{
					return;
				}

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

		private new void Update()
		{
#if DEBUG
			InstantInstallDebug();
#endif
			
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

		private void InstantInstallDebug()
		{
			if (Camera.main == null) return;
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1f,
					1 << LayerMask.NameToLayer("Parts"));
			if (hit.collider == null) return;
			var gameObject = hit.collider.gameObject;
			Part part = null;
			foreach (var modParts in modsParts)
			{
				foreach (var partData in modParts.Value)
				{
					var partName = partData.Value.gameObject.name;
					if (partName == gameObject.name)
					{
						part = partData.Value;
						break;
					}
				}

				if (part != null) break;
			}

			if (part == null) return;

			if (!part.IsFixed()) {
				if (part.IsInstalled()) {
					UserInteraction.ShowGuiInteraction(UserInteraction.Type.None, "Tighten all screws");
					if (instantInstallKeybind.GetKeybindDown()) {
						part.partSave.screws.ForEach(delegate(Screw screw)
						{
							screw.InBy(Screw.maxTightness - screw.tightness);
						});
					}
				} else {
					UserInteraction.ShowGuiInteraction(UserInteraction.Type.None, "Fully install part");
					if (instantInstallKeybind.GetKeybindDown()) {
						part.Install();
						part.partSave.screws.ForEach(delegate (Screw screw) {
							screw.InBy(Screw.maxTightness);
						});
					}
				}
			} else {
				UserInteraction.ShowGuiInteraction(UserInteraction.Type.None, "Loosen all screws");
				if (instantInstallKeybind.GetKeybindDown()) {
					part.partSave.screws.ForEach(delegate (Screw screw) {
						screw.OutBy(Screw.maxTightness);
					});
				}
			}

			
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