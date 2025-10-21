using UnityEngine;

public class HoverIcon : MonoBehaviour
{
    [Header("Hover Settings")]
    [SerializeField] private float amplitude = 0.5f; // height of the hover
    [SerializeField] private float frequency = 1f;   // speed of the hover

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition; // keep it relative to parent
    }

    void Update()
    {
        // hover motion using sine wave
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}
