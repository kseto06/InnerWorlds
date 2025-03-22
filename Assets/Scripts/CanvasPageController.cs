using UnityEngine;

public class CanvasPageController : MonoBehaviour
{
    public Canvas promptInputCanvas;
    public Canvas environmentViewCanvas;

    void Start() { 
        ShowPromptInput(); //Default prompt input on start
    }

    public void ShowPromptInput() {
        promptInputCanvas.enabled = true;
        environmentViewCanvas.enabled = false;
    }

    public void ShowEnvironmentView() {
        promptInputCanvas.enabled = false;
        environmentViewCanvas.enabled = true;
    }
}