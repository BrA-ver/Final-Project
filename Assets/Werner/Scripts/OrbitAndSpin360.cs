using UnityEngine;

public class OrbitAndSpin360_Simple : MonoBehaviour
{
    [Header("Center / Path")]
    public Transform center;

    public float angularSpeedDeg = 60f;

    public Vector3 orbitAxis = Vector3.up;

    public bool useCenterSpaceForAxis = true;

    public bool useInitialOffset = true;

    public float radius = 1.5f;

    public float startAngleDeg = 0f;

    Vector3 _baseOffset;
    float _angleDeg;
    Quaternion _initialRot;

    void OnEnable()
    {
        if (!center)
        {
            enabled = false;
            return;
        }

        if (orbitAxis.sqrMagnitude < 1e-6f) orbitAxis = Vector3.up;
        orbitAxis.Normalize();

        Vector3 axisWorld = GetAxisWorld();

        if (useInitialOffset)
        {
            _baseOffset = transform.position - center.position;
            if (_baseOffset.sqrMagnitude < 1e-6f)
                _baseOffset = AnyPerpendicular(axisWorld) * Mathf.Max(0.01f, radius);
        }
        else
        {
            _baseOffset = AnyPerpendicular(axisWorld) * Mathf.Max(0.01f, radius);
        }

        _initialRot = transform.rotation;
        _angleDeg = startAngleDeg;

        ApplyTransform();
    }

    void Update()
    {
        _angleDeg += angularSpeedDeg * Time.deltaTime;
        ApplyTransform();
    }

    void ApplyTransform()
    {
        Vector3 axisWorld = GetAxisWorld();

        Quaternion orbitRot = Quaternion.AngleAxis(_angleDeg, axisWorld);
        Vector3 rotatedOffset = orbitRot * _baseOffset;
        transform.position = center.position + rotatedOffset;

        Quaternion spin = Quaternion.AngleAxis(_angleDeg, axisWorld);
        transform.rotation = spin * _initialRot;
    }

    Vector3 GetAxisWorld()
    {
        return useCenterSpaceForAxis && center
            ? center.TransformDirection(orbitAxis).normalized
            : orbitAxis.normalized;
    }

    static Vector3 AnyPerpendicular(Vector3 v)
    {
        Vector3 a = Mathf.Abs(v.y) < 0.99f ? Vector3.up : Vector3.right;
        return Vector3.Cross(v, a).normalized;
    }

    public void RecenterFromCurrent()
    {
        if (!center) return;
        _baseOffset = transform.position - center.position;
    }
}
