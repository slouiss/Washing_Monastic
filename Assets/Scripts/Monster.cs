using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;  // Points de vie maximum
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;  // Initialiser la santé actuelle au maximum
    }

    // Méthode pour infliger des dégâts au monstre
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;  // Réduire les points de vie
        Debug.Log("Monster took " + damage + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();  // Si les points de vie sont à 0 ou moins, le monstre meurt
        }
    }

    // Méthode pour gérer la mort du monstre
    private void Die()
    {
        Debug.Log("Monster died!");
        // Vous pouvez ajouter ici des effets visuels ou du son pour la mort
        Destroy(gameObject);  // Détruire l'objet du monstre
    }
}
