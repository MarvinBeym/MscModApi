# Part

Namespace: ``MscModApi.Parts``

## CONSTRUCTOR
> The Part class constructor
> > | Parameter | Description | Example
> > | --------- | ----------- | -------
> > | id | An internal id that is for example used when saving the part data to a file
> > | name | Name of the part (what will the player get displayed when he looks at it
> > | installPosition | installation position on the parent part
> > | installRotation | installation rotation on the parent part
> > | partBaseInfo | The collection required by all parts to have a centralized base data
> > | uninstallWhenParentUninstalls | Should this part be uninstalled from the parent 'before' the parent uninstalls<br>Meaning: if the parent get's removed,<br>first all child parent will be removed, then the parent.
> > | disableCollisionWhenInstalled | Greatly improves performance as nothing can collide with the part when it is installed
> > | prefabName | The name of your prefab from your assets bundle<br>If null will use the id + ".prefab" | "my_super_cool_part.prefab"
> > > Can be used when you might have multiple instances of a part, using the same ``GameObject`` as the part
> > > ```csharp
> > > public Part(string id, string name, GameObject part, Part parentPart, Vector3 installPosition, Vector3 installRotation, PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true, bool disableCollisionWhenInstalled = true)
> > > ```
> > > Used when the part should be attached to a part of the game<br>(like carburetor, ...)
> > > ```csharp
> > > public Part(string id, string name, GameObject parent, Vector3 installPosition, Vector3 installRotation, PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true, bool disableCollisionWhenInstalled = true, string prefabName = null)
> > > ```
> > > Can be used when the part should not have a parent, meaning it can't be installed/fixed, ...
> > > ```csharp
> > > public Part(string id, string name, PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true, bool disableCollisionWhenInstalled = true, string prefabName = null)
> > > ```
> > > Can be used if the part has another ``Part`` as the parent
> > > ```csharp
> > > public Part(string id, string name, Part parentPart, Vector3 installPosition, Vector3 installRotation, PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true, bool disableCollisionWhenInstalled = true, string prefabName = null)
> > > ```

## EnableScrewPlacementMode
> Enables the screw placement mode for this part.
> ```csharp
> public void EnableScrewPlacementMode()
> ```

## SetPosition
> Sets the part's position, only works when part is not installed.
> ```csharp
> public void SetPosition(Vector3 position)
> ```

## GetScrews
> Returns a ``List`` of all screws
> ```csharp
> public List<Screw> GetScrews()
> ```

## SetRotation
> Sets the part's rotation, only works when the part is not installed.
> ```csharp
> public void SetRotation(Quaternion rotation)
> ```

## Install
> Installs the part on it's parent (if possible).
> ```csharp
> public void Install()
> ```

## IsInstalled
> Returns if the part is currently installed on the parent.
> ```csharp
> public bool IsInstalled()
> ```

## IsFixed
> Returns if the part is fixed in place using screws.
> > | Parameter | Description
> > | --------- | -----------
> > | ignoreUnsetScrews | If part has no screw it will just return the ``Part.IsInstalled()`` state
> > ```csharp
> > public bool IsFixed(bool ignoreUnsetScrews = true)
> > ```

## Uninstall
> Uninstalls the part from it's parent (if possible).
> ```csharp
> public void Uninstall()
> ```

## AddClampModel
> Adds a 3D-Model of a clamp (round) to the parent.
> > | Parameter | Description
> > | --------- | -----------
> > | position | Where on the parent to place the clamp
> > | rotation | Rotation of the clamp on the parent
> > | scale | scale of the clamp model
> > ```csharp
> > public void AddClampModel(Vector3 position, Vector3 rotation, Vector3 scale)
> > ```

## ParentFixed
> Returns if the parent is currently fixed in place.
> ```csharp
> public bool ParentFixed()
> ```

## AddScrew
> Add a screw to this part, making it possible to fix it in place.
> ```csharp
> public void AddScrew(Screw screw)
> ```

## AddScrews
> Allows to quickly add multiple screws in one go.  
> Also allows to leave out the scale and size on each screw if they are the same
> > | Parameter | Description
> > | --------- | -----------
> > | screws | An array of ``Screw`` objects
> > | overrideScale | If all screws you want to add have the same scale, you can set it using this parameter to reduce code size
> > | overrideSize | If all screws you want to add have the same wrench size, you can set it using this parameter to reduce code size
> > ```csharp
> > public void AddScrews(Screw[] screws, float overrideScale = 0f, float overrideSize = 0f)
> > ```

## AddPreSaveAction
> Adds an action that gets executed before the part gets saved to the file.  
> Useful if you need to do checks/cleanup.
> ```csharp
> public void AddPreSaveAction(Action action)
> ```

## AddPreInstallAction
> Gets called **before** the part gets installed.
> ```csharp
> public void AddPreInstallAction(Action action)
> ```

## AddPostInstallAction
> Gets called **after** the part gets installed.
> ```csharp
> public void AddPostInstallAction(Action action)
> ```

## AddPreUninstallAction
> Gets called **before** the part gets uninstalled.
> ```csharp
> public void AddPreUninstallAction(Action action)
> ```

## AddPostUninstallAction
> Gets called **after** the part gets uninstalled.
> ```csharp
> public void AddPostUninstallAction(Action action)
> ```

## AddPreFixedAction
> Gets called **before** the part gets fixed.
> ```csharp
> public void AddPreFixedAction(Action action)
> ```

## AddPostFixedAction
> Gets called **after** the part gets fixed.
> ```csharp
> public void AddPostFixedAction(Action action)
> ```

## AddPreUnfixedActions
> Gets called **before** the part gets unfixed.
> ```csharp
> public void AddPreUnfixedActions(Action action)
> ```

## AddPostUnfixedActions
> Gets called **after** the part gets unfixed.
> ```csharp
> public void AddPostUnfixedActions(Action action)
> ```

## AddWhenInstalledMono
> Adds a ``MonoBehaviour`` (Logic that runs in the background) that only runs when the part is installed.
> ```csharp
> public T AddWhenInstalledMono<T>() where T : MonoBehaviour
> ```

## AddWhenUninstalledMono
> Adds a ``MonoBehaviour`` (Logic that runs in the background) that only runs when the part is not installed.
> ```csharp
> public T AddWhenUninstalledMono<T>() where T : MonoBehaviour
> ```

## AddComponent
> Shortcut for ``part.gameObject.AddComponent<Xyz>()``.
> ```csharp
> public T AddComponent<T>() where T : Component
> ```

## GetComponent
> Shortcut for ``part.gameObject.GetComponent<Xyz>()``.
> ```csharp
> public T GetComponent<T>() => gameObject.GetComponent<T>();
> ```

## SetBought
> Sets the part to be bought.  
> Usually no reason to use this manually
> ```csharp
> public void SetBought(bool bought)
> ```

## IsBought
> Returns if the part is bought.
> ```csharp
> public bool IsBought()
> ```

## SetActive
> Shortcut for ``part.gameObject.SetActive(active)``.
> ```csharp
> public void SetActive(bool active)
> ```

## SetDefaultPosition
> The position considered to be the 'spawnpoint' used in [Part.ResetToDefault()](#resettodefault).
> ```csharp
> public void SetDefaultPosition(Vector3 defaultPosition)
> ```

## SetDefaultRotation
> The rotation considered to be the 'spawn rotation' used in [Part.ResetToDefault()](#resettodefault).
> ```csharp
> public void SetDefaultRotation(Vector3 defaultRotation)
> ```

## ResetToDefault
> Reset's the part to the default position & rotation.
> ```csharp
> public void ResetToDefault(bool uninstall = false)
> ```

## BlockInstall
> Prevents installation of the part.
> ```csharp
> public void BlockInstall(bool block)
> ```

## A
> Returns if the installation of the part is blocked.
> ```csharp
> public bool IsInstallBlocked()
> ```

## HasParent
> Returns if the part has a parent.
> ```csharp
> public bool HasParent()
> ```







