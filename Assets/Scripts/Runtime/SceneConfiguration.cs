using System;
using UnityEngine;

namespace BeardedPlatypus.MultipleSceneManager.Runtime
{
    /// <summary>
    /// <see cref="SceneConfiguration"/> describes a set of strings which should be loaded as a single collection.
    /// </summary>
    [CreateAssetMenu(fileName="MultipleSceneConfiguration",
        menuName="BeardedPlatypus/Scenes/MultipleSceneConfiguration")]
    [Serializable]
    public sealed class SceneConfiguration : ScriptableObject
    {
        /// <summary>
        /// The scenes that should be loaded as a single collection.
        /// </summary>
        public string[] scenes = {};
    }
}
