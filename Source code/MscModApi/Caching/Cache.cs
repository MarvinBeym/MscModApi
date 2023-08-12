using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;

namespace MscModApi.Caching
{
	public static class Cache
	{
		public static Dictionary<string, GameObject> cachedGameObjects;

		private static GameObject[] globalCache;

		public static GameObject Find(string name, bool findEvenIfInactive = true)
		{
			try {
				GameObject gameObject = cachedGameObjects[name];
				if (gameObject != null) {
					return gameObject;
				}
			}
			catch
			{
				// ignored. Continues below
			}

			GameObject.FindObjectOfType<GameObject>();

			GameObject foundObject = GameObject.Find(name);

			if (!foundObject && findEvenIfInactive)
			{
				foundObject = FindInGlobal(name);
			}

			cachedGameObjects[name] = foundObject;
			return foundObject;
		}

		private static GameObject FindInGlobal(string name)
		{
			if (globalCache == null)
			{
				globalCache = Resources.FindObjectsOfTypeAll<GameObject>();
			}

			foreach(var gameObject in globalCache)
			{
				string nameToCompareTo = gameObject.name;

				if (gameObject.name.Contains("OptionsMenu"))
				{

				}

				if (name.Contains("/"))
				{
					nameToCompareTo = GetObjectPath(gameObject);
				}
				if (nameToCompareTo == name)
				{
					return gameObject;
				}
			}
			return null;
		}

		public static void Clear()
		{
			cachedGameObjects = new Dictionary<string, GameObject>();
		}

		private static string GetObjectPath(GameObject gameObject)
		{
			Transform currentTransform = gameObject.transform;
			string path = currentTransform.name;

			while (currentTransform.parent != null)
			{
				currentTransform = currentTransform.parent;
				path = currentTransform.name + "/" + path;
			}

			return path;
		}

		public static void LoadCleanup()
		{
			cachedGameObjects = new Dictionary<string, GameObject>();
			globalCache = null;
		}
	}
}