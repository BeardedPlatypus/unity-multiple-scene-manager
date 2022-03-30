using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeardedPlatypus.MultipleSceneManager.Editor
{
    /// <summary>
    /// <see cref="SceneUtils"/> provides some additional convenience functions for handling scenes within the Unity
    /// Editor.
    /// </summary>
    public static class SceneUtils
    {
        /// <summary>
        /// Gets the scenes currently active in the editor.
        /// </summary>
        public static IEnumerable<Scene> ActiveScenes =>
            Enumerable.Range(0, SceneManager.sceneCount)
                      .Select(SceneManager.GetSceneAt);
        
        /// <summary>
        /// Gets the scene paths currently configured in the Build Settings.
        /// </summary>
        public static IEnumerable<string> BuildScenePaths =>
            Enumerable.Range(0, SceneManager.sceneCountInBuildSettings)
                      .Select(SceneUtility.GetScenePathByBuildIndex);
        
        /// <summary>
        /// Unload any active scenes which are not part of <paramref name="selectedScenes"/>.
        /// </summary>
        /// <param name="selectedScenes">The selected scenes that should not be unloaded</param>
        public static void RemoveUnselectedScenes(string[] selectedScenes)
        {
            IEnumerable<Scene> scenesToRemove = ActiveScenes.Where(s => !selectedScenes.Contains(s.name));
            
            foreach (Scene scene in scenesToRemove)
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }

        /// <summary>
        /// Configure the current active scenes to match <paramref name="selectedScenes"/>.
        /// </summary>
        /// <param name="selectedScenes">The selected scenes which the active scenes should match</param>
        public static void ConfigureSelectedScenes(IEnumerable<string> selectedScenes)
        {
            int i = 0;
            foreach (string scene in selectedScenes)
            {
                if (IsOpenSceneAtIndex(scene, i))
                {
                    i += 1;
                    continue;
                }
                
                if (!HasSceneLoaded(scene))
                {
                    string scenePath = GetScenePathByName(scene);

                    if (scenePath is null)
                    {
                        const string msg = "Cannot find {0} in the BuildSettings. Skipping it.\n" +
                                           "Please specify this scene in the BuildSettings to load it correctly.";
                        Debug.LogWarningFormat(msg, scene);
                        continue;
                    }
                    
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }
                
                EditorSceneManager.MoveSceneAfter(
                    SceneManager.GetSceneByName(scene),
                    SceneManager.GetSceneAt(i - 1));
                i += 1;
            }
        }

        /// <summary>
        /// Whether the scene corresponding with the provided <paramref name="sceneName"/> is loaded in the editor.
        /// </summary>
        /// <param name="sceneName">The name of the scene to check.</param>
        /// <returns>
        /// <c>true</c> if the scene is currently loaded; <c>false</c> otherwise.
        /// </returns>
        public static bool HasSceneLoaded(string sceneName) =>
            ActiveScenes.Any(s => s.name == sceneName);

        /// <summary>
        /// Get the scene name corresponding with the scene path.
        /// </summary>
        /// <param name="path">The path to the scene.</param>
        /// <returns>
        /// The name of the scene corresponding with the <paramref name="path"/>.
        /// </returns>
        public static string PathToName(string path) =>
            Path.GetFileNameWithoutExtension(path);

        /// <summary>
        /// Whether the scene corresponding with <paramref name="name"/> is open at index <paramref name="i"/>.
        /// </summary>
        /// <param name="name">The name of the scene</param>
        /// <param name="i">The index at which it should be open</param>
        /// <returns>
        /// <c>true</c> if the scene with <paramref name="name"/> is open at index <paramref name="i"/>;
        /// <c>false</c> otherwise.
        /// </returns>
        public static bool IsOpenSceneAtIndex(string name, int i) => 
            i < SceneManager.sceneCount && SceneManager.GetSceneAt(i).name == name;
        
        /// <summary>
        /// Get the scene path of the scene with name <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the scene to retrieve.</param>
        /// <returns>
        /// The path to the scene corresponding with <paramref name="name"/>.
        /// </returns>
        /// <remarks>
        /// The scene corresponding with <paramref name="name"/> should be defined in the Build Settings,
        /// otherwise null will be returned.
        /// </remarks>
        [CanBeNull] public static string GetScenePathByName(string name) =>
            BuildScenePaths.FirstOrDefault(n => PathToName(n) == name);
        
        /// <summary>
        /// Check whether the provided <paramref name="sceneName"/> exists in the Build Settings.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        /// <returns>
        /// <c>true</c> if the scene is defined in the Build Settings; <c>false</c> otherwise.
        /// </returns>
        public static bool IsInBuildSettings(string sceneName) =>
            BuildScenePaths.Select(SceneUtils.PathToName).Any(n => n == sceneName);

        /// <summary>
        /// Check whether the specified <paramref name="sceneName"/> is in the project.
        /// </summary>
        /// <param name="sceneName">The name of the scene to check.</param>
        /// <returns>
        /// <c>true</c> if the scene exists in the project; <c>false</c> otherwise.
        /// </returns>
        public static bool IsInProject(string sceneName) =>
            Directory.EnumerateFiles(Directory.GetCurrentDirectory(),
                                     $"{sceneName}.unity",
                                     SearchOption.AllDirectories)
                     .Any();
    }
}