using System;
using System.Collections.Generic;
using MSCLoader;
using MscModApi.Caching;
using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.Shopping
{
	public class Shop
	{
		private static ShopInterface shopInterface;

		internal static Dictionary<ShopLocation, Dictionary<ModItem, Dictionary<string, ShopItem>>> shopItems =
			new Dictionary<ShopLocation, Dictionary<ModItem, Dictionary<string, ShopItem>>>();

		public enum ShopLocation
		{
			Fleetari,
			Teimo
		}

		public static class SpawnLocation
		{
			public static class Teimo
			{
				public static Vector3 Backroom { get; } = new Vector3(-1551.568f, 5f, 1186.132f);
				public static Vector3 Counter { get; } = new Vector3(-1551.135f, 5f, 1182.754f);
				public static Vector3 Outside { get; } = new Vector3(-1553.865f, 4f, 1182.825f);
			}

			public static class Fleetari
			{
				public static Vector3 Backroom { get; } = new Vector3(1558.975f, 5.2f, 741.894f);
				public static Vector3 Counter { get; } = new Vector3(1555.082f, 6f, 737.622f);
				public static Vector3 Outside { get; } = new Vector3(1552.154f, 5f, 732.755f);

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

		private static Dictionary<ShopLocation, GameObject> shopCatalogs = new Dictionary<ShopLocation, GameObject>();

		internal static void Init()
		{
			shopCatalogs = new Dictionary<ShopLocation, GameObject>();
			shopInterface = new ShopInterface();

			foreach (var shopLocation in (ShopLocation[]) Enum.GetValues(typeof(ShopLocation))) {
				shopItems[shopLocation] = new Dictionary<ModItem, Dictionary<string, ShopItem>>();
				GameObject shopCatalogParent = null;
				Vector3 position = new Vector3(0, 0, 0);
				Vector3 rotation = new Vector3(0, 0, 0);
				Vector3 scale = new Vector3(1, 1, 1);

				switch (shopLocation)
				{
					case ShopLocation.Teimo:
						shopCatalogParent = Cache.Find("STORE");
						position = new Vector3(-2.96f, 1.31f, -0.34f);
						rotation = new Vector3(0, -180f, 0);
						break;
					case ShopLocation.Fleetari:
						shopCatalogParent = Cache.Find("REPAIRSHOP/inspection_desk 1");
						position = new Vector3(0.8f, -0.2f, 0.35f);
						rotation = new Vector3(0, -90f, -90f);
						scale = new Vector3(-1f, 1, 1f);
						break;
				}

				if (shopCatalogParent == null) continue;
				var shopCatalog = GameObject.Instantiate(Prefabs.shopCatalog);
				shopCatalog.transform.SetParent(shopCatalogParent.transform);
				shopCatalog.transform.localPosition = position;
				shopCatalog.transform.localRotation = Quaternion.Euler(rotation);
				shopCatalog.transform.localScale = scale;
				shopCatalog.name = $"{shopLocation} Shop Catalog(Clone)";
				shopCatalogs.Add(shopLocation, shopCatalog);

			}
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

		internal static void Handle()
		{
			if (shopInterface.IsOpen()) return;
			foreach (var keyValuePair in shopCatalogs)
			{
				var shopLocation = keyValuePair.Key;
				var shopCatalog = keyValuePair.Value;
				if (shopCatalog.IsLookingAt())
				{
					UserInteraction.GuiInteraction($"Open catalog");
					if (UserInteraction.LeftMouseDown)
					{
						shopInterface.Open(shopLocation);
					}
				}
			}
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