# MscModApi

| Component     | Latest tested version                                                 | Link                                                              |
|---------------|-----------------------------------------------------------------------|-------------------------------------------------------------------|
|ModLoader      | ![](https://img.shields.io/badge/v1.2-blue.svg)                       | [GitHub](https://github.com/piotrulos/MSCModLoader)               |
|My Summer car  | ![](https://img.shields.io/badge/experimental_12.10.2021-orange.svg)  | [Steam](https://store.steampowered.com/app/516750/My_Summer_Car/) |

---
## Table of contents

- [MscModApi](#mscmodapi)
    * [Features](#features)
    * [Installation](#installation)
        * [User](#user)
        * [Developer/Mod maker](#developermod-maker)
    * [Usage](#usage)

---

### Features

- [X] Creating parts
  - [X] Installable
  - [X] Screws 
- [X] Replacing original parts with own implementation
- [X] UserInteraction assistant/helper
- [X] Cached GameObject.Find()
- [X] A lot of helpers & extensions to make development easier and reduce amount of code 
- [X] Logger - Central point for logging stuff about your mod
- [ ] Shop
- [ ] Ability to pack multiple parts (same objects) into a **box** that you can **unpack**
- [ ] Ability to pack multiple parts (different objects) into a **kit** that you can **unpack** 

## Installation

### User
1. Go to the [GitHub release page](https://github.com/MarvinBeym/MscModApi/releases)
2. Download the ``.zip`` file you want. (Usually the latest available/at the top)
   - Files are located under the ``Assets`` drop-down.
3. ``Extract`` or ``Open`` the downloaded file using something like [7Zip](https://www.7-zip.de/) or [WinRar](https://winrar.de/index.php).
4. Inside the **Archive** or the **Extracted** folder should be another folder that is named the same as the downloaded ``.zip`` file.
   - Open that folder
5. In there should be multiple files & a folder ``MscModApi.dll``, ``MscModApi.xml`` & ``Assets``
   - The ``MscModApi.xml`` file is not needed for normal usage.
6. Open the folder where you usually install your mods for **My Summer Car**.  
    Examples (depends on what you select in **MSCPatcher**:
    - ``Steam\steamapps\common\My Summer Car\Mods``
    - ``C:\Users\<your windows username>\Documents\My Summer Car\Mods``
    - ``C:\Users\<your windows username>\AppData\LocalLow\Amistech\My Summer Car\Mods``
7. Copy **ALL** three files/folder ``MscModApi.dll``, ``MscModApi.xml`` & ``Assets`` into the **\Mods** folder
8. If you get asked if you would like to replace existing files choose ``YES``
9. Open the game and have fun.

### Developer/Mod maker
1. Follow the installation explained in the [User](#user) section above.
   - But also copy the ``MscModApi.xml`` into your **\Mods** folder. (Optional but recommended)
2. Open your own mod in **Visual Studio** or any other **IDE**.
   - Further steps are explained based on **Visual Studio 2019 Enterprise**
3. Add a new reference in your project
4. Select the ``MscModApi.dll`` in the **\Mods** folder
5. Continue on the documentation that explain how to use it and what can be done & how.  
   [API Documentation](https://marvinbeym.github.io/MscModApi/)
---
