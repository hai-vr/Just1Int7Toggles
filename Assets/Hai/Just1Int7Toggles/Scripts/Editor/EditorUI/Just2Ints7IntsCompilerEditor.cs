using System.Linq;
using Hai.Just1Int7Toggles.Scripts.Components;
using Hai.Just1Int7Toggles.Scripts.Editor.Internal;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Hai.Just1Int7Toggles.Scripts.Editor.EditorUI
{
    [CustomEditor(typeof(Just2Ints7IntsCompiler))]
    public class Just2Ints7IntsCompilerEditor : UnityEditor.Editor
    {
        private const int ListHeight = 80;
        public SerializedProperty groupOfOutfits;

        public ReorderableList groupOfOutfitsReorderableList;

        private void OnEnable()
        {
            groupOfOutfits = serializedObject.FindProperty("groupOfOutfits");

            // reference: https://blog.terresquall.com/2020/03/creating-reorderable-lists-in-the-unity-inspector/
            groupOfOutfitsReorderableList = new ReorderableList(
                serializedObject,
                serializedObject.FindProperty("groupOfOutfits"),
                true, true, true, true
            );
            groupOfOutfitsReorderableList.drawElementCallback = GroupOfOutfitsListElement;
            groupOfOutfitsReorderableList.drawHeaderCallback = OutfitsListHeader;
            groupOfOutfitsReorderableList.elementHeight = ListHeight;
        }

        private bool _foldoutAdvanced;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            groupOfOutfitsReorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private void GroupOfOutfitsListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = groupOfOutfitsReorderableList.serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width - ListHeight, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("value"),
                new GUIContent("Group of Outfits")
            );
            GUI.Label(
                new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width - ListHeight, EditorGUIUtility.singleLineHeight),
                "Bits in use: LOL"
            );

            var innerGroup = (J2I7IGroupOfOutfits)element.FindPropertyRelative("value").objectReferenceValue;
            if (innerGroup != null) {
                var icon = innerGroup.icon;
                if (icon != null) {
                    EditorGUI.DrawPreviewTexture(
                        new Rect(rect.x + rect.width - ListHeight, rect.y, ListHeight, ListHeight),
                        AssetPreview.GetAssetPreview(icon)
                    );
                }
            }
        }

        private static void OutfitsListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Outfits");
        }
    }
}
