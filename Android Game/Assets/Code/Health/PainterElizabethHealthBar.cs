using UnityEngine;

/*
* Class used to display the PainterElizabeth's health.
*/
public class PainterElizabethHealthBar : MonoBehaviour {

    public PainterElizabeth enemy;
    public Transform ForegroundSpirte;
    public SpriteRenderer ForegroundRenderer; // handles the color of the health bar
    public Color MaxHealthColor = new Color(255 / 255f, 63 / 255f, 63 / 255f);
    public Color MinHealthColor = new Color(64 / 255f, 137 / 255f, 255 / 255f);

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {
        var healthPercent = enemy.Health / (float)enemy.MaxHealth;
        ForegroundSpirte.localScale = new Vector3(healthPercent, 1, 1);
        // returns a color between MaxHealthColor and MinHealthColor based on the percentage
        ForegroundRenderer.color = Color.Lerp(MaxHealthColor, MinHealthColor, healthPercent);
    }
}
