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
    [SerializeField] private Button creditsBackButton;  // A button inside the credits panel

    [Header("Scene To Load")]
    [SerializeField] private string gameSceneName = "GameScene"; // Change to your game scene name

    [Header("Gamepad/Keyboard Focus")]
    [SerializeField] private GameObject firstSelectedOnMenu;     // e.g., Start button
    [SerializeField] private GameObject firstSelectedOnCredits;  // e.g., Back button on credits

    private void Awake()
    {
        // Basic null safety so it’s easy to drop in
        if (creditsPanel != null) creditsPanel.SetActive(false);

        if (startButton != null) startButton.onClick.AddListener(OnClickStart);
        if (creditsButton != null) creditsButton.onClick.AddListener(OnClickCredits);
        if (quitButton != null) quitButton.onClick.AddListener(OnClickQuit);
        if (creditsBackButton != null) creditsBackButton.onClick.AddListener(CloseCredits);
    }

    private void OnEnable()
    {
        // Set initial selection for keyboard/controller navigation
        Select(firstSelectedOnMenu != null ? firstSelectedOnMenu : (startButton ? startButton.gameObject : null));
    }

    private void OnClickStart()
    {
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogError("[MainMenu] Game scene name is empty. Set it in the inspector.");
            return;
        }

        // Simple synchronous load (fine for small scenes; swap to async if needed)
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
        // Trap focus inside credits for controllers
        Select(firstSelectedOnCredits != null ? firstSelectedOnCredits : creditsBackButton ? creditsBackButton.gameObject : null);
    }

    public void CloseCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
        Select(firstSelectedOnMenu != null ? firstSelectedOnMenu : (startButton ? startButton.gameObject : null));
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

    private void Select(GameObject go)
    {
        if (go == null) return;
        var es = EventSystem.current;
        if (es == null) return;
        es.SetSelectedGameObject(null);
        es.SetSelectedGameObject(go);
    }
}
