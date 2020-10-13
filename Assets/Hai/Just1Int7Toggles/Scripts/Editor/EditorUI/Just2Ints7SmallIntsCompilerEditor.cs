using Hai.Just1Int7Toggles.Scripts.Components;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditor.EditorGUIUtility;

namespace Hai.Just1Int7Toggles.Scripts.Editor.EditorUI
{
    [CustomEditor(typeof(Just2Ints7SmallIntsCompiler))]
    public class Just2Ints7SmallIntsCompilerEditor : UnityEditor.Editor
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
                    var bitCount = ((J2I7SIGroupOfOutfits) elt.FindPropertyRelative("value").objectReferenceValue).BitCount();
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

            LayoutBitOccupancy();
            EditorGUILayout.Separator();
            LayoutOutfits();
            EditorGUILayout.Separator();
            LayoutAnchors();
            EditorGUILayout.Separator();
            LayoutAnimatorGenerator();
            EditorGUILayout.Separator();
            LayoutMenuGenerator();

            serializedObject.ApplyModifiedProperties();
        }
        private void LayoutBitOccupancy()
        {
            EditorGUILayout.LabelField("Bit occupancy", EditorStyles.boldLabel);

            var compiler = (Just2Ints7SmallIntsCompiler) target;
            EditorGUILayout.LabelField("Main layer: " + compiler.CountBitOccupancyOf(OutfitLayer.MainLayer) + " / 7");
            EditorGUILayout.LabelField("Secondary layer B: " + compiler.CountBitOccupancyOf(OutfitLayer.SecondaryLayerB) +
                                       " / 8");
        }

        private void LayoutOutfits()
        {
            EditorGUILayout.LabelField("Outfits", EditorStyles.boldLabel);
            groupOfOutfitsReorderableList.DoLayoutList();
        }

        private void LayoutAnchors()
        {
            EditorGUILayout.LabelField("Anchors", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox("Anchors are overlapping", MessageType.Error);
            if (GUILayout.Button("Auto-solve anchors"))
            {
                DoAutoSolveAnchors();
            }

            void DoAutoSolveAnchors()
            {
                throw new System.NotImplementedException();
            }
        }

        private void LayoutAnimatorGenerator()
        {
        }

        private void LayoutMenuGenerator()
        {
        }

        private void GroupOfOutfitsListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = groupOfOutfitsReorderableList.serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(RectangleAtLine(rect, 1), element.FindPropertyRelative("value"), new GUIContent("Group of Outfits"));
            EditorGUI.PropertyField(RectangleAtLine(rect, 2), element.FindPropertyRelative("layer"), new GUIContent("Layer"));
            var anchorIsLocked = element.FindPropertyRelative("anchorLocked").boolValue;
            EditorGUI.BeginDisabledGroup(anchorIsLocked);
            EditorGUI.PropertyField(
                RectangleAtLine(rect, 3),
                element.FindPropertyRelative("anchorValue"),
                new GUIContent("Anchor value " + (anchorIsLocked ? " (Locked)" : ""))
            );
            EditorGUI.EndDisabledGroup();
            EditorGUI.PropertyField(RectangleAtLine(rect, 4), element.FindPropertyRelative("anchorLocked"), new GUIContent("Anchor lock"));

            var innerGroup = (J2I7SIGroupOfOutfits) element.FindPropertyRelative("value").objectReferenceValue;
            if (innerGroup != null)
            {
                var outfitsCount = innerGroup.outfits.Count;
                string message;
                if (!innerGroup.IsValidBitStorage())
                {
                    message = innerGroup.name + " (invalid)";
                }
                else
                {
                    message = outfitsCount <= 1
                        ? innerGroup.name + " (" + outfitsCount + " outfits)"
                        : innerGroup.name + " (" + outfitsCount + " toggle)";
                }

                GUI.Label(
                    RectangleAtLine(rect, 0),
                    message,
                    EditorStyles.boldLabel
                );

                var icon = innerGroup.icon;
                if (icon != null)
                {
                    EditorGUI.DrawPreviewTexture(
                        new Rect(rect.x + rect.width - IconSize, rect.y, IconSize, IconSize),
                        AssetPreview.GetAssetPreview(icon)
                    );
                }

                if (innerGroup.IsValidBitStorage())
                {
                    GUI.Label(RectangleAtLine(rect, 6), "Occupies " + innerGroup.BitCount() + " Bits");
                }
                else
                {
                    GUI.Label(RectangleAtLine(rect, 6), "Invalid occupancy");
                }

                // EditorGUI.HelpBox(
                // new Rect(rect.x, rect.y + singleLineHeight * 7, rect.width, singleLineHeight * 2),
                // "Anchor value is out of bounds",
                // MessageType.Error
                // );
            }
        }

        private static Rect RectangleAtLine(Rect rect, int i)
        {
            return new Rect(rect.x, rect.y + singleLineHeight * i, rect.width - IconSize, singleLineHeight);
        }

        private static void OutfitsListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Outfits");
        }
    }
}
