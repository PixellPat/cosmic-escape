using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyEnums;


[CreateAssetMenu(fileName = "NewRoomGenerator", menuName = "Scriptable Objects/Game Data/Room Generator")]
public class RoomGeneratorSO : ScriptableObject
{
    //public GameObject floorTilePrefab;
    //public GameObject wallTilePrefab;
    //public GameObject doorTilePrefab;

    public RoomSO[] roomPrefabs;
    private Transform roomsParent;

    private int roomWidth = 11;
    private int roomHeight = 9;
    private int currentRoomIndex;
    private List<RoomSO> generatedRooms = new List<RoomSO>();


    public void Initialize(Transform parent)
    {
        roomsParent = new GameObject("RoomCenters").transform;
        roomsParent.SetParent(parent);
    }

    public void GenerateRooms(List<Vector2Int> roomLocations)
    {
        int maxX = roomLocations.Max(location => location.x);
        int maxY = roomLocations.Max(location => location.y);

        RoomSO[,] generatedRoomGrid = GenerateAndStoreRooms(roomLocations);
        ConnectRooms(generatedRoomGrid, maxX, maxY);
        DebugLogGeneratedRooms();
    }

    private RoomSO[,] GenerateAndStoreRooms(List<Vector2Int> roomLocations)
    {
        int maxX = roomLocations.Max(location => location.x);
        int maxY = roomLocations.Max(location => location.y);

        RoomSO[,] generatedRoomGrid = new RoomSO[maxX + 1, maxY + 1];

        Vector2Int firstLocation = roomLocations.First(); // Get the first location

        foreach (Vector2Int location in roomLocations)
        {
            bool isStarting = location == firstLocation; // Compare with the first location
            RoomSO roomInstance = InstantiateRoom(location, isStarting);
            generatedRooms.Add(roomInstance);
            generatedRoomGrid[location.x, location.y] = roomInstance;
        }

        return generatedRoomGrid;
    }

    private void ConnectRooms(RoomSO[,] generatedRoomGrid, int maxX, int maxY)
    {
        foreach (RoomSO room in generatedRooms)
        {
            foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
            {
                Vector2Int doorPosition = CalculateDoorPosition(room.gridPosition, direction);
                RoomSO connectedRoom = FindRoomAtLocation(generatedRoomGrid, doorPosition, maxX, maxY);

                if (connectedRoom != null)
                {
                    ConnectRooms(room, connectedRoom, doorPosition, direction);
                }
            }
        }
    }

    private void ConnectRooms(RoomSO room, RoomSO connectedRoom, Vector2Int doorPosition, Direction direction)
    {
        room.AddDoor(doorPosition, direction);

        // Calculate the opposite door position in the connected room
        Vector2Int oppositeDoorPosition = CalculateConnectedDoorPosition(doorPosition, GetOppositeDirection(direction));

        // Connect the connected room's door
        connectedRoom.AddDoor(oppositeDoorPosition, GetOppositeDirection(direction));
    }

    private void DebugLogGeneratedRooms()
    {
        foreach (var room in generatedRooms)
        {
            Debug.Log("Room Location: " + room.gridPosition + "\n");

            foreach (var doorPosition in room.doorPositions)
            {
                Direction doorDirection = room.GetDoorDirection(doorPosition);
                Debug.Log("Door position: " + doorPosition + ", Door direction: " + doorDirection + "\n");
            }
        }
    }

    private RoomSO ChooseRandomRoomPrefab()
    {
        // Implement logic to choose a random room prefab from roomPrefabs array
        if (roomPrefabs.Length == 0)
        {
            Debug.LogWarning("No room prefabs available.");
            return null;
        }

        int randomIndex = Random.Range(0, roomPrefabs.Length);
        return roomPrefabs[randomIndex];
    }

    private Vector3 CalculateSpawnPosition(Vector2Int location)
    {
        float x = location.x * roomWidth;     
        float y = location.y * roomHeight;
        return new Vector3(x, y, 0f);
    }
     
    private RoomSO InstantiateRoom(Vector2Int spawnPosition, bool isStarting = false)
    {
        RoomSO roomInstance = CreateRoom();
        roomInstance.gridPosition = spawnPosition;

        // Initialize roomCenter with a new GameObject and assign its position
        GameObject roomCenterObject = new GameObject("RoomCenter");
        Vector3 position = new Vector3(spawnPosition.x, spawnPosition.y, 0f);
        roomCenterObject.transform.position = position;
        roomInstance.roomCenter = roomCenterObject.transform;

        roomInstance.roomCenter.SetParent(roomsParent);
        roomInstance.isStartingRoom = isStarting;

        // Instantiate floor and wall tiles based on your logic

        return roomInstance;
    }

    private Vector2Int CalculateDoorPosition(Vector2Int roomPosition, Direction direction)
    {
        Vector2Int doorPosition = roomPosition + GetDirectionVector(direction);
        return doorPosition;
    }

    private Vector2Int CalculateConnectedDoorPosition(Vector2Int doorPosition, Direction direction)
    {
        Vector2Int connectedDoorPosition = doorPosition + GetDirectionVector(direction);
        return connectedDoorPosition;
    }

    private Vector2Int CalculateConnectedLocation(Vector2Int currentLocation, Vector2Int doorPosition)
    {
        int xOffset = doorPosition.x - currentLocation.x;
        int yOffset = doorPosition.y - currentLocation.y;

        return new Vector2Int(currentLocation.x + xOffset, currentLocation.y + yOffset);
    }

    private RoomSO FindRoomAtLocation(RoomSO[,] roomGrid, Vector2Int location, int maxX, int maxY)
    {
        int x = location.x;
        int y = location.y;

        if (x >= 0 && x <= maxX && y >= 0 && y <= maxY)
        {
            return roomGrid[x, y];
        }

        return null;
    }

    private Vector2Int GetDirectionVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.North: return new Vector2Int(0, 1);
            case Direction.East: return new Vector2Int(1, 0);
            case Direction.South: return new Vector2Int(0, -1);
            case Direction.West: return new Vector2Int(-1, 0);
            default: return Vector2Int.zero; // Handle default case appropriately
        }
    }

    private Direction GetOppositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.North: return Direction.South;
            case Direction.East: return Direction.West;
            case Direction.South: return Direction.North;
            case Direction.West: return Direction.East;
            default: return Direction.North; // Handle default case appropriately
        }
    }

    public RoomSO GetStartingRoom()
    {
        return generatedRooms.Count > 0 ? generatedRooms[0] : null;
    }

    public RoomSO GetRoomByIndex(int index)
    {
        return index >= 0 && index < generatedRooms.Count ? generatedRooms[index] : null;
    }

    public void SetCurrentRoom(int index)
    {
        // Implement changing the current room based on the index
        // Update player's position, camera, etc.
    }

    private RoomSO GenerateRoom(Vector2Int location)
    {
        // Generate room logic and instantiate tiles here
        RoomSO newRoom = CreateRoom();
        // Additional room generation and tile instantiation
        return newRoom;
    }

    private RoomSO CreateRoom()
    {
        RoomSO room = ScriptableObject.CreateInstance<RoomSO>();

        // Initialize room properties here

        room.roomCenter = new GameObject("RoomCenter").transform; // Initialize roomCenter

        return room;
    }
}