### BepInEx Framework + API

This is the pack of all the things you need to both start using mods, and start making mods using the BepInEx framework.

To install, refer to [installation guide on R2Wiki](https://github.com/risk-of-thunder/R2Wiki/wiki/BepInEx).

### What each folder is for:
`BepInEx/plugins` - This is where normal mods/plugins are placed to be loaded.
For developers: There's no set format for what you need to name your plugins to load; if they're a valid plugin .dll file, they'll be loaded.
However please be considerate and isolate your files in their own folders, to prevent clutter, confusion, and in general, dependency hell. For example: `BepInEx/plugins/YourMod/Plugin.dll`.

`BepInEx/patchers` - These are more advanced types of plugins that need to access Mono.Cecil to edit .dll files during runtime. Only copy paste your plugins here if the author tells you to.
For developers: More info here: https://github.com/BepInEx/BepInEx/wiki/Writing-preloader-patchers

`BepInEx/monomod` - MonoMod patches get placed in here. Only copy paste your plugins here if the author tells you to.

`BepInEx/config` - If your plugin has support for configuration, you can find the config file here to edit it.

`BepInEx/core` - Core BepInEx .dll files, you'll usually never want to touch these files (unless you're updating)


### What is included in this pack

**BepInEx 5.3** - https://github.com/BepInEx/BepInEx
This is what loads all of your plugins/mods. It also includes helper libraries like [MonoMod.RuntimeDetour](https://github.com/MonoMod/MonoMod/blob/master/README-RuntimeDetour.md) and [HarmonyX](https://github.com/BepInEx/HarmonyX/wiki) For information on writing mods, refer to

* [R2Wiki](https://github.com/risk-of-thunder/R2Wiki/wiki)
* [BepInEx docs](https://bepinex.github.io/bepinex_docs/v5.3/articles/index.html)

**BepInEx.MonoMod.Loader** - https://github.com/BepInEx/BepInEx.MonoMod.Loader
Loads MonoMod patchers from `BepInEx/monomod`, read the [MonoMod documentation](https://github.com/MonoMod/MonoMod#using-monomod) for more info.

**Customized BepInEx configuration**
BepInEx config customized for use with RoR2