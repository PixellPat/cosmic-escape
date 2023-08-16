using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyEnums;

public class RandomMapGenerator : MonoBehaviour
{
    private int numRooms;
    private int maxMainPathIterations;
    private int maxForkPathIterations;
    private int minForkLength = 2;
    private int maxForkLength = 6;
    private int straightPathRooms = 3;

    private HashSet<Vector2Int> roomLocations;
    private HashSet<RoomData> roomPathData;
    private Vector2Int startLocation; 

    [SerializeField] private int mapWidth = 40;
    [SerializeField] private int mapHeight = 40;
    [SerializeField] private int mainDirectionBias = 60; 
    [SerializeField] private int forkDirectionBias = 30; 

    [SerializeField] RoomGenerator roomGenerator;


    private void Start()
    {
        Initialize();

        GenerateMap();

        roomGenerator.GenerateRooms(roomPathData);
    }

    private void Initialize()
    {
        startLocation = new Vector2Int(mapWidth / 2, mapHeight / 2);
        numRooms = (int)((mapWidth + mapHeight) / 2 * 0.75f);
        maxMainPathIterations = mapWidth + mapHeight;
        maxForkPathIterations = maxForkLength * minForkLength;
    }

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
            PreviousRoomLocation = new Vector2Int(0,0);
            PreviousRoomDirection = Direction.None; // Initialize to None initially
        }
    }


    public HashSet<RoomData> GenerateMap()
    {
        Vector2Int currentLocation = startLocation;
        int straightCountX = 0;
        int straightCountY = 0;
        int originalNumRooms = numRooms;

        roomLocations = new HashSet<Vector2Int>();
        roomPathData = new HashSet<RoomData>();

        roomLocations.Add(currentLocation);
        roomPathData.Add(new RoomData(currentLocation, false));

        // Randomly select a starting preferred direction (0: Up, 1: Right, 2: Down, 3: Left)
        Direction preferredDirection = (Direction)Random.Range(0, 4);

        // Let the biased direction equal the preferred direction for the first step 
        Direction biasedDirection = preferredDirection;


        for (int i = 0; i < numRooms - 1; i++)
        {
            // Fail safe in case of infinite loops
            if (numRooms >= maxMainPathIterations || numRooms < 0)
            {
                Debug.Log("ERROR: numRooms = " + numRooms + ", " + "(" + currentLocation.x + ", " + currentLocation.y + ")");
                numRooms = originalNumRooms; // Reset numRooms to its original value
                i = -1; // Reset i to -1, so it becomes 0 in the next iteration
                currentLocation = startLocation; // Reset current location to its original value
                straightCountX = 0; // Reset straightCountX to 0
                straightCountY = 0; // Reset straightCountY to 0
                roomLocations.Clear(); // Clear mainRoomLocations list
                roomPathData.Clear(); // Clear roomPathData list

                // Add the starting room locations
                roomLocations.Add(currentLocation);
                roomPathData.Add(new RoomData(currentLocation, false));

                continue; // Restart the loop from the beginning
            }

            Vector2Int nextLocation = currentLocation;

            switch (biasedDirection)
            {
                case Direction.North:
                    nextLocation.y = Mathf.Clamp(currentLocation.y + 1, 0, mapHeight - 1);
                    break;
                case Direction.East:
                    nextLocation.x = Mathf.Clamp(currentLocation.x + 1, 0, mapWidth - 1);
                    break;
                case Direction.South:
                    nextLocation.y = Mathf.Clamp(currentLocation.y - 1, 0, mapHeight - 1);
                    break;
                case Direction.West:
                    nextLocation.x = Mathf.Clamp(currentLocation.x - 1, 0, mapWidth - 1);
                    break;
            }

            // Check if the next room has already been visited and if there are rooms directly up, right, down and left of the current room
            if (!roomLocations.Contains(nextLocation)) 
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

                // If it's going in a straight line for at least 3 steps, create a fork
                if (straightCountX >= straightPathRooms || straightCountY >= straightPathRooms)
                {
                    CreateFork(currentLocation, preferredDirection);
                    straightCountX = 0;
                    straightCountY = 0; 
                }

                currentLocation = nextLocation;
                roomLocations.Add(currentLocation);
                roomPathData.Add(new RoomData(currentLocation, false));
            }
            else
            {
                numRooms++;
            }

            // Add a bias towards the randomly chosen preferred direction
            biasedDirection = GetBiasedDirection(preferredDirection, mainDirectionBias);
        }


        // Make a copy of the last mainRoomLocation added
        RoomData lastMainPathRoomData = roomPathData.Last();

        // Remove the last mainRoomLocation added because it has it's bool set to 'false' for being the last room in it's path
        roomPathData.Remove(roomPathData.Last());

        // Get the previous room location and work out the location of its door that will lead back to the room wence they came from
        Vector2Int previousRoomLocation = roomPathData.Last().Location;
        Direction oneDoorDirection = CalculateLastRoomDoorDirection(lastMainPathRoomData.Location, previousRoomLocation);

        // Add the location and previous location of copy above and make the bool 'true' for being the last room in the main path
        roomPathData.Add(new RoomData(lastMainPathRoomData.Location, previousRoomLocation, true, oneDoorDirection));

        return roomPathData;
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


    private void CreateFork(Vector2Int currentLoc, Direction mainDirection)
    {
        Vector2Int currentLocation = currentLoc;

        // Find a random perpendicular direction
        Direction perpendicularDirection = GetPerpendicularDirection(mainDirection);

        // Let the biased direction equal the perpendicular direction for the first step so the room doesn't position on the main path
        Direction biasedDirection = perpendicularDirection;

        int forkLength = Random.Range(minForkLength, maxForkLength);

        for (int i = 0; i < forkLength; i++)
        {
            // Fail safe in case of infinite loops
            if (forkLength >= maxForkPathIterations || forkLength < 0)
            {
                Debug.Log("ERROR: forkLength = " + forkLength + ", " + "(" + currentLocation + ")");

                forkLength = Random.Range(minForkLength, maxForkLength); // Reset forkLength 
                i = -1; // Reset i to -1, so it becomes 0 in the next iteration
                currentLocation = currentLoc; // Reset current location to its original value
                continue; // Restart the loop from the beginning
            }

            Vector2Int nextLocation = currentLocation;

            // Select coordinates based on the biased direction
            switch (biasedDirection)
            {
                case Direction.North:
                    nextLocation.y = Mathf.Clamp(currentLocation.y + 1, 0, mapHeight - 1);
                    break;
                case Direction.East:
                    nextLocation.x = Mathf.Clamp(currentLocation.x + 1, 0, mapWidth - 1);
                    break;
                case Direction.South:
                    nextLocation.y = Mathf.Clamp(currentLocation.y - 1, 0, mapHeight - 1);
                    break;
                case Direction.West:
                    nextLocation.x = Mathf.Clamp(currentLocation.x - 1, 0, mapWidth - 1);
                    break;
            }


            if (!roomLocations.Contains(nextLocation))
            {
                roomLocations.Add(nextLocation);
                roomPathData.Add(new RoomData(nextLocation, false));
                currentLocation.x = nextLocation.x;
                currentLocation.y = nextLocation.y;
            }
            else
            {
                // Reset the X and Y to the previous values
                forkLength++;
            }

            // Add a bias towards the randomly chosen perpendicular direction
            biasedDirection = GetBiasedDirection(perpendicularDirection, forkDirectionBias);
        }

        // Make a copy of the last forkRoomLocation added
        RoomData lastForkPathRoomData = roomPathData.Last();

        // Remove the last forkRoomLocation added because it has it's bool set to 'false' for being the last room in it's path
        roomPathData.Remove(roomPathData.Last());

        // Get the previous room location and work out the location of its door that will lead back to the room wence they came from
        Vector2Int previousRoomLocation = roomPathData.Last().Location;
        Direction oneDoorDirection = CalculateLastRoomDoorDirection(lastForkPathRoomData.Location, previousRoomLocation);

        // Add the location and previous location of copy above and make the bool 'true' for being the last room in the fork path
        roomPathData.Add(new RoomData(lastForkPathRoomData.Location, previousRoomLocation, true, oneDoorDirection));
    }

    private Direction GetBiasedDirection(Direction preferredDirection, int biasPercentage)
    {
        if (Random.value < biasPercentage / 100f) // Converted percentage to a float between 0.0 and 1.0
            return preferredDirection;

        return (Direction)Random.Range(0, System.Enum.GetValues(typeof(Direction)).Length);
    }

    private bool IsStraightPathX(Vector2Int currentLocation, Vector2Int nextLocation)
    {
        return currentLocation.x != nextLocation.x && currentLocation.y == nextLocation.y;
    }

    private bool IsStraightPathY(Vector2Int currentLocation, Vector2Int nextLocation)
    {
        return currentLocation.x == nextLocation.x && currentLocation.y != nextLocation.y;
    }

    private Direction GetPerpendicularDirection(Direction mainDirection)
    {
        int randomValue = Random.Range(0, 2); // Generate either 0 or 1

        if (mainDirection == Direction.North || mainDirection == Direction.South)
        {
            return randomValue == 0 ? Direction.West : Direction.East;
        }
        else // mainDirection is Right or Left
        {
            return randomValue == 0 ? Direction.North : Direction.South;
        }
    }
}