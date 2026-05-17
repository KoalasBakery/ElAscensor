using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(DialogueContainer))]
public class DialogueContainerEditor : Editor
{
    // Colores del editor
    private static readonly Color headerColor = new Color(0.15f, 0.15f, 0.15f);
    private static readonly Color sectionColor = new Color(0.2f, 0.2f, 0.25f);
    private static readonly Color accentColor = new Color(0.3f, 0.6f, 1f);
    private static readonly Color warningColor = new Color(1f, 0.8f, 0.2f);
    private static readonly Color successColor = new Color(0.4f, 0.9f, 0.4f);
    private static readonly Color dangerColor = new Color(1f, 0.4f, 0.4f);

    private DialogueContainer container;
    private bool showAllDialogues = true;

    private void OnEnable()
    {
        container = (DialogueContainer)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawHeader();
        EditorGUILayout.Space(5);
        DrawInfo();
        EditorGUILayout.Space(5);
        DrawMainDialogues();
        EditorGUILayout.Space(5);
        DrawAllDialogues();
        EditorGUILayout.Space(5);
        DrawActions();

        serializedObject.ApplyModifiedProperties();
    }

    // --- HEADER --- //
    private new void DrawHeader()
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

        EditorGUI.LabelField(titleRect, "Dialogue Container", titleStyle);
        EditorGUI.LabelField(subtitleRect,
            string.IsNullOrEmpty(container.containerName) ?
            "Sin nombre" : container.containerName, subtitleStyle);
    }

    // --- INFO --- //
    private void DrawInfo()
    {
        DrawSection("Información", () =>
        {
            SerializedProperty nameProp = serializedObject.FindProperty("containerName");
            EditorGUILayout.PropertyField(nameProp,
                new GUIContent("Nombre", "Identificador único de este contenedor (ej: Evan, Baul_Cuarto1)"));

            EditorGUILayout.Space(3);

            // EstadIsticas
            int totalDialogues = container.allDialogues?.Count ?? 0;
            int totalLines = 0;
            int totalChoices = 0;

            if (container.allDialogues != null)
            {
                foreach (var d in container.allDialogues)
                {
                    if (d == null) continue;
                    totalLines += d.lines?.Count ?? 0;
                    foreach (var line in d.lines ?? new List<DialogueData.DialogueLine>())
                        if (line.hasChoices)
                            totalChoices += line.choices?.Count ?? 0;
                }
            }

            EditorGUILayout.BeginHorizontal();
            DrawStatBadge("Diálogos", totalDialogues.ToString(), accentColor);
            DrawStatBadge("Líneas", totalLines.ToString(), successColor);
            DrawStatBadge("Choices", totalChoices.ToString(), warningColor);
            EditorGUILayout.EndHorizontal();
        });
    }

    // --- DIALOGOS PRINCIPALES --- //
    private void DrawMainDialogues()
    {
        DrawSection("Diálogos Principales", () =>
        {
            SerializedProperty initialProp = serializedObject.FindProperty("initialDialogue");
            SerializedProperty repeatingProp = serializedObject.FindProperty("repeatingDialogue");

            // Initial
            EditorGUILayout.BeginHorizontal();
            DrawColorDot(successColor);
            EditorGUILayout.PropertyField(initialProp,
                new GUIContent("Inicial", "Primer diálogo al interactuar por primera vez"));
            EditorGUILayout.EndHorizontal();

            if (container.initialDialogue == null)
                DrawWarningBox("⚠️ Sin diálogo inicial asignado");

            EditorGUILayout.Space(3);

            // Repeating
            EditorGUILayout.BeginHorizontal();
            DrawColorDot(warningColor);
            EditorGUILayout.PropertyField(repeatingProp,
                new GUIContent("Repetición", "Diálogo que se muestra si ya completó el inicial"));
            EditorGUILayout.EndHorizontal();

            if (container.repeatingDialogue == null)
                DrawInfoBox("Sin diálogo de repetición (opcional)");
        });
    }

    // --- TODOS LOS DIALOGOS --- //
    private void DrawAllDialogues()
    {
        DrawSection("Todos los Diálogos", () =>
        {
            showAllDialogues = EditorGUILayout.Foldout(showAllDialogues,
                $"Diálogos ({container.allDialogues?.Count ?? 0})", true);

            if (!showAllDialogues) return;

            SerializedProperty listProp = serializedObject.FindProperty("allDialogues");

            if (container.allDialogues == null || container.allDialogues.Count == 0)
            {
                DrawWarningBox("⚠️ No hay diálogos en el contenedor");
            }
            else
            {
                for (int i = 0; i < listProp.arraySize; i++)
                {
                    SerializedProperty element = listProp.GetArrayElementAtIndex(i);
                    DialogueData dialogue = element.objectReferenceValue as DialogueData;

                    EditorGUILayout.BeginHorizontal();

                    // Indicador de estado
                    if (dialogue == null)
                        DrawColorDot(dangerColor);
                    else if (dialogue == container.initialDialogue)
                        DrawColorDot(successColor);
                    else if (dialogue == container.repeatingDialogue)
                        DrawColorDot(warningColor);
                    else
                        DrawColorDot(accentColor);

                    // Campo
                    EditorGUILayout.PropertyField(element,
                        new GUIContent(dialogue != null ? dialogue.name : $"Diálogo {i + 1}"));

                    // Info rapida
                    if (dialogue != null)
                    {
                        int lineCount = dialogue.lines?.Count ?? 0;
                        GUIStyle badgeStyle = new GUIStyle(EditorStyles.miniLabel)
                        {
                            normal = { textColor = Color.gray }
                        };
                        EditorGUILayout.LabelField($"{lineCount} líneas",
                            badgeStyle, GUILayout.Width(60));
                    }

                    // Boton eliminar
                    if (GUILayout.Button("X", GUILayout.Width(25)))
                    {
                        listProp.DeleteArrayElementAtIndex(i);
                        break;
                    }

                    EditorGUILayout.EndHorizontal();

                    // Null warning
                    if (dialogue == null)
                        DrawWarningBox("⚠️ Referencia vacía, elimínala o asigna un diálogo");
                }
            }

            EditorGUILayout.Space(5);

            // Boton agregar
            if (GUILayout.Button("+ Agregar Diálogo Existente", GUILayout.Height(25)))
            {
                listProp.arraySize++;
                listProp.GetArrayElementAtIndex(listProp.arraySize - 1)
                    .objectReferenceValue = null;
            }
        });
    }

    // --- ACCIONES --- //
    private void DrawActions()
    {
        DrawSection("Acciones Rápidas", () =>
        {
            EditorGUILayout.HelpBox(
                "Crea un nuevo DialogueData y lo agrega automáticamente al contenedor.",
                MessageType.Info);

            EditorGUILayout.Space(3);

            if (GUILayout.Button("Crear Nuevo DialogueData", GUILayout.Height(30)))
            {
                CreateNewDialogueData();
            }

            EditorGUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Asignar como Inicial", GUILayout.Height(25)))
            {
                AssignFirstAsInitial();
            }

            if (GUILayout.Button("Asignar como Repetición", GUILayout.Height(25)))
            {
                AssignFirstAsRepeating();
            }

            EditorGUILayout.EndHorizontal();
        });
    }

    // --- HELPERS --- //
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

    private void DrawColorDot(Color color)
    {
        Rect rect = EditorGUILayout.GetControlRect(false,
            EditorGUIUtility.singleLineHeight, GUILayout.Width(12));
        rect.y += 3;
        rect.width = 8;
        rect.height = 8;
        EditorGUI.DrawRect(rect, color);
    }

    private void DrawStatBadge(string label, string value, Color color)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUIStyle valueStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = color }
        };
        GUIStyle labelStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.gray }
        };
        EditorGUILayout.LabelField(value, valueStyle, GUILayout.Height(25));
        EditorGUILayout.LabelField(label, labelStyle);
        EditorGUILayout.EndVertical();
    }

    private void DrawWarningBox(string message)
    {
        EditorGUILayout.HelpBox(message, MessageType.Warning);
    }

    private void DrawInfoBox(string message)
    {
        EditorGUILayout.HelpBox(message, MessageType.Info);
    }

    private void CreateNewDialogueData()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Crear DialogueData",
            "NewDialogue",
            "asset",
            "Elige dónde guardar el nuevo DialogueData");

        if (string.IsNullOrEmpty(path)) return;

        DialogueData newDialogue = CreateInstance<DialogueData>();
        AssetDatabase.CreateAsset(newDialogue, path);
        AssetDatabase.SaveAssets();

        if (container.allDialogues == null)
            container.allDialogues = new List<DialogueData>();

        container.allDialogues.Add(newDialogue);
        EditorUtility.SetDirty(container);
        AssetDatabase.SaveAssets();

        Selection.activeObject = newDialogue;
    }

    private void AssignFirstAsInitial()
    {
        if (container.allDialogues != null && container.allDialogues.Count > 0)
        {
            container.initialDialogue = container.allDialogues[0];
            EditorUtility.SetDirty(container);
        }
    }

    private void AssignFirstAsRepeating()
    {
        if (container.allDialogues != null && container.allDialogues.Count > 1)
        {
            container.repeatingDialogue = container.allDialogues[1];
            EditorUtility.SetDirty(container);
        }
    }
}