using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("Root Panel")]
    [SerializeField] private GameObject tutorialPanel;

    [Header("Elements (in order)")]
    [Tooltip("Each Tutorial Element should be a UI GameObject that contains an Image + TMP_Text.")]
    [SerializeField] private List<GameObject> tutorialElements = new List<GameObject>();

    [Header("Buttons")]
    [SerializeField] private Button prevButton;  
    [SerializeField] private Button nextButton;
    [SerializeField] private Button closeButton;

    [Header("Step UI")]
    [SerializeField] private TMP_Text stepCounterText; 

    private int _index = -1; 

    private void Awake()
    {
        if (prevButton) prevButton.onClick.AddListener(ShowPrev);
        if (nextButton) nextButton.onClick.AddListener(ShowNext);
        if (closeButton) closeButton.onClick.AddListener(CloseTutorial);

        SetAllElementsActive(false);
    }

    private void Start()
    {
        OpenTutorial();
    }

    // --- Public control ---
    public void OpenTutorial()
    {
        if (tutorialPanel) tutorialPanel.SetActive(true);
        _index = -1;
        SetAllElementsActive(false);
        ShowNext(); // jump to first
    }

    public void CloseTutorial()
    {
        if (tutorialPanel) tutorialPanel.SetActive(false);
        SetAllElementsActive(false);
        _index = -1;
    }

    // --- Navigation ---
    public void ShowNext()
    {
        // Hide current
        if (IsValidIndex(_index))
            tutorialElements[_index].SetActive(false);

        // Move forward
        _index++;

        // Past the last? close panel
        if (!IsValidIndex(_index))
        {
            CloseTutorial();
            return;
        }

        tutorialElements[_index].SetActive(true);
        UpdateNavUI();
    }

    public void ShowPrev()
    {
        if (!IsValidIndex(_index))
        {
            // Not started yet; do nothing
            return;
        }

        // Hide current
        tutorialElements[_index].SetActive(false);

        // Move backward but clamp at 0
        _index = Mathf.Max(0, _index - 1);

        tutorialElements[_index].SetActive(true);
        UpdateNavUI();
    }

    // --- Helpers ---
    private void UpdateNavUI()
    {
        // Step counter
        if (stepCounterText)
        {
            int curr = Mathf.Clamp(_index + 1, 0, Mathf.Max(1, tutorialElements.Count));
            stepCounterText.text = $"Step {curr}/{Mathf.Max(1, tutorialElements.Count)}";
        }

        // Prev interactable only after first step
        if (prevButton) prevButton.interactable = (_index > 0);

        // Next label: "Finish" on last step
        if (nextButton)
        {
            var label = nextButton.GetComponentInChildren<TMP_Text>();
            if (label)
                label.text = (_index == tutorialElements.Count - 1) ? "Finish" : "Next";
        }
    }

    private void SetAllElementsActive(bool state)
    {
        for (int i = 0; i < tutorialElements.Count; i++)
        {
            if (tutorialElements[i])
                tutorialElements[i].SetActive(state);
        }
    }

    private bool IsValidIndex(int i) => i >= 0 && i < tutorialElements.Count;
}
