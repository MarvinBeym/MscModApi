using MSCLoader;
using MscModApi.Tools;
using UnityEngine;
using UnityEngine.UI;
using static MscModApi.Shopping.Shop;

namespace MscModApi.Shopping
{
	internal class ModItem
	{
		private ShopLocation shopLocation;
		private ShopInterface shopInterface;
		private Mod mod;
		private Text partCountComp;

		internal ModItem(ShopLocation shopLocation, ShopInterface shopInterface, Mod mod)
		{
			this.shopLocation = shopLocation;
			this.shopInterface = shopInterface;
			this.mod = mod;

			var gameObject = GameObject.Instantiate(Prefabs.modPanel);

			var modImage = gameObject.FindChild("panel/mod_image").GetComponent<Image>();
			modImage.enabled = false; //Tmp until solution is found
			//modImage.sprite = Helper.LoadNewSprite(modImage.sprite, mod.Icon);
			var btnOpenShop = gameObject.FindChild("panel/mod_open_shop").GetComponent<Button>();
			btnOpenShop.onClick.AddListener(delegate
			{
				shopInterface.OnOpenShop(shopLocation, this);
			});
			var modName = gameObject.FindChild("panel/mod_name").GetComponent<Text>();
			modName.text = mod.Name;

			partCountComp = gameObject.FindChild("panel/mod_parts_panel/mod_parts_count").GetComponent<Text>();
			gameObject.transform.SetParent(shopInterface.modsList.transform);
			gameObject.transform.localScale = new Vector3(1, 1, 1);
		}

		internal void SetPartCount(int count)
		{
			partCountComp.text = count.ToString();
		}
	}
}