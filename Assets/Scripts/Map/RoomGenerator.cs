using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyEnums;


public class RoomGenerator : MonoBehaviour
{
    //public GameObject floorTilePrefab;
    //public GameObject wallTilePrefab;
    //public GameObject doorTilePrefab;

    public RoomSO[] roomPrefabs;
    private Transform roomsParent;

    private List<RoomSO> generatedRooms = new List<RoomSO>();

    [SerializeField] RoomSO startRoom;
    [SerializeField] RoomSO basicRoom;
    [SerializeField] RoomSO forkPathRoom;
    [SerializeField] RoomSO endRoom;

    private void Start()
    {
        roomsParent = new GameObject("Rooms").transform;
    }


    /// <summary>
    /// Generates and populates rooms based on the provided list of room data locations,
    /// connecting them and establishing door connections between adjacent rooms.
    /// </summary>
    /// <param name="roomDataList">A list of room data containing location information.</param>
    public void GenerateRooms(List<RandomMapGenerator.RoomData> roomDataList)
    {
        // Find the maximum X and Y coordinates among all room data
        int maxX = roomDataList.Max(roomData => roomData.Location.x);
        int maxY = roomDataList.Max(roomData => roomData.Location.y);

        // Generate room instances and establish connections between them
        RoomSO[,] generatedRoomGrid = GenerateAndStoreRooms(roomDataList);
        ConnectRooms(generatedRoomGrid, maxX, maxY);

        // Call the debugging function to log the generated room data
        DebugLogGeneratedRooms();

        // Additional logic for updating player position, camera, etc.
    }


    /// <summary>
    /// Generates room instances based on the provided list of room data locations,
    /// and stores them in a grid for reference during connection establishment.
    /// </summary>
    /// <param name="roomDataList">A list of room data containing location information.</param>
    /// <returns>A 2D grid of generated rooms with connections yet to be established.</returns>
    private RoomSO[,] GenerateAndStoreRooms(List<RandomMapGenerator.RoomData> roomDataList)
    {
        // Find the maximum X and Y coordinates among all room data
        int maxX = roomDataList.Max(roomData => roomData.Location.x);
        int maxY = roomDataList.Max(roomData => roomData.Location.y);

        // Initialize a 2D grid to store generated room instances
        RoomSO[,] generatedRoomGrid = new RoomSO[maxX + 1, maxY + 1];

        // Get the first room data to determine the starting room
        RandomMapGenerator.RoomData firstRoomData = roomDataList.First();

        // Initialize to None for the first room
        Direction previousRoomDirection = Direction.None;

        // Generate room instances and store them in the grid
        foreach (RandomMapGenerator.RoomData roomData in roomDataList)
        {
            // Determine if the room is the starting room based on comparison with the first room data
            bool isStarting = roomData.Equals(firstRoomData);

            // Instantiate the room instance and store it in the grid
            RoomSO roomInstance = InstantiateRoom(roomData.Location, isStarting);
            generatedRooms.Add(roomInstance);
            generatedRoomGrid[roomData.Location.x, roomData.Location.y] = roomInstance;

            if (roomData.IsLastRoom)
            {
                ConnectLastRoomWithOneDoor(roomInstance, previousRoomDirection);
                roomInstance.SetExitDoorDirection(previousRoomDirection); // Set the exit door direction for the last room
            }

            // Store the current room's door direction
            previousRoomDirection = roomInstance.DoorOutDirection;
        }

        // Return the grid of generated room instances for connection establishment
        return generatedRoomGrid;
    }

    private void ConnectLastRoomWithOneDoor(RoomSO room, Direction previousRoomDirection)
    {
        // Determine the door position based on the direction of the previous room's door
        Vector2Int doorPosition = CalculateDoorPosition(room.gridPosition, previousRoomDirection);

        // Add the door position to the door dictionary
        room.AddDoor(doorPosition, previousRoomDirection);

        // Set the exit door direction for the last room
        room.SetExitDoorDirection(previousRoomDirection);
    }

    /// <summary>
    /// Connects the generated rooms by determining adjacent rooms for each door
    /// of each room and establishing connections between them.
    /// </summary>
    /// <param name="generatedRoomGrid">A 2D grid of generated rooms.</param>
    /// <param name="maxX">The maximum X-coordinate value in the room grid.</param>
    /// <param name="maxY">The maximum Y-coordinate value in the room grid.</param>
    private void ConnectRooms(RoomSO[,] generatedRoomGrid, int maxX, int maxY)
    {
        // Iterate through each room in the list of generated rooms
        foreach (RoomSO room in generatedRooms)
        {
            // Iterate through each direction (North, East, South, West)
            foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
            {
                // Calculate the door position of the current direction for the current room
                Vector2Int doorPosition = CalculateDoorPosition(room.gridPosition, direction);

                // Find a room at the calculated door position in the generated room grid
                RoomSO connectedRoom = FindRoomAtLocation(generatedRoomGrid, doorPosition, maxX, maxY);

                // If a neighboring room is found at the door position
                if (connectedRoom != null)
                {
                    // Establish a connection between the doors of the current room and the connected room
                    ConnectRooms(room, connectedRoom, doorPosition, direction);

                    // Set the direction of the connecting door in the connected room
                    connectedRoom.SetDoorDirection(doorPosition, GetOppositeDirection(direction));
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
            Debug.Log("Room Location: " + room.gridPosition +
                      ", Is Last Room: " + room.HasOneDoor() +
                      ", Exit Door Direction: " + room.GetDoorDirection(room.doorPositions[0]) +
                      "\n");

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

    //private Vector3 CalculateSpawnPosition(Vector2Int location)
    //{
    //    float x = location.x * roomWidth;
    //    float y = location.y * roomHeight;
    //    return new Vector3(x, y, 0f);
    //}

    private RoomSO InstantiateRoom(Vector2Int spawnPosition, bool isStarting = false)
    {
        RoomSO roomInstance = ScriptableObject.CreateInstance<RoomSO>();

        roomInstance.Initialize(spawnPosition, isStarting);

        // Access the room width and height from the RoomSO
        float roomWidth = roomInstance.roomWidth;
        float roomHeight = roomInstance.roomHeight;

        // Calculate the actual position based on room dimensions and spawn position
        Vector3 position = new Vector3(spawnPosition.x * roomWidth, spawnPosition.y * roomHeight, 0f);

        GameObject roomCenterObject = new GameObject("Room");
        roomCenterObject.transform.position = position;
        roomCenterObject.transform.SetParent(roomsParent);

        roomInstance.roomCenter = roomCenterObject.transform;

        // Instantiate the "basicRoom" prefab and assign it to the roomPrefab property
        GameObject roomPrefabInstance = Instantiate(basicRoom.roomPrefab, roomCenterObject.transform);
        roomInstance.roomPrefab = roomPrefabInstance;

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
            case Direction.North: 
                return new Vector2Int(0, 1);
            case Direction.East: 
                return new Vector2Int(1, 0);
            case Direction.South: 
                return new Vector2Int(0, -1);
            case Direction.West: 
                return new Vector2Int(-1, 0);
            default: 
                return Vector2Int.zero; // Handle default case appropriately
        }
    }

    private Direction GetOppositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return Direction.South;
            case Direction.East:
                return Direction.West;
            case Direction.South:
                return Direction.North;
            case Direction.West:
                return Direction.East;
            default:
                return Direction.None; // Handle default case appropriately
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
}