using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using Pathfinding; // Ajoutez ceci

public class RoomManager : MonoBehaviour
{
    [SerializeField] GameObject roomPrefab;
    [SerializeField] GameObject monsterPrefab; 
    [SerializeField] TileBase bossRoomWallTile;
    [SerializeField] TileBase bossRoomFloorTile;
    [SerializeField] private int maxMonstersPerRoom = 5;
    float roomWidth = 17F;
    int roomHeight = 9;
    int gridSizeX = 10;
    int gridSizeY = 10;

    private List<GameObject> roomObjects = new List<GameObject>();

    private bool generationComplete = false;

    private int[,] roomGrid;
    [SerializeField] private int roomCount;
    [SerializeField] private int maxRooms = 15;
    [SerializeField] private int minRooms = 7;

    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();

    public UnityEvent OnGenerationComplete;

    private void Start()
    {
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            RegenerateRooms();

        if (roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            Vector2Int roomIndex = roomQueue.Dequeue();
            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            TryGenerateRoom(new Vector2Int(gridX - 1, gridY)); // Left
            TryGenerateRoom(new Vector2Int(gridX + 1, gridY)); // Right
            TryGenerateRoom(new Vector2Int(gridX, gridY + 1)); // Up
            TryGenerateRoom(new Vector2Int(gridX, gridY - 1)); // Down
        }
        else if (roomCount < minRooms)
        {
            Debug.Log("roomCount was less than minimum amount of rooms, trying again");
            RegenerateRooms();
        }
        else if (!generationComplete)
        {
            Debug.Log($"Generation complete, {roomCount} rooms.");
            IdentifyAndMarkBossRoom();
            generationComplete = true;

            // Scan the graph after generation is complete
            AstarPath.active.Scan();

            OnGenerationComplete?.Invoke();
        }
    }

    private void RegenerateRooms()
    {
        // Clear previous data
        roomObjects.ForEach(Destroy);
        roomObjects.Clear();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector2(
            roomWidth * (gridX - gridSizeX / 2),
            roomHeight * (gridY - gridSizeY / 2));
    }

    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;
        int count = 0;

        if (x > 0 && roomGrid[x - 1, y] != 0) count++; // Left
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0) count++; // Right
        if (y > 0 && roomGrid[x, y - 1] != 0) count++; // Down
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0) count++; // Up

        return count;
    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY || roomGrid[x, y] != 0)
            return false;

        int adjacentRooms = CountAdjacentRooms(roomIndex);

        if (adjacentRooms > 1)
            return false;

        if (roomCount >= maxRooms)
            return false;

        if (Random.value < 0.5f && roomIndex != Vector2Int.zero)
            return false;

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        var newRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        roomObjects.Add(newRoom);

        OpenDoors(newRoom, x, y);

        // Generate monsters in the room
        int monsterCount = Random.Range(1, maxMonstersPerRoom + 1);
        for (int i = 0; i < monsterCount; i++)
        {
            InstantiateMonster(newRoom.transform);
        }

        return true;
    }

    private void IdentifyAndMarkBossRoom()
    {
        List<GameObject> potentialBossRooms = new List<GameObject>();

        foreach (var room in roomObjects)
        {
            var roomScript = room.GetComponent<Room>();
            if (roomScript != null)
            {
                Vector2Int roomIndex = roomScript.RoomIndex;
                if (CountAdjacentRooms(roomIndex) == 1 && roomIndex != new Vector2Int(gridSizeX / 2, gridSizeY / 2))
                {
                    potentialBossRooms.Add(room);
                }
            }
        }

        if (potentialBossRooms.Count > 0)
        {
            GameObject bossRoom = potentialBossRooms[Random.Range(0, potentialBossRooms.Count)];
            bossRoom.tag = "RoomBoss";

            // Change the tiles of the boss room's Tilemaps
            var tilemaps = bossRoom.GetComponentsInChildren<Tilemap>();
            foreach (var tilemap in tilemaps)
            {
                if (tilemap.tag == "WallTilemap" || tilemap.tag == "Door_l" || tilemap.tag == "Door_r" || tilemap.tag == "Door_t" || tilemap.tag == "Door_b")
                {
                    ChangeTiles(tilemap, bossRoomWallTile);
                }
                else if (tilemap.tag == "GroundTilemap")
                {
                    ChangeTiles(tilemap, bossRoomFloorTile);
                }
            }

            Debug.Log("Boss room marked at: " + bossRoom.GetComponent<Room>().RoomIndex);
        }
    }

    private void ChangeTiles(Tilemap tilemap, TileBase newTile)
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                Vector3Int localPlace = (new Vector3Int(x, y, 0));
                Vector3Int place = new Vector3Int(bounds.x + localPlace.x, bounds.y + localPlace.y, 0);
                if (allTiles[x + y * bounds.size.x] != null)
                {
                    tilemap.SetTile(place, newTile);
                }
            }
        }
    }

    void OpenDoors(GameObject room, int x, int y)
    {
        Room newRoomScript = room.GetComponent<Room>();

        Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room topRoomScript = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room bottomRoomScript = GetRoomScriptAt(new Vector2Int(x, y - 1));

        if (x > 0 && roomGrid[x - 1, y] != 0)
        {
            newRoomScript.OpenDoor(Vector2Int.left);
            leftRoomScript.OpenDoor(Vector2Int.right);
        }
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0)
        {
            newRoomScript.OpenDoor(Vector2Int.right);
            rightRoomScript.OpenDoor(Vector2Int.left);
        }
        if (y > 0 && roomGrid[x, y - 1] != 0)
        {
            newRoomScript.OpenDoor(Vector2Int.down);
            bottomRoomScript.OpenDoor(Vector2Int.up);
        }
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0)
        {
            newRoomScript.OpenDoor(Vector2Int.up);
            topRoomScript.OpenDoor(Vector2Int.down);
        }
    }

    Room GetRoomScriptAt(Vector2Int index)
    {
        var roomObject = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index);
        if (roomObject != null)
        {
            return roomObject.GetComponent<Room>();
        }
        return null;
    }

    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex);
        int x = roomIndex.x;
        int y = roomIndex.y;
        roomGrid[x, y] = 1;
        roomCount++;
        var initialRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room-{roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;
        initialRoom.tag = "SpawnRoom";
        roomObjects.Add(initialRoom);
    }

    private void InstantiateMonster(Transform roomTransform)
    {
        float roomWidth = 17.2f;
        float roomHeight = 9f;
        float wallThickness = 1f;

        Vector3 randomPosition = new Vector3(
            Random.Range(-roomWidth / 2 + wallThickness, roomWidth / 2 - wallThickness),
            Random.Range(-roomHeight / 2 + wallThickness, roomHeight / 2 - wallThickness),
            0);

        var monster = Instantiate(monsterPrefab, roomTransform);
        monster.transform.localPosition = randomPosition;
    }

    private void OnDrawGizmos()
    {
        Color gizmoColor = new Color(0f, 1f, 1f, 0.05f);
        Gizmos.color = gizmoColor;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(position, new Vector3(roomWidth, roomHeight, 1));
            }
        }
    }
}
