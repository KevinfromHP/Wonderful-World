## 3.1.1

This update implements support for .NET 3.5 and includes a number of general improvements and fixes

## New Features

### Optimization

Special thanks to Therzok for doing an optimization pass to clean up a number of cases where the code could be more efficient and cleaner.

### Package Manager
* Now updates when package sources are refreshed.
* Now refreshes package sources when opened.
* Now has a Refresh button next to the Filters button which will refresh all available PackageSources
* PackageSources now invoke the SourcesInitialized event when a source has been updated
* PackageSources can register an event handler on the InitializeSources event to be informed when it should update
* Thunderstore API no longer automatically updates on a timer

### 3.5 Migration changes

* Added csc.rsp and mcs.rsp to AssemblyDefinition containing folders to ensure that the correct language version is used for ThunderKit regardless of Scripting Back End choice.
* Removed Async/Await as its not available in 3.5
  * Some cases were replaced with other asynchronous mechanisms.
  * More cases will be moved to asynchronous mechanisms in the future, however there are currently a few that were migrated to synchronous execution.
* Migrate to Directory.GetFiles and Directory.GetDirectories over Directory.EnumerateFiles and Directory.EnumerateDirectories due to lack of support in .NET 3.5


### Zip Changes

* Migrated to SharpCompress from System.IO.Compression
* Updated zip handling in ThunderstoreSource and LocalThunderstoreSource
* Updated Zip PipelineJob to use SharpCompress

### Markdown / Documentation changes
* Improved method of locating Documentation assets
* Significant improvements made to the UIElementRenderer allocations
* Improvements to Regex Usage

### File System changes
* Migrated many file system management facilities to use FileUtil instead of System.IO types
* Updated Copy PipelineJob to use FileUtil  This changes how Copy works, A recursive copy will Replace the designated destination directory, not fill its contents
* Stage Manifest Files now uses FileUtil to deploy files

### BepInEx Template Changes

The BepInEx template has been updated to use some new features that arose out of the .NET 3.5 changes.
First, the template is somewhat large so it has been broken up into 4 Pipelines
1. Stage:  This pipeline executes StageAssetBundles, StageAssemblies, StageDependencies, StageManifestFiles and finally StageThunderstoreManifest.
2. Deploy: Conducts the following Copy jobs
    1. Copy BepInEx to ProjectRoot/ThunderKit/BepInExPack
    2. Copy plugins to ProjectRoot/ThunderKit/BepInExPack/BepInEx/plugins
    3. Copy patchers to ProjectRoot/ThunderKit/BepInExPack/BepInEx/patchers
    4. Copy monomods to ProjectRoot/ThunderKit/BepInExPack/BepInEx/monomod
    5. Copy winhttp.dll to the Games root directory
    6. Copy a BepInEx config targeted by the PathReference named BepInExConfig if it is defined in the project to ProjectRoot/ThunderKit/BepInExPack/BepInEx/config
3. Launch:  Executes the games executable with the necessary command line parameters to load BepInEx
4. Rebuild and Launch:  Executes the 3 prior Pipelines in order.

To get started on a new mod project you only need to copy the Rebuild and Launch pipeline into your Assets folder and then populate the Manifest field.

## 3.0.0 

#### Initial Setup
A Welcome window has been added to ThunderKit to help users set up their project.
This window can be disabled by a toggle it provides.

#### ThunderKit Installer - Removed
The ThunderKit installer has been removed.  The installer caused many development issues and lost
work during the development of ThunderKit. While this issue may not have affected end users, the 
risk associated with the cost of lost work makes this feature dangerous to continue to maintain.

Unity 2018.1-2019.2 users will need to add the Thunderkit dependency to their projects Packages/manifest.json

For Unity 2019.3+ users can add ThunderKit using the Git url and use the [Install from Git](https://docs.unity3d.com/2019.3/Documentation/Manual/upm-ui-giturl.html) option in the Unity Package Manager.

#### ThunderKit Settings

ThunderKit Settings now get a dedicated window from ThunderKit and can be accessed from the main menu under [Tools/ThunderKit/Settings](menulink://Tools/ThunderKit/Settings).
These settings will no longer show up in the Project Settings window.

#### Debugging Features

ComposableObjects now support some debugging features to provide an easy access interface to implementations of Composable Object to report errors in the UI.

ComposableElements now have 2 members, IsErrored and ErrorMessage. The ComposableObjectEditor will change the header color of ComposableElements to red if IsErrored is true, signalling where a problem may be.

Implementations of ComposableObject are responsible for setting the values in IsErrored and ErrorMessage.  

For examples see [Pipeline](assetlink://Packages/com.passivepicasso.thunderkit/Editor/Core/Pipelines/Pipeline.cs) 
and [PathReference](assetlink://Packages/com.passivepicasso.thunderkit/Editor/Core/Paths/PathReference.cs)

If a pipeline encounters a problem it will halt its execution and highlight the step that it faulted on.
The PipelineJobs and Pipeline itself are setup to log exceptions to the Unity console like normal, with these two pieces of information you should be able to quickly identify and rectify problems.

ThunderKit Manifests do not utilize these debugging features as they are only Data Containers, however if worth while usage for debugging issues can be identified then support will be added.

#### Dependencies

Dependency Management in the 3.0.0 update has changed significantly.  Instead of Manifests installing and managing dependencies in its entirety, Manifests will now only 
be responsible for containing dependency references.  

Instead a user will now install packages via the [Package Manager](menulink://Tools/ThunderKit/Packages), and then 
add the Manifest from the Packages folder to the Manifest that requires the dependency

#### Package Manager

ThunderKit now includes a complete Package Manager, available from the main menu under [Tools/ThunderKit/Packages](menulink://Tools/ThunderKit/Packages)

The ThunderKit Package Manager is how you will add and remove all mod dependencies for your project.
If a mod in your project needs to depend on a Mod, Loader, or Library, you have the ability to install these dependencies through the Package Manager.

Currently the Package Manager comes with support for Thunderstore by default, select your Thunderstore community by setting the url from the [ThunderKit Settings](menulink://Tools/ThunderKit/Settings).

You can also create a Local Thunderstore source where you can specify a folder to examine for zip files.
Zip files in Local Thunderstore Sources must conform to Thunderstore's file naming schemes in order to be resolved correctly. 

This scheme is: `Author-ModName-Version.zip`

#### Documentation
Documentation is a major issue for new users and as such ThunderKit now comes with integrated documentation to help onboard new users.
The documentation available from the main menu under Tools/ThunderKit/Documentation

Documentation is a work in progress and improvements will be made as a better understanding is gained about users needs for information.

## 2.2.1
  * Fix issues with assembly identitification

## 2.1.3
  * New Features
    * Establish base for documentation system
    * Establish Package management as a core system
    * Add support to drag and drop Thunderstore package zip files into ThunderstoreManifest dependencies
    * Components of ComposableObjects now provide Copy, Paste, and Duplicate from their menus
    * Ensure a Scripting Define is always added for packages installed by ThunderKit, Define will be the name of the package
  * Improvements
    * Clean and organize systems for managing the loading process of ThunderKit
    * Improve the design of the ThunderKit Installer package to support more versions
    * Use built in Asset Package (unityPackage) options
    * ComposableObject now has an array of ComposableElements instead of ScrtipableObjects
  * Fixes
    * Sort add component options
    * Fix cases where directories are not created when needed
    * Fix some problems with the Thunderstore - BepInEx templates

## 2.1.0 - 2.1.2
  * Fix issues with automatic installer
  * Fix issues with package management

## 2.0.0 - First Major Version update
  * Replace Deployments with new system.
    * Manifest's will now hold all references to files that need to be included or processed for deploying a mod
    * Deployment operations will now be handled by Pipeline's and Pipeline Jobs.
    * Pipelines are containers for pipeline jobs, pipelines with special requirements can be made by creating derivatives of Pipeline.

## Early Versions
* 1.x.x - untracked iterative updates to ThunderKits feature set
* 1.0.0 - Initial Relesae of Thunderkit
