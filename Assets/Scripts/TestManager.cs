using UnityEngine;

public class TestManager : MonoBehaviour
{
    public Canvas promptInputCanvas;
    public Canvas environmentViewCanvas;

    void Start() {
        ShowPromptInput();

        // string testPath = "Palmov Island/Low Poly Atmospheric Locations Pack/Prefabs/Vehicles/space shuttle";
        // GameObject test = Resources.Load<GameObject>(testPath);
        // if (test != null)
        // {
        //     Debug.Log("Successfully loaded prefab manually.");
        //     Instantiate(test, Vector3.zero, Quaternion.identity);
        // }
        // else
        // {
        //     Debug.LogError("Manual prefab load failed. Check Resources path.");
        // }
    }

    public void ShowPromptInput()
    {
        promptInputCanvas.gameObject.SetActive(true);
        environmentViewCanvas.gameObject.SetActive(false);
        Debug.Log("Show prompt input canvas");
    }

    public void ShowEnvironmentView()
    {
        Debug.Log("Show env view called");
        promptInputCanvas.gameObject.SetActive(false);
        environmentViewCanvas.gameObject.SetActive(true);
        Debug.Log("Show env view canvas");
    }
}
