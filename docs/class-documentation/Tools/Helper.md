# Helper

Namespace: ``MscModApi.Tools``

> [!NOTE]
> These are helper functions so that you don't have to implement some of the logic yourself.

> [!WARNING]
> Some of these functions might be untested or might not do exactly what you expect.

## CombinePaths
> Allows combining multiple paths.   
> "Extension" of ``Path.Combine()`` which only allows to combine two paths.
> ```csharp
> public static string CombinePaths(params string[] paths)
> ```

## CombinePathsAndCreateIfNotExists
> Combines multiple paths and creates the directory structure if it doesn't exist.   
> Be careful with this when using/having relative paths,   
> you might/will end up with folders created where you don't want them to be created.
> ```csharp
> public static string CombinePathsAndCreateIfNotExists(params string[] paths)
> ```

## LoadAssetBundle
> An "Extension" of the ModLoader's ``LoadAssets.LoadBundle`` function. 
> If loading fails it will show the reason (Exception message) in the ModConsole   
> and show a dialog asking the user if he wants to quit the game.
> ```csharp
> public static AssetBundle LoadAssetBundle(Mod mod, string fileName)
> ```

## ExitGame
> Short form for ``Application.Quit()``.
> Will close the game.
> ```csharp
> public static void ExitGame()
> ```

## CheckCloseToPosition
> Checks if two **Vector3's** are close to each other.  
> Useful if you want to make parts "interact" with each other   
> like "connect part X to part Y".
> ```csharp
> public static bool CheckCloseToPosition(Vector3 positionToCheck, Vector3 position, float minimumDistance)
> ```

## LoadSaveOrReturnNew
> A better version of ModLoader's ``SaveLoad.DeserializeSaveFile<>()`` function.
> If loading fails for some reason (like save file does not yet exist).  
> It will return a new object of the save class used in the ```<MySaveClass>()```.
> 
> This way you don't end up with a **null** save class.
> ```csharp
> public static T LoadSaveOrReturnNew<T>(Mod mod, string saveFilePath) where T : new()
> ```

## LoadNewSprite
> Loads a sprite either from ``byte[] data`` or a ``filePath``.  
> If loading fails it will return the currently used sprite again.
> ```csharp
> public static Sprite LoadNewSprite(Sprite current, byte[] data, float pivotX = 0.5f, float pivotY = 0.5f, float pixelsPerUnit = 100.0f)
> ```
> ```csharp
> public static Sprite LoadNewSprite(Sprite current, string filePath, float pivotX = 0.5f, float pivotY = 0.5f, float pixelsPerUnit = 100.0f)
> ```

## LoadTexture
> Loads a texture from ``byte[] data``.  
> Usually only used internally by [LoadNewSprite](#loadnewsprite)
> ```csharp
> public static Texture2D LoadTexture(byte[] data)
> ```

## WorkAroundAction
> An empty static action that can be used if needed.  
> Has very limited use cases.
> ```csharp
> public static void WorkAroundAction()
> ```