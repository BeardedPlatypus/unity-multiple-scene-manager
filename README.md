# unity-multiple-scene-manager

unity-multiple-scene-manager provides some utility tooling to handle multiple scenes
within the Unity editor. 

## Usage: Define collections of scenes as SceneConfiguration objects

Multiple scenes can be defined by their names as [`SceneConfiguration`](/Assets/Scripts/Runtime/SceneConfiguration.cs). These objects can be manipulated with a custom editor:

<p align='center'><img align='center' src='https://github.com/BeardedPlatypus/media-storage/blob/main/unity-multiple-scene-manager/scene_configuration.png?raw=true' width='35%'></p>

This editor is defined in [`SceneConfigurationEditor`](/Assets/Scripts/Editor/SceneConfigurationEditor.cs). It provides the functionality to define the different scenes, as well as two utility functions:

- **Load Scenes**: Load the scenes defined into `SceneConfiguration` into the editor.
- **Save Scenes**: Save the scenes currently open in the Unity Editor into the `SceneConfiguration`.

It further provides a warning if the scene is not defined in the Build Settings.

## Installation: UPM branch

The latest version, as well as older versions, are published on the `upm/release` 
branch. These can be added to your Unity project as a package by going to the 
Package Manager window (`Window > Package Manager`), selecting "Add package from git url ..."
and pasting the following url:

```
https://github.com/BeardedPlatypus/unity-multiple-scene-manager.git#upm/release
```

For more information see [the official Unity documentation](https://docs.unity3d.com/Manual/upm-ui-giturl.html).