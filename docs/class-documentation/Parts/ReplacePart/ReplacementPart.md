# ReplacementPart

Namespace: ``MscModApi.Parts.ReplacePart``

## CONSTRUCTOR
> Allows to replace a part of the game with an own implementation of the part.
> Works by 'faking' the installation state of the old part.
> > ```csharp
> > public ReplacementPart(OldPart oldPart, NewPart newPart)
> > ```
> > A **single** [NewPart](class-documentation/Parts/ReplacePart/NewPart.md) replaces a **single** [OldPart](class-documentation/Parts/ReplacePart/OldPart.md).
>
> > ```csharp
> > public ReplacementPart(OldPart[] oldParts, NewPart newPart)
> > ```
> > A **single** [NewPart](class-documentation/Parts/ReplacePart/NewPart.md) replace **multiple** [OldPart](class-documentation/Parts/ReplacePart/OldPart.md)'s.
>
> > ```csharp
> > public ReplacementPart(OldPart oldPart, NewPart[] newParts)
> > ```
> > **Multiple** [NewPart](class-documentation/Parts/ReplacePart/NewPart.md)'s replace a **single** [OldPart](class-documentation/Parts/ReplacePart/OldPart.md).
>
> > ```csharp
> > public ReplacementPart(OldPart[] oldParts, NewPart[] newParts)
> > ```
> > **Multiple** [NewPart](class-documentation/Parts/ReplacePart/NewPart.md)'s replace **Multiple** [OldPart](class-documentation/Parts/ReplacePart/OldPart.md)'s.


## AreAllNewInstalled
> Returns if all [NewPart](class-documentation/Parts/ReplacePart/NewPart.md)'s are installed
> ```csharp
> public bool AreAllNewInstalled()
> ```

## AreAllNewUninstalled
> Returns if all [NewPart](class-documentation/Parts/ReplacePart/NewPart.md)'s are uninstalled
> ```csharp
> public bool AreAllNewUninstalled()
> ```

## AreAllNewFixed
> Returns if all [NewPart](class-documentation/Parts/ReplacePart/NewPart.md)'s are fixed
> > | Parameter | Description
> > | --------- | -----------
> > | ignoreUnsetScrew | If the part has not screws it will just return the install state
> > ```csharp
> > public bool AreAllNewFixed(bool ignoreUnsetScrews =  true)
> > ```

## AreAnyNewFixed
> Returns if any [NewPart](class-documentation/Parts/ReplacePart/NewPart.md)'s are fixed
> > | Parameter | Description
> > | --------- | -----------
> > | ignoreUnsetScrews | If the part has not screws it will just return the install state
> > ```csharp
> > public bool AreAnyNewFixed(bool ignoreUnsetScrews = true)
> > ```

## SetFakedInstallStatus
> Set's all [OldPart](class-documentation/Parts/ReplacePart/OldPart.md)'s to be 'fake' installed.  
> There is rarely a reason to do this manually.  
> This is usually handled automatically by the **Replacement part.
> > | Parameter | Description
> > | --------- | -----------
> > | status | Is the part 'fake' installed & bolted
> > ```csharp
> > public void SetFakedInstallStatus(bool status)
> > ```

## AreAnyOldFixed
> Returns if any [OldPart](class-documentation/Parts/ReplacePart/OldPart.md)'s are fixed.
> ```csharp
> public bool AreAnyOldFixed()
> ```

## AreAllOldFixed
> Returns if all [OldPart](class-documentation/Parts/ReplacePart/OldPart.md)'s are fixed.
> ```csharp
> public bool AreAllOldFixed()
> ```

## AreAnyNewInstalled
> Returns if any [NewPart](class-documentation/Parts/ReplacePart/NewPart.md)'s are installed.
> ```csharp
> public bool AreAnyNewInstalled()
> ```

## AreAnyNewUninstalled
> Returns if any [NewPart](class-documentation/Parts/ReplacePart/NewPart.md)'s are uninstalled.
> ```csharp
> public bool AreAnyNewUninstalled()
> ```

## AreAllOldInstalled
> Returns if all [OldPart](class-documentation/Parts/ReplacePart/OldPart.md)'s are installed.
> ```csharp
> public bool AreAllOldInstalled()
> ```

## AreAllOldUninstalled
> Returns if all [OldPart](class-documentation/Parts/ReplacePart/OldPart.md)'s are uninstalled.
> ```csharp
> public bool AreAllOldUninstalled()
> ```

## AreAnyOldInstalled
> Returns if any [OldPart](class-documentation/Parts/ReplacePart/OldPart.md)'s are installed.
> ```csharp
> public bool AreAnyOldInstalled()
> ```

## AreAnyOldUninstalled
> Returns if any [OldPart](class-documentation/Parts/ReplacePart/OldPart.md)'s are uninstalled.
> ```csharp
> public bool AreAnyOldUninstalled()
> ```

## AddAction
> Adds an action to react to changes on the parts
> > | Parameter | Description |
> > | --------- | ----------- |
> > | actionType | The action type to add an action to (See ``ReplacementPart.ActionType``)
> > | partType | The part type to add the action to (See ``ReplacementPart.PartType``)
> > | action | The action to execute when ``actionType`` happens
> > ```csharp
> > public void AddAction(ActionType actionType, PartType partType, Action action)
> > ```