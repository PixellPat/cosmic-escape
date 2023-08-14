using System.Collections.Generic;
using UnityEngine;
using MyEnums;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu(fileName = "NewRoom", menuName = "Scriptable Objects/Game Data/Room")]
public class RoomSO : ScriptableObject
{
    public int roomWidth = 11;
    public int roomHeight = 9;

    public Vector2Int gridPosition;     // (X, Y) position
    public Vector2Int roomPosition;     // (X, Y) position
    public GameObject roomPrefab;       // Prefab for the room's visual representation
    public Transform roomCenter;        // Center position of the room

    // Store door positions and their corresponding directions
    public HashSet<Vector2Int> doorPositions = new HashSet<Vector2Int>();
    public Dictionary<Vector2Int, Direction> doorDictionary = new Dictionary<Vector2Int, Direction>();

    public bool isStartingRoom;
    public bool isEndOfPathRoom;

    [Header("Tilemap Locations")]
    public List<Vector2Int> floorTileLocations = new List<Vector2Int>();
    public List<Vector2Int> wallTileLocations = new List<Vector2Int>();
    public List<Vector2Int> doorTileLocations = new List<Vector2Int>();

    [Header("Puzzle Settings")]
    public bool hasPuzzle;              // Does this room have a puzzle?
    public PuzzleType puzzleType;       // Type of puzzle in this room

    // Additional properties for puzzle-specific settings
    [Header("Puzzle Details")]
    public int puzzleDifficulty;        // Difficulty level of the puzzle
    public Sprite puzzleImage;          // Image representing the puzzle
    public string puzzleDescription;    // Description of the puzzle


    public void Initialize(Vector2Int position, bool isStartRoom = false, bool isEndRoom = false)
    {
        gridPosition = position;
        isStartingRoom = isStartRoom;
        isEndOfPathRoom = isEndRoom;

        roomPosition = new Vector2Int(gridPosition.x * roomWidth, gridPosition.y * roomHeight);
    }

    public void CalculateTileLocations()
    {
        // Calculate door tile positions based on door positions and directions
        foreach (var doorPosition in doorPositions)
        {
            if (doorPosition != gridPosition)
            {
                Direction doorDirection = GetDoorDirection(doorPosition);
                Vector2Int doorOffset = GetDoorOffset(doorDirection);
                Vector2Int doorTilePosition = roomPosition + doorOffset;
                doorTileLocations.Add(doorTilePosition);
            }
        }

        // Calculate floor tile positions
        for (int x = 1; x < roomWidth - 1; x++)
        {
            for (int y = 1; y < roomHeight - 1; y++)
            {
                floorTileLocations.Add(new Vector2Int(x, y) + roomPosition);
            }
        }

        // Calculate wall tile positions
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                Vector2Int tilePos = new Vector2Int(x, y) + roomPosition;

                // Exclude positions with doors from wall tile positions
                if (!doorTileLocations.Contains(tilePos))
                {
                    // Check if the position is on the room's border
                    if (x == 0 || x == roomWidth - 1 || y == 0 || y == roomHeight - 1)
                    {
                        wallTileLocations.Add(tilePos);
                    }
                }
            }
        }
    }

    public Vector2Int GetDoorOffset(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return new Vector2Int(roomWidth / 2, roomHeight - 1);
            case Direction.East:
                return new Vector2Int(roomWidth - 1, roomHeight / 2);
            case Direction.South:
                return new Vector2Int(roomWidth / 2, 0);
            case Direction.West:
                return new Vector2Int(0, roomHeight / 2);
            default:
                return Vector2Int.zero; // Handle default case appropriately
        }
    }

    public Direction GetDoorDirection(Vector2Int doorPosition)
    {
        if (doorDictionary.TryGetValue(doorPosition, out Direction direction))
        {
            return direction;
        }
        return Direction.None; // Return a default direction if not found
    }

    //public void SetDoorDirection(Vector2Int doorPosition, Direction direction)
    //{
    //    if (doorDictionary.ContainsKey(doorPosition))
    //    {
    //        doorDictionary[doorPosition] = direction;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Door position not found in doorDictionary.");
    //    }
    //}

    
    public void AddDoor(Vector2Int doorPosition, Direction direction)
    {
        doorPositions.Add(doorPosition);
        doorDictionary[doorPosition] = direction;
    }


    public bool HasOneDoor()
    {
        if (doorPositions != null)
        {
            return doorPositions.Count == 1;
        }
        else
        {
            return false;
        }
    }

    // Example method to check if the room has a puzzle
    public bool HasPuzzle()
    {
        return hasPuzzle;
    }

    // Example method to get the type of puzzle in the room
    public PuzzleType GetPuzzleType()
    {
        return puzzleType;
    }

    // Example method to start the puzzle in the room
    public void StartPuzzle()
    {
        // Implement puzzle start logic here
    }

    // Example method to complete the puzzle in the room
    public void CompletePuzzle()
    {
        // Implement puzzle completion logic here
    }

    // Example method to handle interactions when entering the room
    public void OnPlayerEnter()
    {
        // Implement room entry logic here
    }

    // Example method to handle interactions when leaving the room
    public void OnPlayerExit()
    {
        // Implement room exit logic here
    }
}

public enum PuzzleType
{
    None,
    Spike,
    Colour,
    Memory,
    Riddle
    // Add more puzzle types as needed
}
