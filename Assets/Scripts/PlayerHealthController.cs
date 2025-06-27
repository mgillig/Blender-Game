using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Slider healthBar;

    public void SetHealth(int value)
    {
        healthBar.value = value;
    }

    public int GetMaxHealth()
    {
        return (int)healthBar.maxValue;
    }

    public void DisplayHealthBar(bool healthBarActive)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(healthBarActive);
        }
    }
}
