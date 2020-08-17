using Hai.Just1Int7Toggles.Scripts.Components;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Hai.Just1Int7Toggles.Scripts.Editor.EditorUI
{
    [CustomEditor(typeof(J2I7IGroupOfOutfits))]
    public class J2I7IGroupOfOutfitsEditor : UnityEditor.Editor
    {
        private const int IconSize = 80;
        public SerializedProperty name;
        public SerializedProperty animationParameterName;
        public SerializedProperty icon;
        public SerializedProperty menu0to7;
        public SerializedProperty indexDefaultOn;
        public SerializedProperty menu7to15;
        public SerializedProperty outfits;

        public ReorderableList outfitsReorderableList;

        private void OnEnable()
        {
            name = serializedObject.FindProperty("name");
            animationParameterName = serializedObject.FindProperty("animationParameterName");
            icon = serializedObject.FindProperty("icon");
            indexDefaultOn = serializedObject.FindProperty("indexDefaultOn");
            menu0to7 = serializedObject.FindProperty("menu0to7");
            menu7to15 = serializedObject.FindProperty("menu7to15");
            outfits = serializedObject.FindProperty("outfits");

            // reference: https://blog.terresquall.com/2020/03/creating-reorderable-lists-in-the-unity-inspector/
            outfitsReorderableList = new ReorderableList(
                serializedObject,
                serializedObject.FindProperty("outfits"),
                true, true, true, true
            );
            outfitsReorderableList.drawElementCallback = OutfitsListElement;
            outfitsReorderableList.drawHeaderCallback = OutfitsListHeader;
            outfitsReorderableList.onRemoveCallback = list =>
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                if (indexDefaultOn.intValue >= outfits.arraySize)
                {
                    indexDefaultOn.intValue = 0;
                    serializedObject.ApplyModifiedProperties();
                }
            };
            outfitsReorderableList.elementHeight = IconSize;

        }

        private bool _foldoutAdvanced;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(name, new GUIContent("name"));
            EditorGUILayout.PropertyField(animationParameterName, new GUIContent("animationParameterName"));
            EditorGUILayout.PropertyField(icon, new GUIContent("icon"));

            EditorGUILayout.PropertyField(indexDefaultOn, new GUIContent("indexDefaultOn"));

            if (outfits.arraySize > 2)
            {
                EditorGUILayout.PropertyField(menu0to7, new GUIContent("menu0to7"));
            }
            if (outfits.arraySize > 8)
            {
                EditorGUILayout.PropertyField(menu7to15, new GUIContent("menu7to15"));
            }

            EditorGUILayout.Separator();

            LabelDescription();

            EditorGUILayout.Separator();

            outfitsReorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private void LabelDescription()
        {
            if (outfits.arraySize == 0)
            {
                EditorGUILayout.LabelField("Invalid - 0 outfits", EditorStyles.boldLabel);
            }
            else if (outfits.arraySize == 1)
            {
                EditorGUILayout.LabelField("1 bit (Toggle) - 1 to 2 outfits", EditorStyles.boldLabel);
            }
            else if (outfits.arraySize == 2)
            {
                EditorGUILayout.LabelField("1 bit (Swap-Toggle) - 1 to 2 outfits", EditorStyles.boldLabel);
            }
            else if (outfits.arraySize <= 4)
            {
                EditorGUILayout.LabelField("2 bits (Radial Menu) - 3 to 4 outfits", EditorStyles.boldLabel);
            }
            else if (outfits.arraySize <= 8)
            {
                EditorGUILayout.LabelField("3 bits (Radial Menu) - 5 to 8 outfits", EditorStyles.boldLabel);
            }
            else if (outfits.arraySize <= 16)
            {
                EditorGUILayout.LabelField("4 bits (2 Radial Menus) - 9 to 16 outfits", EditorStyles.boldLabel);
            }
            else
            {
                EditorGUILayout.LabelField("Invalid - Mote than 16 outfits", EditorStyles.boldLabel);
            }
        }

        private void OutfitsListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = outfitsReorderableList.serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width - IconSize, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("name"),
                new GUIContent("Name")
            );
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width - IconSize, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("icon"),
                new GUIContent("Icon")
            );
            var icon = element.FindPropertyRelative("icon").objectReferenceValue;
            if (icon != null) {
                EditorGUI.DrawPreviewTexture(
                    new Rect(rect.x + rect.width - IconSize, rect.y, IconSize, IconSize),
                    AssetPreview.GetAssetPreview(icon)
                );
            }
        }

        private static void OutfitsListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Outfits");
        }
    }
}
