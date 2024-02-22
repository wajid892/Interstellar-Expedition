using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.IMGUI.Controls;

[CustomEditor(typeof(AsteroidFieldCreator))]
public class AsteroidFieldCreatorEditor : Editor
{
    //---------------------------------------------
    // useReorderableList
    //---------------------------------------------
    // Set useReorderableList to false to use
    // the standard array list in the inspector
    private bool useReorderableList = true;

    //---------------------------------------------
    // turnOnPrefabLink
    //---------------------------------------------
    // turnOnPrefabLink may or may not work in your Unity version.
    // Seems to be ok in 2018 and up. set to true to use.
    private bool turnOnPrefabLink = true;

    //-----------------------------------------------------
    // End optional configuration - messing with anything 
    // below here may break it.
    //-----------------------------------------------------
    private SerializedProperty totalObjects;
    private SerializedProperty valueForProgressBar;
    private SerializedProperty objectPrefabs;
    private SerializedProperty enablePrefabLink;
    private SerializedProperty keepPrefabLink;
    private SerializedProperty placementAttempts;
    private SerializedProperty rotateDegresPerAttempt;
    private SerializedProperty fieldType;
    private SerializedProperty fieldSize;
    private SerializedProperty ringWidth;
    private SerializedProperty ringThickness;
    private SerializedProperty targetWidth;
    private SerializedProperty targetWidthPositionOffset;
    private SerializedProperty randomScale;
    private SerializedProperty scaleMinSize;
    private SerializedProperty scaleMaxSize;
    private SerializedProperty adjustMassByScale;
    private SerializedProperty massScaleFactor;
    private SerializedProperty randomRotation;
    private SerializedProperty showFieldGizmo;
    private SerializedProperty showBufferGizmo;
    private SerializedProperty enableMeshes;
    private SerializedProperty enableColliders;
    private SerializedProperty showLogging;
    private SerializedProperty placementCoroutineIsRunning;
    
    private bool settingsFold = true;
    private bool visualFold = true;
    private bool prefabsOrderedListFold = true;
    private bool prefabPreviewFold = true;
    private bool placementFold = true;
    private bool fieldSettingsFold = true;
    private bool randomnessSettingsFold = true;
    private float prefabButtonScale = 0.5f;
    private float prefabPreviewButtonWidth = 100f;
    AsteroidFieldCreator scriptHandler = null;

    ReorderableList prefabList;
    private float minimumFieldSize;

    private void OnEnable()
    {
        scriptHandler = (AsteroidFieldCreator)target;
        totalObjects = serializedObject.FindProperty("totalObjects");
        valueForProgressBar = serializedObject.FindProperty("valueForProgressBar");
        objectPrefabs = serializedObject.FindProperty("objectPrefabs");
        enablePrefabLink = serializedObject.FindProperty("enablePrefabLink");
        keepPrefabLink = serializedObject.FindProperty("keepPrefabLink");
        placementAttempts = serializedObject.FindProperty("placementAttempts");
        rotateDegresPerAttempt = serializedObject.FindProperty("rotateDegresPerAttempt");
        showFieldGizmo = serializedObject.FindProperty("showFieldGizmo");
        showBufferGizmo = serializedObject.FindProperty("showBufferGizmo");
        enableMeshes = serializedObject.FindProperty("enableMeshes");
        enableColliders = serializedObject.FindProperty("enableColliders");
        fieldType = serializedObject.FindProperty("fieldType");
        fieldSize = serializedObject.FindProperty("fieldSize");
        ringWidth = serializedObject.FindProperty("ringWidth");
        ringThickness = serializedObject.FindProperty("ringThickness");
        targetWidth = serializedObject.FindProperty("targetWidth");
        targetWidthPositionOffset = serializedObject.FindProperty("targetWidthPositionOffset");
        randomScale = serializedObject.FindProperty("randomScale");
        scaleMinSize = serializedObject.FindProperty("scaleMinSize");
        scaleMaxSize = serializedObject.FindProperty("scaleMaxSize");
        adjustMassByScale = serializedObject.FindProperty("adjustMassByScale");
        massScaleFactor = serializedObject.FindProperty("massScaleFactor");
        randomRotation = serializedObject.FindProperty("randomRotation");
        showLogging = serializedObject.FindProperty("showLogging");
        placementCoroutineIsRunning = serializedObject.FindProperty("placementCoroutineIsRunning");

        prefabList = new ReorderableList(serializedObject, serializedObject.FindProperty("objectPrefabs"), true, true, true, true);
        prefabList.drawElementCallback = CreatePrefabReorderList;
        prefabList.drawHeaderCallback = CreatePrefabReorderListHeader;
    }

    private void OnDisable()
    {

    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        InterceptTools();
        serializedObject.Update();
        enablePrefabLink.boolValue = turnOnPrefabLink;

        DrawProgressBar();
        DrawWarningsBox();
        DrawHelpBox();

        EditorGUI.BeginDisabledGroup(placementCoroutineIsRunning.boolValue);
        DrawButtons();
        DrawUILine(Color.gray, 2, 10);

        settingsFold = EditorGUILayout.Foldout(EditorPrefs.GetBool("settingsFold", false), "Settings", true, EditorStyles.foldout);
        EditorPrefs.SetBool("settingsFold", settingsFold);

        if (settingsFold == true)
        {
            DrawUILine(Color.gray, 2, 10);
            DrawVisualSettings();
            DrawUILine(Color.gray, 2, 10);
            DrawFieldSettings();
            DrawUILine(Color.gray, 2, 10);
            DrawPlacementSettings();
            DrawUILine(Color.gray, 2, 10);
            DrawRandomnessSettings();
            DrawUILine(Color.gray, 2, 10);
            if (useReorderableList == false)
            {
                DrawPrefabStandardList();
            }
            else
            {
                DrawPrefabReorderList();
            }
            DrawUILine(Color.gray, 2, 10);
            DrawPrefabPreview();
            DrawUILine(Color.gray, 2, 10);
            DrawRemoveScripts();
            DrawUILine(Color.gray, 2, 10);
        }
        else
        {
            DrawUILine(Color.gray, 2, 10);
        }
        EditorGUI.EndDisabledGroup();
        serializedObject.ApplyModifiedProperties();
    }

    private void InterceptTools()
    {
        scriptHandler.gameObject.transform.localScale = Vector3.one;
        if (Tools.current == Tool.Scale || Tools.current == Tool.Transform)
        {
            Tools.hidden = true;
        }
        else
        {
            Tools.hidden = false;
        }
    }

    private void DrawFieldSettings()
    {
        fieldSettingsFold = EditorGUILayout.Foldout(EditorPrefs.GetBool("fieldSettingsFold", false), "Field Dimensions/Type", true, EditorStyles.foldout);
        EditorPrefs.SetBool("fieldSettingsFold", fieldSettingsFold);
        if (fieldSettingsFold == true)
        {
            EditorGUILayout.PropertyField(fieldType, new GUIContent("Type", "Shape of the Field. RING = like a doughnut, BLOB = square or sphere bounding area, RIBBON = along a line"));
            EditorGUILayout.PropertyField(fieldSize, new GUIContent("Size", "RING = radius, BLOB = size of sphere, RIBBON = Distance from tool"));
            if (fieldType.enumValueIndex == (int)FieldType.Blob_Sphere_Surface || fieldType.enumValueIndex == (int)FieldType.Ring || fieldType.enumValueIndex == (int)FieldType.Ribbon)
            {
                EditorGUILayout.PropertyField(ringWidth, new GUIContent("Width (Blue)", "RING & RIBBON = Width of placement area along the Z (blue) axsis, BLOB_SPHERE_SURFACE = Width of placement area around the surface or Z (blue) axsis, BLOB OTHERS = n/a"));
            }
            if (fieldType.enumValueIndex == (int)FieldType.Ring || fieldType.enumValueIndex == (int)FieldType.Ribbon)
            {
                EditorGUILayout.PropertyField(ringThickness, new GUIContent("Thickness (Green)", "RING & RIBBON = Height of placement area along the Y (green) axsis, BLOB = n/a"));
                EditorGUILayout.PropertyField(targetWidth, new GUIContent("Length (Red)", "RING & RIBBON = Length of placement area along the X (red) axsis, BLOB = n/a"));
                if(fieldType.enumValueIndex == (int)FieldType.Ribbon)
                {
                    EditorGUILayout.PropertyField(targetWidthPositionOffset, new GUIContent("Length center offset", "RIBBON = Center point offset along the X (red) axsis, ALL OTHER = n/a"));
                }
            }
        }
    }

    private void DrawRandomnessSettings()
    {
        randomnessSettingsFold = EditorGUILayout.Foldout(EditorPrefs.GetBool("randomnessSettingsFold", false), "Randomness Settings", true, EditorStyles.foldout);
        EditorPrefs.SetBool("randomnessSettingsFold", randomnessSettingsFold);
        if (randomnessSettingsFold == true)
        {
            EditorGUILayout.LabelField("Random Scale Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(randomScale, new GUIContent("Random Scale", "Random scale between Min Scale and Max Scale"));
            EditorGUILayout.PropertyField(scaleMinSize, new GUIContent("Min Scale"));
            EditorGUILayout.PropertyField(scaleMaxSize, new GUIContent("Max Scale"));
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mass Scaling Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(adjustMassByScale, new GUIContent("Scale Mass", "Will scale the mass with the object scale. Object scale * Mass Scale Factor. A value of 1 will multiplay mass by full object scale."));
            EditorGUILayout.PropertyField(massScaleFactor, new GUIContent("Mass Scale Factor"));
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Rotation Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(randomRotation, new GUIContent("Random Rotation", "Will rotate the object in some random rotation along the X,Y,Z axsis"));
        }
    }

    private void DrawWarningsBox()
    {
        if(enableColliders.boolValue == false || enableMeshes.boolValue == false)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("WARNING: Colliders and/or Meshes are disabled. Be sure to enable them before you complile (Settings/Visual Settings).", MessageType.Warning, true);
            GUILayout.EndHorizontal();
        }
    }

    private void DrawHelpBox()
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox("Total objects: " + totalObjects.intValue.ToString() + " from " + objectPrefabs.arraySize.ToString() + " prefabs", MessageType.None, true);
        GUILayout.EndHorizontal();
    }

    private void DrawProgressBar()
    {
        if(placementCoroutineIsRunning != null)
        {
            if (placementCoroutineIsRunning.boolValue == true)
            {
                GUILayout.BeginHorizontal();
                EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(false, 18), valueForProgressBar.floatValue, "progress");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                Color normalColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Stop", GUILayout.MinWidth(25f)))
                {
                    scriptHandler.CancelPlacement();
                }
                GUI.backgroundColor = normalColor;
                GUILayout.EndHorizontal();
            }
        }
    }

    private void DrawPlacementSettings()
    {
        placementFold = EditorGUILayout.Foldout(EditorPrefs.GetBool("placementFold", false), "Placement Settings", true, EditorStyles.foldout);
        EditorPrefs.SetBool("placementFold", placementFold);
        if (placementFold == true)
        {
            EditorGUILayout.PropertyField(placementAttempts, new GUIContent("Placement Attempts", "The number of objects the tool will attempt to place within the bounds."));
            if(fieldType.intValue == (int)FieldType.Ring)
            {
                EditorGUILayout.Slider(rotateDegresPerAttempt, -360f, 360f, new GUIContent("Rotate Each Attempt", "RING ONLY: The degrees to rotate after each attempt. 360 attempts rotated 1 degree will create a circle. 180 attempts rotated 1 degree will create a half circle."));
            }
        }
    }

    private void DrawPrefabReorderList()
    {
        prefabsOrderedListFold = EditorGUILayout.Foldout(EditorPrefs.GetBool("prefabsOrderedListFold", false), "Prefabs (" + objectPrefabs.arraySize.ToString() + ")", true, EditorStyles.foldout);
        EditorPrefs.SetBool("prefabsOrderedListFold", prefabsOrderedListFold);
        if (prefabsOrderedListFold == true)
        {
            if(turnOnPrefabLink == true)
            {
                enablePrefabLink.boolValue = true;
                EditorGUILayout.PropertyField(keepPrefabLink, new GUIContent("Keep Prefab Link", "Will instantiate the object and keep the link to the prefab."));
            }
            else
            {
                enablePrefabLink.boolValue = false;
            }
            prefabList.DoLayoutList();
        }
    }

    private void CreatePrefabReorderList(Rect rect, int index, bool isActive, bool isFocused)
    {
            var element = prefabList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth - 70f, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
    }

    private void CreatePrefabReorderListHeader(Rect rec)
    {
        EditorGUI.LabelField(rec, "Drag up and down to reorder");
    }

    private void DrawPrefabStandardList()
    {
        EditorGUILayout.PropertyField(objectPrefabs, new GUIContent("Prefabs"), true);
    }

    private void DrawPrefabPreview()
    {
        prefabPreviewFold = EditorGUILayout.Foldout(EditorPrefs.GetBool("prefabPreviewFold", false), "Prefabs Preview", true, EditorStyles.foldout);
        EditorPrefs.SetBool("prefabPreviewFold", prefabPreviewFold);
        if (prefabPreviewFold == true)
        {
            GUILayout.BeginHorizontal();
            prefabButtonScale = EditorGUILayout.Slider("Image Scale", prefabButtonScale, 0.1f, 1f);
            float buttonSize = prefabPreviewButtonWidth * prefabButtonScale;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            int colCount = 1;
            for (int i = 0; i < objectPrefabs.arraySize; i++)
            {
                if (objectPrefabs.GetArrayElementAtIndex(i).objectReferenceValue != null)
                {
                    GUILayout.BeginVertical();
                    Texture2D tex = AssetPreview.GetAssetPreview(objectPrefabs.GetArrayElementAtIndex(i).objectReferenceValue);
                    EditorGUILayout.LabelField(objectPrefabs.GetArrayElementAtIndex(i).objectReferenceValue.name, GUILayout.Width(buttonSize));

                    if (GUILayout.Button(tex, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GetAssetPath(objectPrefabs.GetArrayElementAtIndex(i).objectReferenceValue));
                    }


                    GUILayout.EndVertical();
                    if ((EditorGUIUtility.currentViewWidth - buttonSize) > (buttonSize * (colCount + 1)))
                    {
                        colCount++;
                    }
                    else
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        colCount = 1;
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
    }

    private void DrawVisualSettings()
    {
        visualFold = EditorGUILayout.Foldout(EditorPrefs.GetBool("gizmoFold", false), "Visual Settings", true, EditorStyles.foldout);
        EditorPrefs.SetBool("gizmoFold", visualFold);
        if (visualFold == true)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(showFieldGizmo, new GUIContent("Show Field", "Displays gizmo to visualize field dimensions"), true);
            EditorGUILayout.PropertyField(enableMeshes, new GUIContent("Enable Meshes", "Disable to increase editor performance with large fields. Do not forget to reenable before compiling."), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(showBufferGizmo, new GUIContent("Show Buffers", "Display gizmo to visualize object buffer zones."), true);
            EditorGUILayout.PropertyField(enableColliders, new GUIContent("Enable Colliders", "Disable to increase editor performance with large fields. Do not forget to reenable before compiling."), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(showLogging, new GUIContent("Show Debug", "Log actions to the console."), true);
            EditorGUILayout.EndHorizontal();

        }
    }

    private void DrawButtons()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create", GUILayout.MinWidth(100f)))
        {
            scriptHandler.StartGenerator();
        }

        if (GUILayout.Button("Update Buffers", GUILayout.MinWidth(100f)))
        {
            scriptHandler.EnforceBufferZones();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Undo Last", GUILayout.MinWidth(100f)))
        {
            scriptHandler.UndoLast();
        }

        if (GUILayout.Button("Delete All", GUILayout.MinWidth(100f)))
        {
            if (EditorUtility.DisplayDialog("Delete all child objects?", "Are you sure you want to delete all child object?", "Delete", "Cancel") == true)
            {
                scriptHandler.ClearAsteroids();
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset Rotation", GUILayout.MinWidth(100f)))
        {
            scriptHandler.transform.rotation = Quaternion.identity;
        }
        if (GUILayout.Button("Reset Generator", GUILayout.MinWidth(100f)))
        {
            scriptHandler.ResetGenerator();
        }
        GUILayout.EndHorizontal();
    }

    private void DrawRemoveScripts()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Scripts", GUILayout.MinWidth(100f)))
        {
            if (EditorUtility.DisplayDialog("Add AstroData scripts to child objects?", "Are you sure you want to add AstroData scripts on child objects that missing it? All objects that are prefabs will be unpacked!", "Add", "Cancel") == true)
            {
                scriptHandler.AddRemoveAstroDataScripts(false);
            }
        }
        if (GUILayout.Button("Remove Scripts", GUILayout.MinWidth(100f)))
        {
            if (EditorUtility.DisplayDialog("Remove all AstroData scripts on child objects?", "Are you sure you want to remove all AstroData scripts on child objects? All objects that are prefabs will be unpacked!", "Remove", "Cancel") == true)
            {
                scriptHandler.AddRemoveAstroDataScripts(true);
            }
        }
        GUILayout.EndHorizontal();
    }

    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }


}


