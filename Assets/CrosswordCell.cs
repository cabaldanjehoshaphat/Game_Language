using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CrosswordCell : MonoBehaviour, IPointerClickHandler
{
    public int row, col;
    public int clueNumber;
    public bool isBlack = false;

    [Header("Cell Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = new Color(1f, 1f, 0.5f);
    [SerializeField] private Color selectedColor = new Color(0.6f, 0.8f, 1f);
    [SerializeField] private Color blackCellColor = new Color(0.2f, 0.2f, 0.2f);

    [HideInInspector] public TMP_Text numberText;
    [HideInInspector] public TMP_Text letterText;

    private Image background;

    private void Awake()
    {
        background = GetComponent<Image>();
        numberText = transform.Find("Number").GetComponent<TMP_Text>();
        letterText = transform.Find("Letter").GetComponent<TMP_Text>();
    }

    public void SetBlack(bool black)
    {
        isBlack = black;
        background.color = black ? blackCellColor : normalColor;

        numberText.gameObject.SetActive(!black);
        letterText.gameObject.SetActive(!black);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CrosswordManager.Instance.SelectCell(this);
    }

    public void SetHighlight(bool highlight, bool isSelected = false)
    {
        if (isBlack) return;

        if (isSelected)
            background.color = selectedColor;
        else if (highlight)
            background.color = highlightColor;
        else
            background.color = normalColor;
    }
}