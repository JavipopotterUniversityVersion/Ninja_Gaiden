using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Helper script to set up mobile controls for Ninja Gaiden
/// Attach this to a Canvas GameObject to automatically configure mobile controls
/// </summary>
public class MobileControlsSetup : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the Floating Joystick (assign from scene)")]
    public Joystick joystick;
    
    [Header("Button Configuration")]
    [Tooltip("Automatically find and configure mobile buttons on Start")]
    public bool autoConfigureButtons = true;
    
    [Header("Visual Settings")]
    [Range(0.3f, 1f)]
    [Tooltip("Button opacity (alpha). Higher = more visible")]
    public float buttonAlpha = 0.85f;
    
    [Tooltip("Minimum button size for good touch targets")]
    public Vector2 minimumButtonSize = new Vector2(120f, 120f);
    
    [Header("Platform Detection")]
    [Tooltip("Show controls only on mobile devices")]
    public bool autoHideOnDesktop = true;
    
    [Tooltip("Force show controls even on desktop (for testing)")]
    public bool forceShowControls = false;

    private void Start()
    {
        ConfigureMobileControls();
    }

    private void ConfigureMobileControls()
    {
        // Determine if we should show mobile controls
        bool shouldShow = ShouldShowMobileControls();
        
        if (!shouldShow && !forceShowControls)
        {
            // Hide mobile controls by disabling children instead of the whole gameObject
            // This allows the script to continue running if needed
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            Debug.Log("Mobile controls hidden on desktop");
            return;
        }
        
        if (autoConfigureButtons)
        {
            OptimizeButtonsForMobile();
        }
        
        Debug.Log("Mobile controls configured and active");
    }

    private void OptimizeButtonsForMobile()
    {
        // Find all Button components in children
        Button[] buttons = GetComponentsInChildren<Button>(true);
        
        foreach (Button button in buttons)
        {
            OptimizeButton(button);
        }
        
        Debug.Log($"Optimized {buttons.Length} buttons for mobile");
    }

    private void OptimizeButton(Button button)
    {
        // Get the RectTransform
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // Ensure minimum size for good touch targets
            Vector2 currentSize = rectTransform.sizeDelta;
            if (currentSize.x < minimumButtonSize.x || currentSize.y < minimumButtonSize.y)
            {
                rectTransform.sizeDelta = new Vector2(
                    Mathf.Max(currentSize.x, minimumButtonSize.x),
                    Mathf.Max(currentSize.y, minimumButtonSize.y)
                );
            }
        }
        
        // Adjust button image alpha for better visibility
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = buttonAlpha;
            buttonImage.color = color;
        }
        
        // Ensure button has proper navigation (for accessibility)
        Navigation nav = button.navigation;
        nav.mode = Navigation.Mode.None; // Disable for touch
        button.navigation = nav;
    }

    private bool ShouldShowMobileControls()
    {
        if (!autoHideOnDesktop)
        {
            return true;
        }

        // Check platform
        #if UNITY_ANDROID || UNITY_IOS
            return true;
        #elif UNITY_EDITOR
            // In editor, check if touch is supported (for testing on desktop with touch)
            return Input.touchSupported || forceShowControls;
        #else
            return Input.touchSupported;
        #endif
    }

    // Editor helper to manually trigger configuration
    [ContextMenu("Configure Mobile Controls")]
    private void ManualConfigure()
    {
        OptimizeButtonsForMobile();
        Debug.Log("Manual configuration complete");
    }
}
