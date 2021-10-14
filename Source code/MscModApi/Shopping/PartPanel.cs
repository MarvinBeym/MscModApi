using MscModApi.Parts;
using MscModApi.Tools;
using System;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.UI;

namespace MscModApi.Shopping
{
	internal class PartPanel
	{
		internal static GameObject prefab;
		internal static GameObject panel;
		private static GameObject list;
		private bool multipleBuyable = false;
		internal Shop.ShopLocation shopLocation;
		private string name;
		private float prize;
		private Vector3 defaultPosition;
		private ShopInterface shopInterface;
		private Part part;
		private GameObject partPanel;
		private GameObject gameObjectToInstantiate;

		public PartPanel(ShopInterface shopInterface, string name, float prize, GameObject gameObjectToInstantiate,
			string iconName, Shop.ShopLocation shopLocation, Shop.SpawnLocation spawnLocation)
		{
			this.shopInterface = shopInterface;
			this.gameObjectToInstantiate = gameObjectToInstantiate;
			this.shopLocation = shopLocation;
			this.name = name;
			defaultPosition = Shop.GetSpawnLocation(shopLocation, spawnLocation);

			partPanel = CreatePartPanelObject(gameObjectToInstantiate, prize, iconName);
		}

		public PartPanel(ShopInterface shopInterface, string name, float prize, Part part, string iconName, Shop.ShopLocation shopLocation, Shop.SpawnLocation spawnLocation)
		{
			this.shopInterface = shopInterface;
			this.shopLocation = shopLocation;
			this.name = name;
			this.prize = prize;
			this.part = part;

			partPanel = CreatePartPanelObject(prefab, prize, iconName);

			defaultPosition = Shop.GetSpawnLocation(shopLocation, spawnLocation);
			part.SetDefaultPosition(defaultPosition);
			if (part.partSave.bought == PartSave.BoughtState.NotConfigured)
			{
				part.partSave.bought = PartSave.BoughtState.No;
			}

			if (!part.GetBought())
			{
				part.SetActive(false);
			}
		}

		private GameObject CreatePartPanelObject(GameObject gameObjectToInstantiate, float prize, string iconName)
		{
			var partPanel = GameObject.Instantiate(prefab);
			var partImage = partPanel.FindChild("panel/part_image").GetComponent<Image>();
			if (iconName == "") {
				partImage.enabled = false;
			} else {
				partImage.sprite = part.partBaseInfo.assetBundle.LoadAsset<Sprite>(iconName) ?? partImage.sprite;
			}

			var btnAddToCart = partPanel.FindChild("panel/part_add_to_cart").GetComponent<Button>();
			btnAddToCart.onClick.AddListener(OnPartAddedToCard);

			var partName = partPanel.FindChild("panel/part_name").GetComponent<Text>();
			partName.text = name;
			var partPrize = partPanel.FindChild("panel/part_prize_panel/part_prize").GetComponent<Text>();
			partPrize.text = prize.ToString();

			partPanel.transform.SetParent(list.transform);
			partPanel.transform.localScale = new Vector3(1, 1, 1);
			return partPanel;
		}

		internal static void Init(GameObject shopInterfaceObject)
		{
			panel = shopInterfaceObject.FindChild("panel/shop/parts_panel");
			list = panel.FindChild("parts_list/list/grid");
		}

		internal void OnPartAddedToCard()
		{
			bool cartItemExists = shopInterface.cartItems.ContainsKey(name);

			if (!multipleBuyable && cartItemExists)
			{
				return;
			}

			if (multipleBuyable && cartItemExists)
			{
				shopInterface.cartItems[name].IncreaseCount();
			}

			shopInterface.cartItems.Add(name, new CartItem(shopInterface, name, prize));

			/*
			   part.SetBought(true);
			   part.SetActive(true);
			   part.ResetToDefault();
			   SetVisible(false);
			 */
		}

		internal bool IsBought() => part.GetBought();

		internal string GetName()
		{
			return name;
		}

		internal void SetVisible(bool visible)
		{
			partPanel.SetActive(visible);
		}

		internal bool GetVisible()
		{
			return partPanel.activeSelf;
		}

		internal Part GetPart()
		{
			return part;
		}
	}
}