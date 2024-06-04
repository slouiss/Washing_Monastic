
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class TriggerChangeRoom : MonoBehaviour
{
    public int down = 9;
    [SerializeField] private string cameraTag = "MainCamera"; // Tag à assigner à la caméra
    private GameObject mainCamera;

    private void Start()
    {
        mainCamera = GameObject.FindWithTag(cameraTag);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            

            print("collision !\n");
        }
    }
}
