using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatGPTTester : MonoBehaviour
{
    public TMP_InputField promptInput;
    public Button sendButton;
    public TMP_Text outputText;
    public AIManager aiManager;

    void Start()
    {
        sendButton.onClick.AddListener(() =>
        {
            string prompt = promptInput.text;
            outputText.text = "Loading...";
            StartCoroutine(aiManager.GetSceneData(prompt, (response) =>
            {
                outputText.text = "Response:\n" + response;
                Debug.Log("GPT Response:\n" + response);
            }));
        });
    }
}