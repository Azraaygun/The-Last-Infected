using UnityEngine;
using TMPro;

public class UIHintManager : MonoBehaviour
{
    public static UIHintManager Instance;

    [Header("UI Reference")]
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI hintText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HideHint();
    }

    public void ShowHint(string message)
    {
        hintText.text = message;
        hintPanel.SetActive(true);
    }

    public void HideHint()
    {
        hintPanel.SetActive(false);
    }

    public bool IsHintVisible()
    {
        return hintPanel.activeSelf;
    }
}
