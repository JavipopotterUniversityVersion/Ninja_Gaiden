using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Enhanced touch screen button that works with InputBuffer system
/// Improved version with better visual feedback for mobile
/// </summary>
[RequireComponent(typeof(Button))]
public class EnhancedTouchScreenButtonInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Input Configuration")]
    [SerializeField] 
    [Tooltip("Name of the input to activate in InputBuffer (e.g., 'JUMP', 'ATTACK')")]
    private string inputName;
    
    [Header("Visual Feedback")]
    [SerializeField]
    [Tooltip("Image component to provide visual feedback")]
    private Image buttonImage;
    
    [SerializeField]
    [Tooltip("Color when button is pressed")]
    private Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    
    [SerializeField]
    [Tooltip("Enable scale animation on press")]
    private bool enablePressScale = true;
    
    [Range(0.85f, 0.98f)]
    [SerializeField]
    [Tooltip("Scale when pressed")]
    private float pressedScale = 0.92f;
    
    [Header("Haptic Feedback")]
    [SerializeField]
    [Tooltip("Enable vibration on mobile devices")]
    private bool enableHaptics = true;
    
    private Button button;
    private Color originalColor;
    private Vector3 originalScale;
    private bool isPressed = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        
        if (buttonImage == null)
        {
            buttonImage = GetComponent<Image>();
        }
        
        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
        }
        
        originalScale = transform.localScale;
        
        // Setup button click event if using Button component
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnButtonClick()
    {
        SendInput();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        
        isPressed = true;
        
        // Visual feedback
        if (buttonImage != null)
        {
            buttonImage.color = pressedColor;
        }
        
        if (enablePressScale)
        {
            transform.localScale = originalScale * pressedScale;
        }
        
        // Haptic feedback
        if (enableHaptics)
        {
            TriggerHapticFeedback();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        
        // Reset visual feedback
        if (buttonImage != null)
        {
            buttonImage.color = originalColor;
        }
        
        if (enablePressScale)
        {
            transform.localScale = originalScale;
        }
    }

    private void SendInput()
    {
        try
        {
            if (InputBuffer.Instance != null && !string.IsNullOrEmpty(inputName))
            {
                InputBuffer.Instance.ActivateInput(inputName);
                Debug.Log($"Mobile button sent input: {inputName}");
            }
            else
            {
                if (InputBuffer.Instance == null)
                {
                    Debug.LogWarning("InputBuffer instance not found! Make sure InputBuffer exists in the scene.");
                }
                if (string.IsNullOrEmpty(inputName))
                {
                    Debug.LogWarning($"Input name not set on button {gameObject.name}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error sending input from mobile button: {e.Message}");
        }
    }

    private bool IsInteractable()
    {
        return button == null || button.interactable;
    }

    private void TriggerHapticFeedback()
    {
        #if UNITY_ANDROID || UNITY_IOS
            if (Application.isMobilePlatform)
            {
                Handheld.Vibrate();
            }
        #endif
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }

    // Editor helper to test button
    [ContextMenu("Test Send Input")]
    private void TestSendInput()
    {
        SendInput();
    }

    // Validate in editor
    private void OnValidate()
    {
        if (buttonImage == null)
        {
            buttonImage = GetComponent<Image>();
        }
        
        if (string.IsNullOrEmpty(inputName))
        {
            Debug.LogWarning($"Please set input name for {gameObject.name}", this);
        }
    }
}
