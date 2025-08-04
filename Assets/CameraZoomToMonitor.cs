using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera monitorCamera;
    public KeyCode switchKey = KeyCode.E;

    private bool isMonitorActive = false;
    public bool canSwitch = false; // <-- вот этот флаг

    void Start()
    {
        mainCamera.enabled = true;
        monitorCamera.enabled = false;
    }

    void Update()
    {
        if (canSwitch && Input.GetKeyDown(switchKey))
        {
            isMonitorActive = !isMonitorActive;

            mainCamera.enabled = !isMonitorActive;
            monitorCamera.enabled = isMonitorActive;
        }
    }
}