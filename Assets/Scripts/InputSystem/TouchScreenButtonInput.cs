using UnityEngine;
using UnityEngine.EventSystems;

public class TouchScreenButtonInput : MonoBehaviour
{
    [SerializeField] string inputName;

    public void SendInput()
    {
        InputBuffer.Instance.ActivateInput(inputName);
    }
}
