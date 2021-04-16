# EnigmaticThunder

## IMPORTANT NOTICES

Since R2API is updated, you should porbably move to that instead of using this. 
This mod uses code from R2API, which is licensed under MIT. This mod is also licensed under MIT.

## About

EnigmaticThunder is a new modding framework for other mods to work in, its creation was fueled by the lack of a working standard API for mod developers to use. As such, it provides many features (most from the old R2API). 

At its core, it should be an easy to use API that's easy to transition to from R2API. See the transitioning section for more details.

## Installation

The contents of `EnigmaticThunder` should be extracted into the `BepInEx` folder, such that the files inside in `plugins` in the archive is inside your `plugins` folder.

## Developing

Much of the documentation is in the included *xmldocs*, and further information may be on the dedicated [the modding discord](https://discord.gg/5MbXZvd).


## Transitioning From R2API
Since EnigmaticThunder provides many features that R2API had, it should be easy to transition from R2API. For example, the functionality BuffAPI offered is now provided in a class called Buffs, contained within the EnigmaticThunder.Modules namespace. The functionality that LoadoutAPI offered is now offered within the class Loadouts, contained within the same namespace. For skin mods and survivor mods alike, it should be simple to transition.

## Bleeding Edge

**Unless you are a mod developer, this section will not be helpful.**
Unfortunately, there's no azure pipeline yet. You can get the mod from [here](https://github.com/AwokeinanEnigma/EnigmaticThunder) and be able to build it without hassle.

## Changelog

**0.1.3**

* [Stop ILLine throwing if R2API is loaded](https://github.com/AwokeinanEnigma/EnigmaticThunder/pull/3)
* [Renamed the class EnigmaticThunder to EnigmaticThunderPlugin](https://github.com/AwokeinanEnigma/EnigmaticThunder/commit/2128677167b35f418b25798a37cea34ea0d34cf7#diff-b08f2dfb665be8bbaa2a0f88754b8466535129fe2f28f626232c8141a21ce589)
* [Alter Soundbank loading](https://github.com/AwokeinanEnigma/EnigmaticThunder/commit/c55e9fd2c996360b440b9dd43fbc5e4c2ce44396#diff-b08f2dfb665be8bbaa2a0f88754b8466535129fe2f28f626232c8141a21ce589)
* [Redid Unloackbles](https://github.com/AwokeinanEnigma/EnigmaticThunder/commit/c55e9fd2c996360b440b9dd43fbc5e4c2ce44396#diff-b08f2dfb665be8bbaa2a0f88754b8466535129fe2f28f626232c8141a21ce589)

**0.1.2**

* Removed MMHOOK from the mod. Now depends on MMHOOK_STANDALONE for compat with R2API.

**0.1.1**

* [Added CommandHelper from R2API](https://github.com/AwokeinanEnigma/EnigmaticThunder/commit/8ec3203604520df71ec73fc3decf3c5c2ca1199a)
* [Integrated IL Line for more helpful errors](https://github.com/AwokeinanEnigma/EnigmaticThunder/pull/2)

**0.1.0**

* Release.
