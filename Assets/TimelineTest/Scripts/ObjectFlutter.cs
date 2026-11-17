using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ObjectFlutter : MonoBehaviour
{
    [SerializeField] private float horizontalAmplitude = 12f;
    [SerializeField] private float verticalAmplitude = 8f;
    [SerializeField] private float horizontalSpeed = 0.8f;
    [SerializeField] private float verticalSpeed = 1.1f;

    private RectTransform rectTransform;
    private Vector2 startingPosition;
    private float phase;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startingPosition = rectTransform.anchoredPosition;
        phase = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        float time = Time.time;
        float horizontalOffset = Mathf.Sin((time * horizontalSpeed) + phase) * horizontalAmplitude;
        float verticalOffset = Mathf.Cos((time * verticalSpeed) + phase) * verticalAmplitude;

        rectTransform.anchoredPosition = startingPosition + new Vector2(horizontalOffset, verticalOffset);
    }
}
