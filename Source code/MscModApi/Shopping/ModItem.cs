using MSCLoader;
using MscModApi.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MscModApi.Shopping.Shop;

namespace MscModApi.Shopping
{
	internal class ModItem
	{
		private ShopLocation shopLocation;
		private ShopInterface shopInterface;
		internal Mod mod;
		private GameObject gameObject;
		private Text partCountComp;
		private List<ShopItem> items = new List<ShopItem>();

		internal ModItem(ShopLocation shopLocation, ShopInterface shopInterface, Mod mod)
		{
			this.shopLocation = shopLocation;
			this.shopInterface = shopInterface;
			this.mod = mod;

			gameObject = GameObject.Instantiate(Prefabs.modPanel);

			var modImage = gameObject.FindChild("panel/mod_image").GetComponent<Image>();
			modImage.enabled = false; //Tmp until solution is found
			//modImage.sprite = Helper.LoadNewSprite(modImage.sprite, mod.Icon);
			var btnOpenShop = gameObject.FindChild("panel/mod_open_shop").GetComponent<Button>();
			btnOpenShop.onClick.AddListener(delegate { shopInterface.OnOpenShop(shopLocation, this); });
			var modName = gameObject.FindChild("panel/mod_name").GetComponent<Text>();
			modName.text = mod.Name;

			partCountComp = gameObject.FindChild("panel/mod_parts_panel/mod_parts_count").GetComponent<Text>();
			gameObject.transform.SetParent(shopInterface.modsList.transform);
			gameObject.transform.localScale = new Vector3(1, 1, 1);
		}

		internal void UpdatePartCount()
		{
			partCountComp.text = GetAvailablePartCount().ToString();
		}

		internal int GetAvailablePartCount()
		{
			int count = 0;
			foreach (ShopItem item in items) {
				count += item.buyable ? 1 : 0;
			}

			return count;
		}

		internal int GetTotalPartCount()
		{
			return items.Count;
		}

		public void Add(ShopItem shopItem)
		{
			GetItems().Add(shopItem);
		}

		public void Show(bool show)
		{
			UpdatePartCount();
			gameObject.SetActive(show);
		}

		public void Open()
		{
			foreach (ShopItem item in GetItems()) {
				item.Show(item.buyable);
			}
		}

		public void Close()
		{
			foreach (ShopItem item in GetItems()) {
				item.Show(false);
			}
		}

		public List<ShopItem> GetItems()
		{
			return items;
		}
	}
}