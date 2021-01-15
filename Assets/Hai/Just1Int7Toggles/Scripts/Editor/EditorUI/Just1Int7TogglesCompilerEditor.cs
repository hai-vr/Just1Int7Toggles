using System.Linq;
using Hai.Just1Int7Toggles.Scripts.Components;
using Hai.Just1Int7Toggles.Scripts.Editor.Internal;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Hai.Just1Int7Toggles.Scripts.Editor.EditorUI
{
    [CustomEditor(typeof(Just1Int7TogglesCompiler))]
    public class Just1Int7TogglesCompilerEditor : UnityEditor.Editor
    {
        public SerializedProperty animatorController;
        public SerializedProperty customEmptyClip;

        public SerializedProperty avatar;

        private void OnEnable()
        {
            AsCompiler().Migrate();

            animatorController = serializedObject.FindProperty("animatorController");
            customEmptyClip = serializedObject.FindProperty("customEmptyClip");

            avatar = serializedObject.FindProperty("avatar");
        }

        private bool _foldoutAdvanced;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (IsLegacy())
            {
                if (GUILayout.Button("Convert to Just Hai Toggles Inventory System", GUILayout.Height(100)))
                {
                    Convert();
                }

                return;
            }

            var compiler = (Just1Int7TogglesCompiler) target;

            compiler.convertedToInventorySystem = true;

            EditorGUILayout.PropertyField(animatorController, new GUIContent("FX Animator Controller to overwrite"));
            EditorGUILayout.PropertyField(avatar, new GUIContent("Avatar"));

            EditorGUILayout.LabelField("Groups", EditorStyles.boldLabel);
            var toggleGroups = compiler.GetComponents<JustHaiToggleGroup>();
            var expressionParameters = compiler.avatar != null ? compiler.avatar.expressionParameters : null;
            if (avatar.serializedObject.targetObject == null)
            {
                EditorGUILayout.HelpBox("Avatar is missing.", MessageType.Error);
            }
            else if (expressionParameters == null)
            {
                EditorGUILayout.HelpBox("Avatar does not have Expression Parameters.\nPlease add one.", MessageType.Warning);
            }
            var anyEmpty = 0;
            foreach (var group in toggleGroups)
            {
                if (!string.IsNullOrEmpty(@group.parameterName))
                {
                    var param = expressionParameters != null ? expressionParameters.FindParameter(@group.parameterName) : null;

                    EditorGUILayout.LabelField("- " + group.parameterName);
                    if (expressionParameters != null)
                    {
                        ParameterFixes(param, expressionParameters, @group);
                    }
                }
                else
                {
                    anyEmpty++;
                }
            }

            if (anyEmpty > 0)
            {
                EditorGUILayout.HelpBox($"There are {anyEmpty} groups that do not have parameter names yet.", MessageType.Error);
            }

            if (expressionParameters != null)
            {
                LegacyRemoval(expressionParameters);
                NoNameRemoval(expressionParameters);
            }

            EditorGUI.BeginDisabledGroup(anyEmpty > 0);
            if (GUILayout.Button("+ Add group..."))
            {
                compiler.gameObject.AddComponent<JustHaiToggleGroup>();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.LabelField("Generator", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(
                ThereIsNoAnimatorController() || ThereIsNoAvatar()
            );

            bool ThereIsNoAnimatorController()
            {
                return animatorController.objectReferenceValue == null;
            }

            bool ThereIsNoAvatar()
            {
                return avatar.objectReferenceValue == null;
            }

            if (GUILayout.Button("Generate FX Animator layers"))
            {
                DoGenerateEverything();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            _foldoutAdvanced = EditorGUILayout.Foldout(_foldoutAdvanced, "Advanced");
            if (_foldoutAdvanced)
            {
                EditorGUILayout.LabelField("Fine tuning", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(customEmptyClip, new GUIContent("Custom 2-frame empty animation clip (optional)"));
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void NoNameRemoval(VRCExpressionParameters expressionParameters)
        {
            var anyBlankParameter = expressionParameters.parameters.Any(parameter => parameter.name == "");

            if (anyBlankParameter)
            {
                EditorGUILayout.LabelField("Clean up", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("Your avatar has Expression Parameters with no name. This will take up unnecessary memory that Just Hai Toggles Inventory System won't be able to use.", MessageType.Error);
                if (GUILayout.Button("Fix: Remove empty Expression Parameters"))
                {
                    expressionParameters.parameters = expressionParameters.parameters
                        .Where(parameter => parameter.name != "")
                        .ToArray();
                }
            }
        }

        private static void LegacyRemoval(VRCExpressionParameters expressionParameters)
        {
            var legacyParamA = expressionParameters.FindParameter("J1I7T_A_Sync");
            var legacyParamB = expressionParameters.FindParameter("J1I7T_B_Sync");
            if (legacyParamA != null || legacyParamB != null)
            {
                EditorGUILayout.LabelField("Legacy", EditorStyles.boldLabel);
                if (legacyParamA != null)
                {
                    EditorGUILayout.HelpBox("Your avatar has an Expression Parameter with name J1I7T_A_Sync. It is no longer used.", MessageType.Error);
                }

                if (legacyParamB != null)
                {
                    EditorGUILayout.HelpBox("Your avatar has an Expression Parameter with name J1I7T_B_Sync. It is no longer used.", MessageType.Error);
                }

                if (GUILayout.Button("Fix: Remove legacy Expression Parameters"))
                {
                    expressionParameters.parameters = expressionParameters.parameters
                        .Where(parameter => parameter.name != "J1I7T_A_Sync")
                        .Where(parameter => parameter.name != "J1I7T_B_Sync")
                        .ToArray();
                }
            }
        }

        private static void ParameterFixes(VRCExpressionParameters.Parameter param, VRCExpressionParameters expressionParameters, JustHaiToggleGroup @group)
        {
            if (param == null)
            {
                EditorGUILayout.HelpBox("Expression Parameter does not exist", MessageType.Error);
                if (GUILayout.Button("Fix: Create Expression Parameter"))
                {
                    var newParameters = new VRCExpressionParameters.Parameter[expressionParameters.parameters.Length + 1];
                    expressionParameters.parameters.CopyTo(newParameters, 0);
                    expressionParameters.parameters = newParameters;
                    expressionParameters.parameters[expressionParameters.parameters.Length - 1] = new VRCExpressionParameters.Parameter()
                    {
                        name = @group.parameterName,
                        defaultValue = @group.hintEnabled ? 1f : 0f,
                        saved = true,
                        valueType = VRCExpressionParameters.ValueType.Bool
                    };
                }
            }
            else if (param.valueType != VRCExpressionParameters.ValueType.Bool)
            {
                EditorGUILayout.HelpBox($"Expression Parameter exists but it is not a Bool (currently: {param.valueType})", MessageType.Error);
                if (GUILayout.Button("Fix: Change Expression Parameter Type to Bool"))
                {
                    param.valueType = VRCExpressionParameters.ValueType.Bool;
                    param.defaultValue = @group.hintEnabled ? 1f : 0f;
                }
            }
            else if (param.defaultValue != (@group.hintEnabled ? 1f : 0f))
            {
                EditorGUILayout.HelpBox($"Expression Parameter default value is not the same", MessageType.Warning);
                if (GUILayout.Button("Fix: Change Expression Parameter Default Value"))
                {
                    param.defaultValue = @group.hintEnabled ? 1f : 0f;
                }
            }
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
                        compiler.GetComponents<JustHaiToggleGroup>()
                    )
            ).DoOverwriteCommonLogic();
        }

        private bool IsLegacy()
        {
            return !((Just1Int7TogglesCompiler)target).convertedToInventorySystem && ((Just1Int7TogglesCompiler)target).togglables != null && ((Just1Int7TogglesCompiler)target).togglables.Length > 0;
        }

        private void Convert()
        {
            if (!IsLegacy()) return;

            var compiler = (Just1Int7TogglesCompiler) serializedObject.targetObject;

            for (var index = 0; index < compiler.togglables.Length; index++)
            {
                var togglableContainer = compiler.togglables[index];
                var enabled = compiler.enableds[index];
                var items = togglableContainer.values;
                if (items.Count > 0)
                {
                    var jhtg = CreateNewToggleGroup();
                    jhtg.hintEnabled = enabled;
                    jhtg.togglables = items.Select(togglable => new TogglableItemV2
                    {
                        item = togglable.item,
                        initialState = togglable.initialState == J1I7TToggleableInitialState.Normal ? ToggleableInitialStateV2.Normal : ToggleableInitialStateV2.Inverse,
                    }).ToList();
                }
            }

            compiler.togglables = new TogglableContainer[0];
            compiler.convertedToInventorySystem = true;
        }

        private JustHaiToggleGroup CreateNewToggleGroup()
        {
            return ((Just1Int7TogglesCompiler) serializedObject.targetObject).gameObject.AddComponent<JustHaiToggleGroup>();
        }
    }
}
