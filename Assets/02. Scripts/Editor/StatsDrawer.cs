using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Stats))]
public class StatsDrawer : Editor
{
    SerializedProperty isDamageableProperty;
    SerializedProperty hpStatProperty;
    SerializedProperty skillCostStatProperty;
    SerializedProperty statOverridesProperty;

    void OnEnable()
    {
        isDamageableProperty = serializedObject.FindProperty("isDamageable");
        hpStatProperty = serializedObject.FindProperty("hpStat");
        skillCostStatProperty = serializedObject.FindProperty("skillCostStat");
        statOverridesProperty = serializedObject.FindProperty("statOverrides");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(isDamageableProperty);

        if (isDamageableProperty.boolValue)
        {
            EditorGUILayout.PropertyField(hpStatProperty);
            hpStatProperty.objectReferenceValue = null;
        }
        
        EditorGUILayout.PropertyField(skillCostStatProperty);
        EditorGUILayout.PropertyField(statOverridesProperty, true);

        serializedObject.ApplyModifiedProperties();
    }
}
