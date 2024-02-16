using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary> Inspector上での編集を禁止します </summary>
public class UnEditableAttribute : PropertyAttribute
{
    public UnEditableAttribute() { }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(UnEditableAttribute))]
public class UnEditableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif