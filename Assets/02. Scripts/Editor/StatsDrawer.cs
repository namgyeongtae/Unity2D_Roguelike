using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Stats))]
public class StatsDrawer : Editor
{
    SerializedProperty isDamageableProperty;
    SerializedProperty hpStatProperty;
    SerializedProperty maxHpStatProperty;
    SerializedProperty skillCostStatProperty;
    SerializedProperty statOverridesProperty;

    void OnEnable()
    {
        isDamageableProperty = serializedObject.FindProperty("isDamageable");
        hpStatProperty = serializedObject.FindProperty("hpStat");
        maxHpStatProperty = serializedObject.FindProperty("maxHpStat");
        skillCostStatProperty = serializedObject.FindProperty("skillCostStat");
        statOverridesProperty = serializedObject.FindProperty("statOverrides");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(isDamageableProperty);

        if (isDamageableProperty.boolValue)
        {
            EditorGUILayout.PropertyField(maxHpStatProperty);
            EditorGUILayout.PropertyField(hpStatProperty);
            EditorGUILayout.PropertyField(skillCostStatProperty);
        }
        else
        {
            hpStatProperty.objectReferenceValue = null;
            maxHpStatProperty.objectReferenceValue = null;
            skillCostStatProperty.objectReferenceValue = null;
        }
        
        EditorGUILayout.PropertyField(statOverridesProperty, true);

        serializedObject.ApplyModifiedProperties();
    }
}
