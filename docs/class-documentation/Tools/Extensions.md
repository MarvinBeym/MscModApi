# Extensions

Namespace: ``MscModApi.Tools``

> [!NOTE]
> **Extensions** are functions that get called on a specific **type** like int, bool, string, or any type of object.
> 
> This makes it possible to extend "classes" (or other types) with additional functions/features without having to modify the base class.
> 
> The first parameter in an extension functions defines on which **type** this function can be called using the **this** keyword:
> ```csharp
>                               ⇩⇩   ⇩⇩
>                               ⇩⇩   ⇩⇩
>                               ⇩⇩   ⇩⇩
> public static string ToOnOff(this bool value)
> ```
> 
> Example usage (using ToOnOff):
> ```csharp
> bool myBooleanValue = false;
> string stringRepresentation = myBooleanValue.ToOnOff();
> //stringRepresentation = "Off"
> ```

## CompareQuaternion
> Compares two Quaternions (rotation)
> ```csharp
> public static bool CompareQuaternion(this Quaternion a, Quaternion b, float tolerance = 0).
> ```

## ToOnOff
> Short form of [ToXY](#toxy) using **On** & **Off**
> 
> ```csharp
> public static string ToOnOff(this bool value)
> ```

## ToXY
> Converts a **boolean** value to a **string** value using the passed **trueText** & **falseText**.
> ```csharp
> public static string ToXY(this bool value, string trueText, string falseText)
> ```

## SetNameLayerTag
> Allows setting GameObject's **Name**, tag & layer in one call.  
> Also fixes naming using the [FixName](#fixname) function.
> ```csharp
> public static void SetNameLayerTag(this GameObject gameObject, string name, string tag = "PART", string layer = "Parts")
> ```

## FixName
> Applies name fixing to a GameObject's **name**.   
> Like removing duplicate **(Clone)**.
> ```csharp
> public static void FixName(this GameObject gameObject)
> ```

## CompareVector3
> Compares two **Vector3+* (position/location).
> ```csharp
> public static bool CompareVector3(this Vector3 vector3, Vector3 other, float tolerance = 0.05f)
> ```

## FindFsm
> Tries to find a PlayMaker FSM on a GameObject using its name.  
> Returns null if not found.
> ```csharp
> public static PlayMakerFSM FindFsm(this GameObject gameObject, string fsmName)
> ```

## CopyVector3
> Creates a new **Vector3** from another one
> ```csharp
> public static Vector3 CopyVector3(this Vector3 old)
> ```

## FindChild
> Short form of ```gameObject.transform.FindChild(childName)?.gameObject;```
> ```csharp
> public static GameObject FindChild(this GameObject gameObject, string childName)
> ```

## InvokeAll
> Invokes all **Actions** in a list.
> ```csharp
> public static void InvokeAll(this List<Action> actions)
> ```

## CloneToNew
> Clones a **Screw** by creating a new object from the passed screw's data.  
> With normal "basic" usage, this function should not be needed.
> ```csharp
> public static Screw CloneToNew(this Screw screw)
> ```
> ```csharp
> public static Screw[] CloneToNew(this Screw[] screws)
> ```