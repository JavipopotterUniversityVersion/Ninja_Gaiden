using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Enhanced mobile button with better visual feedback and sizing
/// Attach to UI buttons for improved mobile experience
/// </summary>
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(RectTransform))]
public class EnhancedMobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Visual Feedback")]
    [Tooltip("Scale button when pressed")]
    public bool enablePressScale = true;
    
    [Range(0.8f, 1f)]
    [Tooltip("Scale multiplier when pressed (e.g., 0.9 = 90% size)")]
    public float pressedScale = 0.92f;
    
    [Tooltip("Enable press effect animation")]
    public bool enablePressAnimation = true;
    
    [Tooltip("Animation speed")]
    public float animationSpeed = 10f;
    
    [Header("Haptic Feedback")]
    [Tooltip("Vibrate on button press (mobile only)")]
    public bool enableHapticFeedback = true;
    
    [Header("Auto-Configuration")]
    [Tooltip("Automatically optimize button on Start")]
    public bool autoOptimize = true;
    
    [Range(0.5f, 1f)]
    [Tooltip("Button opacity")]
    public float buttonAlpha = 0.85f;
    
    [Tooltip("Minimum recommended size for touch targets")]
    public Vector2 minimumSize = new Vector2(100f, 100f);
    
    private Button button;
    private RectTransform rectTransform;
    private Image buttonImage;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isPressed = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        buttonImage = GetComponent<Image>();
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    private void Start()
    {
        if (autoOptimize)
        {
            OptimizeButton();
        }
    }

    private void Update()
    {
        // Only animate if needed (when there's a difference between current and target scale)
        // Use sqrMagnitude for better performance (avoids square root calculation)
        if (enablePressAnimation && enablePressScale && (transform.localScale - targetScale).sqrMagnitude > 0.000001f)
        {
            // Smoothly animate to target scale
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.deltaTime * animationSpeed
            );
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;
        
        isPressed = true;
        
        if (enablePressScale)
        {
            targetScale = originalScale * pressedScale;
            
            if (!enablePressAnimation)
            {
                transform.localScale = targetScale;
            }
        }
        
        if (enableHapticFeedback)
        {
            TriggerHapticFeedback();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        
        if (enablePressScale)
        {
            targetScale = originalScale;
            
            if (!enablePressAnimation)
            {
                transform.localScale = originalScale;
            }
        }
    }

    private void OptimizeButton()
    {
        // Ensure minimum size
        Vector2 currentSize = rectTransform.sizeDelta;
        if (currentSize.x < minimumSize.x || currentSize.y < minimumSize.y)
        {
            rectTransform.sizeDelta = new Vector2(
                Mathf.Max(currentSize.x, minimumSize.x),
                Mathf.Max(currentSize.y, minimumSize.y)
            );
        }
        
        // Set alpha
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = buttonAlpha;
            buttonImage.color = color;
        }
        
        // Disable navigation (not needed for touch)
        Navigation nav = button.navigation;
        nav.mode = Navigation.Mode.None;
        button.navigation = nav;
    }

    private void TriggerHapticFeedback()
    {
        // Haptic feedback only works on mobile devices
        #if UNITY_ANDROID || UNITY_IOS
            if (Application.isMobilePlatform)
            {
                // Light haptic feedback
                Handheld.Vibrate();
            }
        #endif
    }

    // Public method to manually optimize at runtime
    [ContextMenu("Optimize This Button")]
    public void ManualOptimize()
    {
        OptimizeButton();
    }
}
