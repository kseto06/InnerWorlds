using UnityEngine;

public class EnvironmentSceneInitializer : MonoBehaviour
{
    void Start()
    {
        Debug.Log("EnvSceneInit called");

        string json = PlayerPrefs.GetString("env_json", "");
        if (!string.IsNullOrEmpty(json))
        {
            EnvironmentLoader loader = FindAnyObjectByType<EnvironmentLoader>();
            if (loader != null)
            {
                loader.GenerateEnvironmentFromJson(json);
            }
        }
    }
}
