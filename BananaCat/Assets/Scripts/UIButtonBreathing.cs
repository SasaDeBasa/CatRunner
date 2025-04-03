using UnityEngine;

public class UIButtonBreathing : MonoBehaviour
{
    public float scaleAmount = 1.1f; // Expand up to 110% size
    public float duration = 1f; // Time for one cycle

    void Start()
    {
        LeanTween.scale(gameObject, Vector3.one * scaleAmount, duration)
            .setLoopPingPong()
            .setEaseInOutSine()
            .setIgnoreTimeScale(true); // Ignore time scale for smoother animation
    }
}
