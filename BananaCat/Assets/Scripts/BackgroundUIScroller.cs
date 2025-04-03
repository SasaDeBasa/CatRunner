using UnityEngine;
using UnityEngine.UI;

public class BackgroundUIScroller : MonoBehaviour
{
    public RawImage backgroundImage;
    public float scrollSpeedX = 0.1f; // Speed in X direction
    public float scrollSpeedY = 0f;   // Speed in Y direction

    void Update()
    {
        backgroundImage.uvRect = new Rect(
            backgroundImage.uvRect.x + scrollSpeedX * Time.deltaTime,
            backgroundImage.uvRect.y + scrollSpeedY * Time.deltaTime,
            backgroundImage.uvRect.width,
            backgroundImage.uvRect.height
        );
    }
}
