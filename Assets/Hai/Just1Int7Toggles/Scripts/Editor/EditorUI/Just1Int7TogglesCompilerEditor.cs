using System.Linq;
using Hai.Just1Int7Toggles.Scripts.Components;
using Hai.Just1Int7Toggles.Scripts.Editor.Internal;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Hai.Just1Int7Toggles.Scripts.Editor.EditorUI
{
    [CustomEditor(typeof(Just1Int7TogglesCompiler))]
    public class Just1Int7TogglesCompilerEditor : UnityEditor.Editor
    {
        public SerializedProperty animatorController;
        public SerializedProperty customEmptyClip;
        
        public SerializedProperty avatar;
        
        public SerializedProperty alsoGenerateLayerB;
        
        public ReorderableList[] togglablesReorderableList;
        public SerializedProperty enableds;
        
        private void OnEnable()
        {
            AsCompiler().Migrate();
            
            animatorController = serializedObject.FindProperty("animatorController");
            customEmptyClip = serializedObject.FindProperty("customEmptyClip");
            
            avatar = serializedObject.FindProperty("avatar");
            alsoGenerateLayerB = serializedObject.FindProperty("alsoGenerateLayerB");
 
            enableds = serializedObject.FindProperty("enableds"); 
            
            // reference: https://blog.terresquall.com/2020/03/creating-reorderable-lists-in-the-unity-inspector/
            togglablesReorderableList = Enumerable.Repeat(0, 15)
                .Select((ignore, index) =>
                {
                    var reorderableList = new ReorderableList(
                        serializedObject,
                        serializedObject.FindProperty("togglables").GetArrayElementAtIndex(index).FindPropertyRelative("values"),
                        true, true, true, true
                    );
                    reorderableList.drawElementCallback = (rect, oIndex, active, focused) => TogglablesListElement(index, rect, oIndex, active, focused);
                    reorderableList.drawHeaderCallback = rect => TogglablesListHeader(index, rect);
                    
                    return reorderableList;
                })
                .ToArray();
        }

        private bool _foldoutAdvanced;
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(animatorController, new GUIContent("FX Animator Controller to overwrite"));
            EditorGUILayout.PropertyField(avatar, new GUIContent("Avatar"));

            for (var index = 0; index < 7; index++)
            {
                EditorForItem(index);
            }
            
            EditorGUILayout.LabelField("Complex generator", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(alsoGenerateLayerB, new GUIContent("Enable complex generator"));
            if (alsoGenerateLayerB.boolValue) {
                EditorGUILayout.HelpBox(@"CAUTION: Adding more than 7 items is highly discouraged.
The generated animator is way more complex, and should be avoided if unused.
If you have 7 items or less, put them all in slots 1-7 and uncheck ""Enable complex generator"".", MessageType.Warning);

                for (var index = 7; index < 15; index++)
                {
                    EditorForItem(index);
                }
            }
            
            EditorGUI.BeginDisabledGroup(
                ThereIsNoAnimatorController() || ThereIsNoAvatar() || MoreItemsAreEnabledButNothingIsSet()
            );

            bool ThereIsNoAnimatorController()
            {
                return animatorController.objectReferenceValue == null;
            }

            bool ThereIsNoAvatar()
            {
                return avatar.objectReferenceValue == null;
            }

            bool MoreItemsAreEnabledButNothingIsSet()
            {
                var compiler = AsCompiler();
                if (!alsoGenerateLayerB.boolValue)
                {
                    return false;
                }
                for (var i = 7; i < 15; i++)
                {
                    if (
                        compiler.togglables[i].values.Count != 0 && 
                        compiler.togglables[i].values.Count(togglable => togglable.item != null) != 0
                    ) {
                        return false;
                    }
                }

                return true;
            }
            
            if (GUILayout.Button(alsoGenerateLayerB.boolValue ? "Generate complex FX Animator layers" : "Generate FX Animator layers"))
            {
                DoGenerateEverything();
            }
            
            EditorGUI.EndDisabledGroup();

            if (MoreItemsAreEnabledButNothingIsSet())
            {
                EditorGUILayout.HelpBox(@"Cannot generate as slots 8-15 are empty.
Please uncheck ""Enable complex generator"".", MessageType.Error);
            }
            
            
            EditorGUILayout.Space();
        
            _foldoutAdvanced = EditorGUILayout.Foldout(_foldoutAdvanced, "Advanced");
            if (_foldoutAdvanced)
            {
                EditorGUILayout.LabelField("Fine tuning", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(customEmptyClip, new GUIContent("Custom 2-frame empty animation clip (optional)"));
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void EditorForItem(int index)
        {
            EditorGUILayout.LabelField("Item #" + (index + 1), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableds.GetArrayElementAtIndex(index), new GUIContent("Default state"));
            togglablesReorderableList[index].DoLayoutList();
            EditorGUILayout.Separator();
        }

        private Just1Int7TogglesCompiler AsCompiler()
        {
            return (Just1Int7TogglesCompiler) target;
        }

        private void DoGenerateEverything()
        {
            var compiler = AsCompiler();
            new Just1Int7TogglesCompilerInternal(
                    compiler.animatorController,
                    compiler.customEmptyClip,
                    new TogglesManifest(
                        compiler.avatar.gameObject,
                        compiler.togglables,
                        compiler.enableds
                    ),
                    compiler.alsoGenerateLayerB
            ).DoOverwriteCommonLogic();
        }
        
        private void TogglablesListElement(int itemIndex, Rect rect, int index, bool isActive, bool isFocused)
        {        
            var element = togglablesReorderableList[itemIndex].serializedProperty.GetArrayElementAtIndex(index);

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

        private static void TogglablesListHeader(int index, Rect rect)
        {
            EditorGUI.LabelField(rect, "Togglables (J1I7T_" + (index < 7 ? "A" : "B") + "_" + (index < 7 ? index : index - 7) + ")");
        }
    }
}
