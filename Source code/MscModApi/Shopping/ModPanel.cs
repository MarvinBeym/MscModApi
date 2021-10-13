using System;
using System.Collections.Generic;
using System.Linq;
using MSCLoader;
using MscModApi.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MscModApi.Shopping
{
	internal class ModPanel
	{
		internal static GameObject prefab;
		internal static GameObject panel;
		internal static GameObject list;

		private ShopInterface shopInterface;
		internal Mod mod;
		private GameObject modPanel;
		private Text modPartsCount;

		internal List<PartPanel> partPanels = new List<PartPanel>();
		internal List<PartPanel> fleetariPanels = new List<PartPanel>();
		internal List<PartPanel> teimoPanels = new List<PartPanel>();

		internal static void Init(GameObject shopInterfaceObject)
		{
			panel = shopInterfaceObject.FindChild("panel/shop/mods_panel");
			list = panel.FindChild("mod_list/list/grid");
		}

		internal ModPanel(ShopInterface shopInterface, Mod mod)
		{
			this.shopInterface = shopInterface;
			this.mod = mod;
			modPanel = GameObject.Instantiate(prefab);
			var modImage = modPanel.FindChild("mod_image").GetComponent<Image>();
			//modImage.sprite = Helper.LoadNewSprite(modImage.sprite, mod.Icon);
			var btnOpenShop = modPanel.FindChild("mod_open_shop").GetComponent<Button>();
			btnOpenShop.onClick.AddListener(delegate
			{
				shopInterface.ChangeModPanel(this);
			});
			var modName = modPanel.FindChild("mod_name").GetComponent<Text>();
			modName.text = mod.Name;

			modPartsCount = modPanel.FindChild("mod_parts_panel/mod_parts_count").GetComponent<Text>();
			modPanel.transform.SetParent(list.transform);
			modPanel.transform.localScale = new Vector3(1, 1, 1);
		}

		internal void UpdatePartCounter(Shop.ShopLocation shopLocation)
		{
			var count = 0;
			switch (shopLocation) {
				case Shop.ShopLocation.Teimo:
					count += teimoPanels.Sum(partPanel => partPanel.GetVisible() ? 1 : 0);
					break;
				case Shop.ShopLocation.Fleetari:
					count += fleetariPanels.Sum(partPanel => partPanel.GetVisible() ? 1 : 0);
					break;
			}

			modPartsCount.text = count.ToString();
		}

		internal void SetVisible(bool visible)
		{
			modPanel.SetActive(visible);
		}
	}
}