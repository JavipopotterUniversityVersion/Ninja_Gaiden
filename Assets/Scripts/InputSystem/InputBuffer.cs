using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InputBuffer : MonoBehaviour
{
    static InputBuffer _instance;
    public static InputBuffer Instance => _instance;
    [SerializeField] SerializableDictionary<string, InputBufferData> inputBuffer = new SerializableDictionary<string, InputBufferData>();
    UnityEvent<string> _onInputActivated = new UnityEvent<string>();
    public UnityEvent<string> OnInputActivated => _onInputActivated;

    public bool CheckInput(string inputName)
    {
        bool result = false;
        if (inputBuffer.ContainsKey(inputName))
        {
            InputBufferData inputData = inputBuffer[inputName];
            result = inputData.Check();
        }
        else Debug.LogWarning($"Input '{inputName}' not found in input buffer.");
        return result;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ActivateInput(string inputName)
    {
        if (inputBuffer.ContainsKey(inputName))
        {
            InputBufferData inputData = inputBuffer[inputName];
            if (!inputData.IsActive())
            {
                inputData.Activate();
                OnInputActivated.Invoke(inputName);
                StartCoroutine(CountDownInput(inputName, inputData.duration));
            }
        }
        else 
        {
            Debug.LogWarning($"Input '{inputName}' not found in input buffer.");
        }
    }

    IEnumerator CountDownInput(string inputName, float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        inputBuffer[inputName].Deactivate();
    }

    [System.Serializable]
    struct InputBufferData
    {
        public float duration;
        private bool isActive;

        public InputBufferData(float duration)
        {
            this.duration = duration;
            isActive = false;
        }

        public void Activate() => isActive = true;

        public void Deactivate() => isActive = false;

        public readonly bool IsActive() => isActive;

        public bool Check()
        {
            if (isActive)
            {
                isActive = false;
                return true;
            }
            return false;
        }
    }
}
