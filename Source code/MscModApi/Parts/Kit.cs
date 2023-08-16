using MscModApi.Shopping;
using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.Parts
{
	public class Kit : PartBox
	{
		private BoxLogic logic;
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
			gameObject = boxModel;

			AddParts(parts);

			if (!bought) {
				foreach (Part part in this.parts) {
					part.Uninstall();
					part.active = false;
				}
			}

			active = false;
			logic = gameObject.AddComponent<BoxLogic>();
			logic.Init(this);
		}

		internal static void LoadAssets(AssetBundle assetBundle)
		{
			boxTemplateModel = assetBundle.LoadAsset<GameObject>("cardboard_box.prefab");
		}
	}
}