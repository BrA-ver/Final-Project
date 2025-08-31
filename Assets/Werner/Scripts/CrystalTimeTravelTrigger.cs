using UnityEngine;

public class CrystalTimeTravelTrigger : MonoBehaviour
{
    public float cooldown = 0.5f;
    private float nextAllowedTime;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (Time.time < nextAllowedTime) return;

        var tt = other.GetComponent<TimeTravel>();
        if (tt != null)
        {
            tt.TryStartTimeTravel();
            nextAllowedTime = Time.time + cooldown;
        }
    }
}
