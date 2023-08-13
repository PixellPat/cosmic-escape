using System.Collections.Generic;
using UnityEngine;
using MyEnums;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewRoom", menuName = "Scriptable Objects/Game Data/Room")]
public class RoomSO : ScriptableObject
{
    public int roomWidth = 11;
    public int roomHeight = 9;

    public Vector2Int gridPosition;     // (X, Y) position
    public GameObject roomPrefab;       // Prefab for the room's visual representation
    public Transform roomCenter;        // Center position of the room

    // Store door positions and their corresponding directions
    public List<Vector2Int> doorPositions = new List<Vector2Int>();
    public Dictionary<Vector2Int, Direction> doorDictionary = new Dictionary<Vector2Int, Direction>();

    public bool isStartingRoom;

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

    public Direction DoorOutDirection { get; private set; }


    public void Initialize(Vector2Int position, bool isStarting = false)
    {
        gridPosition = position;
        isStartingRoom = isStarting;
    }

    public void CalculateTileLocations()
    {
        floorTileLocations = new List<Vector2Int>();
        wallTileLocations = new List<Vector2Int>();
        doorTileLocations = new List<Vector2Int>();

        // Calculate wall tile positions while avoiding doors
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                Vector2Int tilePos = new Vector2Int(x, y);

                if (!doorTileLocations.Contains(tilePos))
                {
                    wallTileLocations.Add(tilePos);

                    // Calculate the door positions based on the wall's position
                    // and add them to doorTileLocations
                    if (y == 0 && x != 0 && x != roomWidth - 1) // Top wall
                    {
                        doorTileLocations.Add(new Vector2Int(tilePos.x, tilePos.y + 1));
                    }
                    else if (y == roomHeight - 1 && x != 0 && x != roomWidth - 1) // Bottom wall
                    {
                        doorTileLocations.Add(new Vector2Int(tilePos.x, tilePos.y - 1));
                    }
                    else if (x == 0 && y != 0 && y != roomHeight - 1) // Left wall
                    {
                        doorTileLocations.Add(new Vector2Int(tilePos.x + 1, tilePos.y));
                    }
                    else if (x == roomWidth - 1 && y != 0 && y != roomHeight - 1) // Right wall
                    {
                        doorTileLocations.Add(new Vector2Int(tilePos.x - 1, tilePos.y));
                    }
                }
            }
        }

        // Calculate floor tile positions
        for (int x = 1; x < roomWidth - 1; x++)
        {
            for (int y = 1; y < roomHeight - 1; y++)
            {
                floorTileLocations.Add(new Vector2Int(x, y));
            }
        }
    }

    public Vector2Int GetDirectionVector(Direction direction)
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

    public Direction GetOppositeDirection(Direction direction)
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

    public Direction GetDoorDirection(Vector2Int doorPosition)
    {
        if (doorDictionary.TryGetValue(doorPosition, out Direction direction))
        {
            return direction;
        }
        return Direction.North; // Return a default direction if not found
    }

    public void SetDoorDirection(Vector2Int doorPosition, Direction direction)
    {
        if (doorDictionary.ContainsKey(doorPosition))
        {
            doorDictionary[doorPosition] = direction;
        }
        else
        {
            Debug.LogWarning("Door position not found in doorDictionary.");
        }
    }

    // Set the exit door direction based on the previous room direction
    public void SetExitDoorDirection(Direction previousRoomDirection)
    {
        if (doorPositions.Count > 0)
        {
            Vector2Int exitDoorPosition = doorPositions[doorPositions.Count - 1]; // Use the same door position
            doorDictionary[exitDoorPosition] = previousRoomDirection; // Set the exit door direction based on the previous room's direction
        }
        else
        {
            Debug.LogWarning("No exit door position available.");
        }
    }


    public void AddDoor(Vector2Int doorPosition, Direction direction)
    {
        doorPositions.Add(doorPosition);
        doorDictionary[doorPosition] = direction;
    }

    public bool HasDoor(Vector2Int doorPosition)
    {
        return doorDictionary.ContainsKey(doorPosition);
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
