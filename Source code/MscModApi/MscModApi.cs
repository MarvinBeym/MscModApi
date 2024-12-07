using System;
using MSCLoader;
using MscModApi.Parts;
using MscModApi.Shopping;
using MscModApi.Tools;
using System.Collections.Generic;
using MscModApi.Caching;
using MscModApi.Commands;
using MscModApi.PaintingSystem;
using MscModApi.Parts.EventSystem;
using MscModApi.Parts.PartBox;
using MscModApi.Parts.ReplacePart;
using MscModApi.Saving;
using UnityEngine;

namespace MscModApi
{
	/// <summary>
	/// The MscModApi mod class
	/// </summary>
	/// <seealso cref="MSCLoader.Mod" />
	public class MscModApi : Mod
	{
		public override string ID => "MscModApi";
		public override string Name => "MscModApi";
		public override string Author => "DonnerPlays";
		public override string Version => "1.4.2";

		public override string Description =>
			"A general modding 'help' featuring things like installable/boltable parts, shop, part boxing, utility tools & more.";
		
		public override bool UseAssetsFolder => true;
		private static SettingsCheckBox showBoltSizeSetting;

		private static SettingsCheckBox enableInstantInstall;
		public static SettingsCheckBox disableLoadingMovementLock;

		private const string assetsFile = "msc-mod-api.unity3d";
		private Tool tool;

		public static GameObject mscModApiGameObject;
		public static ReplacedGamePartsDelayedInitializer replacedGamePartsDelayedInitializer;

		internal static Dictionary<string, string> modSaveFileMapping;
		internal static Dictionary<string, Dictionary<string, Part>> modsParts;
		internal static Dictionary<string, Screw> screws;
		private Screw previousScrew;

#if DEBUG
		private Keybind instantInstallKeybind;
#endif

		private bool updateLocked = true;

		internal static bool ShowScrewSize => (bool)showBoltSizeSetting.GetValue();

		public override void ModSetup()
		{
			SetupFunction(Setup.OnGUI, OnGui);
			SetupFunction(Setup.PreLoad, PreLoad);
			SetupFunction(Setup.OnLoad, Load);
			SetupFunction(Setup.PostLoad, PostLoad);

			SetupFunction(Setup.OnMenuLoad, MenuLoad);
			SetupFunction(Setup.OnSave, Save);
			SetupFunction(Setup.Update, Update);
		}

		public override void ModSettings()
		{
			showBoltSizeSetting = Settings.AddCheckBox(this, "showBoltSizeSetting", "Show screw size", false);
			disableLoadingMovementLock = Settings.AddCheckBox(
				this, 
				"disableLoadingMovementLock",
				"Disable movement lock while loading", 
				false
			);


#if DEBUG
			Keybind.AddHeader(this, "Developer Area");
			instantInstallKeybind = Keybind.Add(this, "instant-install", "Instant install part looking at", KeyCode.UpArrow);
			enableInstantInstall = Settings.AddCheckBox(this, "enableInstantInstall", "Enable Instant Part install", false);
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
			SaveLoad.SerializeSaveFile(mod, new Dictionary<string, SerializableColor>(),
				"paintingSystem_saveFile.json");
			SaveLoad.SerializeSaveFile(mod, new Dictionary<string, Dictionary<string, GamePartSave>>(), ReplacedGameParts.saveFileName);
		}


		private void MenuLoad()
		{
			ModConsole.Print(
				$"<color=white>You are running <color=blue>{Name}</color> [<color=green>v{Version}</color>]</color>");

			//Do cleanup of static fields to avoid problems with reloading (going/getting to menu and then going back into game)
			LoadCleanup();

			Logger.InitLogger(this);
			LoadAssets();
			ConsoleCommand.Add(new ScrewPlacementModCommand(this, modsParts));
		}

		private void Load()
		{
			
			updateLocked = false;
			tool = new Tool();
			PaintingSystem.PaintingSystem.Init();
			Shop.Init();
			
			mscModApiGameObject = new GameObject(ID);
			replacedGamePartsDelayedInitializer = mscModApiGameObject.AddComponent<ReplacedGamePartsDelayedInitializer>();
		}
		
		private new void PostLoad()
		{
			replacedGamePartsDelayedInitializer.InitOnceByUpdateFrame();
		}

		private void Save()
		{
			PartBox.Save();
			PaintingSystem.PaintingSystem.Save();

			foreach (var modParts in modsParts)
			{
				var mod = ModLoader.GetMod(modParts.Key);

				if (!modSaveFileMapping.TryGetValue(mod.ID, out var saveFileName))
				{
					//save file for mod can't be found, skip the whole mod.
					continue;
				}

				var modPartSaves = new Dictionary<string, PartSave>();

				foreach (var partData in modParts.Value)
				{
					var id = partData.Key;
					var part = partData.Value;
					try
					{
						part.CustomSaveSaving(mod, $"{id}_saveFile.json");
					}
					catch (Exception eee)
					{
						// ignored
					}

					part.GetEventListeners(PartEvent.Time.Pre, PartEvent.Type.Save).InvokeAll();

					var partSave = part.partSave;
					partSave.position = part.gameObject.transform.position;
					partSave.rotation = part.gameObject.transform.rotation;

					modPartSaves.Add(id, partSave);
				}

				SaveLoad.SerializeSaveFile<Dictionary<string, PartSave>>(mod, modPartSaves, saveFileName);
			}

			ReplacedGameParts.Save();
		}
        
		private new void Update()
		{
			if (updateLocked)
			{
				return;
			}

			Shop.Handle();
#if DEBUG
			InstantInstallDebug();
#endif

			var toolInHand = tool.GetToolInHand();
			if (toolInHand == Tool.ToolType.None)
			{
				if (previousScrew != null)
				{
					previousScrew.Highlight(false);
					previousScrew = null;
				}

				return;
			}

			Screw screw = DetectScrew();

			if (screw == null) return;

			if (screw.part.screwPlacementMode)
			{
				return;
			}

			if (ShowScrewSize && screw.showSize)
			{
				UserInteraction.GuiInteraction($"Screw size: {screw.size.ToString("#.#").Replace(".00", "")}mm");
			}

			if (!tool.CheckScrewFits(screw)) return;

			screw.Highlight(true);

			if (!tool.CheckBoltingSpeed()) return;

			if (UserInteraction.MouseScrollWheel.Up)
			{
				switch (toolInHand)
				{
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
			}
			else if (UserInteraction.MouseScrollWheel.Down)
			{
				switch (toolInHand)
				{
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
			if (
				!enableInstantInstall.GetValue()
				|| Camera.main == null
				|| !UserInteraction.EmptyHand()
				|| CarH.playerInCar
			) {
				return;
			}

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

			if (part == null || !part.hasParent || part.screwPlacementMode)
			{
				return;
			}

			if (part.installBlocked)
			{
				UserInteraction.GuiInteraction("Installation is blocked");
				return;
			}


			if (!part.bolted || !part.hasBolts)
			{
				if (part.installed && part.hasBolts)
				{
					UserInteraction.GuiInteraction("Tighten all screws");
					if (instantInstallKeybind.GetKeybindDown())
					{
						part.partSave.screws.ForEach(delegate (Screw screw)
						{
							screw.InBy(Screw.maxTightness - screw.tightness);
						});
					}
				}
				else if (!part.installed)
				{
					UserInteraction.GuiInteraction("Fully install part");
					if (instantInstallKeybind.GetKeybindDown())
					{
						part.Install();
						part.partSave.screws.ForEach(delegate (Screw screw) { screw.InBy(Screw.maxTightness); });
					}
				}
			}
			else if (part.screws.Count > 0)
			{
				UserInteraction.GuiInteraction("Loosen all screws");
				if (instantInstallKeybind.GetKeybindDown())
				{
					part.partSave.screws.ForEach(delegate (Screw screw) { screw.OutBy(Screw.maxTightness); });
				}
			}
		}
#endif
		private Screw DetectScrew()
		{
			if (previousScrew != null)
			{
				previousScrew.Highlight(false);
				previousScrew = null;
			}

			if (Camera.main == null)
			{
				return null;
			}

			if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 1f,
					1 << LayerMask.NameToLayer("DontCollide")))
			{
				return null;
			}

			if (!hit.collider)
			{
				return null;
			}

			var hitObject = hit.collider.gameObject;
			if (hitObject == null || !hitObject.name.Contains("_screw_")) return null;

			if (!screws.TryGetValue(hitObject.name, out var screw)) return null;
			if (!screw.part.installed) return null;

			previousScrew = screw;

			return screw;
		}

		private void LoadAssets()
		{
			var assetBundle = Helper.LoadAssetBundle(this, assetsFile);
			Screw.LoadAssets(assetBundle);
			Part.LoadAssets(assetBundle);
			Shop.LoadAssets(assetBundle);
			Kit.LoadAssets(assetBundle);
			Box.LoadAssets(assetBundle);

			assetBundle.Unload(false);
		}

		public static void LoadCleanup()
		{
			Cache.LoadCleanup();
			CarH.LoadCleanup();
			Game.LoadCleanup();
			PaintingSystem.PaintingSystem.LoadCleanup();
			Screw.LoadCleanup();
			Shop.LoadCleanup();
			Logger.LoadCleanup();
			ScrewPlacementAssist.LoadCleanup();
			UserInteraction.LoadCleanup();
			Tool.LoadCleanup();
			SatsumaGamePart.LoadCleanup();
			PartBox.LoadCleanup();
			modSaveFileMapping = new Dictionary<string, string>();
			modsParts = new Dictionary<string, Dictionary<string, Part>>();
			screws = new Dictionary<string, Screw>();
			ReplacedGameParts.LoadCleanup();
		}
	}
}