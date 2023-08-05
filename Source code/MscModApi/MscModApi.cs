using System;
using MSCLoader;
using MscModApi.Parts;
using MscModApi.Shopping;
using MscModApi.Tools;
using System.Collections.Generic;
using MscModApi.Commands;
using MscModApi.PaintingSystem;
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
		public override string Version => "1.1.1";

		public override string Description =>
			"This allows developers to make their parts installable on the car. Also adds screws";

		public override bool UseAssetsFolder => true;
		private static Settings showBoltSizeSetting = new Settings("showBoltSizeSetting", "Show screw size", false);

		private static Settings enableInstantInstall =
			new Settings("enableInstantInstall", "Enable Instant Part install", false);
		private const string assetsFile = "msc-mod-api.unity3d";
		private Tool tool;

		internal static Dictionary<string, string> modSaveFileMapping;
		internal static Dictionary<string, Dictionary<string, Part>> modsParts;
		internal static List<PartBox> partBoxes = new List<PartBox>();
		internal static Dictionary<string, Screw> screws;
		private Screw previousScrew;

#if DEBUG
		private Keybind instantInstallKeybind;
#endif

		/// <summary>Enables the screw placement for all parts.</summary>
		/// <param name="mod">The mod.</param>
		[Obsolete("Only kept for compatibility, use part.screwPlacementMode = true/false instead. Won't do anything!")]
		public static void EnableScrewPlacementForAllParts(Mod mod)
		{
			//Don't do anything
		}

		internal static bool ShowScrewSize => (bool)showBoltSizeSetting.Value;

		public override void ModSetup()
		{
			SetupFunction(Setup.OnGUI, OnGui);
			SetupFunction(Setup.OnLoad, Load);
			SetupFunction(Setup.OnMenuLoad, MenuLoad);
			SetupFunction(Setup.OnSave, Save);
			SetupFunction(Setup.Update, Update);

			modSaveFileMapping = new Dictionary<string, string>();
			modsParts = new Dictionary<string, Dictionary<string, Part>>();
			screws = new Dictionary<string, Screw>();
		}

		public override void ModSettings()
		{
			Settings.AddCheckBox(this, showBoltSizeSetting);
			Keybind.AddHeader(this, "Developer area - Screw placement mode");
#if DEBUG
			instantInstallKeybind = Keybind.Add(this, "instant-install", "Instant install part looking at", KeyCode.UpArrow);
			Settings.AddCheckBox(this, enableInstantInstall);
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
			SaveLoad.SerializeSaveFile(mod, new Dictionary<string, SerializableColor>(), "color_saveFile.json");
		}

		private void MenuLoad()
		{
			ModConsole.Print($"<color=white>You are running <color=blue>{Name}</color> [<color=green>v{Version}</color>]</color>");
			Logger.InitLogger(this);
			LoadAssets();
			ConsoleCommand.Add(new ScrewPlacementModCommand(this, modsParts));

		}

		private void Load()
		{
			tool = new Tool();
			PaintingSystem.PaintingSystem.Init();
			Shop.Init();
		}

		private void Save()
		{
			foreach (PartBox partBox in partBoxes)
			{
				partBox.CheckUnpackedOnSave();
			}

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
			if (!(bool)showBoltSizeSetting.Value)
			{
				return;
			}

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

			if (part == null || !part.hasParent || part.screwPlacementMode ||
			    part.uninstallWhenParentUninstalls && !part.parentInstalled) return;

			if (part.installBlocked)
			{
				UserInteraction.GuiInteraction("Installation is blocked");
				return;
			}


			if (!part.IsFixed())
			{
				if (part.installed)
				{
					UserInteraction.GuiInteraction("Tighten all screws");
					if (instantInstallKeybind.GetKeybindDown())
					{
						part.partSave.screws.ForEach(delegate(Screw screw)
						{
							screw.InBy(Screw.maxTightness - screw.tightness);
						});
					}
				}
				else
				{
					UserInteraction.GuiInteraction("Fully install part");
					if (instantInstallKeybind.GetKeybindDown())
					{
						part.Install();
						part.partSave.screws.ForEach(delegate(Screw screw) { screw.InBy(Screw.maxTightness); });
					}
				}
			}
			else
			{
				UserInteraction.GuiInteraction("Loosen all screws");
				if (instantInstallKeybind.GetKeybindDown())
				{
					part.partSave.screws.ForEach(delegate(Screw screw) { screw.OutBy(Screw.maxTightness); });
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
	}
}