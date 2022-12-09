using MscModApi.Parts;
using MscModApi.Tools;
using System;
using System.Linq;
using UnityEngine;
using static MscModApi.Shopping.Shop;

namespace MscModApi.Shopping
{
	public class Kit : PartBox
	{
		public GameObject kitBox;
		public Part[] parts;
		private KitLogic logic;
		public int spawnedCounter = 0;
		private static GameObject boxModel;

		public Kit(string name, Part[] parts)
		{
			GameObject kitBox = GameObject.Instantiate(boxModel);
			Setup(name, kitBox, parts);
		}

		public Kit(string name, Vector3 scale, Part[] parts)
		{
			GameObject kitBox = GameObject.Instantiate(boxModel);
			kitBox.GetComponent<BoxCollider>().size = scale;
			kitBox.transform.FindChild("default").localScale = scale;
			Setup(name, kitBox, parts);
		}

		public Kit(string name, GameObject kitBox, Part[] parts)
		{
			Setup(name, kitBox, parts);
		}

		private void Setup(string name, GameObject kitBox, Part[] parts)
		{
			this.kitBox = kitBox;
			this.kitBox.SetNameLayerTag(name + "(Clone)");

			this.parts = parts;
			if (!AnyBought())
			{
				foreach (Part part in parts)
				{
					part.Uninstall();
					part.SetActive(false);
				}
				kitBox.SetActive(false);
			}

			logic = kitBox.AddComponent<KitLogic>();
			logic.Init(this);
		}

		internal override void CheckUnpackedOnSave()
		{
			if (!AnyBought())
			{
				return;
			}

			if (spawnedCounter < parts.Length)
			{
				foreach (Part part in parts)
				{
					if (part.IsInstalled() || part.gameObject.activeSelf)
					{
						continue;
					}

					part.SetPosition(kitBox.transform.position);
					part.SetActive(true);
				}
			}

			kitBox.SetActive(false);
			kitBox.transform.position = new Vector3(0, 0, 0);
			kitBox.transform.localPosition = new Vector3(0, 0, 0);
		}

		public bool IsBought()
		{
			return parts.All(part => part.IsBought());
		}

		public bool AnyBought()
		{
			return parts.Any(part => part.IsBought());
		}

		internal static void LoadAssets(AssetBundle assetBundle)
		{
			boxModel = assetBundle.LoadAsset<GameObject>("cardboard_box.prefab");
		}
	}
}