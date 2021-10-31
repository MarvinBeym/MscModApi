# ReplacementPart

Namespace: ``MscModApi.Parts.ReplacePart``

## CONSTRUCTOR
> Allows to replace a part of the game with an own implementation of the part.
> Works by 'faking' the installation state of the old part.
> > ```csharp
> > public ReplacementPart(OldPart oldPart, NewPart newPart)
> > ```
> > A **single** [NewPart](NewPart.md) replaces a **single** [OldPart](OldPart.md).
>
> > ```csharp
> > public ReplacementPart(OldPart[] oldParts, NewPart newPart)
> > ```
> > A **single** [NewPart](NewPart.md) replace **multiple** [OldPart](OldPart.md)'s.
>
> > ```csharp
> > public ReplacementPart(OldPart oldPart, NewPart[] newParts)
> > ```
> > **Multiple** [NewPart](NewPart.md)'s replace a **single** [OldPart](OldPart.md).
>
> > ```csharp
> > public ReplacementPart(OldPart[] oldParts, NewPart[] newParts)
> > ```
> > **Multiple** [NewPart](NewPart.md)'s replace **Multiple** [OldPart](OldPart.md)'s.


## AreAllNewInstalled
> Returns if all [NewPart](NewPart.md)'s are installed
> ```csharp
> public bool AreAllNewInstalled()
> ```

## AreAllNewUninstalled
> Returns if all [NewPart](NewPart.md)'s are uninstalled
> ```csharp
> public bool AreAllNewUninstalled()
> ```

## AreAllNewFixed
> Returns if all [NewPart](NewPart.md)'s are fixed
> ```csharp
> public bool AreAllNewFixed(bool ignoreUnsetScrews =  true)
> ```

## AreAnyNewFixed
> Returns if any [NewPart](NewPart.md)'s are fixed
> ```csharp
> public bool AreAnyNewFixed(bool ignoreUnsetScrews = true)
> ```

## SetFakedInstallStatus
> Set's all [OldPart](OldPart.md)'s to be 'fake' installed.  
> There is rarely a reason to do this manually.  
> This is usually handled automatically by the **Replacement part.
> ```csharp
> public void SetFakedInstallStatus(bool status)
> ```

## AreAnyOldFixed
> Returns if any [OldPart](OldPart.md)'s are fixed.
> ```csharp
> public bool AreAnyOldFixed()
> ```

## AreAllOldFixed
> Returns if all [OldPart](OldPart.md)'s are fixed.
> ```csharp
> public bool AreAllOldFixed()
> ```

## AreAnyNewInstalled
> Returns if any [NewPart](NewPart.md)'s are installed.
> ```csharp
> public bool AreAnyNewInstalled()
> ```

## AreAnyNewUninstalled
> Returns if any [NewPart](NewPart.md)'s are uninstalled.
> ```csharp
> public bool AreAnyNewUninstalled()
> ```

## AreAllOldInstalled
> Returns if all [OldPart](OldPart.md)'s are installed.
> ```csharp
> public bool AreAllOldInstalled()
> ```

## AreAllOldUninstalled
> Returns if all [OldPart](OldPart.md)'s are uninstalled.
> ```csharp
> public bool AreAllOldUninstalled()
> ```

## AreAnyOldInstalled
> Returns if any [OldPart](OldPart.md)'s are installed.
> ```csharp
> public bool AreAnyOldInstalled()
> ```

## AreAnyOldUninstalled
> Returns if any [OldPart](OldPart.md)'s are uninstalled.
> ```csharp
> public bool AreAnyOldUninstalled()
> ```

## AddAction
> Adds an action to react to changes on the parts
> ```csharp
> public void AddAction(ActionType actionType, PartType partType, Action action)
> ```