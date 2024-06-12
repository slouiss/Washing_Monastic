using UnityEngine;

public class CollisionSetup : MonoBehaviour
{
    void Start()
    {
        // Désactiver la collision entre le joueur et les obstacles
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("groudwall"), true);

        // Assurer la collision entre les monstres et les obstacles
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Mobs"), LayerMask.NameToLayer("groundwall"), false);
    }
}
