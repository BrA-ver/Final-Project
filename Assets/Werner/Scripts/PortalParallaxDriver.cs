using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PortalParallaxDriver : MonoBehaviour
{
    [Tooltip("Assign your Player or the Main Camera (if it follows the player).")]
    public Transform player;

    private static readonly int PlayerPosID = Shader.PropertyToID("_PlayerPos");
    private static readonly int PortalPosID = Shader.PropertyToID("_PortalPos");

    private Renderer _renderer;
    private MaterialPropertyBlock _mpb;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
    }

    private void LateUpdate()
    {
        if (!player) return;

        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetVector(PlayerPosID, player.position);
        _mpb.SetVector(PortalPosID, transform.position);
        _renderer.SetPropertyBlock(_mpb);
    }
}
