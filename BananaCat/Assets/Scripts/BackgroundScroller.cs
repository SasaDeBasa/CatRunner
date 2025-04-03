using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private RawImage backgroundImage; // Assign in Inspector
    [SerializeField] private float scrollSpeedX = 0.1f; // Speed for horizontal scrolling
    [SerializeField] private float scrollSpeedY = 0.05f; // Speed for vertical scrolling

    void Update()
    {
        // Move texture using uvRect
        backgroundImage.uvRect = new Rect(
            backgroundImage.uvRect.x + scrollSpeedX * Time.deltaTime,
            backgroundImage.uvRect.y + scrollSpeedY * Time.deltaTime,
            backgroundImage.uvRect.width,
            backgroundImage.uvRect.height
        );
    }
}
