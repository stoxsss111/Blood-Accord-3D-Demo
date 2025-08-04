using UnityEngine;
using UnityEngine.UI;

using UnityEngine;

public class ShowPressEHint : MonoBehaviour
{
    public GameObject hintUI;
    public CameraSwitcher cameraSwitcher;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hintUI.SetActive(true);
            cameraSwitcher.canSwitch = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hintUI.SetActive(false);
            cameraSwitcher.canSwitch = false;
        }
    }
}
