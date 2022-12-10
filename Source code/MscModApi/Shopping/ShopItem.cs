using System;
using MscModApi.Parts;
using MscModApi.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MscModApi.Shopping
{
	public class ShopItem
	{
		private string name;
		private float prize;
		private Vector3 spawnLocation;
		private string imageAssetName;
		private ShopInterface shopInterface;
		internal GameObject partItemGameObject;
		internal Action onPurchaseAction;
		private ShopBaseInfo baseInfo;

		private bool multiPurchase = false;
		private Text itemCountComp;
		private Text itemPrizeComp;

		internal int itemCount = 1;
		internal float baseItemPrize = 0;
		internal GameObject cartItemGameObject;
		private bool buyable = true;

		public ShopItem(string name, float prize, Vector3 spawnLocation, Action onPurchaseAction, string imageAssetName = "", bool multiPurchase = true)
		{
			SetMultiPurchase(multiPurchase);
			Setup(name, prize, spawnLocation, imageAssetName);
			this.onPurchaseAction = onPurchaseAction;
		}

		public ShopItem(string name, float prize, Vector3 spawnLocation, Part part, string imageAssetName = "")
		{
			Setup(name, prize, spawnLocation, imageAssetName);
			
			if (part.partSave.bought == PartSave.BoughtState.NotConfigured) {
				part.partSave.bought = PartSave.BoughtState.No;
			}

			part.SetDefaultPosition(spawnLocation);
			part.SetActive(part.IsBought());
			buyable = !part.IsBought();

			onPurchaseAction = delegate
			{
				OnPartPurchase(part);
			};
		}

		internal bool IsBuyable()
		{
			return buyable;
		}

		private void Setup(string name, float prize, Vector3 spawnLocation, string imageAssetName)
		{
			this.name = name;
			this.prize = prize;
			this.spawnLocation = spawnLocation;
			this.imageAssetName = imageAssetName;
			baseItemPrize = prize;
		}

		internal void Create(ShopInterface shopInterface)
		{
			this.shopInterface = shopInterface;
			partItemGameObject = GameObject.Instantiate(Shop.Prefabs.partPanel);
			var partImageComp = partItemGameObject.FindChild("panel/part_image").GetComponent<Image>();
			if (imageAssetName == "") {
				partImageComp.enabled = false;
			} else {
				partImageComp.sprite = baseInfo.assetBundle.LoadAsset<Sprite>(imageAssetName) ?? partImageComp.sprite;
			}

			var btnAddToCart = partItemGameObject.FindChild("panel/part_add_to_cart").GetComponent<Button>();
			btnAddToCart.onClick.AddListener(delegate {
				shopInterface.OnAddToCart(this);
			});

			var partNameComp = partItemGameObject.FindChild("panel/part_name").GetComponent<Text>();
			partNameComp.text = name;
			var partPrizeComp = partItemGameObject.FindChild("panel/part_prize_panel/part_prize").GetComponent<Text>();
			partPrizeComp.text = prize.ToString();

			partItemGameObject.transform.SetParent(shopInterface.partsList.transform);
			partItemGameObject.transform.localScale = new Vector3(1, 1, 1);
		}

		private void OnPartPurchase(Part part)
		{
			part.SetBought(true);
			part.SetActive(true);
			part.ResetToDefault();
		}

		internal void AddToCart()
		{
			cartItemGameObject = GameObject.Instantiate(Shop.Prefabs.cartItem);
			var itemNameComp = cartItemGameObject.FindChild("panel/part_name").GetComponent<Text>();
			itemNameComp.text = name;
			itemPrizeComp = cartItemGameObject.FindChild("panel/item_prize_panel/item_prize").GetComponent<Text>();
			itemPrizeComp.text = prize.ToString();

			cartItemGameObject.FindChild("panel/btnRemoveFromCart").GetComponent<Button>().onClick.AddListener(delegate
			{
				shopInterface.OnRemoveFromCart(this);
			});

			itemCountComp = cartItemGameObject.FindChild("panel/item_count_panel/item_count").GetComponent<Text>();
			itemCountComp.text = itemCount.ToString();

			cartItemGameObject.transform.SetParent(shopInterface.cartList.transform);
			cartItemGameObject.transform.localScale = new Vector3(1, 1, 1);
		}

		internal void IncreaseCount()
		{
			itemCount++;
			itemCountComp.text = itemCount.ToString();
			itemPrizeComp.text = (itemCount * baseItemPrize).ToString();
		}

		internal void DecreaseCount()
		{
			itemCount--;
			itemCountComp.text = itemCount.ToString();
			itemPrizeComp.text = (itemCount * baseItemPrize).ToString();
		}

		internal string GetName()
		{
			return name;
		}

		internal void SetBaseInfo(ShopBaseInfo baseInfo)
		{
			this.baseInfo = baseInfo;
		}

		public void SetMultiPurchase(bool multiPurchase)
		{
			this.multiPurchase = multiPurchase;
		}

		public bool IsMultiPurchase()
		{
			return multiPurchase;
		}

		public void Show(bool show)
		{
			partItemGameObject?.SetActive(show);
		}
	}
}