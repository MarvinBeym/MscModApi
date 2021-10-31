# NewPart

Namespace: ``MscModApi.Parts.ReplacePart``

## CONSTRUCTOR
> The NewPart constructor  
> canBeInstalledWithoutReplacing: Can the part be installed  
> without deactivating the installation of an [OldPart](OldPart.md).
> ```csharp
> public NewPart(Part part, bool canBeInstalledWithoutReplacing = false)
> ```

## IsInstalled
> Shortcut for [Part.IsInstalled()](../Part.md#isinstalled).
> ```csharp
> public bool IsInstalled()
> ```

## IsFixed
> Shortcut for [Part.IsFixed()](../Part.md#isfixed).
> ```csharp
> public bool IsFixed(bool ignoreUnsetScrews = true)
> ```

## BlockInstall
> Shortcut for [Part.BlockInstall()](../Part.md#isfixed).  
> Also accounts for ``canBeInstalledWithoutReplacing``.
> ```csharp
> public void BlockInstall(bool block)
> ```

## CanBeInstalledWithoutReplacing
> Returns the value for ``canBeInstalledWithoutReplacing``
> ```csharp
> public bool CanBeInstalledWithoutReplacing()
> ```