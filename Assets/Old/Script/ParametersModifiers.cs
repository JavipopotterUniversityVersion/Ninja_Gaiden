using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ParametersModifiers : MonoBehaviour
{
    [SerializeField] EventTrigger _speedUpButton;
    [SerializeField] EventTrigger _speedDownButton;
    [SerializeField] TextMeshProUGUI _speedText;

    [SerializeField] EventTrigger _airSpeedUpButton;
    [SerializeField] EventTrigger _airSpeedDownButton;
    [SerializeField] TextMeshProUGUI _airSpeedText;

    [SerializeField] Player _player;

    const float MODIFIER_VALUE = 0.1f;

    private void Awake()
    {
        _speedText.text = $"Speed: {_player.Speed:F2}";
        _airSpeedText.text = $"Air Speed: {_player.AirSpeed:F2}";
        
        AddTriggerEntry(_speedUpButton, (data) =>
        {
            _player.Speed += MODIFIER_VALUE;
            _speedText.text = $"Speed: {_player.Speed:F2}";
        });

        AddTriggerEntry(_speedDownButton, (data) =>
        {
            _player.Speed -= MODIFIER_VALUE;
            _speedText.text = $"Speed: {_player.Speed:F2}";
        });

        AddTriggerEntry(_airSpeedUpButton, (data) =>
        {
            _player.AirSpeed += MODIFIER_VALUE;
            _airSpeedText.text = $"Air Speed: {_player.AirSpeed:F2}";
        });

        AddTriggerEntry(_airSpeedDownButton, (data) =>
        {
            _player.AirSpeed -= MODIFIER_VALUE;
            _airSpeedText.text = $"Air Speed: {_player.AirSpeed:F2}";
        });
    }

    void AddTriggerEntry(EventTrigger trigger, UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }
}
