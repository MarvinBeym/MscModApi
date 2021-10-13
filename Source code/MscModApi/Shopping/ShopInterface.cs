using System;
using System.Collections.Generic;
using System.Linq;
using MSCLoader;
using MscModApi.Parts;
using MscModApi.Tools;
using UnityEngine;
using UnityEngine.UI;
using static MscModApi.Shopping.Shop;

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

		internal void AddPartPanel(ModPanel modPanel, string name, float prize, Part part, string iconName, ShopLocation shopLocation, SpawnLocation spawnLocation)
		{

			List<PartPanel> partPanels = new List<PartPanel>();
			switch (shopLocation)
			{
				case ShopLocation.Teimo:
					partPanels = modPanel.teimoPanels;
					
					break;
				case ShopLocation.Fleetari:
					partPanels = modPanel.fleetariPanels;
					break;
			}

			bool partPanelAlreadyExists = false;
			foreach (var panel in modPanel.partPanels)
			{
				if (panel.GetName() == name || panel.GetPart() == part || panel.GetPart().id == part.id)
				{
					partPanelAlreadyExists = true;
				}
			}

			if (!partPanelAlreadyExists)
			{
				var partPanel = new PartPanel(this, name, prize, part, iconName, shopLocation, spawnLocation);
				partPanels.Add(partPanel);
				modPanel.partPanels.Add(partPanel);
				modPanel.UpdatePartCounter(shopLocation);
			}

		}

		internal ModPanel AddModPanel(Mod mod, Shop.ShopLocation shopLocation)
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

		internal void Open(Shop.ShopLocation shopLocation)
		{
			switch (shopLocation)
			{
				case ShopLocation.Teimo:
					foreach (var modPanel in modPanels) {
						var countVisible = 0;
						foreach (var partPanel in modPanel.teimoPanels) {
							partPanel.SetVisible(partPanel.shopLocation == shopLocation);
							countVisible += partPanel.GetVisible() ? 1 : 0;
						}

						modPanel.SetVisible(countVisible > 0);

					}
					break;
				case ShopLocation.Fleetari:
					foreach (var modPanel in modPanels)
					{
						var countVisible = 0;
						foreach (var partPanel in modPanel.fleetariPanels)
						{
							partPanel.SetVisible(partPanel.shopLocation == shopLocation);
							countVisible += partPanel.GetVisible() ? 1 : 0;
						}

						modPanel.SetVisible(countVisible > 0);

					}
					break;
			}

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