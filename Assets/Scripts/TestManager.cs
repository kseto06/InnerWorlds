using UnityEngine;

public class TestManager : MonoBehaviour
{
    public Canvas promptInputCanvas;
    public Canvas environmentViewCanvas;
    public bool isInputActive = true;

    void Start() {
        ShowPromptInput();
    }

    public void ShowPromptInput()
    {
        promptInputCanvas.gameObject.SetActive(true);
        environmentViewCanvas.gameObject.SetActive(false);
        Debug.Log("Show prompt input canvas");
        isInputActive = true;
    }

    public void ShowEnvironmentView()
    {
        Debug.Log("Show env view called");
        promptInputCanvas.gameObject.SetActive(false);
        environmentViewCanvas.gameObject.SetActive(true);
        Debug.Log("Show env view canvas");
        isInputActive = false;
    }
}
