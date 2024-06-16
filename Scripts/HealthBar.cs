using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage; // Asigna esta imagen en el Inspector
    private float maxHealth;
    private float currentHealth;

    public void Initialize(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = currentHealth / maxHealth;
        }
    }
}
