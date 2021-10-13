using System;
using System.Collections.Generic;
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
		internal List<PartPanel> partPanels = new List<PartPanel>();
		private Text modPartsCount;

		internal static void Init(GameObject shopInterfaceObject)
		{
			panel = shopInterfaceObject.FindChild("panel/shop/mods_panel");
			list = panel.FindChild("mod_list/list/grid");
		}

		internal ModPanel(ShopInterface shopInterface, Mod mod)
		{
			this.shopInterface = shopInterface;
			this.mod = mod;
			var panel = GameObject.Instantiate(prefab);
			var modImage = panel.FindChild("mod_image").GetComponent<Image>();
			//modImage.sprite = Helper.LoadNewSprite(modImage.sprite, mod.Icon);
			var btnOpenShop = panel.FindChild("mod_open_shop").GetComponent<Button>();
			btnOpenShop.onClick.AddListener(delegate
			{
				shopInterface.ChangeModPanel(this);
			});
			var modName = panel.FindChild("mod_name").GetComponent<Text>();
			modName.text = mod.Name;

			modPartsCount = panel.FindChild("mod_parts_panel/mod_parts_count").GetComponent<Text>();
			panel.transform.SetParent(list.transform);
			panel.transform.localScale = new Vector3(1, 1, 1);
		}

		internal void UpdatePartCounter()
		{
			modPartsCount.text = partPanels.Count.ToString();
		}
	}
}