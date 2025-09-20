using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [field: SerializeField] public bool IsInteracting { get; set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }

    }
}
