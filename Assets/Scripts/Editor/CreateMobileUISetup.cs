using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
/// <summary>
/// Creates a complete mobile controls UI setup
/// Use: Tools -> Mobile Controls -> Create Mobile UI Setup
/// </summary>
public class CreateMobileUISetup : EditorWindow
{
    private enum ControlsType
    {
        NewStateMachine,  // For new StateMachine system with InputBuffer
        OldPlayer         // For old Player.cs system
    }
    
    private ControlsType controlsType = ControlsType.NewStateMachine;
    private bool includeJoystick = true;
    private bool includeJumpButton = true;
    private bool includeAttackButton = true;
    private bool includeRunButton = false;
    private Vector2 joystickPosition = new Vector2(150, 150);
    private float joystickSize = 180f;
    private Vector2 jumpButtonPosition = new Vector2(-150, 150);
    private Vector2 attackButtonPosition = new Vector2(-150, 280);
    private float buttonSize = 120f;

    [MenuItem("Tools/Mobile Controls/Create Mobile UI Setup")]
    public static void ShowWindow()
    {
        GetWindow<CreateMobileUISetup>("Mobile UI Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Mobile Controls Setup", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        controlsType = (ControlsType)EditorGUILayout.EnumPopup("System Type", controlsType);
        
        EditorGUILayout.HelpBox(
            controlsType == ControlsType.NewStateMachine 
                ? "Creates buttons for StateMachine + InputBuffer system"
                : "Creates buttons for old Player.cs system",
            MessageType.Info
        );
        
        EditorGUILayout.Space();
        GUILayout.Label("Controls to Include", EditorStyles.boldLabel);
        
        includeJoystick = EditorGUILayout.Toggle("Joystick", includeJoystick);
        includeJumpButton = EditorGUILayout.Toggle("Jump Button", includeJumpButton);
        includeAttackButton = EditorGUILayout.Toggle("Attack Button", includeAttackButton);
        includeRunButton = EditorGUILayout.Toggle("Run Button", includeRunButton);
        
        EditorGUILayout.Space();
        GUILayout.Label("Layout Settings", EditorStyles.boldLabel);
        
        if (includeJoystick)
        {
            joystickSize = EditorGUILayout.FloatField("Joystick Size", joystickSize);
            joystickPosition = EditorGUILayout.Vector2Field("Joystick Position", joystickPosition);
        }
        
        buttonSize = EditorGUILayout.FloatField("Button Size", buttonSize);
        
        if (includeJumpButton)
        {
            jumpButtonPosition = EditorGUILayout.Vector2Field("Jump Position", jumpButtonPosition);
        }
        
        if (includeAttackButton)
        {
            attackButtonPosition = EditorGUILayout.Vector2Field("Attack Position", attackButtonPosition);
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Create Mobile UI", GUILayout.Height(40)))
        {
            CreateMobileUI();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "This will create a new Canvas with mobile controls in your scene. " +
            "Make sure you have a Player object to connect the buttons to.",
            MessageType.Warning
        );
    }

    private void CreateMobileUI()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("MobileControlsCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Ensure EventSystem exists
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
        
        // Create container for controls
        GameObject controlsContainer = new GameObject("MobileControls");
        controlsContainer.transform.SetParent(canvasObj.transform, false);
        
        RectTransform containerRect = controlsContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.sizeDelta = Vector2.zero;
        
        // Add MobileControlsSetup component
        MobileControlsSetup setup = canvasObj.AddComponent<MobileControlsSetup>();
        
        // Create Joystick
        if (includeJoystick)
        {
            CreateJoystick(controlsContainer.transform, setup);
        }
        
        // Create buttons container
        GameObject buttonsContainer = new GameObject("Buttons");
        buttonsContainer.transform.SetParent(controlsContainer.transform, false);
        
        // Create action buttons
        if (includeJumpButton)
        {
            CreateActionButton(buttonsContainer.transform, "JumpButton", "JUMP", jumpButtonPosition);
        }
        
        if (includeAttackButton)
        {
            CreateActionButton(buttonsContainer.transform, "AttackButton", "ATTACK", attackButtonPosition);
        }
        
        Selection.activeGameObject = canvasObj;
        EditorGUIUtility.PingObject(canvasObj);
        
        Debug.Log("✓ Mobile UI created successfully! Configure button events in Inspector.");
    }

    private void CreateJoystick(Transform parent, MobileControlsSetup setup)
    {
        // Try to instantiate from existing prefab
        string[] guids = AssetDatabase.FindAssets("Floating Joystick t:Prefab");
        
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                GameObject joystickObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
                
                RectTransform rect = joystickObj.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(0, 0);
                    rect.anchoredPosition = joystickPosition;
                    rect.sizeDelta = new Vector2(joystickSize, joystickSize);
                }
                
                // Assign to setup
                Joystick joystick = joystickObj.GetComponent<Joystick>();
                if (joystick != null)
                {
                    SerializedObject serializedSetup = new SerializedObject(setup);
                    SerializedProperty joystickProp = serializedSetup.FindProperty("joystick");
                    joystickProp.objectReferenceValue = joystick;
                    serializedSetup.ApplyModifiedProperties();
                }
                
                Debug.Log("✓ Added Floating Joystick from prefab");
                return;
            }
        }
        
        Debug.LogWarning("Floating Joystick prefab not found. Please add manually from Assets/Joystick Pack/Prefabs/");
    }

    private void CreateActionButton(Transform parent, string name, string inputName, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        // Setup RectTransform
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 0); // Bottom-right anchor
        rect.anchorMax = new Vector2(1, 0);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(buttonSize, buttonSize);
        
        // Add Image
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(1, 1, 1, 0.8f);
        
        // Add Button
        Button button = buttonObj.AddComponent<Button>();
        
        // Add appropriate input component based on system type
        if (controlsType == ControlsType.NewStateMachine)
        {
            EnhancedTouchScreenButtonInput touchInput = buttonObj.AddComponent<EnhancedTouchScreenButtonInput>();
            // Note: inputName needs to be set via SerializedObject
            SerializedObject serializedInput = new SerializedObject(touchInput);
            SerializedProperty inputNameProp = serializedInput.FindProperty("inputName");
            inputNameProp.stringValue = inputName;
            serializedInput.ApplyModifiedProperties();
        }
        else
        {
            // For old system, user needs to manually connect button events
            Debug.Log($"Button '{name}' created. Connect to Player.SaltoTactil() or Player.AtaqueTactil() in Inspector.");
        }
        
        // Add text label
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        Text text = textObj.AddComponent<Text>();
        text.text = inputName;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 24;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        // Add enhanced button component
        buttonObj.AddComponent<EnhancedMobileButton>();
        
        Debug.Log($"✓ Created {name} button");
    }
}
#endif
