using UnityEngine;

public class TimeRift : Interactable
{
    [Header("Time Rifts")]
    [SerializeField] TimeRift targetRift; // Reference to the rift this will travel to

    Player player;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    public override void Interact()
    {
        base.Interact();
        // Move player to target Rift
        TeleportPlayerToRift();
    }

    void TeleportPlayerToRift()
    {
        if (targetRift == null)
            return;

        player.Controller.enabled = false;
        player.transform.position = targetRift.transform.position;
        player.Controller.enabled = true;
    }
}
