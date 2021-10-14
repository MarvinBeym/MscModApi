using MscModApi.Tools;
using System;
using System.Collections.Generic;
using MscModApi.Caching;
using UnityEngine;
using UnityEngine.UI;
using static MscModApi.Shopping.Shop;

namespace MscModApi.Shopping
{
	internal class ShopInterface
	{
		private GameObject gameObject;
		internal GameObject partsPanel;
		internal GameObject modsPanel;
		internal GameObject cartPanel;

		internal GameObject partsList;
		internal GameObject modsList;
		internal GameObject cartList;
		private Text moneyComp;
		private Text totalCostComp;

		private List<ShopItem> shoppingCart = new List<ShopItem>();

		private float totalCost = 0;

		internal ShopInterface()
		{
			gameObject = GameObject.Instantiate(Shop.Prefabs.shopInterface);
			gameObject.SetActive(false);
			partsPanel = gameObject.FindChild("panel/shop/parts_panel");
			modsPanel = gameObject.FindChild("panel/shop/mods_panel");
			cartPanel = gameObject.FindChild("panel/cart");

			partsPanel.FindChild("btnBack").GetComponent<Button>().onClick.AddListener(OnBack);
			cartPanel.FindChild("btnClose").GetComponent<Button>().onClick.AddListener(Close);

			moneyComp = cartPanel.FindChild("money").GetComponent<Text>();
			totalCostComp = cartPanel.FindChild("totalCost").GetComponent<Text>();
			cartPanel.FindChild("btnBuy").GetComponent<Button>().onClick.AddListener(OnCheckout);

			partsList = partsPanel.FindChild("parts_list/list/grid");
			modsList = modsPanel.FindChild("mod_list/list/grid");
			cartList = cartPanel.FindChild("cart_list/list/grid");
		}

		internal void Open(Shop.ShopLocation shopLocation)
		{
			moneyComp.text = Game.money.ToString();
			gameObject.SetActive(true);

			foreach (var keyValuePair in shopItems[shopLocation])
			{
				keyValuePair.Key.SetActive(true);
			}

			foreach (var keyValuePair in shopItems)
			{
				if (keyValuePair.Key != shopLocation)
				{
					foreach (var valuePair in keyValuePair.Value)
					{
						valuePair.Key.SetActive(false);
					}
				}
			}

		}

		internal void Close()
		{
			OnBack();
			gameObject.SetActive(false);
		}

		internal void OnBack()
		{
			modsPanel.SetActive(true);
			partsPanel.SetActive(false);
		}

		internal void OnOpenShop(ShopLocation shopLocation, ModItem modItem)
		{
			modsPanel.SetActive(false);
			partsPanel.SetActive(true);

			foreach (var keyValuePair in shopItems[shopLocation][modItem])
			{
				var shopItem = keyValuePair.Value;
				shopItem.SetActive(true);
			}

			SetShopItemsActiveForModItem(modItem, true);
		}

		private void SetShopItemsActiveForModItem(ModItem modItem, bool active)
		{
			foreach (var shopLocationMap in shopItems)
			{
				var location = shopLocationMap.Key;
				var modItemMap = shopLocationMap.Value;
				foreach (var valuePair in modItemMap)
				{
					var shopItemMap = valuePair.Value;
					if (modItem == valuePair.Key)
					{
						foreach (var keyValuePair in shopItemMap) {
							var shopItem = keyValuePair.Value;
							shopItem.SetActive(active);
						}
					}
					else
					{
						foreach (var keyValuePair in shopItemMap) {
							var shopItem = keyValuePair.Value;
							shopItem.SetActive(!active);
						}
					}

				}
			}
		}

		internal void OnAddToCart(ShopItem shopItem)
		{
			if (shoppingCart.Contains(shopItem))
			{
				if (shopItem.IsMultiPurchase())
				{
					shopItem.IncreaseCount();
					totalCost += shopItem.baseItemPrize;
					totalCostComp.text = totalCost.ToString();
				}

				return;
			}

			shopItem.AddToCart();

			shoppingCart.Add(shopItem);
			totalCost += shopItem.baseItemPrize;
			totalCostComp.text = totalCost.ToString();
		}

		internal void OnRemoveFromCart(ShopItem shopItem)
		{
			totalCost -= shopItem.baseItemPrize;
			totalCostComp.text = totalCost.ToString();
			if (!shopItem.IsMultiPurchase())
			{
				GameObject.Destroy(shopItem.cartItemGameObject);
				shoppingCart.Remove(shopItem);
			}
			else
			{
				if (shopItem.itemCount <= 1)
				{
					GameObject.Destroy(shopItem.cartItemGameObject);
					shoppingCart.Remove(shopItem);
				}
				else
				{
					shopItem.DecreaseCount();
				}
				
			}
		}
		internal void OnCheckout()
		{
			Game.money -= totalCost;
			totalCost = 0;
			totalCostComp.text = totalCost.ToString();
			foreach (var shopItem in shoppingCart) {
				if (shopItem.IsMultiPurchase())
				{
					for (var i = 0; i < shopItem.itemCount; i++)
					{
						shopItem.onPurchaseAction.Invoke();
					}
				}
				else
				{
					shopItem.onPurchaseAction.Invoke();
				}
				GameObject.Destroy(shopItem.cartItemGameObject);
			}
			shoppingCart.Clear();
			gameObject.PlayCheckout();
			Close();
		}
	}
}