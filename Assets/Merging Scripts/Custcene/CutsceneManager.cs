using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance;

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
    }

    private void OnEnable()
    {
        nextButton.onClick.AddListener(NextPage);
    }

    private void OnDisable()
    {
        nextButton.onClick.RemoveListener(NextPage);
        CaseFile.instance.onVerdictMade -= StartCutscene;
    }

    private void Start()
    {
        CaseFile.instance.onVerdictMade += StartCutscene;

        BG.SetActive(false);

        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "PrototypeFirst")
            StartCutscene(introCutscene);
    }

    public void StartCutscene(Cutscene targetCutScene)
    {

        if (InputHandler.instance != null)
            InputHandler.instance.UnlockCursor();

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
            if (BackgroundMusicManager.Instance != null)
                BackgroundMusicManager.Instance.StopMusic();

            panel.gameObject.SetActive(false);
            BG.SetActive(false);

            if (InputHandler.instance != null)
                InputHandler.instance.LockCursor();

            if (currentCutscene.Name != "Intro")
            {
                SceneManager.LoadScene(0);
                
            }
            return;
        }

        ShowCutscenePage();
    }
}
