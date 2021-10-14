using System;
using System.Collections.Generic;
using System.Linq;
using MSCLoader;
using MscModApi.Caching;
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

		internal List<ModPanel> modPanels = new List<ModPanel>();

		internal Dictionary<string, CartItem> cartItems = new Dictionary<string, CartItem>();
		private Button btnBuy;
		private Text money;
		internal float totalCostValue;
		internal Text totalCost;
		private Button btnBack;

		internal ShopInterface()
		{
			shopInterface = GameObject.Instantiate(prefab);
			shopInterface.SetActive(false);
			var cart = shopInterface.FindChild("panel/cart/");


			var btnClose = cart.FindChild("btnClose").GetComponent<Button>();
			btnClose.onClick.AddListener(Close);
			btnBack = shopInterface.FindChild("panel/shop/parts_panel/btnBack").GetComponent<Button>();
			btnBack.onClick.AddListener(OnBtnBack);
			PartPanel.Init(shopInterface);
			ModPanel.Init(shopInterface);
			CartItem.Init(shopInterface);

			btnBuy = cart.FindChild("btnBuy").GetComponent<Button>();
			money = cart.FindChild("money").GetComponent<Text>();
			totalCost = cart.FindChild("totalCost").GetComponent<Text>();
		}

		internal void AddMultiBuyPartPanel(ModPanel modPanel, string name, float prize, GameObject partToInstantiate,
			string iconName, ShopLocation shopLocation, SpawnLocation spawnLocation)
		{
			var partPanels = GetPartPanels(modPanel, shopLocation);

			if (!CheckPartPanelDoesNotExist(modPanel, name)) {
				var partPanel = new PartPanel(this, name, prize, partToInstantiate, iconName, shopLocation, spawnLocation);
				partPanels.Add(partPanel);
				modPanel.partPanels.Add(partPanel);
				modPanel.UpdatePartCounter(shopLocation);
			}
		}

		private List<PartPanel> GetPartPanels(ModPanel modPanel, ShopLocation shopLocation)
		{
			switch (shopLocation) {
				case ShopLocation.Teimo:
					return modPanel.teimoPanels;
				case ShopLocation.Fleetari:
					return modPanel.fleetariPanels;
			}

			return null;
		}

		private bool CheckPartPanelDoesNotExist(ModPanel modPanel, string name)
		{
			foreach (var panel in modPanel.partPanels) {
				if (panel.GetName() == name)
				{
					return false;
				}
			}

			return true;
		}

		internal void AddPartPanel(ModPanel modPanel, string name, float prize, Part part, string iconName, ShopLocation shopLocation, SpawnLocation spawnLocation)
		{
			var partPanels = GetPartPanels(modPanel, shopLocation);

			if (CheckPartPanelDoesNotExist(modPanel, name))
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
			money.text = Game.money.ToString();
			switch (shopLocation)
			{
				case ShopLocation.Teimo:
					foreach (var modPanel in modPanels) {
						var countVisible = 0;
						foreach (var partPanel in modPanel.teimoPanels) {
							partPanel.SetVisible(partPanel.shopLocation == shopLocation && !partPanel.IsBought());
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
							partPanel.SetVisible(partPanel.shopLocation == shopLocation && !partPanel.IsBought());
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
			totalCost.text = "0";
			shopInterface.SetActive(false);
			foreach (var cartItem in cartItems)
			{
				cartItem.Value.RemoveFromCart();
			}
			OnBtnBack();
		}

		internal void OnBtnBack()
		{
			PartPanel.panel.SetActive(false);
			ModPanel.panel.SetActive(true);
		}
	}
}