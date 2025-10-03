using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;       
    [SerializeField] private Button resumeButton;     
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("Settings")]
    [Tooltip("Optional: Set your main menu scene name here.")]
    [SerializeField] private string mainMenuScene = "MainScene";
    [Tooltip("Unlock and show cursor while paused.")]
    [SerializeField] private bool showCursorWhenPaused = true;

    private bool _isPaused;
    private float _prevTimeScale = 1f;

    private void Awake()
    {
        if (pausePanel) pausePanel.SetActive(false);

        if (resumeButton) resumeButton.onClick.AddListener(Resume);
        if (mainMenuButton) mainMenuButton.onClick.AddListener(GoToMainMenu);
        if (quitButton) quitButton.onClick.AddListener(QuitGame);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        if (_isPaused) return;

        _prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;                // pause game time
        AudioListener.pause = true;         // pause all audio

        if (pausePanel) pausePanel.SetActive(true);
        _isPaused = true;

        if (showCursorWhenPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        var es = EventSystem.current;
        if (es && resumeButton)
        {
            es.SetSelectedGameObject(null);
            es.SetSelectedGameObject(resumeButton.gameObject);
        }
    }

    public void Resume()
    {
        if (!_isPaused) return;

        Time.timeScale = _prevTimeScale;
        AudioListener.pause = false;

        if (pausePanel) pausePanel.SetActive(false);
        _isPaused = false;
    }

    public void GoToMainMenu()
    {
        // Restored before scene load
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (!string.IsNullOrEmpty(mainMenuScene))
            SceneManager.LoadScene(mainMenuScene);
        else
            Debug.LogError("[PauseMenu] Main menu scene name not set.");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDisable()
    {
        // Safety: if the object is disabled while paused, restore timescale/audio
        if (_isPaused)
        {
            Time.timeScale = _prevTimeScale;
            AudioListener.pause = false;
            _isPaused = false;
        }
    }
}
