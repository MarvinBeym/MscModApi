using MSCLoader;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MscModApi.Tools
{
	public static class Helper
	{
		public static string CombinePaths(params string[] paths)
		{
			if (paths == null) {
				throw new ArgumentNullException(nameof(paths));
			}

			return paths.Aggregate(Path.Combine);
		}

		internal static AssetBundle LoadAssetBundle(Mod mod, string fileName)
		{
			try {
				return LoadAssets.LoadBundle(mod, fileName);
			} catch {
				string message = $"AssetBundle file '{fileName}' could not be loaded";
				ModConsole.Error(message);
				ModUI.ShowYesNoMessage($"{message}\n\nClose Game? - RECOMMENDED", ExitGame);
			}

			return null;
		}

		public static void ExitGame()
		{
			Application.Quit();
		}

		public static PlayMakerFSM FindFsm(this GameObject gameObject, string fsmName)
		{
			return gameObject.GetComponents<PlayMakerFSM>().FirstOrDefault(fsm => fsm.FsmName == fsmName);
		}

		internal static Vector3 CopyVector3(Vector3 old)
		{
			return new Vector3(old.x, old.y, old.z);
		}

		public static bool CheckCloseToPosition(Vector3 positionOfPartTocheck, Vector3 position, float minimumDistance)
		{
			try
			{
				return Vector3.Distance(positionOfPartTocheck, position) <= minimumDistance;
			} catch {
				return false;
			}
		}

		public static T LoadSaveOrReturnNew<T>(Mod mod, string saveFilePath) where T : new()
		{
			var path = Path.Combine(ModLoader.GetModSettingsFolder(mod), saveFilePath);

			T save;

			if (!File.Exists(path))
			{
				save = new T();
			}
			else
			{
				save = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));

				if (save == null)
				{
					save = new T();
				}
			}

			return save;
		}

		internal static GameObject LoadPartAndSetName(AssetBundle assetsBundle, string prefabName, string name, bool addClone = true)
		{
			var gameObject = GameObject.Instantiate(assetsBundle.LoadAsset(prefabName) as GameObject);
			gameObject.SetNameLayerTag(name + (addClone ? "(Clone)" : ""), "PART", "Parts");

			return gameObject;
		}

		public static void SetNameLayerTag(this GameObject gameObject, string name, string tag = "PART", string layer = "Parts")
		{
			gameObject.name = name;
			gameObject.tag = tag;
			gameObject.layer = LayerMask.NameToLayer(layer);
			gameObject.FixName();
		}

		public static void FixName(this GameObject gameObject)
		{
			gameObject.name = Regex.Replace(
				gameObject.name,
				"\\(Clone\\){1,}", "(Clone)"
			);
		}

		public static bool CompareVector3(this Vector3 vector3, Vector3 other, float tolerance = 0.05f)
		{
			return Math.Abs(vector3.x - other.x) < tolerance && Math.Abs(vector3.y - other.y) < tolerance && Math.Abs(vector3.z - other.z) < tolerance;
		}

		public static Sprite LoadNewSprite(Sprite sprite, string FilePath, float pivotX = 0.5f, float pivotY = 0.5f,
			float PixelsPerUnit = 100.0f)
		{
			if (File.Exists(FilePath) && Path.GetExtension(FilePath) == ".png") {
				var spriteTexture = LoadTexture(FilePath);
				var newSprite = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height),
					new Vector2(pivotX, pivotY), PixelsPerUnit);

				return newSprite;
			}

			return sprite;
		}

		public static bool CompareQuaternion(this Quaternion a, Quaternion b, float tolerance = 0)
		{
			return 1 - Mathf.Abs(Quaternion.Dot(a, b)) < tolerance;
		}

		public static Texture2D LoadTexture(string FilePath)
		{
			if (File.Exists(FilePath)) {
				var FileData = File.ReadAllBytes(FilePath);
				var Tex2D = new Texture2D(2, 2);
				if (Tex2D.LoadImage(FileData))
					return Tex2D;
			}

			return null;
		}
	}
}