# PartBaseInfo

Namespace: ``MscModApi.Parts``

!> Your mod should only ever have a single object of the ``PartBaseInfo`` class.<br>Define it at a global place and always use that one.

## CONSTRUCTOR
> The PartBaseInfo constructor  
> * mod: Your mod object
> * assetBundle: your loaded assetBundle (See [Helper.LoadAssetBundle()](class-documentation/Tools/Helper.md#loadassetbundle))
> * partsList: An optional list. All parts created by your mod will be added to this list.
> * saveFilePath: What should the save file be called  
>   where all parts of your mod get saved in.
> ```csharp
> public PartBaseInfo(Mod mod, AssetBundle assetBundle, List<Part> partsList = null, string saveFilePath = "parts_saveFile.json")
> ```