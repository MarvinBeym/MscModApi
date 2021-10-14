using System;
using MSCLoader;
using MscModApi.Parts;
using UnityEngine;

namespace MscModApi.Shopping
{
	public static class Shop
	{
		internal enum ShopLocation
		{
			Fleetari,
			Teimo
		}
		public enum SpawnLocation
		{
			Outside,
			Backroom,
			Counter,
		}

		private static ShopInterface shopInterface;

		internal static void LoadAssets(AssetBundle assetBundle)
		{
			ShopInterface.prefab = assetBundle.LoadAsset<GameObject>("shop_interface.prefab");
			PartPanel.prefab = assetBundle.LoadAsset<GameObject>("part_panel.prefab");
			CartItem.cartItemPrefab = assetBundle.LoadAsset<GameObject>("cart_item.prefab");
			ModPanel.prefab = assetBundle.LoadAsset<GameObject>("mod_panel.prefab");
		}

		internal static Vector3 GetSpawnLocation(ShopLocation shopLocation, SpawnLocation spawnLocation)
		{
			switch (shopLocation)
			{
				case ShopLocation.Teimo:
					switch (spawnLocation)
					{
						case SpawnLocation.Backroom:
							return new Vector3(-1551.568f, 5f, 1186.132f);
						case SpawnLocation.Counter:
							return new Vector3(-1551.135f, 5f, 1182.754f);
						case SpawnLocation.Outside:
							return new Vector3(-1553.865f, 4f, 1182.825f);
					}

					break;
				case ShopLocation.Fleetari:
					switch (spawnLocation) {
						case SpawnLocation.Backroom:
							return new Vector3(1558.975f, 5.2f, 741.894f);
						case SpawnLocation.Counter:
							return new Vector3(1555.082f, 6f, 737.622f);
						case SpawnLocation.Outside:
							return new Vector3(1552.154f, 5f, 732.755f);
					}
					break;
			}

			return Vector3.zero;
		}

		internal static void Init()
		{
			shopInterface = new ShopInterface();
		}

		public static void Open()
		{
			shopInterface.Open(ShopLocation.Fleetari);
		}

		public static void AddToTeimo(Mod mod, string name, float prize, Part part, SpawnLocation spawnLocation, string iconName = null)
		{
			Add(mod, name, prize, part, ShopLocation.Teimo, spawnLocation, iconName ?? "");
		}

		public static void AddMultiToTeimo(Mod mod, string name, float prize, GameObject partToInstantiate, SpawnLocation spawnLocation, string iconName = null)
		{
			AddMulti(mod, name, prize, partToInstantiate, ShopLocation.Teimo, spawnLocation, iconName ?? "");
		}

		public static void AddMultiToFleetari(Mod mod, string name, float prize, GameObject partToInstantiate, SpawnLocation spawnLocation, string iconName = null)
		{
			AddMulti(mod, name, prize, partToInstantiate, ShopLocation.Fleetari, spawnLocation, iconName ?? "");
		}

		public static void AddToFleetari(Mod mod, string name, float prize, Part part, SpawnLocation spawnLocation, string iconName = null)
		{
			Add(mod, name, prize, part, ShopLocation.Fleetari, spawnLocation, iconName ?? "");
		}

		private static void AddMulti(Mod mod, string name, float prize, GameObject partToInstantiate, ShopLocation shopLocation, SpawnLocation spawnLocation, string iconName)
		{
			var modPanel = shopInterface.AddModPanel(mod, shopLocation);
			shopInterface.AddMultiBuyPartPanel(modPanel, name, prize, partToInstantiate, iconName ?? "", shopLocation, spawnLocation);
		}

		private static void Add(Mod mod, string name, float prize, Part part, ShopLocation shopLocation, SpawnLocation spawnLocation, string iconName)
		{
			var modPanel = shopInterface.AddModPanel(mod, shopLocation);
			shopInterface.AddPartPanel(modPanel, name, prize, part, iconName, shopLocation, spawnLocation);
		}
	}
}