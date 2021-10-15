using System;
using System.Collections.Generic;
using MSCLoader;
using MscModApi.Caching;
using UnityEngine;

namespace MscModApi.Shopping
{
	public class Shop
	{
		private static ShopInterface shopInterface;

		internal static Dictionary<ShopLocation, Dictionary<ModItem, Dictionary<string, ShopItem>>> shopItems =
			new Dictionary<ShopLocation, Dictionary<ModItem, Dictionary<string, ShopItem>>>();

		internal static Dictionary<Mod, ModItem> modItemMapping = new Dictionary<Mod, ModItem>();

		public enum ShopLocation
		{
			Fleetari,
			Teimo
		}

		public static class SpawnLocation
		{
			public static class Teimo
			{
				public static Vector3 Backroom = new Vector3(-1551.568f, 5f, 1186.132f);
				public static Vector3 Counter = new Vector3(-1551.135f, 5f, 1182.754f);
				public static Vector3 Outside = new Vector3(-1553.865f, 4f, 1182.825f);
			}

			public static class Fleetari
			{
				public static Vector3 Backroom = new Vector3(1558.975f, 5.2f, 741.894f);
				public static Vector3 Counter = new Vector3(1555.082f, 6f, 737.622f);
				public static Vector3 Outside = new Vector3(1552.154f, 5f, 732.755f);

			}
		}

		internal static class Prefabs
		{
			public static GameObject shopInterface;
			public static GameObject modPanel;
			public static GameObject partPanel;
			public static GameObject cartItem;
			internal static GameObject shopCatalog;
		}

		internal static void Init()
		{
			shopInterface = new ShopInterface();

			shopItems[ShopLocation.Teimo] = new Dictionary<ModItem, Dictionary<string, ShopItem>>();
			shopItems[ShopLocation.Fleetari] = new Dictionary<ModItem, Dictionary<string, ShopItem>>();

			var t = Cache.Find("REPAIRSHOP/inspection_desk 1");
			var t1 = GameObject.Instantiate(Prefabs.shopCatalog);
			t1.transform.SetParent(t.transform);
			t1.transform.localRotation = Quaternion.Euler(0, -90f, -90f);
			t1.transform.localPosition = new Vector3(0.8f, -0.2f, 0.35f);
			t1.transform.localScale = new Vector3(-1f, 1, 1f);
			t1.name = "Shop Catalog";
		}

		public static void Open(ShopLocation shopLocation)
		{
			shopInterface.Open(shopLocation);
		}

		public static void Close()
		{
			shopInterface.Close();
		}

		public static void Add(ShopBaseInfo baseInfo, ShopLocation shopLocation, ShopItem[] shopItems)
		{
			foreach (var shopItem in shopItems)
			{
				Add(baseInfo, shopLocation, shopItem);
			}
		}

		public static void Add(ShopBaseInfo baseInfo, ShopLocation shopLocation, ShopItem shopItem)
		{
			shopItem.SetBaseInfo(baseInfo);
			ModItem modItem = null;

			foreach (var keyValuePair in shopItems[shopLocation])
			{
				if (keyValuePair.Key.mod == baseInfo.mod)
				{
					modItem = keyValuePair.Key;
				}
			}

			if (modItem == null)
			{
				modItem = new ModItem(shopLocation, shopInterface, baseInfo.mod);
				shopItems[shopLocation].Add(modItem, new Dictionary<string, ShopItem>());
			}

			if (shopItems[shopLocation][modItem].ContainsKey(shopItem.GetName()))
			{
				return;
			}

			shopItems[shopLocation][modItem].Add(shopItem.GetName(), shopItem);

			shopItem.Create(shopInterface);
		}

		internal static void LoadAssets(AssetBundle assetBundle)
		{
			Prefabs.shopCatalog = assetBundle.LoadAsset<GameObject>("shop_catalog.prefab");
			Prefabs.shopInterface = assetBundle.LoadAsset<GameObject>("shop_interface.prefab");
			Prefabs.partPanel = assetBundle.LoadAsset<GameObject>("part_panel.prefab");
			Prefabs.modPanel = assetBundle.LoadAsset<GameObject>("mod_panel.prefab");
			Prefabs.cartItem = assetBundle.LoadAsset<GameObject>("cart_item.prefab");
		}
	}
}