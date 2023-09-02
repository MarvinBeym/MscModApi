using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;

namespace MscModApi.Caching
{
	/// <summary>
	/// Cached GameObject.Find implementation for improved performance
	/// </summary>
	public static class Cache
	{
		private static Dictionary<string, GameObject> cachedGameObjects;

		private static GameObject[] globalCache;

		/// <summary>
		/// Works similar to GameObject.Find except that objects are cached once they are found, further "Cache.Find" calls will result in a quicker value return.
		/// Also supports path search like 'Car/Engine/MyCoolPart/MyInnerPart/ThePart'
		/// </summary>
		/// <param name="name">The name or path of the gameobject.</param>
		/// <param name="findEvenIfInactive">Uses an alternative way to find the object, if the part is not active when the .Find happens, without this, 'null' would be returned. Should rarely be disabled</param>
		/// <returns></returns>
		public static GameObject Find(string name, bool findEvenIfInactive = true)
		{
			try {
				GameObject gameObject = cachedGameObjects[name];
				if (gameObject != null) {
					return gameObject;
				}
			}
			catch {
				// ignored. Continues below
			}

			GameObject.FindObjectOfType<GameObject>();

			GameObject foundObject = GameObject.Find(name);

			if (!foundObject && findEvenIfInactive) {
				foundObject = FindInGlobal(name);
			}

			cachedGameObjects[name] = foundObject;
			return foundObject;
		}

		private static GameObject FindInGlobal(string name)
		{
			if (globalCache == null) {
				globalCache = Resources.FindObjectsOfTypeAll<GameObject>();
			}

			foreach (var gameObject in globalCache) {
				string nameToCompareTo = gameObject.name;

				if (gameObject.name.Contains("OptionsMenu")) {
				}

				if (name.Contains("/")) {
					nameToCompareTo = GetObjectPath(gameObject);
				}

				if (nameToCompareTo == name) {
					return gameObject;
				}
			}

			return null;
		}

		private static string GetObjectPath(GameObject gameObject)
		{
			Transform currentTransform = gameObject.transform;
			string path = currentTransform.name;

			while (currentTransform.parent != null) {
				currentTransform = currentTransform.parent;
				path = currentTransform.name + "/" + path;
			}

			return path;
		}

		/// <summary>
		/// Called when the MscModApi mod loads to cleanup static data
		/// </summary>
		public static void LoadCleanup()
		{
			cachedGameObjects = new Dictionary<string, GameObject>();
			globalCache = null;
		}
	}
}