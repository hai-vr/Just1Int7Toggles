using System.Collections.Generic;
using Hai.Just1Int7Toggles.Scripts.Components;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditor.EditorGUIUtility;

namespace Hai.Just1Int7Toggles.Scripts.Editor.EditorUI
{
    [CustomEditor(typeof(Just2Ints7IntsCompiler))]
    public class Just2Ints7IntsCompilerEditor : UnityEditor.Editor
    {
        private const int IconSize = 80;
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
            groupOfOutfitsReorderableList.onAddCallback = list =>
            {
                ReorderableList.defaultBehaviours.DoAddButton(list);
                int nextAnchor;
                if (groupOfOutfits.arraySize > 1)
                {
                    var elt = groupOfOutfits.GetArrayElementAtIndex(groupOfOutfits.arraySize - 2);
                    var bitCount = ((J2I7IGroupOfOutfits) elt.FindPropertyRelative("value").objectReferenceValue).BitCount();
                    nextAnchor = elt.FindPropertyRelative("anchorValue").intValue + (bitCount == null ? 0 : (int)bitCount);
                }
                else
                {
                    nextAnchor = 64;
                }

                groupOfOutfits.GetArrayElementAtIndex(groupOfOutfits.arraySize - 1).FindPropertyRelative("anchorValue")
                    .intValue = nextAnchor;
                serializedObject.ApplyModifiedProperties();

            };
            groupOfOutfitsReorderableList.elementHeight = IconSize + 80;
        }

        private bool _foldoutAdvanced;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var compiler = (Just2Ints7IntsCompiler) target;
            EditorGUILayout.LabelField("Bit occupation", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Main layer: " + compiler.CountBitOccupationOf(OutfitLayer.MainLayer) + " / 7");
            EditorGUILayout.LabelField("Secondary layer B: " + compiler.CountBitOccupationOf(OutfitLayer.SecondaryLayerB) + " / 8");
            EditorGUILayout.Separator();

            groupOfOutfitsReorderableList.DoLayoutList();

            EditorGUILayout.LabelField("Anchors", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Anchors are overlapping", MessageType.Error);
            if (GUILayout.Button("Auto-solve anchors"))
            {
                DoAutoSolveAnchors();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DoAutoSolveAnchors()
        {
            throw new System.NotImplementedException();
        }

        private void GroupOfOutfitsListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = groupOfOutfitsReorderableList.serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + singleLineHeight * 1, rect.width - IconSize, singleLineHeight),
                element.FindPropertyRelative("value"),
                new GUIContent("Group of Outfits")
            );
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + singleLineHeight * 2, rect.width - IconSize, singleLineHeight),
                element.FindPropertyRelative("layer"),
                new GUIContent("Layer")
            );
            var anchorIsLocked = element.FindPropertyRelative("anchorLocked").boolValue;
            EditorGUI.BeginDisabledGroup(anchorIsLocked);
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + singleLineHeight * 3, rect.width - IconSize, singleLineHeight),
                element.FindPropertyRelative("anchorValue"),
                new GUIContent("Anchor value " + (anchorIsLocked ? " (Locked)" : ""))
            );
            EditorGUI.EndDisabledGroup();
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + singleLineHeight * 4, rect.width - IconSize, singleLineHeight),
                element.FindPropertyRelative("anchorLocked"),
                new GUIContent("Anchor lock")
            );

            var innerGroup = (J2I7IGroupOfOutfits)element.FindPropertyRelative("value").objectReferenceValue;
            if (innerGroup != null) {
                GUI.Label(
                    new Rect(rect.x, rect.y + singleLineHeight * 0, rect.width - IconSize, singleLineHeight),
                    innerGroup.name + " (" + innerGroup.outfits.Count + " outfits)",
                    EditorStyles.boldLabel
                );

                var icon = innerGroup.icon;
                if (icon != null) {
                    EditorGUI.DrawPreviewTexture(
                        new Rect(rect.x + rect.width - IconSize, rect.y, IconSize, IconSize),
                        AssetPreview.GetAssetPreview(icon)
                    );
                }

                GUI.Label(
                    new Rect(rect.x, rect.y + singleLineHeight * 6, rect.width - IconSize, singleLineHeight),
                    "Occupies " + innerGroup.BitCount() + " Bits"
                );

                // EditorGUI.HelpBox(
                    // new Rect(rect.x, rect.y + singleLineHeight * 7, rect.width, singleLineHeight * 2),
                    // "Anchor value is out of bounds",
                    // MessageType.Error
                // );
            }
        }

        private static void OutfitsListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Outfits");
        }
    }
}
