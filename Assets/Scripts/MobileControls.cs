using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages mobile control UI visibility and functionality
/// Automatically shows/hides controls based on platform
/// </summary>
public class MobileControls : MonoBehaviour
{
    [Header("Control Containers")]
    [SerializeField] private GameObject joystickContainer;
    [SerializeField] private GameObject buttonContainer;
    
    [Header("Auto-detect Platform")]
    [SerializeField] private bool autoDetectMobile = true;
    [SerializeField] private bool forceShowControls = false;

    private void Start()
    {
        SetupMobileControls();
    }

    private void SetupMobileControls()
    {
        bool shouldShowControls = forceShowControls || ShouldShowMobileControls();
        
        if (joystickContainer != null)
        {
            joystickContainer.SetActive(shouldShowControls);
        }
        
        if (buttonContainer != null)
        {
            buttonContainer.SetActive(shouldShowControls);
        }
        
        Debug.Log($"Mobile Controls: {(shouldShowControls ? "Enabled" : "Disabled")}");
    }

    private bool ShouldShowMobileControls()
    {
        if (!autoDetectMobile)
        {
            return true;
        }

        // Check if running on mobile platform
        #if UNITY_ANDROID || UNITY_IOS
            return true;
        #else
            // Check if touch is supported on desktop (for testing)
            return Input.touchSupported;
        #endif
    }

    // Public method to toggle controls at runtime
    public void ToggleControls(bool show)
    {
        if (joystickContainer != null)
        {
            joystickContainer.SetActive(show);
        }
        
        if (buttonContainer != null)
        {
            buttonContainer.SetActive(show);
        }
    }
}
