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
		private static GameObject boxTemplateModel;

		public Kit(string name, Part[] parts)
		{
			GameObject kitBox = GameObject.Instantiate(boxTemplateModel);
			Setup(name, kitBox, parts);
		}

		public Kit(string name, Vector3 scale, Part[] parts)
		{
			GameObject kitBox = GameObject.Instantiate(boxTemplateModel);
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

			AddParts(parts); 
			
			if (!bought)
			{
				foreach (Part part in this.parts)
				{
					part.Uninstall();
					part.active = false;
				}

				active = false;
			}

			logic = boxModel.AddComponent<KitLogic>();
			logic.Init(this);

			this.boxModel = boxModel;
		}

		internal static void LoadAssets(AssetBundle assetBundle)
		{
			boxTemplateModel = assetBundle.LoadAsset<GameObject>("cardboard_box.prefab");
		}
	}
}