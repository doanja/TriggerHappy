using UnityEngine;

/*
 * Displays Boss's health bar.
 */
public class BossHealthBar : MonoBehaviour {

    public BossAI Enemy;
    public Transform ForegroundSpirte;
    public SpriteRenderer ForegroundRenderer; // handles the color of the health bar
    public Color MaxHealthColor = new Color(255 / 255f, 63 / 255f, 63 / 255f);
    public Color MinHealthColor = new Color(64 / 255f, 137 / 255f, 255 / 255f);
	
	// Update is called once per frame
	void Update () {
        var healthPercent = Enemy.CurrentHealth / (float)Enemy.MaxHealth;
        ForegroundSpirte.localScale = new Vector3(healthPercent, 1, 1);
        ForegroundRenderer.color = Color.Lerp(MaxHealthColor, MinHealthColor, healthPercent);
    }
}
