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

		public Kit(string name, GameObject customBoxModel, Part[] parts)
		{
			Setup(name, customBoxModel, parts);
		}

		private void Setup(string name, GameObject boxModel, Part[] parts)
		{
			boxModel.SetNameLayerTag(name + "(Clone)");

			SetParts(parts);
			if (!AnyBought())
			{
				foreach (Part part in parts)
				{
					part.Uninstall();
					part.SetActive(false);
				}
				boxModel.SetActive(false);
			}

			logic = boxModel.AddComponent<KitLogic>();
			logic.Init(this);

			SetBoxGameObject(boxModel);
		}

		internal override void CheckUnpackedOnSave()
		{
			if (!AnyBought())
			{
				return;
			}

			GameObject box = GetBoxGameObject();

			if (spawnedCounter < GetPartCount())
			{
				foreach (Part part in GetParts())
				{
					if (part.IsInstalled() || part.gameObject.activeSelf)
					{
						continue;
					}

					part.SetPosition(box.transform.position);
					part.SetActive(true);
				}
			}

			box.SetActive(false);
			box.transform.position = new Vector3(0, 0, 0);
			box.transform.localPosition = new Vector3(0, 0, 0);
		}

		public bool IsBought()
		{
			return GetParts().All(part => part.IsBought());
		}

		public bool AnyBought()
		{
			return GetParts().Any(part => part.IsBought());
		}


		internal static void LoadAssets(AssetBundle assetBundle)
		{
			boxModel = assetBundle.LoadAsset<GameObject>("cardboard_box.prefab");
		}
	}
}