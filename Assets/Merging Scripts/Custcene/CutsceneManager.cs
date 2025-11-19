using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance;

    [SerializeField] Cutscene[] cutscenes;

    [SerializeField] Button nextButton;

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
        //StartCutscene("Intro");
    }

    private void LateUpdate()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartCutscene(string cutscenName)
    {
        Debug.Log("Cutsene Started");
        BG.SetActive(true);

        foreach (Cutscene cutscene in cutscenes)
        {
            if (cutscene.Name == cutscenName)
            {
                currentCutscene = cutscenes[0];
                ShowCutscenePage();
            }
        }
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
            // Reset Index And End The Cutscene
            panel.gameObject.SetActive(false);


            SceneManager.LoadScene(0);
            return;
        }

        ShowCutscenePage();
    }
}
