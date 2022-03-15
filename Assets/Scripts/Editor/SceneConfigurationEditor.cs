using System.Linq;
using BeardedPlatypus.MultipleSceneManager.Runtime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BeardedPlatypus.MultipleSceneManager.Editor
{
    [CustomEditor(typeof(SceneConfiguration))]
    public class SceneConfigurationEditor : UnityEditor.Editor
    {
        private const string CheckPath = "Assets/Sprites/check_green.png";
        private const string WarningPath = "Assets/Sprites/warning_yellow.png";
        private const string ErrorPath = "Assets/Sprites/warning_red.png";

        private Texture2D _checkTex;
        private Texture2D _warningTex; 
        private Texture2D _errorTex;
            
        private ReorderableList _scenes;

        private void OnEnable()
        {
            Texture2D t = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Textures/texture.jpg", typeof(Texture2D));
            
            _scenes = new ReorderableList(serializedObject, 
                                          serializedObject.FindProperty("scenes"), 
                                          true, 
                                          true, 
                                          true, 
                                          true)
            {
                drawElementCallback = DrawSceneElement
            };

            _checkTex = (Texture2D) AssetDatabase.LoadAssetAtPath(CheckPath, typeof(Texture2D));
            _warningTex = (Texture2D) AssetDatabase.LoadAssetAtPath(WarningPath, typeof(Texture2D));
            _errorTex = (Texture2D) AssetDatabase.LoadAssetAtPath(ErrorPath, typeof(Texture2D));
        }


        public override void OnInspectorGUI()
        {
            DrawCustomSceneList();
        }

        private void DrawCustomSceneList()
        {
            serializedObject.Update();
            _scenes.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSceneElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 2;
            
            var sceneElement = _scenes.serializedProperty.GetArrayElementAtIndex(index);

            Rect sceneNameFieldRect = 
                new Rect(rect.x, 
                         rect.y, 
                         rect.width - EditorGUIUtility.singleLineHeight - 9, 
                         EditorGUIUtility.singleLineHeight);
            var label = new GUIContent($"Element {index}");
            EditorGUI.PropertyField(sceneNameFieldRect, sceneElement, label);

            if (string.IsNullOrWhiteSpace(sceneElement.stringValue))
            {
                return;
            }
            
            Rect iconRect = 
                new Rect(rect.x + (rect.width - EditorGUIUtility.singleLineHeight - 5) + 2, 
                         rect.y + 2, 
                         EditorGUIUtility.singleLineHeight + 1, 
                         EditorGUIUtility.singleLineHeight - 4);

            EditorGUI.LabelField(iconRect,
                ShouldWarn(sceneElement.stringValue) 
                    ? new GUIContent(_warningTex, "Scene not defined in Build Settings.") 
                    : new GUIContent(_checkTex));
        }

        private static bool ShouldWarn(string sceneName) =>
            !SceneUtils.IsInBuildSettings(sceneName);

    }
}
