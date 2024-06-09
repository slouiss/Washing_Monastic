using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private int damage = 10;  // Dégâts infligés par l'arme

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            Monster monster = collision.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(damage);  // Infliger des dégâts au monstre
            }
        }
    }
}
