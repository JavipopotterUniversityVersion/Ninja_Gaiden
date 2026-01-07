using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor utility to improve mobile controls in Ninja Gaiden scenes
/// Use: Tools -> Mobile Controls -> Improve Mobile UI
/// </summary>
public class MobileControlsEditor : Editor
{
    [MenuItem("Tools/Mobile Controls/Improve Mobile UI in Current Scene")]
    public static void ImproveMobileControls()
    {
        int improvedCount = 0;
        
        // Find all buttons in the scene
        Button[] buttons = Object.FindObjectsOfType<Button>(true);
        
        foreach (Button button in buttons)
        {
            // Only process mobile control buttons (not debug/cheat buttons)
            string buttonName = button.gameObject.name;
            if (IsMobileControlButton(buttonName))
            {
                ImproveButton(button);
                improvedCount++;
            }
        }
        
        // Find and improve joystick
        Joystick[] joysticks = Object.FindObjectsOfType<Joystick>(true);
        foreach (Joystick joystick in joysticks)
        {
            ImproveJoystick(joystick);
            improvedCount++;
        }
        
        // Mark scene as dirty so changes are saved
        if (improvedCount > 0)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"✓ Improved {improvedCount} mobile controls!");
        }
        else
        {
            Debug.LogWarning("No mobile controls found to improve.");
        }
    }
    
    private static bool IsMobileControlButton(string name)
    {
        // Identify mobile control buttons (not debug/cheat buttons)
        return name == "ButtonA" || name == "ButtonY" || name == "ButtonB" || 
               name == "ButtonX" || name == "ButtonCORRER" || name.Contains("Jump") || 
               name.Contains("Attack");
    }
    
    private static void ImproveButton(Button button)
    {
        // Improve button size
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Vector2 currentSize = rectTransform.sizeDelta;
            float minSize = 100f; // Minimum touch target size
            
            if (currentSize.x < minSize || currentSize.y < minSize)
            {
                rectTransform.sizeDelta = new Vector2(
                    Mathf.Max(currentSize.x, minSize),
                    Mathf.Max(currentSize.y, minSize)
                );
                Debug.Log($"Resized {button.name} to {rectTransform.sizeDelta}");
            }
        }
        
        // Improve button visibility
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            if (color.a < 0.75f)
            {
                color.a = 0.85f; // Increase opacity
                buttonImage.color = color;
                Debug.Log($"Increased opacity of {button.name} to {color.a}");
            }
        }
        
        // Ensure button responds to touch
        button.transition = Selectable.Transition.ColorTint;
        
        // Mark object as dirty
        EditorUtility.SetDirty(button);
        EditorUtility.SetDirty(rectTransform);
        if (buttonImage != null)
        {
            EditorUtility.SetDirty(buttonImage);
        }
    }
    
    private static void ImproveJoystick(Joystick joystick)
    {
        // Improve joystick size
        RectTransform rectTransform = joystick.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Vector2 currentSize = rectTransform.sizeDelta;
            float recommendedSize = 180f;
            
            if (currentSize.x < recommendedSize * 0.8f)
            {
                rectTransform.sizeDelta = new Vector2(recommendedSize, recommendedSize);
                Debug.Log($"Resized joystick to {recommendedSize}x{recommendedSize}");
                EditorUtility.SetDirty(rectTransform);
            }
        }
        
        // Improve joystick visibility if it has an Image
        Image[] images = joystick.GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            Color color = image.color;
            if (color.a < 0.6f)
            {
                color.a = 0.75f;
                image.color = color;
                Debug.Log($"Increased joystick component opacity to {color.a}");
                EditorUtility.SetDirty(image);
            }
        }
        
        EditorUtility.SetDirty(joystick);
    }
    
    [MenuItem("Tools/Mobile Controls/Add Mobile Controls Setup to Canvas")]
    public static void AddMobileControlsSetup()
    {
        // Find Canvas in scene
        Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
        
        if (canvases.Length == 0)
        {
            Debug.LogError("No Canvas found in scene. Please create a Canvas first.");
            return;
        }
        
        Canvas mainCanvas = canvases[0];
        
        // Check if already has MobileControlsSetup
        if (mainCanvas.GetComponent<MobileControlsSetup>() != null)
        {
            Debug.LogWarning("Canvas already has MobileControlsSetup component.");
            return;
        }
        
        // Add the component
        MobileControlsSetup setup = mainCanvas.gameObject.AddComponent<MobileControlsSetup>();
        
        // Try to find and assign joystick
        Joystick joystick = Object.FindObjectOfType<Joystick>();
        if (joystick != null)
        {
            SerializedObject serializedSetup = new SerializedObject(setup);
            SerializedProperty joystickProp = serializedSetup.FindProperty("joystick");
            joystickProp.objectReferenceValue = joystick;
            serializedSetup.ApplyModifiedProperties();
            
            Debug.Log("✓ Added MobileControlsSetup to Canvas and assigned Joystick!");
        }
        else
        {
            Debug.Log("✓ Added MobileControlsSetup to Canvas. Please assign Joystick manually.");
        }
        
        EditorUtility.SetDirty(mainCanvas);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
