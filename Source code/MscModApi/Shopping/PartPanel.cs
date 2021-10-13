using MscModApi.Parts;
using MscModApi.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MscModApi.Shopping
{
	internal class PartPanel
	{
		internal static GameObject prefab;
		internal static GameObject panel;
		private static GameObject list;
		private ShopInterface shopInterface;
		private Part part;

		public PartPanel(ShopInterface shopInterface, string name, float prize, Part part, string iconName)
		{
			this.shopInterface = shopInterface;
			this.part = part;
			var panel = GameObject.Instantiate(prefab);
			var partImage = panel.FindChild("part_image").GetComponent<Image>();
			partImage.sprite = part.partBaseInfo.assetBundle.LoadAsset<Sprite>(iconName) ?? partImage.sprite;
			var btnBuyPart = panel.FindChild("part_buy").GetComponent<Button>();
			btnBuyPart.onClick.AddListener(OnPartBought);

			var partName = panel.FindChild("part_name").GetComponent<Text>();
			partName.text = name;
			var partPrize = panel.FindChild("part_prize_panel/part_prize").GetComponent<Text>();
			partPrize.text = prize.ToString();

			panel.transform.SetParent(list.transform);
			panel.transform.localScale = new Vector3(1, 1, 1);
		}

		internal static void Init(GameObject shopInterfaceObject)
		{
			panel = shopInterfaceObject.FindChild("panel/shop/parts_panel");
			list = panel.FindChild("parts_list/list/grid");
		}

		internal void OnPartBought()
		{

		}
	}
}