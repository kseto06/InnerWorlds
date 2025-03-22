using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Linq;
#endif

public class AIManager : MonoBehaviour
{
    public string openAIApiKey;
    public static string json_content;

    [TextArea(4, 12)]
    public string systemPrompt = "";

#if UNITY_EDITOR
    [ContextMenu("Generate System Prompt from Prefabs")]
    public void GenerateSystemPromptFromPrefabs()
    {
        string basePath = "Assets/Palmov Island/Low Poly Atmospheric Locations Pack/Prefabs";
        var categories = new Dictionary<string, List<string>>();

        // Construct text prompts
        string prompt = "You are an AI that transforms natural language prompts into 3D landscape scenes. " +
                        "Use the generateScene function to place prefabs into the world. " +
                        "IMPORTANT: Only use the following prefab names for the 'name' field. " +
                        "Each entry includes the approximate size of the object (width x height x depth in Unity units). " +
                        "Each object should only contain 'name', 'category', 'x', 'y', and 'z' fields. No descriptions, colors, or emotions. " +
                        "Ensure the scene includes a large land object (e.g: terrain or flat ground) as the base where all other objects will be placed. This land should span the entire area and be suitable for supporting environmental features like trees, rocks, or buildings." +
                        "You must also generate a MINIMUM of 10 objects.";

        foreach (string folder in Directory.GetDirectories(basePath, "*", SearchOption.AllDirectories))
        {
            string categoryPath = folder.Replace(basePath + "/", "");
            string categoryName = Path.GetFileName(folder);
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folder });

            List<string> prefabDescriptions = new();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (prefab != null)
                {
                    Bounds bounds = GetBounds(prefab);
                    string size = $"{bounds.size.x:F1} x {bounds.size.y:F1} x {bounds.size.z:F1}";
                    string name = prefab.name;
                    string relativeCategory = assetPath.Replace(basePath + "/", "").Replace($"/{name}.prefab", "");
                    prefabDescriptions.Add($"{name} (Category: {relativeCategory}, Size: {size})");
                }
            }

            if (prefabDescriptions.Count > 0)
            {
                prompt += $"- {categoryName}: ";
                foreach (string entry in prefabDescriptions)
                {
                    prompt += $" - {entry}";
                }
            }
        }

        prompt += "Do not invent new object names. Only return JSON with prefab name and position fields: name, category, x, y, z. ";
        systemPrompt = prompt;
        Debug.Log("System prompt generated");
    }

    private Bounds GetBounds(GameObject prefab)
    {
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return new Bounds(Vector3.zero, Vector3.zero);

        Bounds combinedBounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers.Skip(1))
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }

        return combinedBounds;
    }
#endif

    public IEnumerator GetSceneData(string userPrompt, Action<string> callback)
    {
        string endpoint = "https://api.openai.com/v1/chat/completions";
        string json =
            "{\n" +
            "  \"model\": \"gpt-3.5-turbo-1106\",\n" +
            "  \"messages\": [\n" +
            "    { \"role\": \"system\", \"content\": \"" + EscapeJson(systemPrompt) + "\" },\n" +
            "    { \"role\": \"user\", \"content\": \"" + EscapeJson(userPrompt) + "\" }\n" +
            "  ],\n" +
            "  \"tools\": [\n" +
            "    {\n" +
            "      \"type\": \"function\",\n" +
            "      \"function\": {\n" +
            "        \"name\": \"generateScene\",\n" +
            "        \"description\": \"Creates a 3D landscape scene from a natural language prompt\",\n" +
            "        \"parameters\": {\n" +
            "          \"type\": \"object\",\n" +
            "          \"properties\": {\n" +
            "            \"objects\": {\n" +
            "              \"type\": \"array\",\n" +
            "              \"items\": {\n" +
            "                \"type\": \"object\",\n" +
            "                \"properties\": {\n" +
            "                  \"name\": { \"type\": \"string\" },\n" +
            "                  \"category\": { \"type\": \"string\" },\n" +
            "                  \"x\": { \"type\": \"number\" },\n" +
            "                  \"y\": { \"type\": \"number\" },\n" +
            "                  \"z\": { \"type\": \"number\" }\n" +
            "                },\n" +
            "                \"required\": [\"name\", \"category\", \"x\", \"y\", \"z\"]\n" +
            "              }\n" +
            "            }\n" +
            "          },\n" +
            "          \"required\": [\"objects\"]\n" +
            "        }\n" +
            "      }\n" +
            "    }\n" +
            "  ],\n" +
            "  \"tool_choice\": \"auto\",\n" +
            "  \"temperature\": 0.8\n" +
            "}";

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

            UnityWebRequest request = new UnityWebRequest(endpoint, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + openAIApiKey);

            yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            //Extract the response
            string response = request.downloadHandler.text;
            Debug.Log("GPT Response:\n" + response);
            json_content = ExtractStructuredScene(response);

            // Store the data for the env scene:
            PlayerPrefs.SetString("env_json", json_content);

            //Load objects into the new Environment
            var manager = FindAnyObjectByType<TestManager>();
            if (manager == null)
            {
                Debug.LogError("❌ TestManager not found in scene.");
            }
            else
            {
                Debug.Log("✅ TestManager found. Calling ShowEnvironmentView()");
                manager.ShowEnvironmentView();
            }

            FindAnyObjectByType<EnvironmentLoader>()?.GenerateEnvironmentFromJson(json_content);

            callback?.Invoke(json_content);
        }
        else
        {
            string errorBody = request.downloadHandler.text;
            Debug.LogError("Full Error Body:\n" + request.downloadHandler.text);
            Debug.LogError("Raw JSON Sent:\n" + json);

        }
    }

    private string EscapeJson(string str)
    {
        return str.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    private string ExtractStructuredScene(string response)
    {
        var parsed = JsonUtility.FromJson<OpenAIResponseWrapper>(response);
        return parsed.choices[0].message.tool_calls[0].function.arguments;
    }

    [Serializable]
    public class OpenAIResponseWrapper
    {
        public Choice[] choices;

        [Serializable]
        public class Choice
        {
            public Message message;
        }

        [Serializable]
        public class Message
        {
            public ToolCall[] tool_calls;
        }

        [Serializable]
        public class ToolCall
        {
            public FunctionCall function;
        }

        [Serializable]
        public class FunctionCall
        {
            public string name;
            public string arguments;
        }
    }
}