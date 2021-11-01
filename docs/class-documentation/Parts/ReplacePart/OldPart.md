# OldPart

Namespace: ``MscModApi.Parts.ReplacePart``

## CONSTRUCTOR
> The OldPart constructor
> > | Parameter | Description | Example
> > | --------- | ----------- | -------
> > | oldFsmGameObject | The main GameObject that handles the part's logic | Cache.Find("Racing Carburator"))
> > | allowSettingFakedStatus | Will prevent the old part from being set to faked install
> ```csharp
> public OldPart(GameObject oldFsmGameObject, bool allowSettingFakedStatus = true)
> ```

## IsInstallBlocked
> Returns if the installation is blocked.
> ```csharp
> public bool IsInstallBlocked()
> ```

## BlockInstall
> Blocks the installation of the part.
> ```csharp
> public void BlockInstall(bool blocked)
> ```

## IsInstalled
> Returns if the part is currently installed.
> ```csharp
> public bool IsInstalled()
> ```

## IsFixed
> Returns if the part is currently fixed in place (screws tight).
> ```csharp
> public bool IsFixed()
> ```

## Uninstall
> Will uninstall the part if it is installed.
> ```csharp
> public void Uninstall()
> ```