using BeardedPlatypus.MultipleSceneManager.Runtime;
using UnityEditor;
using UnityEditorInternal;

namespace BeardedPlatypus.MultipleSceneManager.Editor
{
    [CustomEditor(typeof(SceneConfiguration))]
    public class SceneConfigurationEditor : UnityEditor.Editor
    {
        private ReorderableList _scenes;

        private void OnEnable()
        {
            _scenes = new ReorderableList(serializedObject, 
                                          serializedObject.FindProperty("scenes"), 
                                          true, 
                                          true, 
                                          true, 
                                          true);
        }


        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawCustomSceneList();
        }

        private void DrawCustomSceneList()
        {
            serializedObject.Update();
            _scenes.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
