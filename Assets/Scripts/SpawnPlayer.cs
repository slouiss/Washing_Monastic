using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    public float camdistance = 9;
    private GameObject playerInstance;

    private void Start()
    {
        RoomManager roomManager = FindObjectOfType<RoomManager>();
        if (roomManager != null)
        {
            roomManager.OnGenerationComplete.AddListener(SpawnPlayer);
        }
        else
        {
            Debug.LogError("RoomManager not found in the scene.");
        }
    }

    private void SpawnPlayer()
    {
        if (playerPrefab != null && playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("Player spawned at (0, 0)");

            // Rendre la caméra principale enfant du joueur
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.transform.SetParent(playerInstance.transform);
                mainCamera.transform.localPosition = new Vector3(0, 0, camdistance); // Ajustez la position locale selon vos besoins
                mainCamera.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.LogError("Main camera not found in the scene.");
            }
        }
    }
}
