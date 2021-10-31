# UserInteraction

Namespace: ``MscModApi.Tools``

> [!NOTE]
> Allows interacting with the user/player (like showing messages)

## GuiInteraction
> Show text and/or icons to user.
> ```csharp
> public static void GuiInteraction(string text = "")
> ```
> ```csharp
> public static void GuiInteraction(Type type)
> ```
> ```csharp
> public static void GuiInteraction(Type type, string text)
> ```

## PlayAssemble
> Plays the **assemble** sound on a GameObject.
> ```csharp
> public static void PlayAssemble(this GameObject gameObject)
> ```

## PlayDisassemble
> Plays the **disassemble** sound on a GameObject.
> ```csharp
> public static void PlayDisassemble(this GameObject gameObject)
> ```

## PlayTouch
> Plays the **touch** sound on a GameObject.
> ```csharp
> public static void PlayTouch(this GameObject gameObject)
> ```

## PlayBuy
> Plays the **buy** sound on a GameObject.
> ```csharp
> public static void PlayBuy(this GameObject gameObject)
> ```

## PlayCheckout
> Plays the **checkout** sound on a GameObject.
> ```csharp
> public static void PlayCheckout(this GameObject gameObject)
> ```

## IsLookingAt
> Returns true if the user is looking at the GameObject this function is called on.
> ```csharp
> public static bool IsLookingAt(this GameObject gameObject)
> ```

## IsHolding
> Returns true if the user is holding the GameObject this function is called on.
> ```csharp
> public static bool IsHolding(this GameObject gameObject)
> ```

## EmptyHand
> Returns true if the user is not holding any GameObject.
> ```csharp
> public static bool EmptyHand()
> ```

## LeftMouseDown
> Returns if the user pressed the left mouse button.
> ```csharp
> public static bool LeftMouseDown => Input.GetMouseButtonDown(0);
> ```

## LeftMouseDownContinuous
> Returns if the user is pressing down the left mouse button.
> ```csharp
> public static bool LeftMouseDownContinuous => Input.GetMouseButton(0);
> ```

## RightMouseDown
> Returns if the user pressed the right mouse button.
> ```csharp
> public static bool RightMouseDown => Input.GetMouseButtonDown(1);
> ```

## UseButtonDown
> Returns if the user pressed the **USE** button.
> ```csharp
> public static bool UseButtonDown => cInput.GetKeyDown("Use");
> ```

## MouseScrollWheel.Up
> Returns if the user is scrolling up.
> ```csharp
> public static bool Up => Input.GetAxis("Mouse ScrollWheel") > 0f;
> ```

## MouseScrollWheel.Down
> Returns if the user is scrolling down.
> ```csharp
> public static bool Down => Input.GetAxis("Mouse ScrollWheel") < 0f;
> ```