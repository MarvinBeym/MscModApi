using MSCLoader;
using MscModApi.Tools;
using System.Collections.Generic;
using System.IO;
using MscModApi.Caching;
using MscModApi.Parts;
using MscModApi.Shopping;
using UnityEngine;
using UnityEngine.UI;

namespace MscModApi
{
	public class MscModApi : Mod
	{
		public override string ID => "MscModApi";
		public override string Name => "MscModApi";
		public override string Author => "DonnerPlays";
		public override string Version => "1.0.3";
		
		public override string Description =>
			"This allows developers to make their parts installable on the car. Also adds screws";

		public override bool UseAssetsFolder => true;
		private static Settings showBoltSizeSetting = new Settings("showBoltSizeSetting", "Show screw size", false);

		private const string assetsFile = "msc-mod-api.unity3d";
		private Tool tool;

		internal static Dictionary<string, string> modSaveFileMapping;
		internal static Dictionary<string, Dictionary<string, Part>> modsParts;
		internal static Dictionary<string, Screw> screws;
		private Screw previousScrew;
		
#if DEBUG
		private Keybind instantInstallKeybind;
#endif

		internal static List<Mod> globalScrewPlacementModeEnabled = new List<Mod>();

		public static void EnableScrewPlacementForAllParts(Mod mod)
		{
			globalScrewPlacementModeEnabled.Add(mod);
		}

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
#if DEBUG
			instantInstallKeybind = Keybind.Add(this, "instant-install", "Instant install part looking at", KeyCode.UpArrow);
#endif
			ScrewPlacementAssist.ModSettings(this);
		}

		public void OnGui()
		{
			ScrewPlacementAssist.OnGui();
		}

		public static void NewGameCleanUp(Mod mod, string saveFileName = "parts_saveFile.json")
		{
			SaveLoad.SerializeSaveFile(mod, new Dictionary<string, PartSave>(), saveFileName);
		}

		private void Load()
		{
			ModConsole.Print($"<color=white>You are running <color=blue>{Name}</color> [<color=green>v{Version}</color>]</color>");
			Logger.InitLogger(this);
			tool = new Tool();
			Shop.Init();
		}

		private void Save()
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
					try
					{
						part.CustomSaveSaving(mod, $"{id}_saveFile.json");
					}
					catch
					{
						// ignored
					}

					part.preSaveActions.InvokeAll();

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
			Shop.Handle();
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
				UserInteraction.GuiInteraction($"Screw size: {screw.size.ToString("#.#").Replace(".00", "")}mm");
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
		}
#if DEBUG
		private void InstantInstallDebug()
		{
			if (Camera.main == null || !UserInteraction.EmptyHand()) return;
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

			if (part == null || !part.HasParent() || part.IsInScrewPlacementMode() || part.uninstallWhenParentUninstalls && !part.ParentInstalled()) return;

			if (part.IsInstallBlocked())
			{
				UserInteraction.GuiInteraction("Installation is blocked");
				return;
			}


			if (!part.IsFixed()) {
				if (part.IsInstalled()) {
					UserInteraction.GuiInteraction("Tighten all screws");
					if (instantInstallKeybind.GetKeybindDown()) {
						part.partSave.screws.ForEach(delegate(Screw screw)
						{
							screw.InBy(Screw.maxTightness - screw.tightness);
						});
					}
				} else {
					UserInteraction.GuiInteraction("Fully install part");
					if (instantInstallKeybind.GetKeybindDown()) {
						part.Install();
						part.partSave.screws.ForEach(delegate (Screw screw) {
							screw.InBy(Screw.maxTightness);
						});
					}
				}
			} else {
				UserInteraction.GuiInteraction( "Loosen all screws");
				if (instantInstallKeybind.GetKeybindDown()) {
					part.partSave.screws.ForEach(delegate (Screw screw) {
						screw.OutBy(Screw.maxTightness);
					});
				}
			}

			
		}
#endif
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
			
			var assetBundle = Helper.LoadAssetBundle(this, assetsFile);
			Screw.LoadAssets(assetBundle);
			Part.LoadAssets(assetBundle);
			Shop.LoadAssets(assetBundle);

			assetBundle.Unload(false);
		}
	}
}