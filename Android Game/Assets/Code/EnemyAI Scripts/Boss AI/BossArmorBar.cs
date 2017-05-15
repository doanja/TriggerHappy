using UnityEngine;

/*
 * Displays Boss's armorh bar.
 */
public class BossArmorBar : MonoBehaviour
{

    public BossAI Enemy;
    public Transform ForegroundSpirte;
    public SpriteRenderer ForegroundRenderer; // handles the color of the Armor bar
    public Color MaxArmorColor = new Color(255 / 255f, 63 / 255f, 63 / 255f);
    public Color MinArmorColor = new Color(64 / 255f, 137 / 255f, 255 / 255f);

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var ArmorPercent = Enemy.CurrentArmor / (float)Enemy.MaxArmor;
        ForegroundSpirte.localScale = new Vector3(ArmorPercent, 1, 1);
        ForegroundRenderer.color = Color.Lerp(MaxArmorColor, MinArmorColor, ArmorPercent);
    }
}
