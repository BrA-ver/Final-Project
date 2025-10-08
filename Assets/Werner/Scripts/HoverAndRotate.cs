using UnityEngine;

public class HoverAndRotate : MonoBehaviour
{
    [Header("Hover")]
    public float hoverAmplitude = 0.25f;

    public float hoverFrequency = 1.0f;

    public Vector3 hoverAxis = Vector3.up;

    public float phaseOffset = 0f;

    public bool useLocalSpaceForHover = true;

    [Header("Rotation")]
    public float rotationSpeed = 45f;

    public bool clockwise = true;

    public Vector3 rotationAxis = Vector3.up;

    public Space rotationSpace = Space.Self;

    Vector3 _basePos;

    void OnEnable()
    {
        _basePos = useLocalSpaceForHover ? transform.localPosition : transform.position;
        if (hoverAxis.sqrMagnitude < 1e-6f) hoverAxis = Vector3.up;
        hoverAxis.Normalize();
        if (rotationAxis.sqrMagnitude < 1e-6f) rotationAxis = Vector3.up;
        rotationAxis.Normalize();
    }

    void Update()
    {
        float t = Time.time + phaseOffset;
        float sine = Mathf.Sin(t * Mathf.PI * 2f * Mathf.Max(0f, hoverFrequency));
        Vector3 offset = hoverAxis * (sine * Mathf.Max(0f, hoverAmplitude));

        if (useLocalSpaceForHover)
            transform.localPosition = _basePos + offset;
        else
            transform.position = _basePos + offset;

        float sign = clockwise ? -1f : 1f;
        transform.Rotate(rotationAxis, rotationSpeed * sign * Time.deltaTime, rotationSpace);
    }

    public void RecenterHover()
    {
        _basePos = useLocalSpaceForHover ? transform.localPosition : transform.position;
    }
}
