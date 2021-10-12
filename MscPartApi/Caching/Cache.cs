using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

namespace MscPartApi.Caching
{
	public static class Cache
	{
		public static Dictionary<string, GameObject> cachedGameObjects = new Dictionary<string, GameObject>();

		public static GameObject Find(string name)
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

			GameObject foundObject = GameObject.Find(name);
			cachedGameObjects[name] = foundObject;
			return foundObject;
		}

		public static void Clear()
		{
			cachedGameObjects = new Dictionary<string, GameObject>();
		}
	}
}