# Screw

Namespace: ``MscModApi.Parts``

## CONSTRUCTOR
> The screw class constructor.
> > | Parameter | Description
> > | --------- | -----------
> > | position | Where on the part the screw is placed
> > | rotation | Rotation of the screw on the part
> > | type | The 3D-Model to use
> > | scale | Screw model scaling
> > | size | The wrench size needed to tighten this screw
> > | allowShowSize | Can the screw size be shown to the player when he has the feature enabled
> > ```csharp
> > public Screw(Vector3 position, Vector3 rotation, Type type = Type.Normal, float scale = 1, float size = 10, bool allowShowSize = true)
> > ```

## In
> Tightens the screw by one 'rotation'
> 
> There should be no real reason to use this
> > | Parameter | Description
> > | --------- | -----------
> > | useAudio | If set to ``false`` won't play a sound
> > ```csharp
> > public void In(bool useAudio = true)
> > ```

## Out
> Loosen the screw by one 'rotation'
>
> There should be no real reason to use this
> > | Parameter | Description
> > | --------- | -----------
> > | useAudio | If set to ``false`` won't play a sound
> > ```csharp
> > public void Out(bool useAudio = true)
> > ```

## InBy
> Tightens a screw by x 'rotation'
> > | Parameter | Description
> > | --------- | -----------
> > | by | By how much to tighten the screw
> > | useAudio | If set to ``false`` won't play a sound
> > | setTightnessToZero | will set the tightness to 0 at the start (rare use cases)
> > ```csharp
> > public void InBy(int by, bool useAudio = false, bool setTightnessToZero = false)
> > ```

## OutBy
> Loosens a screw by x 'rotation'
> > | Parameter | Description
> > | --------- | -----------
> > | by | By how much to loosen the screw
> > | useAudio | If set to ``false`` won't play a sound
> > | setTightnessToZero | will set the tightness to 0 at the start (rare use cases)
> > ```csharp
> > public void OutBy(int by, bool useAudio = false, bool setTightnessToZero = false)
> > ```