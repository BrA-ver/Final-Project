using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance;

    // ⭐ Global flag for input + cursor control
    public static bool IsCutsceneActive { get; set; }

    [SerializeField] Button nextButton;
    [SerializeField] Cutscene introCutscene;

    Cutscene currentCutscene;
    int index;

    [Header("UI")]
    [SerializeField] GameObject BG;
    [SerializeField] Image panel;

    private void Awake()
    {
        instance = this;

        string sceneName = SceneManager.GetActiveScene().name;

        // ⭐ PrototypeFirst ALWAYS begins with a cutscene
        if (sceneName == "PrototypeFirst")
            IsCutsceneActive = true;

        // ⭐ Force cursor unlocked BEFORE ANY Start() runs
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnEnable()
    {
        nextButton.onClick.AddListener(NextPage);
    }

    private void OnDisable()
    {
        nextButton.onClick.RemoveListener(NextPage);

        if (CaseFile.instance != null)
            CaseFile.instance.onVerdictMade -= StartCutscene;
    }

    private void Start()
    {
        if (CaseFile.instance != null)
            CaseFile.instance.onVerdictMade += StartCutscene;

        BG.SetActive(false);

        // ⭐ Auto-start intro cutscene
        if (SceneManager.GetActiveScene().name == "PrototypeFirst")
            StartCutscene(introCutscene);
    }

    public void StartCutscene(Cutscene targetCutScene)
    {
        IsCutsceneActive = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        BG.SetActive(true);
        panel.gameObject.SetActive(true);

        index = 0;
        currentCutscene = targetCutScene;

        ShowCutscenePage();
    }

    void ShowCutscenePage()
    {
        panel.sprite = currentCutscene.images[index];
    }

    void NextPage()
    {
        index++;

        if (index >= currentCutscene.images.Length)
        {
            // Stop music
            if (BackgroundMusicManager.Instance != null)
                BackgroundMusicManager.Instance.StopMusic();

            panel.gameObject.SetActive(false);
            BG.SetActive(false);

            // ⭐ End cutscene
            IsCutsceneActive = false;

            // ⭐ Relock cursor for gameplay
            if (InputHandler.instance != null)
                InputHandler.instance.LockCursor();
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            // Load menu if it’s not the intro
            if (currentCutscene.Name != "Intro")
                SceneManager.LoadScene(0);

            return;
        }

        ShowCutscenePage();
    }
}
