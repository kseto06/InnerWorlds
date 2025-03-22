using UnityEngine;
using System.Collections.Generic;

public class EnvironmentLoader : MonoBehaviour
{
    //Arraylist to store objects
    public List<ResolvedEnvironmentObject> loadedObjects = new List<ResolvedEnvironmentObject>();

    public void GenerateEnvironmentFromJson(string json_content) {
        loadedObjects.Clear();
        EnvironmentDataWrapper data = JsonUtility.FromJson<EnvironmentDataWrapper>(json_content); //Load json_content from AIManager

        // Load prefabs
        foreach (var obj in data.objects) {
            GameObject prefab = LoadPrefab(obj.category, obj.name);

            if (prefab != null) {
                //Resolve objects to store and cache
                var resolved = new ResolvedEnvironmentObject {
                    prefab = prefab,
                    position = new Vector3(obj.x, obj.y, obj.z),
                    rotation = Quaternion.identity,
                };
                loadedObjects.Add(resolved);
            } else {
                Debug.LogWarning($"Prefab not found for {obj.category}/{obj.name}");
            }
        }
        Debug.Log($"Loaded {data.objects.Length} objects into the env");
        FindAnyObjectByType<EnvironmentGenerator>()?.GenerateScene(loadedObjects);
    }

    public GameObject LoadPrefab(string category, string prefabName) {
        string path = $"Palmov Island/Low Poly Atmospheric Locations Pack/Prefabs/{category}/{prefabName}";
        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab == null) {
            Debug.LogError($"Error loading prefab: {path}");
            return null;
        }

        return prefab;
    }

    // Objects:
    [System.Serializable]
    public class EnvironmentObject {
        public string category;
        public string name;
        public float x, y, z;
    }

    [System.Serializable]
    public class PositionData
    {
        public float x, y, z;
    }

    [System.Serializable]
    public class RotationData
    {
        public float x, y, z;
    }

    [System.Serializable]
    public class EnvironmentDataWrapper
    {
        public EnvironmentObject[] objects;
    }

    public class ResolvedEnvironmentObject {
        public GameObject prefab;
        public Vector3 position;
        public Quaternion rotation;
    }
}
