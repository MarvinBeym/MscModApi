using MscModApi.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MscModApi.Shopping
{
	internal class CartItem
	{
		internal static GameObject cartItemPrefab;
		internal static GameObject panel;
		private static GameObject list;

		private ShopInterface shopInterface;
		private GameObject cartItem;
		private string name;
		private float basePrize = 0;
		internal float totalPrize = 0;
		internal int count = 1;
		private Text itemPrize;
		private Text itemCount;

		internal CartItem(ShopInterface shopInterface, string name, float prize)
		{
			this.name = name;
			this.basePrize = prize;
			totalPrize = basePrize;
			this.shopInterface = shopInterface;
			cartItem = GameObject.Instantiate(cartItemPrefab);
			var btnRemoveFromCart = cartItem.FindChild("panel/btnRemoveFromCart").GetComponent<Button>();
			btnRemoveFromCart.onClick.AddListener(DecreaseCount);
			var partName = cartItem.FindChild("panel/part_name").GetComponent<Text>();
			itemPrize = cartItem.FindChild("panel/item_prize_panel/item_prize").GetComponent<Text>();
			itemCount = cartItem.FindChild("panel/item_count_panel/item_count").GetComponent<Text>();

			partName.text = name;
			UpdateItem(true);

			cartItem.transform.SetParent(list.transform);
			cartItem.transform.localScale = new Vector3(1, 1, 1);
		}

		internal static void Init(GameObject shopInterfaceObject)
		{
			list = shopInterfaceObject.FindChild("panel/cart/cart_list/list/grid");
		}

		internal void IncreaseCount()
		{
			count++;
			UpdateItem(true);
		}

		private void DecreaseCount()
		{
			if (count > 1)
			{
				count--;
				UpdateItem(false);
			}
			else
			{
				RemoveFromCart();
			}
		}

		internal void RemoveFromCart()
		{
			shopInterface.cartItems.Remove(name);
			GameObject.Destroy(cartItem);
		}

		private void UpdateItem(bool increase)
		{
			totalPrize = count * basePrize;
			itemPrize.text = totalPrize.ToString();
			itemCount.text = count.ToString();

			if (increase)
			{
				shopInterface.totalCostValue += basePrize;
			}
			else
			{
				shopInterface.totalCostValue -= basePrize;
			}
		}
	}
}