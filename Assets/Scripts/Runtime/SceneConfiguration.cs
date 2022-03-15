using System;
using UnityEngine;

namespace BeardedPlatypus.MultipleSceneManager.Runtime
{
    [CreateAssetMenu(fileName="MultipleSceneConfiguration",
        menuName="BeardedPlatypus/Scenes/MultipleSceneConfiguration")]
    [Serializable]
    public sealed class SceneConfiguration : ScriptableObject
    {
        public string[] scenes = {};
    }
}
