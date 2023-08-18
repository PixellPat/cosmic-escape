using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyEnums;

public class RandomMapGenerator : MonoBehaviour
{
    private const int MAX_RETRIES_MAIN_PATH = 10;
    private const int MAX_RETRIES_FORK_PATH = 5;
    private int numRooms = 25;
    private int minForkLength = 2;
    private int maxForkLength = 6;
    private int straightPathRooms = 3;
    private int straightCountX = 0;
    private int straightCountY = 0;

    private List<Vector2Int> roomLocations;
    private List<RoomData> roomPathData;
    private Vector2Int startLocation;
    private DefaultMapLayoutSO selectedFailsafe;

    [SerializeField] private int mapWidth = 40;
    [SerializeField] private int mapHeight = 40;
    [SerializeField] private int mainDirectionBias = 60;
    [SerializeField] private int forkDirectionBias = 30;
    [SerializeField] private RoomGenerator roomGenerator;
    [SerializeField] private List<DefaultMapLayoutSO> failSafeMaps;


    private void Start()
    {
        Initialize();
        List<RoomData> roomData = GenerateMap();
        roomGenerator.GenerateRooms(roomData);
    }

    private void Initialize()
    {
        selectedFailsafe = GetRandomFailsafeMap();
        startLocation = new Vector2Int(mapWidth / 2, mapHeight / 2);
    }

    [System.Serializable]
    public struct RoomData
    {
        public Vector2Int Location;
        public bool IsEndOfPathRoom;
        public Vector2Int PreviousRoomLocation;
        public Direction PreviousRoomDirection;

        public RoomData(Vector2Int location, Vector2Int previousRoomLocation, bool isEndOfPathRoom, Direction previousRoomDirection)
        {
            Location = location;
            IsEndOfPathRoom = isEndOfPathRoom;
            PreviousRoomLocation = previousRoomLocation;
            PreviousRoomDirection = previousRoomDirection;
        }

        public RoomData(Vector2Int location, bool isEndOfPathRoom)
        {
            Location = location;
            IsEndOfPathRoom = isEndOfPathRoom;
            PreviousRoomLocation = new Vector2Int(0, 0);
            PreviousRoomDirection = Direction.None; // Initialize to None initially
        }
    }


    private List<RoomData> GenerateMap()
    {
        Vector2Int currentLocation = startLocation;

        roomLocations = new List<Vector2Int>();
        roomPathData = new List<RoomData>();

        roomLocations.Add(currentLocation);
        roomPathData.Add(new RoomData(currentLocation, false));

        // Randomly select a starting preferred direction (0: Up, 1: Right, 2: Down, 3: Left)
        Direction preferredDirection = (Direction)Random.Range(0, 4);

        // Let the biased direction equal the preferred direction for the first step 
        Direction biasedDirection = preferredDirection;


        int retries = 0;

        for (int i = 0; i < numRooms - 1; i++)
        {
            Vector2Int nextLocation = GetNextLocation(currentLocation, biasedDirection);

            // Check if the next room has already been visited and if there are rooms directly up, right, down and left of the current room
            if (IsRoomLocationAvailable(nextLocation))
            {
                retries = 0;

                ProcessPathDirection(currentLocation, nextLocation);

                // If it's going in a straight line for at least 3 steps, create a fork
                if (straightCountX >= straightPathRooms || straightCountY >= straightPathRooms)
                {
                    bool isForkGenSuccessful = CreateFork(currentLocation, preferredDirection);
                    if (!isForkGenSuccessful)
                        return selectedFailsafe.roomPathData;

                    straightCountX = 0;
                    straightCountY = 0;
                }

                currentLocation = nextLocation;
                AddRoomToMap(currentLocation);
            }
            else // room location exists 
            {
                retries++;
                numRooms++;

                if (retries >= MAX_RETRIES_MAIN_PATH)  // Check if maximum retries reached
                {
                    Direction randomDirection = ChooseDirection(currentLocation);

                    if (randomDirection != Direction.None)
                    {
                        nextLocation = GetNextLocation(currentLocation, randomDirection);

                        if (IsRoomLocationAvailable(nextLocation))
                        {
                            retries = 0; // Reset retries

                            ProcessPathDirection(currentLocation, nextLocation);

                            // If it's going in a straight line for at least 3 steps, create a fork
                            if (straightCountX >= straightPathRooms || straightCountY >= straightPathRooms)
                            {
                                if (!CreateFork(currentLocation, preferredDirection))
                                {
                                    return selectedFailsafe.roomPathData;
                                }

                                straightCountX = 0;
                                straightCountY = 0;
                            }

                            currentLocation = nextLocation;
                            AddRoomToMap(currentLocation);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Max retries reached, returning failsafe map.");
                        Debug.LogWarning("Random map generation failed! Using failsafe map: " + selectedFailsafe.name);
                        Debug.LogWarning("ERROR: numRooms = " + numRooms + ", " + "(" + currentLocation.x + ", " + currentLocation.y + ")");

                        return selectedFailsafe.roomPathData;
                    }
                }
            }

            // Add a bias towards the randomly chosen preferred direction
            biasedDirection = GetBiasedDirection(preferredDirection, mainDirectionBias);
        }

        UpdateLastRoomData();

        return roomPathData;
    }

    private bool CreateFork(Vector2Int currentLoc, Direction mainDirection)
    {
        int retries = 0;

        Vector2Int currentLocation = currentLoc;

        // Find a random perpendicular direction
        Direction perpendicularDirection = GetPerpendicularDirection(mainDirection);

        // Let the biased direction equal the perpendicular direction for the first step so the room doesn't position on the main path
        Direction biasedDirection = perpendicularDirection;

        int forkLength = Random.Range(minForkLength, maxForkLength);

        for (int i = 0; i < forkLength; i++)
        {
            Vector2Int nextLocation = GetNextLocation(currentLocation, biasedDirection);

            if (IsRoomLocationAvailable(nextLocation))
            {
                retries = 0;

                AddRoomToMap(nextLocation);
                currentLocation.x = nextLocation.x;
                currentLocation.y = nextLocation.y;
            }
            else
            {
                retries++;
                forkLength++;

                if (retries >= MAX_RETRIES_FORK_PATH)  // Check if maximum retries reached for forks
                {
                    Direction randomDirection = ChooseDirection(currentLocation);

                    if (randomDirection != Direction.None)
                    {
                        nextLocation = GetNextLocation(currentLocation, randomDirection);

                        if (IsRoomLocationAvailable(nextLocation))
                        {
                            retries = 0;

                            AddRoomToMap(nextLocation);
                            currentLocation = nextLocation;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Fork generation max retries reached, returning failsafe map.");
                        Debug.LogWarning("Fork generation failed! Using failsafe map: " + selectedFailsafe.name);
                        Debug.LogWarning("ERROR: forkLength = " + forkLength + ", " + "(" + currentLocation + ")");

                        return false;
                    }

                }
            }

            // Add a bias towards the randomly chosen perpendicular direction
            biasedDirection = GetBiasedDirection(perpendicularDirection, forkDirectionBias);
        }

        UpdateLastRoomData();

        return true;
    }

    private void UpdateLastRoomData()
    {
        // Make a copy of the last room location added
        RoomData lastPathRoomData = roomPathData.Last();

        // Remove the last room location added because its bool is set to 'false' for being the last room in its path
        roomPathData.Remove(lastPathRoomData);

        // Get the previous room location and determine the location of its door that will lead back to the previous room
        Vector2Int previousRoomLocation = roomPathData.Last().Location;
        Direction oneDoorDirection = CalculateLastRoomDoorDirection(lastPathRoomData.Location, previousRoomLocation);

        // Add the location and previous location of the copy above and set the bool to 'true' for being the last room in the path
        roomPathData.Add(new RoomData(lastPathRoomData.Location, previousRoomLocation, true, oneDoorDirection));
    }

    private void ProcessPathDirection(Vector2Int currentLocation, Vector2Int nextLocation)
    {
        Direction pathDirection = GetPathDirection(currentLocation, nextLocation);
        switch (pathDirection)
        {
            case Direction.East:
            case Direction.West:
                straightCountX++;
                straightCountY = 0; // Reset the Y counter
                break;

            case Direction.North:
            case Direction.South:
                straightCountY++;
                straightCountX = 0; // Reset the X counter
                break;

            case Direction.None:
                // Handle error or exception here if you expect this should never happen
                break;
        }
    }

    private void AddRoomToMap(Vector2Int location)
    {
        roomLocations.Add(location);
        roomPathData.Add(new RoomData(location, false));
    }

    private Direction ChooseDirection(Vector2Int currentLocation)
    {
        List<Direction> availableDirections = GetAvailableDirections(currentLocation);

        if (availableDirections.Any())
            return availableDirections[Random.Range(0, availableDirections.Count)];
        else
            return Direction.None;
    }

    private List<Direction> GetAvailableDirections(Vector2Int location)
    {
        List<Direction> directions = new List<Direction>();

        foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
        {
            Vector2Int nextLocation = GetNextLocation(location, dir);
            if (nextLocation != location && IsRoomLocationAvailable(nextLocation))
            {
                directions.Add(dir);
            }
        }

        return directions;
    }

    private bool IsRoomLocationAvailable(Vector2Int location)
    {
        return !roomLocations.Contains(location);
    }

    private Vector2Int GetNextLocation(Vector2Int currentLocation, Direction direction)
    {
        Vector2Int nextLocation = currentLocation;

        switch (direction)
        {
            case Direction.North:
                nextLocation.y++;
                break;
            case Direction.East:
                nextLocation.x++;
                break;
            case Direction.South:
                nextLocation.y--;
                break;
            case Direction.West:
                nextLocation.x--;
                break;
        }

        // Ensure the rooms don't go beyond the map bounds
        if (nextLocation.x < 0 || nextLocation.x >= mapWidth || nextLocation.y < 0 || nextLocation.y >= mapHeight)
            return currentLocation;

        return nextLocation;
    }

    private DefaultMapLayoutSO GetRandomFailsafeMap()
    {
        int randomIndex = Random.Range(0, failSafeMaps.Count);
        return failSafeMaps[randomIndex];
    }

    private Direction GetPathDirection(Vector2Int currentLocation, Vector2Int nextLocation)
    {
        if (currentLocation.x != nextLocation.x && currentLocation.y == nextLocation.y)
        {
            return (currentLocation.x < nextLocation.x) ? Direction.East : Direction.West;
        }
        else if (currentLocation.x == nextLocation.x && currentLocation.y != nextLocation.y)
        {
            return (currentLocation.y < nextLocation.y) ? Direction.North : Direction.South;
        }

        Debug.LogWarning(Direction.None.ToString() + "No direction");
        return Direction.None; // No direction or diagonal (shouldn't occur in your current setup)
    }

    private Direction CalculateLastRoomDoorDirection(Vector2Int currentRoomLocation, Vector2Int previousRoomLocation)
    {
        // Calculate the difference between the current and previous room locations
        Vector2Int difference = currentRoomLocation - previousRoomLocation;

        // Determine the one-door direction based on the difference
        if (difference == Vector2Int.up)
        {
            return Direction.South;
        }
        else if (difference == Vector2Int.right)
        {
            return Direction.West;
        }
        else if (difference == Vector2Int.down)
        {
            return Direction.North;
        }
        else if (difference == Vector2Int.left)
        {
            return Direction.East;
        }
        else
        {
            // Handle the case where the rooms are not adjacent
            return Direction.None;
        }
    }

    private Direction GetBiasedDirection(Direction preferredDirection, int biasPercentage)
    {
        if (Random.value < biasPercentage / 100f) // Converted percentage to a float between 0.0 and 1.0
            return preferredDirection;

        return (Direction)Random.Range(0, System.Enum.GetValues(typeof(Direction)).Length);
    }

    private Direction GetPerpendicularDirection(Direction mainDirection)
    {
        int randomValue = Random.Range(0, 2); // Generate either 0 or 1

        if (mainDirection == Direction.North || mainDirection == Direction.South)
        {
            return randomValue == 0 ? Direction.West : Direction.East;
        }
        else // The mainDirection will Right or Left
        {
            return randomValue == 0 ? Direction.North : Direction.South;
        }
    }
}



//// Use this Class when you need to populate DefaultMapLayoutSO for the failSafeMaps list.
//defaultMapAssetCreator.CreateDefaultMapLayout(roomPathData);