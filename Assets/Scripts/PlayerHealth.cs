using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider healthBar;

    public void SetHealth(int value)
    {
        healthBar.value = value;
    }

    public int GetMaxHealth()
    {
        return (int)healthBar.maxValue;
    }
}
