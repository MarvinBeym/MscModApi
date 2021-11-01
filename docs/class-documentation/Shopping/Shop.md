# Shop

Namespace: ``MscModApi.Shopping``

## SpawnLocation
?> A collection of predefined spawn locations you can use
> Teimo
> > SpawnLocation.Teimo.Backroom = new Vector3(-1551.568f, 5f, 1186.132f),  
> > SpawnLocation.Teimo.Counter = new Vector3(-1551.135f, 5f, 1182.754f),  
> > SpawnLocation.Teimo.Outside = new Vector3(-1553.865f, 4f, 1182.825f),
> 
> Fleetari  
> > SpawnLocation.Teimo.Backroom = new Vector3(1558.975f, 5.2f, 741.894f),  
> > SpawnLocation.Teimo.Counter = new Vector3(1555.082f, 6f, 737.622f),  
> > SpawnLocation.Teimo.Outside = new Vector3(1552.154f, 5f, 732.755f), 

## Add
> The main function to be used.  
> This adds a [ShopItem](class-documentation/Shopping/ShopItem.md) to the shop system.
> > | Parameter | Description
> > | --------- | -----------
> > | baseInfo | the base info used internally
> > | shopLocation | enum in which shop the item should be purchasable
> > | shopItem/s | the [ShopItem](class-documentation/Shopping/ShopItem.md) object's to add to the shop
> > Adds multiple shop items at once.
> > ```csharp
> > public static void Add(ShopBaseInfo baseInfo, ShopLocation shopLocation, ShopItem[] shopItems)
> > ```
> > Adds a single shop item.
> > ```csharp
> > public static void Add(ShopBaseInfo baseInfo, ShopLocation shopLocation, ShopItem shopItem)
> > ```