using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item))]
public class ItemEditor : IdentifiedObjectEditor
{
    private SerializedProperty typeProperty;
    private SerializedProperty equipmentSlotProperty;

    private SerializedProperty itemPrefabProperty;
    private SerializedProperty isNeedTargetProperty;
    private SerializedProperty targetTypeSelfProperty;

    private SerializedProperty effectProperty;
    
    protected override void OnEnable()
    {
        base.OnEnable();

        typeProperty = serializedObject.FindProperty("type");
        equipmentSlotProperty = serializedObject.FindProperty("equipmentSlot");
        
        itemPrefabProperty = serializedObject.FindProperty("itemPrefab");
        isNeedTargetProperty = serializedObject.FindProperty("isNeedTarget");
        targetTypeSelfProperty = serializedObject.FindProperty("targetType");
        
        effectProperty = serializedObject.FindProperty("effect");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        if (!DrawFoldoutTitle("Setting"))
            return;
        
        /* EditorGUILayout.BeginHorizontal();
        { */
            //EditorGUILayout.PropertyField(typeProperty);

        typeProperty.enumValueIndex = GUILayout.Toolbar(typeProperty.enumValueIndex, typeProperty.enumDisplayNames);

        if (typeProperty.enumValueIndex == (int)ItemType.Equipment)
        {
            // EditorGUILayout.PropertyField(equipmentSlotProperty);
            equipmentSlotProperty.enumValueIndex = GUILayout.Toolbar(equipmentSlotProperty.enumValueIndex, equipmentSlotProperty.enumDisplayNames);
        }
        //}
        // EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(isNeedTargetProperty);

        if (isNeedTargetProperty.boolValue)
        {
            CustomEditorUtility.DrawEnumToolbar(targetTypeSelfProperty);
        }
        
        EditorGUILayout.PropertyField(itemPrefabProperty);

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("Effect");
            var newObj = EditorGUILayout.ObjectField(effectProperty.objectReferenceValue, typeof(Effect), false);

            effectProperty.objectReferenceValue = newObj;
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
