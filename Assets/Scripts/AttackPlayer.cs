using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject weaponPrefab;  // Prefab de l'arme
    [SerializeField] private float attackDuration = 0.2f;  // Durée de l'attaque

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Si le bouton gauche de la souris est cliqué
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - transform.position).normalized;

            Attack(direction);
        }
    }

    private void Attack(Vector2 direction)
    {
      
        GameObject weaponInstance = Instantiate(weaponPrefab, transform.position, Quaternion.identity);
        weaponInstance.transform.up = direction; 

        // Détruire le prefab de l'arme après la durée de l'attaque
        Destroy(weaponInstance, attackDuration);
    }
}
