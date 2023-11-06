using System;
using System.Linq;
using MscModApi.Parts;
using MscModApi.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MscModApi.Shopping
{
	public class ShopItem
	{
		public BasicPart part
		{
			get;
			protected set;
		}

		public string name
		{
			get;
			protected set;
		}

		public float prize
		{
			get;
			protected set;
		}
		private Vector3 spawnLocation;
		protected string imageAssetName;
		private ShopInterface shopInterface;
		internal GameObject partItemGameObject;
		internal Action onPurchaseAction;
		private ShopBaseInfo baseInfo;

		/// <summary>
		/// If the item in the shop can be bought multiple times (won't be removed from the shop on purchase)
		/// </summary>
		public bool multiPurchase
		{
			get;
			protected set;
		}
		private Text itemCountComp;
		private Text itemPrizeComp;

		internal int itemCount = 1;
		internal float baseItemPrize = 0;
		internal GameObject cartItemGameObject;

		/// <summary>
		/// Is the item currently buyable
		/// </summary>
		public bool buyable
		{
			get
			{
				if (multiPurchase)
				{
					return true;
				}

				return !part.bought;
			}
		}

		public ShopItem(string name, float prize, Vector3 spawnLocation, Action onPurchaseAction,
			string imageAssetName = "", bool multiPurchase = true)
		{
			Setup(name, prize, spawnLocation, imageAssetName);
			this.onPurchaseAction = onPurchaseAction;
		}

		public ShopItem(string name, float prize, Vector3 spawnLocation, PartBox partBox, string imageAssetName = "")
		{
			Setup(name, prize, spawnLocation, imageAssetName);
			foreach (Part partBoxChild in partBox.childs) {
				if (partBoxChild.partSave.bought == PartSave.BoughtState.NotConfigured)
				{
					partBoxChild.partSave.bought = PartSave.BoughtState.No;
				}

				partBoxChild.defaultPosition = spawnLocation;
				partBoxChild.active = partBoxChild.bought;
			}

			onPurchaseAction = delegate
			{
				foreach (Part partBoxChild in partBox.childs) {
					partBoxChild.bought = true;
					partBoxChild.defaultPosition = spawnLocation;
				}

				partBox.active = true;
				partBox.position = spawnLocation;
				partBox.rotation = new Vector3(0, 0, 0);
			};

			part = partBox;
		}


		public ShopItem(string name, float prize, Vector3 spawnLocation, Part part, string imageAssetName = "")
		{
			Setup(name, prize, spawnLocation, imageAssetName);

			if (part.partSave.bought == PartSave.BoughtState.NotConfigured)
			{
				part.partSave.bought = PartSave.BoughtState.No;
			}

			part.defaultPosition = spawnLocation;
			part.active = part.bought;
			this.part = part;

			onPurchaseAction = delegate { OnPartPurchase(part); };
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
			}
			else {
				partImageComp.sprite = baseInfo.assetBundle.LoadAsset<Sprite>(imageAssetName) ?? partImageComp.sprite;
			}

			var btnAddToCart = partItemGameObject.FindChild("panel/part_add_to_cart").GetComponent<Button>();
			btnAddToCart.onClick.AddListener(delegate { shopInterface.OnAddToCart(this); });

			var partNameComp = partItemGameObject.FindChild("panel/part_name").GetComponent<Text>();
			partNameComp.text = name;
			var partPrizeComp = partItemGameObject.FindChild("panel/part_prize_panel/part_prize").GetComponent<Text>();
			partPrizeComp.text = prize.ToString();

			partItemGameObject.transform.SetParent(shopInterface.partsList.transform);
			partItemGameObject.transform.localScale = new Vector3(1, 1, 1);
		}

		private void OnPartPurchase(Part part)
		{
			part.bought = true;
			part.active = true;
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

		public void Show(bool show)
		{
			partItemGameObject?.SetActive(show);
		}
	}
}