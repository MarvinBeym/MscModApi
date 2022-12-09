using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;

namespace MscModApi.Caching
{
	public static class Cache
	{
		public static Dictionary<string, GameObject> cachedGameObjects = new Dictionary<string, GameObject>();

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
				if (gameObject.name == name)
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
	}
}