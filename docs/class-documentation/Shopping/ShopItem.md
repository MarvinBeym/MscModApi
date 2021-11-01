# ShopItem

Namespace: ``MscModApi.Shopping``

## CONSTRUCTOR
> The ShopItem constructor
> > | Parameter | Description | Example
> > | --------- | ----------- | -------
> > | name | The name of the shop item how it is shown in the shop interface |
> > | prize | How much the part costs |
> > | spawnLocation | Where the part should spawn<br>(See [Shop.SpawnLocation](class-documentation/Shopping/Shop.md#spawnlcation) |
> > | imageAssetName | Optional image to use as found iny your assets bundle | "my_fancy_shop_item_image.png"
> >
> > > Used if you want to create the part at 'runtime'.<br>Meaning the part does not exist yet and gets created when the item is bought.
> > >
> > > | Parameter | Description
> > > | --------- | -----------
> > > | onPurchaseAction | Gets executed when the ShopItem gets bought
> > > | multiPurchase | can multiple instances of this item be bought.
> > > ```csharp
> > > public ShopItem(string name, float prize, Vector3 spawnLocation, Action onPurchaseAction, string imageAssetName = "", bool multiPurchase = true)
> > > ```
> >
> > > Used when the part is already predefined. Will handle everything on its own.
> > > default location will also be set to the ``spawnLocation`` 
> > >
> > > | Parameter | Description
> > > | --------- | -----------
> > > | part | The part that will be spawned at the ``spawnLocation`` on purchase
> > > ```csharp
> > > public ShopItem(string name, float prize, Vector3 spawnLocation, Part part, string imageAssetName = "")
> > > ```

## SetMultiPurchase
> Changes the multiPurchase flag, only works with constructor that has the ``onPurchaseAction`` parameter
> ```csharp
> public void SetMultiPurchase(bool multiPurchase)
> ```

## IsMultiPurchase
> Returns if the shop item can be purchased multiple times
> ```csharp
> public bool IsMultiPurchase()
> ```