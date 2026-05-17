using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TMPro;

[CustomEditor(typeof(DialogueData))]
public class DialogueDataEditor : Editor
{
    private static readonly Color headerColor = new Color(0.15f, 0.15f, 0.15f);
    private static readonly Color sectionColor = new Color(0.2f, 0.2f, 0.25f);
    private static readonly Color accentColor = new Color(0.3f, 0.6f, 1f);
    private static readonly Color warningColor = new Color(1f, 0.8f, 0.2f);
    private static readonly Color successColor = new Color(0.4f, 0.9f, 0.4f);
    private static readonly Color choiceColor = new Color(0.8f, 0.4f, 1f);
    private static readonly Color flagColor = new Color(1f, 0.5f, 0.2f);

    private DialogueData data;
    private List<bool> lineFoldouts = new List<bool>();

    private void OnEnable()
    {
        data = (DialogueData)target;
    }

    private void RefreshFoldouts(int count)
    {
        while (lineFoldouts.Count < count) lineFoldouts.Add(true);
        while (lineFoldouts.Count > count) lineFoldouts.RemoveAt(lineFoldouts.Count - 1);
    }

    public override void OnInspectorGUI()
    {
        if (data == null) return;
        serializedObject.Update();

        SerializedProperty linesProp = serializedObject.FindProperty("lines");
        RefreshFoldouts(linesProp.arraySize);

        DrawHeader(linesProp.arraySize);
        EditorGUILayout.Space(5);
        DrawSettings();
        EditorGUILayout.Space(5);
        DrawLines(linesProp);
        EditorGUILayout.Space(5);
        DrawAddLineButton(linesProp);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawHeader(int lineCount)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 50);
        EditorGUI.DrawRect(rect, headerColor);

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = accentColor }
        };
        GUIStyle subtitleStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 10,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.gray }
        };

        Rect titleRect = new Rect(rect.x, rect.y + 5, rect.width, 25);
        Rect subtitleRect = new Rect(rect.x, rect.y + 28, rect.width, 15);

        EditorGUI.LabelField(titleRect, "Dialogue Data", titleStyle);
        EditorGUI.LabelField(subtitleRect, $"{lineCount} linea(s)", subtitleStyle);
    }

    private void DrawSettings()
    {
        DrawSection("Configuracion", () =>
        {
            SerializedProperty isRepeatable = serializedObject.FindProperty("isRepeatable");
            SerializedProperty completedFlag = serializedObject.FindProperty("completedFlagKey");

            EditorGUILayout.PropertyField(isRepeatable,
                new GUIContent("Repetible",
                "Si esta activo, el dialogo puede iniciarse multiples veces."));

            EditorGUILayout.PropertyField(completedFlag,
                new GUIContent("Flag al completar",
                "Esta flag se activa automaticamente cuando termina el dialogo."));

            if (!string.IsNullOrEmpty(completedFlag.stringValue))
                EditorGUILayout.HelpBox(
                    $"Al terminar este dialogo se activara: \"{completedFlag.stringValue}\"",
                    MessageType.Info);
        });
    }

    private void DrawLines(SerializedProperty linesProp)
    {
        DrawSection("Lineas de Dialogo", () =>
        {
            if (linesProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox(
                    "No hay lineas. Agrega una con el boton de abajo.",
                    MessageType.Warning);
                return;
            }

            for (int i = 0; i < linesProp.arraySize; i++)
            {
                SerializedProperty lineProp = linesProp.GetArrayElementAtIndex(i);
                DrawLine(lineProp, i, linesProp);
                EditorGUILayout.Space(4);
            }
        });
    }

    private void DrawLine(SerializedProperty lineProp, int index, SerializedProperty linesProp)
    {
        SerializedProperty hasChoicesProp = lineProp.FindPropertyRelative("hasChoices");
        SerializedProperty flagKeyProp = lineProp.FindPropertyRelative("setFlagKey");
        SerializedProperty textKeyProp = lineProp.FindPropertyRelative("dialogueTextKey");

        bool hasChoices = hasChoicesProp.boolValue;
        string flagKey = flagKeyProp.stringValue;
        string textKey = textKeyProp.stringValue;

        // Color del header segun tipo
        Color lineColor = hasChoices ? choiceColor :
                         !string.IsNullOrEmpty(flagKey) ? flagColor :
                         sectionColor;

        Rect headerRect = EditorGUILayout.GetControlRect(false, 28);
        EditorGUI.DrawRect(headerRect, lineColor * 0.7f);

        string lineIcon = hasChoices ? "[CHOICE]" :
                          !string.IsNullOrEmpty(flagKey) ? "[FLAG]" : "[LINE]";
        string preview = string.IsNullOrEmpty(textKey) ?
                          "Sin texto asignado" : $"Key: {textKey}";
        string lineTitle = $"{lineIcon} Linea {index + 1}  --  {preview}";

        GUIStyle headerStyle = new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white },
            onNormal = { textColor = Color.white }
        };

        Rect foldoutRect = new Rect(
            headerRect.x + 5, headerRect.y + 5,
            headerRect.width - 90, headerRect.height);

        lineFoldouts[index] = EditorGUI.Foldout(
            foldoutRect, lineFoldouts[index], lineTitle, true, headerStyle);

        // Boton subir
        Rect btnRect = new Rect(
            headerRect.x + headerRect.width - 82,
            headerRect.y + 4, 25, 20);

        if (index > 0 && GUI.Button(btnRect, "^"))
        {
            linesProp.MoveArrayElement(index, index - 1);
            return;
        }

        // Boton bajar
        btnRect.x += 27;
        if (index < linesProp.arraySize - 1 && GUI.Button(btnRect, "v"))
        {
            linesProp.MoveArrayElement(index, index + 1);
            return;
        }

        // Boton eliminar
        btnRect.x += 27;
        btnRect.width = 25;
        GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
        if (GUI.Button(btnRect, "X"))
        {
            linesProp.DeleteArrayElementAtIndex(index);
            if (index < lineFoldouts.Count)
                lineFoldouts.RemoveAt(index);
            GUI.backgroundColor = Color.white;
            return;
        }
        GUI.backgroundColor = Color.white;

        if (!lineFoldouts[index]) return;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.Space(3);

        // Localizacion
        DrawSubSection("Localizacion", accentColor, () =>
        {
            EditorGUILayout.PropertyField(
                lineProp.FindPropertyRelative("speakerNameKey"),
                new GUIContent("Key del nombre",
                "Key en la Localization Table para el nombre. Vacio = usa entityName."));
            EditorGUILayout.PropertyField(
                lineProp.FindPropertyRelative("dialogueTextKey"),
                new GUIContent("Key del texto",
                "Key en la Localization Table. Ejemplo: Evan.Saludo01"));
        });

        EditorGUILayout.Space(3);

        // Portrait
        DrawSubSection("Portrait", successColor, () =>
        {
            EditorGUILayout.PropertyField(
                lineProp.FindPropertyRelative("speakerPortrait"),
                new GUIContent("Imagen del speaker",
                "Sprite del personaje. Opcional."));
        });

        EditorGUILayout.Space(3);

        // Estilo
        DrawSubSection("Estilo de Texto", warningColor, () =>
        {
            EditorGUILayout.PropertyField(
                lineProp.FindPropertyRelative("customFont"),
                new GUIContent("Tipografia",
                "Fuente personalizada para esta linea. Vacio = fuente por defecto."));
            EditorGUILayout.PropertyField(
                lineProp.FindPropertyRelative("textColor"),
                new GUIContent("Color del texto",
                "Blanco = color por defecto."));
            EditorGUILayout.PropertyField(
                lineProp.FindPropertyRelative("fontSize"),
                new GUIContent("Tamano del texto",
                "0 = tamano por defecto del DialogueUI."));
            EditorGUILayout.PropertyField(
                lineProp.FindPropertyRelative("textEffect"),
                new GUIContent("Efecto",
                "Wave = ondulacion, Shake = temblor, FadeIn = aparicion suave."));
            EditorGUILayout.PropertyField(
                lineProp.FindPropertyRelative("customTypingSpeed"),
                new GUIContent("Velocidad typewriter",
                "0 = velocidad por defecto del DialogueUI."));
        });

        EditorGUILayout.Space(3);

        // Choices
        DrawSubSection("Choices", choiceColor, () =>
        {
            EditorGUILayout.PropertyField(hasChoicesProp,
                new GUIContent("Tiene opciones",
                "El jugador vera botones para elegir una respuesta."));

            if (hasChoicesProp.boolValue)
            {
                SerializedProperty choices = lineProp.FindPropertyRelative("choices");
                EditorGUILayout.Space(3);

                if (choices.arraySize == 0)
                    EditorGUILayout.HelpBox(
                        "Agrega al menos una opcion.", MessageType.Warning);

                for (int c = 0; c < choices.arraySize; c++)
                {
                    DrawChoice(choices.GetArrayElementAtIndex(c), c, choices);
                    EditorGUILayout.Space(2);
                }

                EditorGUILayout.Space(3);
                if (GUILayout.Button("+ Agregar Opcion", GUILayout.Height(22)))
                    choices.arraySize++;
            }
        });

        EditorGUILayout.Space(3);

        // Eventos
        DrawSubSection("Eventos", flagColor, () =>
        {
            EditorGUILayout.PropertyField(flagKeyProp,
                new GUIContent("Activar flag",
                "Flag que se activa al mostrar esta linea."));

            if (!string.IsNullOrEmpty(flagKeyProp.stringValue))
                EditorGUILayout.PropertyField(
                    lineProp.FindPropertyRelative("setFlagValue"),
                    new GUIContent("Valor de la flag"));

            EditorGUILayout.PropertyField(
                lineProp.FindPropertyRelative("nextDialogue"),
                new GUIContent("Siguiente dialogo",
                "Si se asigna, al terminar esta linea inicia este dialogo."));
        });

        EditorGUILayout.Space(3);
        EditorGUILayout.EndVertical();
    }

    private void DrawChoice(SerializedProperty choice, int index, SerializedProperty choices)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();
        GUIStyle choiceLabelStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            normal = { textColor = choiceColor }
        };
        EditorGUILayout.LabelField($"Opcion {index + 1}", choiceLabelStyle);

        GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
        if (GUILayout.Button("X", GUILayout.Width(25)))
        {
            choices.DeleteArrayElementAtIndex(index);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            return;
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(
            choice.FindPropertyRelative("choiceTextKey"),
            new GUIContent("Key del texto",
            "Key en la Localization Table para el texto de esta opcion."));

        EditorGUILayout.PropertyField(
            choice.FindPropertyRelative("nextDialogue"),
            new GUIContent("Dialogo siguiente",
            "DialogueData que se inicia al elegir esta opcion."));

        EditorGUILayout.PropertyField(
            choice.FindPropertyRelative("requiredFlagKey"),
            new GUIContent("Flag requerida",
            "Esta opcion solo aparece si la flag esta activa. Vacio = siempre visible."));

        SerializedProperty setFlagKey = choice.FindPropertyRelative("setFlagKey");
        EditorGUILayout.PropertyField(setFlagKey,
            new GUIContent("Activar flag",
            "Flag que se activa al elegir esta opcion."));

        if (!string.IsNullOrEmpty(setFlagKey.stringValue))
            EditorGUILayout.PropertyField(
                choice.FindPropertyRelative("setFlagValue"),
                new GUIContent("Valor de la flag"));

        EditorGUILayout.EndVertical();
    }

    private void DrawAddLineButton(SerializedProperty linesProp)
    {
        GUI.backgroundColor = successColor;
        if (GUILayout.Button("+ Agregar Linea", GUILayout.Height(35)))
        {
            linesProp.arraySize++;
            lineFoldouts.Add(true);
        }
        GUI.backgroundColor = Color.white;
    }

    private void DrawSection(string title, System.Action content)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 24);
        EditorGUI.DrawRect(rect, sectionColor);

        GUIStyle sectionStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            normal = { textColor = Color.white },
            padding = new RectOffset(8, 0, 4, 0)
        };
        EditorGUI.LabelField(rect, title, sectionStyle);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.Space(3);
        content?.Invoke();
        EditorGUILayout.Space(3);
        EditorGUILayout.EndVertical();
    }

    private void DrawSubSection(string title, Color color, System.Action content)
    {
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
        {
            normal = { textColor = color },
            fontSize = 11
        };
        EditorGUILayout.LabelField(title, style);
        content?.Invoke();
    }
}