using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    [Header("Credits UI (optional)")]
    [SerializeField] private GameObject creditsPanel;   // Hide by default
    [SerializeField] private Button creditsBackButton;  

    [Header("Scene To Load")]
    [SerializeField] private string gameSceneName = "GameScene";

    private void Awake()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);

        if (startButton != null) startButton.onClick.AddListener(OnClickStart);
        if (creditsButton != null) creditsButton.onClick.AddListener(OnClickCredits);
        if (quitButton != null) quitButton.onClick.AddListener(OnClickQuit);
        if (creditsBackButton != null) creditsBackButton.onClick.AddListener(CloseCredits);
    }

    private void OnClickStart()
    {
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogError("[MainMenu] Game scene name is empty. Set it in the inspector.");
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    private void OnClickCredits()
    {
        if (creditsPanel == null)
        {
            Debug.LogWarning("[MainMenu] No credits panel assigned.");
            return;
        }

        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    private void OnClickQuit()
    {
#if UNITY_EDITOR
        // Stop play mode in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
