using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

[CustomEditor(typeof(IdentifiedObject), true)]
public class IdentifiedObjectEditor : Editor
{
    #region 1-6
    // SerializedProperty�� ���� ���� �ִ� ��ü�� public Ȥ�� [SerializeField] ��Ʈ����Ʈ�� ����
    // Serailize�� �������� �����ϱ� ���� class
    private SerializedProperty categoriesProperty;
    private SerializedProperty iconProperty;
    private SerializedProperty idProperty;
    private SerializedProperty codeNameProperty;
    private SerializedProperty displayNameProperty;
    private SerializedProperty descriptionProperty;

    // Inspector �󿡼� ������ ������ �� �ִ� List
    private ReorderableList categories;

    // text�� �а� �����ִ� Style(=Skin) ������ ���� ����
    private GUIStyle textAreaStyle;
    
    // Title�� Foldout Expand ���¸� �����ϴ� ����
    private readonly Dictionary<string, bool> isFoldoutExpandedesByTitle = new();
    #endregion

    #region 1-7
    protected virtual void OnEnable()
    {
        // Inspector���� description�� �����ϴٰ� �ٸ� Inspector View�� �Ѿ�� ��쿡,
        // ��Ŀ���� Ǯ���� �ʰ� ������ �����ϴ� desription ������ �״�� ���̴� ������ �ذ��ϱ����� ��Ŀ���� Ǯ����
        GUIUtility.keyboardControl = 0;

        // serializedObject�� ���� ���� Editor���� ���� �ִ� IdentifiedObject�� ����
        // ��ü���� Serialize �������� ã�ƿ�
        categoriesProperty = serializedObject.FindProperty("categories");
        iconProperty = serializedObject.FindProperty("icon");
        idProperty = serializedObject.FindProperty("id");
        codeNameProperty = serializedObject.FindProperty("codeName");
        displayNameProperty = serializedObject.FindProperty("displayName");
        descriptionProperty = serializedObject.FindProperty("description");
                
        categories = new(serializedObject, categoriesProperty);
        // List�� Prefix Label�� ��� �׸��� ����
        categories.drawHeaderCallback = rect => EditorGUI.LabelField(rect, categoriesProperty.displayName);
        // List�� Element�� ��� �׸��� ����
        categories.drawElementCallback = (rect, index, isActive, isFocused) => {
            rect = new Rect(rect.x, rect.y + 2f, rect.width, EditorGUIUtility.singleLineHeight);
            // EditorGUILayout�� EditorGUI�� ������
            // EditorGUILayout�� GUI�� �׸��� ������ ���� ��ġ�� �ڵ����� ��������
            // EditorGUI�� ����ڰ� ���� GUI�� �׸� ��ġ�� �����������
            EditorGUI.PropertyField(rect, categoriesProperty.GetArrayElementAtIndex(index), GUIContent.none);
        };
    }

    private void StyleSetup()
    {
        if (textAreaStyle == null)
        {
            // Style�� �⺻ ����� textArea.
            textAreaStyle = new(EditorStyles.textArea);
            // ���ڿ��� TextBox ������ �� ���������� ��.
            textAreaStyle.wordWrap = true;
        }
    }

    protected bool DrawFoldoutTitle(string text)
        => CustomEditorUtility.DrawFoldoutTitle(isFoldoutExpandedesByTitle, text);
    #endregion

    #region 1-8
    public override void OnInspectorGUI()
    {
        StyleSetup();

        // ��ü�� Serialize �������� ���� ������Ʈ��.
        serializedObject.Update();

        // List�� �׷���
        categories.DoLayoutList();

        if(DrawFoldoutTitle("Infomation"))
        {
            // (1) ���ݺ��� �׸� ��ü�� ���η� �����ϸ�, ����� �׵θ� �ִ� ȸ������ ä��(=HelpBox�� ����Ƽ ���ο� ���ǵǾ� �ִ� Skin��)
            // �߰�ȣ�� �ۼ��� �ʿ�� ������ ��Ȯ�� ������ ���� �־��� ���̱� ������ ��Ÿ�Ͽ� ���� �߰�ȣ�� �ȳ־ ��.
            EditorGUILayout.BeginHorizontal("HelpBox");
            {
                //Sprite�� Preview�� �� �� �ְ� ������ �׷���
                iconProperty.objectReferenceValue = EditorGUILayout.ObjectField(GUIContent.none, iconProperty.objectReferenceValue,
                    typeof(Sprite), false, GUILayout.Width(65));

                // (2) ���ݺ��� �׸� ��ü�� ���η� �����Ѵ�.
                // �� icon ������ ���ʿ� �׷�����, ���ݺ��� �׸� �������� �����ʿ� ���η� �׷���.
                EditorGUILayout.BeginVertical();
                {
                    // (3) ���ݺ��� �׸� ��ü�� ���η� �����Ѵ�.
                    // id ������ prefix(= inspector���� ���̴� ������ �̸�)�� ���� �������ֱ� ���� ���� Line�� ���� ����.
                    EditorGUILayout.BeginHorizontal();
                    {
                        // ���� ���� Disable, ID�� Database���� ���� Set���� ���̱� ������ ����ڰ� ���� �������� ���ϵ��� ��.
                        GUI.enabled = false;
                        // ������ ���� ��Ī(Prefix) ����
                        EditorGUILayout.PrefixLabel("ID");
                        // id ������ �׸��� Prefix�� �׸�������(=GUIContent.none); 
                        EditorGUILayout.PropertyField(idProperty, GUIContent.none);
                        // ���� ���� Enable
                        GUI.enabled = true;
                    }
                    // (3) ���� ���� ����
                    EditorGUILayout.EndHorizontal();

                    // ���ݺ��� ������ �����Ǿ����� �˻��Ѵ�.
                    EditorGUI.BeginChangeCheck();
                    var prevCodeName = codeNameProperty.stringValue;
                    // codeName ������ �׸���, ����ڰ� Enter Ű�� ���� ������ �� ������ ������.
                    EditorGUILayout.DelayedTextField(codeNameProperty);
                    // ������ �����Ǿ����� Ȯ��, codeName ������ �����Ǿ��ٸ� ������ ������ ���� ��ü�� �̸��� �ٲ���.
                    if (EditorGUI.EndChangeCheck())
                    {
                        // ���� ��ü�� ����Ƽ ������Ʈ���� �ּҸ� ������.
                        // target == IdentifiedObject, var identifiedObject = target as IdentifiecObject �̷� ������ ����� �� ����.
                        // serializeObject.targetObject == target
                        var assetPath = AssetDatabase.GetAssetPath(target);
                        // ���ο� �̸��� '(������ Type)_(codeName)'
                        var newName = $"{target.GetType().Name.ToUpper()}_{codeNameProperty.stringValue}";

                        // Serialize �������� �� ��ȭ�� ������(=��ũ�� ������)
                        // �� �۾��� ������ ������ �ٲ� ���� ������� �ʾƼ� ���� ������ ���ư�
                        serializedObject.ApplyModifiedProperties();

                        // ��ü�� Project View���� ���̴� �̸��� ������. ���� ���� �̸��� ���� ��ü�� ���� ��� ������.
                        var message = AssetDatabase.RenameAsset(assetPath, newName);
                        // �������� ��� ��ü�� ���� �̸��� �ٲ���. �ܺ� �̸��� ���� �̸��� �ٸ� �� ����Ƽ���� ����� ����,
                        // ���� ������Ʈ������ ������ ����ų ���ɼ��� ���⿡ �׻� �̸��� ��ġ���������
                        if (string.IsNullOrEmpty(message))
                            target.name = newName;
                        else
                            codeNameProperty.stringValue = prevCodeName;
                    }

                    // displayName ������ �׷���
                    EditorGUILayout.PropertyField(displayNameProperty);
                }
                // (2) ���� ���� ����
                EditorGUILayout.EndVertical();
            }
            // (1) ���� ���� ����
            EditorGUILayout.EndHorizontal();

            // ���� ���� ����, �⺻������ ���� ������ Default �����̱� ������ ���� ���� ���ο� ����ϴ°� �ƴ϶��
            // ���� ���� ������ ���� �ʿ䰡 ������ �� ��쿡�� HelpBox�� ���θ� ȸ������ ä������� ���� ���� ������ ��
            EditorGUILayout.BeginVertical("HelpBox");
            {
                // Description�̶�� Lebel�� �����
                EditorGUILayout.LabelField("Description");
                // TextField�� ���� ����(TextArea)�� �׷���
                descriptionProperty.stringValue = EditorGUILayout.TextArea(descriptionProperty.stringValue,
                    textAreaStyle, GUILayout.Height(60));
            }
            EditorGUILayout.EndVertical();
            // ���� ���� ����
        }

        // Serialize �������� �� ��ȭ�� ������(=��ũ�� ������)
        // �� �۾��� ������ ������ �ٲ� ���� ������� �ʾƼ� ���� ������ ���ư�
        serializedObject.ApplyModifiedProperties();
    }
    #endregion

    protected bool DrawRemovableLevelFoldout(SerializedProperty datasProperty, SerializedProperty targetProperty, 
        int targetIndex, bool isDrawRemoveButton = true)
    {
        // Data를 삭제했는지에 대한 결과
        bool isRemoveButtonClicked = false;

        EditorGUILayout.BeginHorizontal();
        {
            GUI.color = Color.green;
            var level = targetProperty.FindPropertyRelative("level").intValue;
            // Data의 Level을 보여주는 Foldout GUI를 그려줌
            targetProperty.isExpanded = EditorGUILayout.Foldout(targetProperty.isExpanded, $"Level {level}");
            GUI.color = Color.white;

            if (isDrawRemoveButton)
            {
                GUI.color = Color.red;
                if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(20f)))
                {
                    isRemoveButtonClicked = true;
                    // EffectDatas에서 현재 Data를 Index를 이용해 지움
                    datasProperty.DeleteArrayElementAtIndex(targetIndex);
                }
                GUI.color = Color.white;
            }
        }
        EditorGUILayout.EndHorizontal();

        return isRemoveButtonClicked;
    }

    protected void DrawAutoSortLevelProperty(SerializedProperty datasProperty, SerializedProperty levelProperty, 
        int index, bool isEditable)
    {
        if (!isEditable)    // 그냥 level 1(index = 0) 의 EffectData는 고정시킴. 수정할 수 없음음
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(levelProperty);
            GUI.enabled = true;
        }
        else
        {
            // Property가 수정되었는지 감시 시작
            EditorGUI.BeginChangeCheck();
            // 수정 전 Level을 기록해둠
            var prevValue = levelProperty.intValue;
            // levelProperty를 Delayed 방식으로 그려줌
            // 키보드 Enter Key를 눌러야 입력한 값이 반영됨, Enter Key를 누르지 않고 빠져나가면 원래 값으로 돌아옴.
            EditorGUILayout.DelayedIntField(levelProperty);
            // Property가 수정되었을 경우 true 반환
            if (EditorGUI.EndChangeCheck())
            {
                if (levelProperty.intValue <= 1)
                    levelProperty.intValue = prevValue; // 1보다 작을 수 없으므로 원래 값으로 되돌림
                else
                {
                    // EffectDatas를 순회하여 같은 level을 가진 data가 이미 있으면 수정 전 level로 되돌림
                    for (int i = 0; i < datasProperty.arraySize; i++)
                    {
                        // 확인해야하는 Data가 현재 Data와 동일하다면 Skip
                        if (index == i)
                            continue;

                        var element = datasProperty.GetArrayElementAtIndex(i);
                        // Level이 똑같으면 현재 Data의 Level을 수정 전으로 되돌림
                        if (element.FindPropertyRelative("level").intValue == levelProperty.intValue)
                        {
                            levelProperty.intValue = prevValue;
                            break;
                        }
                    }

                    // Level이 정상적으로 수정되었다면 오름차순 정렬 작업 실행
                    if (levelProperty.intValue != prevValue)
                    {
                        // 현재 Data의 Level이 i번째 Data의 Level보다 작으면, 현재 Data를 i번째로 옮김
                        // ex. 1 2 4 5 (3) => 1 2 (3) 4 5
                        for (int moveIndex = 1; moveIndex < datasProperty.arraySize; moveIndex++)
                        {
                            if (moveIndex == index)
                                continue;
                            
                            var element = datasProperty.GetArrayElementAtIndex(moveIndex).FindPropertyRelative("level");
                            if (levelProperty.intValue < element.intValue || moveIndex == (datasProperty.arraySize - 1))
                            {
                                datasProperty.MoveArrayElement(index, moveIndex);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
