using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary> Inspector上での変数名表記を変更します </summary>
public class LabelAttribute : PropertyAttribute
{
    public readonly string Value;

    public LabelAttribute(string value) => Value = value;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var newLabel = attribute as LabelAttribute;

        EditorGUI.PropertyField(position, property, new GUIContent(newLabel.Value), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property, true);
}
#endif