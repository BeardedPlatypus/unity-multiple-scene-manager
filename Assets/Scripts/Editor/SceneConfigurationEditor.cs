using System.IO;
using System.Linq;
using BeardedPlatypus.MultipleSceneManager.Runtime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BeardedPlatypus.MultipleSceneManager.Editor
{
    /// <summary>
    /// <see cref="SceneConfigurationEditor"/> provides the custom inspector for the <see cref="SceneConfiguration"/>.
    /// </summary>
    [CustomEditor(typeof(SceneConfiguration))]
    public class SceneConfigurationEditor : UnityEditor.Editor
    {
        private static readonly float IconDimension = EditorGUIUtility.singleLineHeight - 4.0F;
        private const float IconPadding = 1.0F;
        private const float VerticalSpacing = 1.0F;
        private const float HorizontalSpacing = 4.0F;

        private Texture2D _checkTex;
        private Texture2D _warningTex; 
            
        private ReorderableList _scenes;

        private void OnEnable()
        {
            InitializeTextures();
            InitializeReorderableList();
        }

        private void InitializeReorderableList()
        {
            _scenes = new ReorderableList(serializedObject, 
                                          serializedObject.FindProperty("scenes"), 
                                          true, 
                                          true, 
                                          true, 
                                          true)
            {
                drawElementCallback = DrawSceneElement,
                drawHeaderCallback = DrawHeader,
            };
        }

        private void InitializeTextures()
        {
            var checkPath = GetIconPath("check_green.png");
            _checkTex = (Texture2D) AssetDatabase.LoadAssetAtPath(checkPath, typeof(Texture2D));
            var warningPath = GetIconPath("warning_yellow.png");
            _warningTex = (Texture2D) AssetDatabase.LoadAssetAtPath(warningPath, typeof(Texture2D));
        }

        private static string GetIconPath(string iconName)
        {
            string relativePath = $"Editor/Sprites/{iconName}";
            
            string packagePath = $"Packages/com.beardedplatypus.multiple-scene-manager/{relativePath}";
            if (File.Exists(packagePath)) return packagePath;

            string projectPath = $"Assets/Scripts/{relativePath}";
            if (File.Exists(projectPath)) return projectPath;

            return null;
        }

        public override void OnInspectorGUI()
        {
            DrawCustomSceneList();
            DrawSceneManagementButtons();
        }

        private void DrawCustomSceneList()
        {
            serializedObject.Update();
            _scenes.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSceneManagementButtons()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Active Scenes"))
            {
                SaveActiveScenes();
            }
            
            if (GUILayout.Button("Load Stored Scenes"))
            {
                LoadActiveScenes();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void LoadActiveScenes()
        {
            var configuration = (SceneConfiguration) target;
            SceneUtils.RemoveUnselectedScenes(configuration.scenes);
            SceneUtils.ConfigureSelectedScenes(configuration.scenes);
        }

        private void SaveActiveScenes()
        {
            serializedObject.Update();
            SerializedProperty sceneProp = serializedObject.FindProperty("scenes");
            sceneProp.ClearArray();

            foreach (var scene in SceneUtils.ActiveScenes.Reverse())
            {
                sceneProp.InsertArrayElementAtIndex(0);
                sceneProp.GetArrayElementAtIndex(0).stringValue = scene.name;
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSceneElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += VerticalSpacing;
            
            var sceneElement = _scenes.serializedProperty.GetArrayElementAtIndex(index);
            DrawPropertyField(rect, sceneElement, index);
            DrawIcon(rect, sceneElement);
        }

        private void DrawPropertyField(Rect parentRect, SerializedProperty sceneElement, int index)
        {
            Rect sceneNameFieldRect = new Rect(parentRect.x, 
                                               parentRect.y, 
                                               GetPropertyFieldWidth(parentRect.width), 
                                               EditorGUIUtility.singleLineHeight);
            var label = new GUIContent($"Element {index}");
            EditorGUI.PropertyField(sceneNameFieldRect, sceneElement, label);
        }
        
        private static float GetPropertyFieldWidth(float parentWidth) =>
            parentWidth - IconDimension - 2 * IconPadding - HorizontalSpacing;

        private void DrawIcon(Rect parentRect, SerializedProperty sceneElement)
        {
            // Do not draw an icon if the field is empty.
            if (string.IsNullOrWhiteSpace(sceneElement.stringValue))
            {
                return;
            }

            float iconOffset = GetPropertyFieldWidth(parentRect.width) + HorizontalSpacing;
            Rect iconRect = 
                new Rect(parentRect.x + iconOffset + IconPadding, 
                         parentRect.y + IconPadding, 
                         IconDimension, 
                         IconDimension);

            EditorGUI.LabelField(iconRect,
                ShouldWarn(sceneElement.stringValue) 
                    ? new GUIContent(_warningTex, "Scene not defined in Build Settings.") 
                    : new GUIContent(_checkTex));
        }

        private static void DrawHeader(Rect rect) => 
            EditorGUI.LabelField(rect, "Scenes:");

        private static bool ShouldWarn(string sceneName) =>
            !SceneUtils.IsInBuildSettings(sceneName);
    }
}
