using UnityEngine;

/*
* Resource: 
*
* This class displays in-game text such as points the player currently has,
* health recovered, damage taken, etc.
*/
public class FloatingText : MonoBehaviour {

    private static readonly GUISkin Skin = Resources.Load<GUISkin>("GameSkin"); // from Unity

    /*
    * @param text, the text to display
    * @param style, the style and format of the text
    * @positioner, the position of the text
    * Displays the floating text
    */
    public static FloatingText Show(string text, string style, IFloatingTextPositioner positioner)
    {
        var go = new GameObject("Floating Text");
        var floatingText = go.AddComponent<FloatingText>();
        floatingText.Style = Skin.GetStyle(style);
        floatingText._positioner = positioner;
        floatingText._content = new GUIContent(text);
        return floatingText;
    }

    private GUIContent _content;                    // the GUIContent
    private IFloatingTextPositioner _positioner;    // instance of IFloatingTextPositioner

    public string Text { get { return _content.text; } set { _content.text = value;  } }    // the text to be displayed
    public GUIStyle Style { get; set; }                                                     // the style of the text

    // Handles format and positioning of the text
	public void OnGUI()
    {
        var position = new Vector2();
        var contentSize = Style.CalcSize(_content);
        if(!_positioner.GetPosition(ref position, _content, contentSize))
        {
            Destroy(gameObject);
            return;
        }

        GUI.Label(new Rect(position.x, position.y, contentSize.x, contentSize.y), _content, Style);
    }
}
