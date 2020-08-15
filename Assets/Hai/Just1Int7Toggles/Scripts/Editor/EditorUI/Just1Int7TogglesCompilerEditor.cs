using Hai._1Int7Toggles.Scripts.Components;
using Hai.Just1Int7Toggles.Scripts.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace Hai.Just1Int7Toggles.Scripts.Editor.EditorUI
{
    [CustomEditor(typeof(Just1Int7TogglesCompiler))]
    [CanEditMultipleObjects]
    public class Just1Int7TogglesCompilerEditor : UnityEditor.Editor
    {
        public SerializedProperty animatorController;
        public SerializedProperty customEmptyClip;
        
        public SerializedProperty avatar;
        
        // FIXME: bad structure
        public SerializedProperty item1;
        public SerializedProperty enabled1;
        public SerializedProperty item2;
        public SerializedProperty enabled2;
        public SerializedProperty item3;
        public SerializedProperty enabled3;
        public SerializedProperty item4;
        public SerializedProperty enabled4;
        public SerializedProperty item5;
        public SerializedProperty enabled5;
        public SerializedProperty item6;
        public SerializedProperty enabled6;
        public SerializedProperty item7;
        public SerializedProperty enabled7;
        public SerializedProperty alsoGenerateLayerB;
        public SerializedProperty item8;
        public SerializedProperty enabled8;
        public SerializedProperty item9;
        public SerializedProperty enabled9;
        public SerializedProperty item10;
        public SerializedProperty enabled10;
        public SerializedProperty item11;
        public SerializedProperty enabled11;
        public SerializedProperty item12;
        public SerializedProperty enabled12;
        public SerializedProperty item13;
        public SerializedProperty enabled13;
        public SerializedProperty item14;
        public SerializedProperty enabled14;
        public SerializedProperty item15;
        public SerializedProperty enabled15;
        
        private void OnEnable()
        {
            animatorController = serializedObject.FindProperty("animatorController");
            customEmptyClip = serializedObject.FindProperty("customEmptyClip");
            avatar = serializedObject.FindProperty("avatar");
            item1 = serializedObject.FindProperty("item1");
            enabled1 = serializedObject.FindProperty("enabled1");
            item2 = serializedObject.FindProperty("item2");
            enabled2 = serializedObject.FindProperty("enabled2");
            item3 = serializedObject.FindProperty("item3");
            enabled3 = serializedObject.FindProperty("enabled3");
            item4 = serializedObject.FindProperty("item4");
            enabled4 = serializedObject.FindProperty("enabled4");
            item5 = serializedObject.FindProperty("item5");
            enabled5 = serializedObject.FindProperty("enabled5");
            item6 = serializedObject.FindProperty("item6");
            enabled6 = serializedObject.FindProperty("enabled6");
            item7 = serializedObject.FindProperty("item7");
            enabled7 = serializedObject.FindProperty("enabled7");
            alsoGenerateLayerB = serializedObject.FindProperty("alsoGenerateLayerB");
            item8 = serializedObject.FindProperty("item8");
            enabled8 = serializedObject.FindProperty("enabled8");
            item9 = serializedObject.FindProperty("item9");
            enabled9 = serializedObject.FindProperty("enabled9");
            item10 = serializedObject.FindProperty("item10");
            enabled10 = serializedObject.FindProperty("enabled10");
            item11 = serializedObject.FindProperty("item11");
            enabled11 = serializedObject.FindProperty("enabled11");
            item12 = serializedObject.FindProperty("item12");
            enabled12 = serializedObject.FindProperty("enabled12");
            item13 = serializedObject.FindProperty("item13");
            enabled13 = serializedObject.FindProperty("enabled13");
            item14 = serializedObject.FindProperty("item14");
            enabled14 = serializedObject.FindProperty("enabled14");
            item15 = serializedObject.FindProperty("item15");
            enabled15 = serializedObject.FindProperty("enabled15");
        }
        
        private bool _foldoutAdvanced;
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(animatorController, new GUIContent("FX Animator Controller to overwrite"));
            EditorGUILayout.PropertyField(avatar, new GUIContent("Avatar"));
            
            EditorGUILayout.LabelField("Item #1", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(item1, new GUIContent("Item (J1I7T_A_0)"));
            EditorGUILayout.PropertyField(enabled1, new GUIContent("Default state"));
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Item #2", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(item2, new GUIContent("Item (J1I7T_A_1)"));
            EditorGUILayout.PropertyField(enabled2, new GUIContent("Default state"));
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Item #3", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(item3, new GUIContent("Item (J1I7T_A_2)"));
            EditorGUILayout.PropertyField(enabled3, new GUIContent("Default state"));
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Item #4", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(item4, new GUIContent("Item (J1I7T_A_3)"));
            EditorGUILayout.PropertyField(enabled4, new GUIContent("Default state"));
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Item #5", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(item5, new GUIContent("Item (J1I7T_A_4)"));
            EditorGUILayout.PropertyField(enabled5, new GUIContent("Default state"));
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Item #6", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(item6, new GUIContent("Item (J1I7T_A_5)"));
            EditorGUILayout.PropertyField(enabled6, new GUIContent("Default state"));
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Item #7", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(item7, new GUIContent("Item (J1I7T_A_6)"));
            EditorGUILayout.PropertyField(enabled7, new GUIContent("Default state"));
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Complex generator", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(alsoGenerateLayerB, new GUIContent("Enable complex generator"));
            if (alsoGenerateLayerB.boolValue) {
                EditorGUILayout.HelpBox(@"CAUTION: Adding more than 7 items is highly discouraged.
The generated animator is way more complex, and should be avoided if unused.
If you have 7 items or less, put them all in slots 1-7 and uncheck ""Enable complex generator"".", MessageType.Warning);

                EditorGUILayout.LabelField("Item #8", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(item8, new GUIContent("Item (J1I7T_B_0)"));
                EditorGUILayout.PropertyField(enabled8, new GUIContent("Default state"));
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("Item #9", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(item9, new GUIContent("Item (J1I7T_B_1)"));
                EditorGUILayout.PropertyField(enabled9, new GUIContent("Default state"));
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("Item #10", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(item10, new GUIContent("Item (J1I7T_B_2)"));
                EditorGUILayout.PropertyField(enabled10, new GUIContent("Default state"));
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("Item #11", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(item11, new GUIContent("Item (J1I7T_B_3)"));
                EditorGUILayout.PropertyField(enabled11, new GUIContent("Default state"));
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("Item #12", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(item12, new GUIContent("Item (J1I7T_B_4)"));
                EditorGUILayout.PropertyField(enabled12, new GUIContent("Default state"));
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("Item #13", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(item13, new GUIContent("Item (J1I7T_B_5)"));
                EditorGUILayout.PropertyField(enabled13, new GUIContent("Default state"));
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("Item #14", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(item14, new GUIContent("Item (J1I7T_B_6)"));
                EditorGUILayout.PropertyField(enabled14, new GUIContent("Default state"));
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("Item #15", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(item15, new GUIContent("Item (J1I7T_B_7)"));
                EditorGUILayout.PropertyField(enabled15, new GUIContent("Default state"));
                EditorGUILayout.Space();
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
                return alsoGenerateLayerB.boolValue
                       && item8.objectReferenceValue == null
                       && item9.objectReferenceValue == null
                       && item10.objectReferenceValue == null
                       && item11.objectReferenceValue == null
                       && item12.objectReferenceValue == null
                       && item13.objectReferenceValue == null
                       && item14.objectReferenceValue == null
                       && item15.objectReferenceValue == null;
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
                        new ToggleEntry(compiler.item1, compiler.enabled1),
                        new ToggleEntry(compiler.item2, compiler.enabled2),
                        new ToggleEntry(compiler.item3, compiler.enabled3),
                        new ToggleEntry(compiler.item4, compiler.enabled4),
                        new ToggleEntry(compiler.item5, compiler.enabled5),
                        new ToggleEntry(compiler.item6, compiler.enabled6),
                        new ToggleEntry(compiler.item7, compiler.enabled7),
                        new ToggleEntry(compiler.item8, compiler.enabled8),
                        new ToggleEntry(compiler.item9, compiler.enabled9),
                        new ToggleEntry(compiler.item10, compiler.enabled10),
                        new ToggleEntry(compiler.item11, compiler.enabled11),
                        new ToggleEntry(compiler.item12, compiler.enabled12),
                        new ToggleEntry(compiler.item13, compiler.enabled13),
                        new ToggleEntry(compiler.item14, compiler.enabled14),
                        new ToggleEntry(compiler.item15, compiler.enabled15)
                    ),
                    compiler.alsoGenerateLayerB
            ).DoOverwriteCommonLogic();
        }
    }
}