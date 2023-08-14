using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyEnums;


public class RoomGenerator : MonoBehaviour
{
    private Transform roomsParent;
    private List<RoomSO> generatedRooms = new List<RoomSO>();
    
    [SerializeField] private RoomSO[] roomPrefabs;
    [SerializeField] private TilemapController tilemapController;

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
        // Generate room instances and establish connections between them
        GenerateAndStoreRooms(roomDataList);
        ConnectRooms();

        // After generating and storing rooms+
        foreach (RoomSO room in generatedRooms)
        {
            room.CalculateTileLocations(); // Calculate tile locations for each room
            tilemapController.PlaceTiles(room);
        }

        // Call the debugging function to log the generated room data
        DebugLogGeneratedRooms();

        // Additional logic for updating player position, camera, etc.
    }


    /// <summary>
    /// Generates room instances based on the provided list of room data locations.
    /// </summary>
    /// <param name="roomDataList">A list of room data containing location information.</param>
    private void GenerateAndStoreRooms(List<RandomMapGenerator.RoomData> roomDataList)
    {
        // Get the first room data to determine the starting room
         RandomMapGenerator.RoomData firstRoomData = roomDataList.First();

        // Generate room instances and store them in the grid
        foreach (RandomMapGenerator.RoomData roomData in roomDataList)
        {
            // Determine if the room is the starting room based on comparison with the first room data
            bool isStarting = roomData.Equals(firstRoomData);
            RoomSO roomInstance = InstantiateRoom(roomData.Location, isStarting, roomData.IsEndOfPathRoom);
            generatedRooms.Add(roomInstance);

            if (roomData.IsEndOfPathRoom)
            {
                Direction endRoomDoorDirection = roomData.PreviousRoomDirection;
                Vector2Int previousRoomLocation = roomData.PreviousRoomLocation;

                RoomSO previousRoom = FindRoomAtLocation(previousRoomLocation);
                if (previousRoom != null)
                {
                    Direction oppositeDirection = GetOppositeDirection(endRoomDoorDirection);

                    roomInstance.AddDoor(previousRoomLocation, endRoomDoorDirection); // Add the exit door information directly to the room
                    previousRoom.AddDoor(roomData.Location, oppositeDirection); // Add the entrance door information to the previous room
                }
            }
        }
    }


    /// <summary>
    /// Connects the generated rooms by determining adjacent rooms for each door
    /// of each room and establishing connections between them.
    /// </summary>
    private void ConnectRooms()
    {
        // Iterate through each room in the list of generated rooms
        foreach (RoomSO room in generatedRooms)
        {
            if (!room.isEndOfPathRoom)
            {
                foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
                {
                    if (direction != Direction.None) // Check if direction is not None
                    {
                        Vector2Int doorPosition = CalculateDoorPosition(room.gridPosition, direction);
                        RoomSO connectedRoom = FindRoomAtLocation(doorPosition);
                        if (connectedRoom != null)
                        {
                            ConnectRooms(room, connectedRoom, doorPosition, direction);
                        }
                    }
                }
            }
        }
    }


    private void ConnectRooms(RoomSO currentRoom, RoomSO nextRoom, Vector2Int doorPosition, Direction direction)
    {
        currentRoom.AddDoor(doorPosition, direction);

        //// Calculate the opposite door position in the next room
        //Vector2Int oppositeDoorPosition = CalculateConnectedDoorPosition(doorPosition, GetOppositeDirection(direction));

        // Connect the next room's door
        nextRoom.AddDoor(currentRoom.gridPosition, GetOppositeDirection(direction)); // Change oppositeDoorPosition to currentRoom.Location
    }

    private void DebugLogGeneratedRooms()
    {
        foreach (var room in generatedRooms)
        {
            Debug.Log("Room Location: " + room.gridPosition +
                      ", Is Last Room: " + room.isEndOfPathRoom +
                      ", Exit Door Direction: " + room.GetDoorDirection(room.doorPositions.FirstOrDefault()) +
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


    private RoomSO InstantiateRoom(Vector2Int mapPosition, bool isStarting = false, bool isEndRoom = false)
    {
        RoomSO roomInstance = ScriptableObject.CreateInstance<RoomSO>();


        //// Access the room width and height from the RoomSO
        //int roomWidth = roomInstance.roomWidth;
        //int roomHeight = roomInstance.roomHeight;

        //// Calculate the actual position based on room dimensions and map position
        //Vector2Int roomPosition = new Vector2Int(
        //    mapPosition.x * (roomWidth - 1), // Subtract 1 to account for the center pivot
        //    mapPosition.y * (roomHeight - 1)); // Subtract 1 to account for the center pivot
            
        roomInstance.Initialize(mapPosition, isStarting, isEndRoom);

        //GameObject roomCenterObject = new GameObject("Room");
        //roomCenterObject.transform.position = position;
        //roomCenterObject.transform.SetParent(roomsParent);

        //roomInstance.roomCenter = roomCenterObject.transform;

        //// Use ChooseRandomRoomPrefab() to get a random room prefab
        //RoomSO randomRoomPrefab = ChooseRandomRoomPrefab();
        //if (randomRoomPrefab != null)
        //{
        //    // Instantiate the randomly chosen room prefab and assign it to the roomPrefab property
        //    GameObject roomPrefabInstance = Instantiate(randomRoomPrefab.roomPrefab, roomCenterObject.transform);
        //    roomInstance.roomPrefab = roomPrefabInstance;
        //}

        return roomInstance;
    }

    private Vector2Int CalculateDoorPosition(Vector2Int roomPosition, Direction doorDirection)
    {
        Vector2Int doorPosition = roomPosition + GetDirectionVector(doorDirection);
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

    private RoomSO FindRoomAtLocation(Vector2Int location)
    {
        return generatedRooms.FirstOrDefault(room => room.gridPosition == location);
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