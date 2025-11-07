using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

[CustomEditor(typeof(MonoBehaviour), true)]
[CanEditMultipleObjects]
public class ShowIfEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty prop = serializedObject.GetIterator();
        bool enterChildren = true;

        while (prop.NextVisible(enterChildren))
        {
            enterChildren = true;

            // 스크립트 필드는 항상 표시
            if (prop.name == "m_Script")
            {
                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.PropertyField(prop, true);
                continue;
            }

            // 현재 필드가 리스트나 배열인지 확인
            bool isListRoot = prop.isArray && prop.propertyType == SerializedPropertyType.Generic;

            // 필드 정보 찾기
            FieldInfo field = prop.serializedObject.targetObject.GetType()
                               .GetField(prop.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            bool showField = true;

            if (field != null)
            {
                var showIfAttrs = field.GetCustomAttributes(typeof(ShowIfAttribute), true)
                                       .Cast<ShowIfAttribute>()
                                       .ToArray();

                foreach (var showIf in showIfAttrs)
                {
                    SerializedProperty conditionProp = serializedObject.FindProperty(showIf.ConditionFieldName);

                    if (conditionProp == null ||
                        conditionProp.propertyType != SerializedPropertyType.Boolean ||
                        !conditionProp.boolValue)
                    {
                        showField = false;
                        break;
                    }
                }
            }

            if (!showField)
            {
                // 리스트 내부 자식은 건너뜀
                enterChildren = false;
                continue;
            }

            // 리스트 루트면 한 번만 그리기
            if (isListRoot)
            {
                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false; // 내부 요소는 수동으로 그리지 않음
                continue;
            }

            // 일반 필드
            EditorGUILayout.PropertyField(prop, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
