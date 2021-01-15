using Hai.Just1Int7Toggles.Scripts.Components;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Hai.Just1Int7Toggles.Scripts.Editor.EditorUI
{
    [CustomEditor(typeof(JustHaiToggleGroup))]
    public class JustHaiToggleGroupEditor : UnityEditor.Editor
    {
        public ReorderableList togglablesReorderableList;
        public SerializedProperty parameterName;
        public SerializedProperty hintEnabled;

        private void OnEnable()
        {
            parameterName = serializedObject.FindProperty("parameterName");
            hintEnabled = serializedObject.FindProperty("hintEnabled");

            // reference: https://blog.terresquall.com/2020/03/creating-reorderable-lists-in-the-unity-inspector/
            var reorderableList = new ReorderableList(
                serializedObject,
                serializedObject.FindProperty("togglables"),
                true, true, true, true
            );
            reorderableList.drawElementCallback = (rect, oIndex, active, focused) => TogglablesListElement(rect, oIndex, active, focused);
            reorderableList.drawHeaderCallback = rect => TogglablesListHeader(rect);
            togglablesReorderableList = reorderableList;
        }

        private bool _foldoutAdvanced;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(parameterName, new GUIContent("Parameter Name"));
            if (parameterName.stringValue.Length == 0)
            {
                EditorGUILayout.HelpBox("Set a parameter name for use by the Expressions Menu.", MessageType.Error);
            }

            EditorGUILayout.PropertyField(hintEnabled, new GUIContent("Default state"));
            togglablesReorderableList.DoLayoutList();
            EditorGUILayout.Separator();

            serializedObject.ApplyModifiedProperties();
        }

        private void TogglablesListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = togglablesReorderableList.serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width - 100, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("item"),
                GUIContent.none
            );

            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - 100, rect.y, 80, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("initialState"),
                GUIContent.none
            );
        }

        private static void TogglablesListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Togglables");
        }
    }
}
