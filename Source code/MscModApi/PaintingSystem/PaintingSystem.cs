using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using MSCLoader;
using MscModApi.Caching;
using MscModApi.Parts;
using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.PaintingSystem
{
	public static class PaintingSystem
	{
		private enum State
		{
			Painting,
			NotPainting
		}

		private static GameObject sprayCan;
		private static PlayMakerFSM sprayCanFsm;
		private static FsmColor sprayCanColorFsm;
		private static State state = State.NotPainting;

		private static Dictionary<string, List<PaintingStorage>> storage =
			new Dictionary<string, List<PaintingStorage>>();

		private static Material[] availableMaterials = new Material[0];

		private static Dictionary<string, Dictionary<string, SerializableColor>> modSave =
			new Dictionary<string, Dictionary<string, SerializableColor>>();

		public static PaintingStorage Setup(Mod mod, string id, GameObject paintDetector,
			Dictionary<GameObject, List<string>> paintConfig)
		{
			if (!storage.ContainsKey(mod.ID))
			{
				storage.Add(mod.ID, new List<PaintingStorage>());
			}

			Dictionary<string, SerializableColor> colorSave = new Dictionary<string, SerializableColor>();
			if (!modSave.ContainsKey(mod.ID))
			{
				colorSave = Helper.LoadSaveOrReturnNew<Dictionary<string, SerializableColor>>(mod,
					"color_saveFile.json");
				modSave.Add(mod.ID, colorSave);
			}
			else
			{
				colorSave = modSave[mod.ID];
			}


			PaintingSystemLogic logic = paintDetector.AddComponent<PaintingSystemLogic>();

			Dictionary<GameObject, List<Material>> collectedPaintMaterialConfig =
				new Dictionary<GameObject, List<Material>>();

			foreach (var pair in paintConfig)
			{
				List<string> rebuildMaterialList = new List<string>();

				foreach (var materialName in pair.Value)
				{
					rebuildMaterialList.Add(materialName);
					rebuildMaterialList.Add(materialName + " (Instance)");
				}

				List<Material> materialsToPaint = new List<Material>();

				foreach (Renderer renderer in pair.Key.GetComponentsInChildren<Renderer>(true))
				{
					if (rebuildMaterialList.Contains(renderer.material.name) || pair.Value.Count == 0)
					{
						materialsToPaint.Add(renderer.material);
					}
					/*
					 Disabled/Unused until required => Use main material only for now
					foreach (Material material in renderer.materials)
					{
						if (rebuildMaterialList.Contains(material.name) || pair.Value.Count == 0)
						{
							materialsToPaint.Add(material);
						}
					}
					*/
				}

				collectedPaintMaterialConfig.Add(pair.Key, materialsToPaint);
			}

			PaintingStorage paintingStorage = new PaintingStorage(mod, id, collectedPaintMaterialConfig);

			if (colorSave.ContainsKey(id))
			{
				paintingStorage.SetColor(colorSave[id]);
			}

			storage[mod.ID].Add(paintingStorage);
			logic.Init(paintingStorage);

			return paintingStorage;
		}

		public static PaintingStorage Setup(Mod mod, Part paintDetector,
			Dictionary<GameObject, List<string>> paintConfig)
		{
			return Setup(mod, paintDetector.id, paintDetector.gameObject, paintConfig);
		}

		public static PaintingStorage Setup(Mod mod, Part paintDetector, GameObject objectToPaint,
			List<string> materialsToPaint)
		{
			Dictionary<GameObject, List<string>> paintConfig = new Dictionary<GameObject, List<string>>
			{
				{ objectToPaint, materialsToPaint }
			};

			return Setup(mod, paintDetector.id, paintDetector.gameObject, paintConfig);
		}

		public static PaintingStorage Setup(Mod mod, Part paintDetector, GameObject objectToPaint,
			string materialToPaint)
		{
			Dictionary<GameObject, List<string>> paintConfig = new Dictionary<GameObject, List<string>>
			{
				{ objectToPaint, new List<string> { materialToPaint } }
			};

			return Setup(mod, paintDetector.id, paintDetector.gameObject, paintConfig);
		}

		public static PaintingStorage Setup(Mod mod, Part paintDetector, GameObject objectToPaint)
		{
			Dictionary<GameObject, List<string>> paintConfig = new Dictionary<GameObject, List<string>>
			{
				{ objectToPaint, new List<string>() }
			};

			return Setup(mod, paintDetector.id, paintDetector.gameObject, paintConfig);
		}

		public static PaintingStorage Setup(Mod mod, Part paintDetectorAndObjectToPaint)
		{
			Dictionary<GameObject, List<string>> paintConfig = new Dictionary<GameObject, List<string>>
			{
				{ paintDetectorAndObjectToPaint.gameObject, new List<string>() }
			};

			return Setup(mod, paintDetectorAndObjectToPaint.id, paintDetectorAndObjectToPaint.gameObject, paintConfig);
		}

		internal static void Init()
		{
			sprayCan = Cache.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/SprayCan");
			sprayCanFsm = sprayCan.GetComponent<PlayMakerFSM>();
			sprayCanColorFsm = sprayCanFsm.FsmVariables.FindFsmColor("SprayColor");

			sprayCanFsm.InitializeFSM();

			FsmHook.FsmInject(sprayCan, "Stage 1", delegate() { state = State.NotPainting; });
			FsmHook.FsmInject(sprayCan, "Painting", delegate() { state = State.Painting; });

			availableMaterials = Resources.FindObjectsOfTypeAll<Material>();
		}

		internal static bool IsPainting()
		{
			return state == State.Painting;
		}

		internal static Color GetCurrentColor()
		{
			return sprayCanColorFsm.Value;
		}

		internal static Material FindMaterial(string name)
		{
			foreach (Material material in availableMaterials)
			{
				if (material.name == name)
				{
					return material;
				}
			}

			return null;
		}

		public static void Save()
		{
			Dictionary<string, SerializableColor> colorSave = new Dictionary<string, SerializableColor>();
			Mod mod = null;
			foreach (var storagePair in storage)
			{
				foreach (var pair in storagePair.Value)
				{
					mod = pair.GetMod();
					colorSave.Add(pair.GetPaintingId(), pair.GetCurrentColor());
				}
			}

			if (mod != null)
			{
				SaveLoad.SerializeSaveFile(mod, colorSave, "color_saveFile.json");
			}
		}
	}
}