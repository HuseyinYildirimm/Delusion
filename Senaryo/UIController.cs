using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public float shakeAmount = 10f;
    public float shakeSpeed = 50f;
    public float x_offset = 0f;
    public float y_offset = 0f;

    private Vector2 initialPos;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPos = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
        float shakeY = Mathf.Cos(Time.time * shakeSpeed) * shakeAmount;
        Vector2 shakeOffset = new Vector2(shakeX, shakeY);

        Vector2 newPos = initialPos + shakeOffset + new Vector2(x_offset, y_offset);
        rectTransform.anchoredPosition = newPos;
    }
}
