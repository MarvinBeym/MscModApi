using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MSCLoader;
using Newtonsoft.Json;
using UnityEngine;

namespace MscPartApi.Tools
{
	internal static class Helper
	{
		internal static string CombinePaths(params string[] paths)
		{
			if (paths == null)
			{
				throw new ArgumentNullException(nameof(paths));
			}

			return paths.Aggregate(Path.Combine);
		}

		internal static AssetBundle LoadAssetBundle(Mod mod, string fileName)
		{
			try
			{
				return LoadAssets.LoadBundle(mod, fileName);
			}
			catch
			{
				string message = $"AssetBundle file '{fileName}' could not be loaded";
				ModConsole.Error(message);
				ModUI.ShowYesNoMessage($"{message}\n\nClose Game? - RECOMMENDED", ExitGame);
			}

			return null;
		}

		internal static void ExitGame()
		{
			Application.Quit();
		}

		internal static PlayMakerFSM FindFsm(this GameObject gameObject, string fsmName)
		{
			return gameObject.GetComponents<PlayMakerFSM>().FirstOrDefault(fsm => fsm.FsmName == fsmName);
		}

		internal static Vector3 CopyVector3(Vector3 old)
		{
			return new Vector3(old.x, old.y, old.z);
		}

		internal static T LoadSaveOrReturnNew<T>(Mod mod, string saveFilePath) where T : new()
		{
			var path = Path.Combine(ModLoader.GetModSettingsFolder(mod), saveFilePath);
			return !File.Exists(path) ? new T() : JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
		}

		internal static GameObject LoadPartAndSetName(AssetBundle assetsBundle, string prefabName, string name)
		{
			var gameObject = GameObject.Instantiate(assetsBundle.LoadAsset(prefabName) as GameObject);
			gameObject.SetNameLayerTag(name + "(Clone)", "PART", "Parts");

			return gameObject;
		}

		internal static void SetNameLayerTag(this GameObject gameObject, string name, string tag, string layer)
		{
			gameObject.name = name;
			gameObject.tag = tag;
			gameObject.layer = LayerMask.NameToLayer(layer);
			gameObject.FixName();
		}

		internal static void FixName(this GameObject gameObject)
		{
			gameObject.name = Regex.Replace(
				gameObject.name,
				"\\(Clone\\){1,}", "(Clone)"
			);
		}
	}
}