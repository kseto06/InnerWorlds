using UnityEngine;
using System.Collections.Generic;

public class EnvironmentGenerator : MonoBehaviour 
{
    public Transform environmentParent;
    public void GenerateScene(List<EnvironmentLoader.ResolvedEnvironmentObject> objects) {
        //Instantiate each object into the environment
        foreach (var obj in objects) {
            GameObject instance = Instantiate(obj.prefab, obj.position, obj.rotation);
            if (environmentParent != null) {
                instance.transform.parent = environmentParent;
            }
        }

        Debug.Log($"Generated {objects.Count} objects into the env");
    }
}