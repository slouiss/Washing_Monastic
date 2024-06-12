using UnityEngine;

public class MonsterCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player collided with monster");

            // Find the Room script attached to the room this monster is in
            Room room = GetComponentInParent<Room>();
            if (room != null)
            {
                Debug.Log("Room script found, closing doors");
                room.CloseAllDoors();
            }
            else
            {
                Debug.LogWarning("Room script not found on parent object");
            }
        }
    }
}
