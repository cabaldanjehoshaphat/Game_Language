using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WordSearchCell : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    [Header("Colors!")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color foundColor = Color.green;

    private Image background;
    public int row;
    public int column;

    public char letter;

    public TMP_Text letterText;

    private void Awake()
    {
        background = GetComponent<Image>();
        letterText = GetComponentInChildren<TMP_Text>();
    }

    public void SetLetter(char c)
    {
        letter = c;
        letterText.text = c.ToString();
    }

    public void Highlight()
    {
        background.color = selectedColor;
    }

    public void LowLight()
    {
        background.color = normalColor;
    }

    public void MarkFound()
    {
        background.color = foundColor;
    }

    public void SetHighlight(Color color)
    {
        background.color = color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        WordSearchManager.Instance.StartSelection(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            WordSearchManager.Instance.ContinueSelection(this);
        }
    }
}