using UnityEngine;
using System.Collections;

public class DisappearingPlatform : MonoBehaviour
{
    public bool toDisappear = false;

    private Renderer renderer;
    private Collider collider;

    void Start() {
        renderer = GetComponent<Renderer>();
        collider = GetComponent<Collider>();
    }

    public void StartDisappearance() {
        toDisappear = true;
        if (toDisappear) {
            StartCoroutine(Disappear());
        }
    }

    public IEnumerator Disappear() {
        renderer.enabled = false;
        collider.enabled = false;

        yield return null;
    }
}