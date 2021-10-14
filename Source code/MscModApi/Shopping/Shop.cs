using System;
using System.Collections.Generic;
using MSCLoader;
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
		}

		internal static void Init()
		{
			shopInterface = new ShopInterface();

			shopItems[ShopLocation.Teimo] = new Dictionary<ModItem, Dictionary<string, ShopItem>>();
			shopItems[ShopLocation.Fleetari] = new Dictionary<ModItem, Dictionary<string, ShopItem>>();
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
			Prefabs.shopInterface = assetBundle.LoadAsset<GameObject>("shop_interface.prefab");
			Prefabs.partPanel = assetBundle.LoadAsset<GameObject>("part_panel.prefab");
			Prefabs.modPanel = assetBundle.LoadAsset<GameObject>("mod_panel.prefab");
			Prefabs.cartItem = assetBundle.LoadAsset<GameObject>("cart_item.prefab");
		}
	}
}