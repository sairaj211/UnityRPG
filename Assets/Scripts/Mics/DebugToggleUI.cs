using UnityEngine;
using UnityEngine.UI;

public class DebugToggleUI : MonoBehaviour
{
    [SerializeField] private Toggle debugToggle;

    private void Start()
    {
        // Set initial value
        debugToggle.isOn = DebugManager.IsDebugEnabled;
        // Subscribe to toggle event
        debugToggle.onValueChanged.AddListener(OnDebugToggleChanged);
    }

    private void OnDestroy()
    {
        debugToggle.onValueChanged.RemoveListener(OnDebugToggleChanged);
    }

    private void OnDebugToggleChanged(bool isOn)
    {
        DebugManager.IsDebugEnabled = isOn;
    }
}