using System;
using System.Collections.Generic;
using System.Linq;
using MSCLoader;
using MscModApi.Parts;
using MscModApi.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MscModApi.Shopping
{
	internal class ShopInterface
	{
		internal GameObject shopInterface;
		internal static GameObject prefab;
		private Button btnClose;
		private Button btnBack;
		private GameObject cardList;

		internal List<ModPanel> modPanels = new List<ModPanel>();

		internal ShopInterface()
		{
			shopInterface = GameObject.Instantiate(prefab);
			shopInterface.SetActive(false);
			btnClose = shopInterface.FindChild("panel/cart/btnClose").GetComponent<Button>();
			btnClose.onClick.AddListener(Close);
			btnBack = shopInterface.FindChild("panel/shop/parts_panel/btnBack").GetComponent<Button>();
			btnBack.onClick.AddListener(OnBtnBack);
			PartPanel.Init(shopInterface);
			ModPanel.Init(shopInterface);
			cardList = shopInterface.FindChild("panel/cart/cart_list/list/grid");
		}

		internal void AddPartPanel(ModPanel modPanel, string name, float prize, Part part, string iconName)
		{
			modPanel.partPanels.Add(new PartPanel(this, name, prize, part, iconName));
			modPanel.UpdatePartCounter();
		}

		internal ModPanel AddModPanel(Mod mod)
		{
			ModPanel modPanel = null;
			modPanels.ForEach(delegate(ModPanel panel)
			{
				if (panel.mod == mod)
				{
					modPanel = panel;
				}
			});

			if (modPanel != null) return modPanel;
			modPanel = new ModPanel(this, mod);
			modPanels.Add(modPanel);

			return modPanel;
		}

		internal void ChangeModPanel(ModPanel modPanel)
		{
			PartPanel.panel.SetActive(true);
			ModPanel.panel.SetActive(false);
			btnBack.enabled = true;
		}

		internal void Open()
		{
			shopInterface.SetActive(true);
		}

		internal void Close()
		{
			shopInterface.SetActive(false);
			OnBtnBack();
		}

		internal void OnBtnBack()
		{
			PartPanel.panel.SetActive(false);
			ModPanel.panel.SetActive(true);
		}
	}
}