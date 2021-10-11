# MscPartApi

| Component     | Latest tested version                                                 | Link                                                              |
|---------------|-----------------------------------------------------------------------|-------------------------------------------------------------------|
|ModLoader      | ![](https://img.shields.io/badge/v1.2-blue.svg)                       | [GitHub](https://github.com/piotrulos/MSCModLoader)               |
|My Summer car  | ![](https://img.shields.io/badge/experimental_11.10.2021-orange.svg)  | [Steam](https://store.steampowered.com/app/516750/My_Summer_Car/) |

---
## Table of contents

- [MscPartApi](#mscpartapi)
    * [Installation](#installation)
        * [User](#user)
        * [Developer/Mod maker](#developermod-maker)
    * [Usage](#usage)

---

## Installation

### User
1. Go to the [GitHub release page](https://github.com/MarvinBeym/MscPartApi/releases)
2. Download the ``.zip`` file you want. (Usually the latest available/at the top)
   - Files are located under the ``Assets`` drop-down.
3. ``Extract`` or ``Open`` the downloaded file using something like [7Zip](https://www.7-zip.de/) or [WinRar](https://winrar.de/index.php).
4. Inside the **Archive** or the **Extracted** folder should be another folder that is named the same as the downloaded ``.zip`` file.
   - Open that folder
5. In there should be multiple files & a folder ``MscPartApi.dll``, ``MscPartApi.xml`` & ``Assets``
   - The ``MscPartApi.xml`` file is not needed for normal usage.
6. Open the folder where you usually install your mods for **My Summer Car**.  
    Examples (depends on what you select in **MSCPatcher**:
    - ``Steam\steamapps\common\My Summer Car\Mods``
    - ``C:\Users\<your windows username>\Documents\My Summer Car\Mods``
    - ``C:\Users\<your windows username>\AppData\LocalLow\Amistech\My Summer Car\Mods``
7. Copy **ALL** three files/folder ``MscPartApi.dll``, ``MscPartApi.xml`` & ``Assets`` into the **\Mods** folder
8. If you get asked if you would like to replace existing files choose ``YES``
9. Open the game and have fun.

### Developer/Mod maker
1. Follow the installation explained in the [User](#user) section above.
   - But also copy the ``MscPartApi.xml`` into your **\Mods** folder. (Optional but recommended)
2. Open your own mod in **Visual Studio** or any other **IDE**.
   - Further steps are explained based on **Visual Studio 2019 Enterprise**
3. Add a new reference in your project
4. Select the ``MscPartApi.dll`` in the **\Mods** folder
5. Continue on the wiki pages that explain how to use it and what can be done & how.  
   [Mod maker wiki](https://github.com/MarvinBeym/MscPartApi/wiki/Development-Mod-making-Using-the-api)
---
