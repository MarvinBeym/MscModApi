# Cache

Namespace: ``MscModApi.Caching``

?> Caching is the recommended way of finding your GameObjects.<br/>It replaces Unity's ``GameObject.Find()`` implementation<br/>It works by saving a found GameObject in a static dictionary<br/><br/>(search_string => GameObject)<br/>The next time you (or any other mod maker using **MscModApi**) searches for the GameObject.<br/><br/>It get's the desired GameObject from the static dictionary instead of searching it again.

!> Searching for a GameObject is really expensive.  
Which is why caching can really help the game.

## Find
> Replaces ```GameObject.Find()```
> > | Parameter | Description
> > | --------- | -----------
> > | name | The GameObject name to search for (Same as GameObject.Find(name))
> > ```csharp
> > public static GameObject Find(string name)
> > ```

## Clear
> Clears the cached dictionary.  
> There should rarely be a reason to clear the dictionary.  
> Usage of this function is not recommended.
> ```csharp
> public static void Clear()
> ```