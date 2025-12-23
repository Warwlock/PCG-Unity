# PCG-Unity
An open-source similar version of Unreal Engine's Procedural Content Generation (PCG) framework for Unity. A node-based system for generating procedural worlds.

Reminders
------------
This is an expreimental package for my game. I will continue to develop it but don't expect it to be fully complete. You can always create issues if you encounter a bug or you can make a feature request.

Requirements
------------
- Unity 6000.2.12f1 or above. (Probably it supports older versions but I didn't testes, so I limited to Unity 6.)

Installation
------------
### Option 1: Install via UPM
- Go to `Edit --> Project Settings --> Package Manager`
- Under Scoped Registries create a new one.
- Enter this:
   - **Name:** Warwlock
   - **URL:** `https://registry.npmjs.com`
   - **Scope(s):** com.warwlock
- Go to `Window --> Package Manager` and click on `+` icon.
- Click on `Install package by name...` and then enter this: `com.warwlock.pcgunity`.

### Option 2: Install via Package Manager (Git URL)
- Open the **Package Manager** and select **Add package from git URL** from the add menu.
- First enter **`https://github.com/Warwlock/NodeGraphProcessor.git`** to install NodeGraphProcessor package.
- Enter **`https://github.com/Warwlock/PCG-Unity.git`** to install this package.
- If Unity could not find **git**, consider installing it [here](https://git-scm.com/downloads).

### Option 3: Manual Install
- Download the source code as a ZIP file. Also download https://github.com/Warwlock/NodeGraphProcessor as ZIP file
- Extract ZIP files and put them either in `Assets` folder or `Packages` folder.
- Install required Unity Packages from Package Manager: `Mathematics` and `Collections`

References
------------
- https://github.com/krubbles/NoiseDotNet
- https://github.com/keijiro/StickShow
