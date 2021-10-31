# Logger

Namespace: ``MscModApi.Tools``

> [!NOTE]
> A simple logging implementation.
> 
> Allows logging messages to a centralized file.
> 
> Log file is located in the MscModApi's ``Mod Settings`` folder.

## New
> Logs a new message
> 
> > ```csharp
> > public static void New(string message)
> > ```
> > ```
> > [31.10.2021 16:40:17] Base message
> > ```
>
> > ```csharp
> > public static void New(string message, string additionalInfo)
> > ```
> > ```
> > [31.10.2021 16:40:17] Base message
> > => Additional infos: Additional info
> > ```
> 
> > ```csharp
> > public static void New(string message, Exception ex)
> > ```
> > ```
> > [31.10.2021 16:40:17] Base message
> > => Exception message: Exception message
> > ```
> 
> > ```csharp
> > public static void New(string message, string additionalInfo, Exception ex)
> > ```
> > ```
> > [31.10.2021 16:40:17] Base message
> > => Additional infos: Additional info
> > => Exception message: Exception message
> > ```