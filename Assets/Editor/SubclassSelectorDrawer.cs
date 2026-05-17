using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

[CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
public class SubclassSelectorDrawer : PropertyDrawer
{
    private static readonly float HelpBoxHeight = EditorGUIUtility.singleLineHeight * 2.5f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.ManagedReference)
        {
            EditorGUI.HelpBox(position, "SubclassSelector solo funciona con [SerializeReference]", MessageType.Error);
            return;
        }

        // Rectángulos
        Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect dropdownRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);

        // Tipo actual
        Type baseType = fieldInfo.FieldType;
        Type currentType = property.managedReferenceValue?.GetType();

        // Dibujar label
        EditorGUI.LabelField(labelRect, label);

        // Dropdown
        string currentTypeName = currentType != null ? currentType.Name : "Null (None)";

        if (EditorGUI.DropdownButton(dropdownRect, new GUIContent(currentTypeName), FocusType.Keyboard))
        {
            GenericMenu menu = new GenericMenu();

            // Opción None
            menu.AddItem(new GUIContent("None"), currentType == null, () =>
            {
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            });

            menu.AddSeparator("");

            // Todas las subclases
            var types = GetAllSubclasses(baseType);
            foreach (var type in types)
            {
                bool isSelected = type == currentType;
                string name = ObjectNames.NicifyVariableName(type.Name);

                menu.AddItem(new GUIContent(name), isSelected, () =>
                {
                    // Crear nueva instancia del tipo seleccionado
                    object instance = Activator.CreateInstance(type);
                    property.managedReferenceValue = instance;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }

        // Dibujar los campos del objeto referenciado
        if (property.managedReferenceValue != null)
        {
            Rect contentRect = new Rect(position.x, dropdownRect.yMax + 4, position.width, position.height);
            EditorGUI.PropertyField(contentRect, property, GUIContent.none, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.ManagedReference)
            return EditorGUIUtility.singleLineHeight * 3;

        float height = EditorGUIUtility.singleLineHeight * 2 + 6; // label + dropdown

        if (property.managedReferenceValue != null)
        {
            height += EditorGUI.GetPropertyHeight(property, true);
        }

        return height;
    }

    private List<Type> GetAllSubclasses(Type baseType)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t))
            .OrderBy(t => t.Name)
            .ToList();
    }
}