using System;
using MSCLoader;
using MscModApi.Parts;
using UnityEngine;

namespace MscModApi.Shopping
{
	public static class Shop
	{
		private static ShopInterface shopInterface;

		internal static void LoadAssets(AssetBundle assetBundle)
		{
			ShopInterface.prefab = assetBundle.LoadAsset<GameObject>("shop_interface.prefab");
			PartPanel.prefab = assetBundle.LoadAsset<GameObject>("part_panel.prefab");
			ModPanel.prefab = assetBundle.LoadAsset<GameObject>("mod_panel.prefab");
		}

		internal static void Init()
		{
			shopInterface = new ShopInterface();
		}

		public static void Open()
		{
			shopInterface.Open();
		}

		public static void Add(Mod mod, string name, float prize, Part part, string iconName = null)
		{
			var modPanel = shopInterface.AddModPanel(mod);
			shopInterface.AddPartPanel(modPanel, name, prize, part, iconName);
		}
	}
}