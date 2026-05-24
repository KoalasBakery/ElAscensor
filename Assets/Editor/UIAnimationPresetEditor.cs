using UnityEditor;
using UnityEngine;
using static UIAnimationPreset;

[CustomEditor(typeof(UIAnimationPreset))]
public class UIAnimationPresetEditor : Editor
{
    private static readonly Color headerColor = new Color(0.15f, 0.15f, 0.15f);
    private static readonly Color sectionColor = new Color(0.2f, 0.2f, 0.25f);
    private static readonly Color accentColor = new Color(0.3f, 0.6f, 1f);
    private static readonly Color fadeColor = new Color(0.4f, 0.9f, 0.4f);
    private static readonly Color slideColor = new Color(1f, 0.6f, 0.2f);
    private static readonly Color scaleColor = new Color(0.8f, 0.4f, 1f);
    private static readonly Color shakeColor = new Color(1f, 0.3f, 0.3f);
    private static readonly Color rotateColor = new Color(0.3f, 0.8f, 1f);

    private UIAnimationPreset preset;

    private void OnEnable()
    {
        preset = (UIAnimationPreset)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawHeader();
        EditorGUILayout.Space(5);
        DrawGeneral();
        EditorGUILayout.Space(5);
        DrawEaseSettings();
        EditorGUILayout.Space(5);
        DrawAnimationSpecific();
        EditorGUILayout.Space(5);
        DrawInfo();

        serializedObject.ApplyModifiedProperties();
    }

    private new void DrawHeader()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 50);
        EditorGUI.DrawRect(rect, headerColor);

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = GetTypeColor() }
        };
        GUIStyle subtitleStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 10,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.gray }
        };

        Rect titleRect = new Rect(rect.x, rect.y + 5, rect.width, 25);
        Rect subtitleRect = new Rect(rect.x, rect.y + 28, rect.width, 15);

        EditorGUI.LabelField(titleRect, "Animation Preset", titleStyle);
        EditorGUI.LabelField(subtitleRect, preset.animationType.ToString(), subtitleStyle);
    }

    private void DrawGeneral()
    {
        DrawSection("Configuracion General", accentColor, () =>
        {
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("animationType"),
                new GUIContent("Tipo de Animacion",
                "Tipo de animacion que se va a reproducir."));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("duration"),
                new GUIContent("Duracion",
                "Duracion de la animacion en segundos."));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("delay"),
                new GUIContent("Delay",
                "Tiempo de espera antes de iniciar la animacion."));
        });
    }

    private void DrawEaseSettings()
    {
        DrawSection("Ease", accentColor, () =>
        {
            SerializedProperty easeTypeProp = serializedObject.FindProperty("easeType");
            EditorGUILayout.PropertyField(easeTypeProp,
                new GUIContent("Tipo de Ease",
                "Controla la velocidad en cada punto de la animacion.\n\n" +
                "Linear    = velocidad constante\n" +
                "EaseIn    = arranca lento, termina rapido\n" +
                "EaseOut   = arranca rapido, termina lento\n" +
                "EaseInOut = suave al inicio y al final\n" +
                "Bounce    = rebota al llegar\n" +
                "Elastic   = efecto elastico al llegar\n" +
                "BackIn    = se echa hacia atras antes de avanzar\n" +
                "BackOut   = se pasa y regresa al destino"));

            // Preview visual del ease
            EditorGUILayout.Space(3);
            DrawEasePreview(preset.easeType);

            // Curva personalizada solo si es necesario
            if (preset.easeType == EaseType.Linear)
            {
                EditorGUILayout.HelpBox(
                    "Linear: velocidad constante de inicio a fin.",
                    MessageType.Info);
            }

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("customCurve"),
                new GUIContent("Curva Personalizada",
                "Se usa como fallback si el ease type no tiene formula definida."));
        });
    }

    private void DrawEasePreview(EaseType easeType)
    {
        // INTENTO xd de descripcion visual del ease seleccionado
        string description = easeType switch
        {
            EaseType.Linear => "-> -> -> -> -> -> -> ->",
            EaseType.EaseIn => "-> ->  ->   ->    ->",
            EaseType.EaseOut => "->    ->   ->  -> ->",
            EaseType.EaseInOut => "-> ->  ->  ->  -> ->",
            EaseType.Bounce => "->        -> ~ ~ ~",
            EaseType.Elastic => "->        ->  ~  ~",
            EaseType.BackIn => "<- -> ->        ->",
            EaseType.BackOut => "->        -> <- ->",
            _ => ""
        };

        if (!string.IsNullOrEmpty(description))
        {
            GUIStyle previewStyle = new GUIStyle(EditorStyles.helpBox)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = GetTypeColor() },
                fontSize = 11
            };
            EditorGUILayout.LabelField(description, previewStyle,
                GUILayout.Height(25));
        }
    }

    private void DrawAnimationSpecific()
    {
        switch (preset.animationType)
        {
            case UIAnimationType.FadeIn:
            case UIAnimationType.FadeOut:
                DrawFadeSettings();
                break;

            case UIAnimationType.SlideInRight:
            case UIAnimationType.SlideInLeft:
            case UIAnimationType.SlideInUp:
            case UIAnimationType.SlideInDown:
            case UIAnimationType.SlideOutRight:
            case UIAnimationType.SlideOutLeft:
            case UIAnimationType.SlideOutUp:
            case UIAnimationType.SlideOutDown:
                DrawSlideSettings();
                break;

            case UIAnimationType.ScaleIn:
            case UIAnimationType.ScaleOut:
                DrawScaleSettings();
                break;

            case UIAnimationType.Shake:
                DrawShakeSettings();
                break;

            case UIAnimationType.Punch:
                DrawPunchSettings();
                break;

            case UIAnimationType.Rotate:
                DrawRotateSettings();
                break;

            case UIAnimationType.Loop:
                DrawLoopSettings();
                break;

            case UIAnimationType.ColorTween:
                DrawColorTweenSettings();
                break;

            case UIAnimationType.FillAmount:
                DrawFillAmountSettings();
                break;

            case UIAnimationType.Stagger:
                DrawStaggerSettings();
                break;
        }
    }

    private void DrawFadeSettings()
    {
        DrawSection("Configuracion Fade", fadeColor, () =>
        {
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("fadeFrom"),
                new GUIContent("Alpha Inicial",
                "Valor de alpha al inicio. 0 = transparente, 1 = opaco."));
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("fadeTo"),
                new GUIContent("Alpha Final",
                "Valor de alpha al final. 0 = transparente, 1 = opaco."));

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUIStyle style = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = fadeColor }
            };
            EditorGUILayout.LabelField(
                $"Alpha: {preset.fadeFrom:F1} -> {preset.fadeTo:F1}", style);
            EditorGUILayout.EndHorizontal();
        });
    }

    private void DrawSlideSettings()
    {
        DrawSection("Configuracion Slide", slideColor, () =>
        {
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("slideDistance"),
                new GUIContent("Distancia",
                "Distancia en pixeles que se desplaza el objeto."));

            string direction = preset.animationType.ToString()
                .Replace("SlideIn", "Entra desde: ")
                .Replace("SlideOut", "Sale hacia: ");

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUIStyle style = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = slideColor }
            };
            EditorGUILayout.LabelField(direction, style);
            EditorGUILayout.EndHorizontal();
        });
    }

    private void DrawScaleSettings()
    {
        DrawSection("Configuracion Scale", scaleColor, () =>
        {
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("scaleFrom"),
                new GUIContent("Scale Inicial",
                "Escala al inicio. 0 = invisible, 1 = tamanio normal."));
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("scaleTo"),
                new GUIContent("Scale Final",
                "Escala al final. 0 = invisible, 1 = tamanio normal."));

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUIStyle style = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = scaleColor }
            };
            EditorGUILayout.LabelField(
                $"Scale: {preset.scaleFrom:F1}x -> {preset.scaleTo:F1}x", style);
            EditorGUILayout.EndHorizontal();
        });
    }

    private void DrawShakeSettings()
    {
        DrawSection("Configuracion Shake", shakeColor, () =>
        {
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("shakeStrength"),
                new GUIContent("Intensidad",
                "Que tan fuerte es el shake. Valores mayores = mas violento."));
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("shakeVibrato"),
                new GUIContent("Vibraciones",
                "Cuantas veces vibra por segundo."));
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("shakeFadeOut"),
                new GUIContent("Fade Out",
                "Si el shake se va suavizando hasta detenerse."));
        });
    }

    private void DrawPunchSettings()
    {
        DrawSection("Configuracion Punch", scaleColor, () =>
        {
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("punchStrength"),
                new GUIContent("Fuerza",
                "Que tan grande es el punch. 1 = duplica el tamanio momentaneamente."));
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("punchVibrato"),
                new GUIContent("Vibraciones",
                "Cuantas veces rebota antes de detenerse."));
        });
    }

    private void DrawRotateSettings()
    {
        DrawSection("Configuracion Rotate", rotateColor, () =>
        {
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("rotateFrom"),
                new GUIContent("Rotacion Inicial",
                "Angulo inicial en grados."));
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("rotateTo"),
                new GUIContent("Rotacion Final",
                "Angulo final en grados."));
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("rotateLoop"),
                new GUIContent("Loop",
                "Si la rotacion se repite indefinidamente."));

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUIStyle style = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = rotateColor }
            };
            EditorGUILayout.LabelField(
                $"Rotacion: {preset.rotateFrom:F0} -> {preset.rotateTo:F0} grados",
                style);
            EditorGUILayout.EndHorizontal();
        });
    }

    private void DrawLoopSettings()
    {
        DrawSection("Configuracion Loop", accentColor, () =>
        {
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("loopType"),
                new GUIContent("Tipo de Loop",
                "Restart = vuelve al inicio cada vez\n" +
                "PingPong = va y viene"));
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("loopCount"),
                new GUIContent("Repeticiones",
                "Cuantas veces se repite. -1 = infinito."));
        });
    }

    private void DrawColorTweenSettings()
    {
        DrawSection("Configuracion Color", fadeColor, () =>
        {
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("colorFrom"),
                new GUIContent("Color Inicial",
                "Color al inicio de la animacion."));
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("colorTo"),
                new GUIContent("Color Final",
                "Color al final de la animacion."));

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUI.DrawRect(
                EditorGUILayout.GetControlRect(false, 20, GUILayout.Width(60)),
                preset.colorFrom);
            GUIStyle arrow = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("->", arrow, GUILayout.Width(30));
            EditorGUI.DrawRect(
                EditorGUILayout.GetControlRect(false, 20, GUILayout.Width(60)),
                preset.colorTo);
            EditorGUILayout.EndHorizontal();
        });
    }

    private void DrawFillAmountSettings()
    {
        DrawSection("Configuracion Fill", accentColor, () =>
        {
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("fillFrom"),
                new GUIContent("Fill Inicial",
                "Fill amount al inicio. 0 = vacio, 1 = lleno."));
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("fillTo"),
                new GUIContent("Fill Final",
                "Fill amount al final. 0 = vacio, 1 = lleno."));

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUIStyle style = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = accentColor }
            };
            EditorGUILayout.LabelField(
                $"Fill: {preset.fillFrom:F2} -> {preset.fillTo:F2}", style);
            EditorGUILayout.EndHorizontal();
        });
    }

    private void DrawStaggerSettings()
    {
        DrawSection("Configuracion Stagger", slideColor, () =>
        {
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("staggerDelay"),
                new GUIContent("Delay entre elementos",
                "Tiempo de espera entre la animacion de cada elemento."));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("slideDistance"),
                new GUIContent("Distancia de entrada",
                "Distancia desde donde aparece cada elemento."));

            EditorGUILayout.HelpBox(
                "Usar con UIAnimationManager.Instance.PlayStagger()",
                MessageType.Info);
        });
    }

    private void DrawInfo()
    {
        DrawSection("Info", accentColor, () =>
        {
            EditorGUILayout.BeginHorizontal();
            DrawStatBadge("Duracion", $"{preset.duration:F1}s", accentColor);
            DrawStatBadge("Delay", $"{preset.delay:F1}s", Color.gray);
            DrawStatBadge("Ease", preset.easeType.ToString(), GetTypeColor());
            EditorGUILayout.EndHorizontal();
        });
    }

    // --- HELPERS --- //
    private Color GetTypeColor()
    {
        if (preset == null) return accentColor;
        string typeName = preset.animationType.ToString();
        if (typeName.Contains("Fade")) return fadeColor;
        if (typeName.Contains("Slide")) return slideColor;
        if (typeName.Contains("Scale") ||
            typeName.Contains("Punch")) return scaleColor;
        if (typeName.Contains("Shake")) return shakeColor;
        if (typeName.Contains("Rotate")) return rotateColor;
        return accentColor;
    }

    private void DrawSection(string title, Color color, System.Action content)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 24);
        EditorGUI.DrawRect(rect, sectionColor);

        GUIStyle sectionStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            normal = { textColor = color },
            padding = new RectOffset(8, 0, 4, 0)
        };
        EditorGUI.LabelField(rect, title, sectionStyle);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.Space(3);
        content?.Invoke();
        EditorGUILayout.Space(3);
        EditorGUILayout.EndVertical();
    }

    private void DrawStatBadge(string label, string value, Color color)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUIStyle valueStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = color }
        };
        GUIStyle labelStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.gray }
        };
        EditorGUILayout.LabelField(value, valueStyle, GUILayout.Height(20));
        EditorGUILayout.LabelField(label, labelStyle);
        EditorGUILayout.EndVertical();
    }
}