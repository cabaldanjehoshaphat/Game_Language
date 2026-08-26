using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Tile : MonoBehaviour
{
    [System.Serializable] // display data of script in inspector editor
    public class State
    {
        public Color fillColor;
        public Color outlineColor;
    }

    private TextMeshProUGUI text;

    // getting the Image function in the inspector
    private Image fill;
    // getting the Outline function in the inspector
    private Outline outline;

    public State state {get; private set;}
    public char letter {get; private set;}

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        fill = GetComponent<Image>();
        outline = GetComponent<Outline>();
    }

    public void SetLetter(char letter)
    {
        this.letter = letter;
        text.text = letter.ToString();
    }

    public void SetState(State state)
    {
        this.state = state;
        fill.color = state.fillColor;
        outline.effectColor = state.outlineColor;
    }
}
