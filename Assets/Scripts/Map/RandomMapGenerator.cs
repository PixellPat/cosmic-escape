using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyEnums;

public class RandomMapGenerator : MonoBehaviour
{
    private const int maxMainPathIterations = 100;
    private const int maxForkPathIterations = 30;

    private List<Vector2Int> roomLocations;
    private List<RoomData> roomPathData;
    private RoomData previousRoomData;

    [SerializeField] private int mapWidth = 40;
    [SerializeField] private int mapHeight = 40;
    [SerializeField] private int numRooms = 30;

    [SerializeField] private int startX = 20; // Starting X Position
    [SerializeField] private int startY = 20; // Starting Y Position
    [SerializeField] private int mainDirectionBias = 60; // Bias for the main path direction
    [SerializeField] private int forkDirectionBias = 30; // Bias for the fork path direction

    [SerializeField] RoomGenerator roomGenerator;


    public struct RoomData
    {
        public Vector2Int Location;
        public bool IsEndOfPathRoom;
        public Vector2Int PreviousRoomLocation; // Store the previous room's location
        public Direction PreviousRoomDirection; // Store the calculated one-door direction

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


    private void Start()
    {
        GenerateMap();

        // Once the map is generated, trigger the RoomGenerator
        roomGenerator.GenerateRooms(roomPathData);
    }

    public List<RoomData> GenerateMap()
    {
        int currentX = startX;
        int currentY = startY;
        int straightCountX = 0;
        int straightCountY = 0;
        int originalNumRooms = numRooms;

        // Initialize the Lists
        roomLocations = new List<Vector2Int>();
        roomPathData = new List<RoomData>();

        // Add the starting room locations
        roomLocations.Add(new Vector2Int(startX, startY));
        roomPathData.Add(new RoomData(new Vector2Int(startX, startY), false));

        // Randomly select a starting preferred direction (0: Up, 1: Right, 2: Down, 3: Left)
        Direction preferredDirection = (Direction)Random.Range(0, 4);

        // Let the biased direction equal the preferred direction for the first step 
        Direction biasedDirection = preferredDirection;


        for (int i = 0; i < numRooms - 1; i++)
        {
            // Fail safe in case of infinite loops
            if (numRooms >= maxMainPathIterations || numRooms < 0)
            {
                Debug.Log("ERROR: numRooms = " + numRooms + " ===============================");

                numRooms = originalNumRooms; // Reset numRooms to its original value
                i = -1; // Reset i to -1, so it becomes 0 in the next iteration
                currentX = startX; // Reset currentX to its original value
                currentY = startY; // Reset currentY to its original value
                straightCountX = 0; // Reset straightCountX to 0
                straightCountY = 0; // Reset straightCountY to 0
                roomLocations.Clear(); // Clear mainRoomLocations list
                roomPathData.Clear(); // Clear roomPathData list

                // Add the starting room locations
                roomLocations.Add(new Vector2Int(startX, startY));
                roomPathData.Add(new RoomData(new Vector2Int(startX, startY), false));

                continue; // Restart the loop from the beginning
            }

            int nextX = currentX;
            int nextY = currentY;

            switch (biasedDirection)
            {
                case Direction.North:
                    nextY = Mathf.Clamp(currentY + 1, 0, mapHeight - 1);
                    break;
                case Direction.East:
                    nextX = Mathf.Clamp(currentX + 1, 0, mapWidth - 1);
                    break;
                case Direction.South:
                    nextY = Mathf.Clamp(currentY - 1, 0, mapHeight - 1);
                    break;
                case Direction.West:
                    nextX = Mathf.Clamp(currentX - 1, 0, mapWidth - 1);
                    break;
            }

            // Check if the next room has already been visited and if there are rooms directly up, right, down and left of the current room
            if (!roomLocations.Contains(new Vector2Int(nextX, nextY))) 
            {
                if (IsStraightPathX(currentX, currentY, nextX, nextY) && straightCountY == 0) // Right of Left    *TODO: straightCountY <= 1 
                {
                    // Increase straight count of X if it's going in a straight line
                    straightCountX++;
                }
                else
                {
                    // If it's not going in a straight line, reset the straight count for X
                    straightCountX = 0;
                }

                if (IsStraightPathY(currentX, currentY, nextX, nextY) && straightCountX == 0) // Up or Down       *TODO: straightCountY <= 1
                {
                    // Increase straight count of Y if it's going in a straight line
                    straightCountY++;
                }
                else
                {
                    // If it's not going in a straight line, reset the straight count for Y
                    straightCountY = 0;
                }


                // If it's going in a straight line for at least 3 steps, create a fork
                if (straightCountX >= 3 || straightCountY >= 3)
                {
                    CreateFork(currentX, currentY, preferredDirection);
                    straightCountX = 0; // Reset straight count after creating a fork
                    straightCountY = 0; // Reset straight count after creating a fork
                }

                currentX = nextX;
                currentY = nextY;
                roomLocations.Add(new Vector2Int(currentX, currentY));
                roomPathData.Add(new RoomData(new Vector2Int(currentX, currentY), false));
            }
            else
            {
                numRooms++;
            }

            // Add a bias towards the randomly chosen preferred direction
            biasedDirection = Random.Range(0, 100) < mainDirectionBias ? preferredDirection : (Direction)Random.Range(0, 4);
        }


        // Make a copy of the last mainRoomLocation added
        RoomData lastMainPathRoomData = roomPathData.Last();

        // Remove the last mainRoomLocation added because it has it's bool set to 'false' for being the last room in it's path
        roomPathData.RemoveAt(roomPathData.Count - 1);

        // Get the previous room location and work out the location of its door that will lead back to the room wence they came from
        Vector2Int previousRoomLocation = roomPathData.Last().Location;
        Direction oneDoorDirection = CalculateLastRoomDoorDirection(lastMainPathRoomData.Location, previousRoomLocation);

        // Add the location and previous location of copy above and make the bool 'true' for being the last room in the main path
        roomPathData.Add(new RoomData(lastMainPathRoomData.Location, previousRoomLocation, true, oneDoorDirection));

        return roomPathData;
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


    private void CreateFork(int x, int y, Direction mainDirection)
    {
        int currentX = x;
        int currentY = y;

        // Find a random perpendicular direction
        Direction perpendicularDirection = GetPerpendicularDirection(mainDirection);

        // Let the biased direction equal the perpendicular direction for the first step so the room doesn't position on the main path
        Direction biasedDirection = perpendicularDirection;

        // Choose the length of the fork path
        int forkLength = Random.Range(2, 6);

        for (int i = 0; i < forkLength; i++)
        {
            // Fail safe in case of infinite loops
            if (forkLength >= maxForkPathIterations || forkLength < 0)
            {
                Debug.Log("ERROR: forkLength = " + forkLength + " ===============================");

                forkLength = Random.Range(2, 6); // Reset forkLength 
                i = -1; // Reset i to -1, so it becomes 0 in the next iteration
                currentX = x; // Reset currentX to its original value
                currentY = y; // Reset currentY to its original value
                continue; // Restart the loop from the beginning
            }

            int nextX = currentX;
            int nextY = currentY;

            // Select coordinates based on the biased direction
            switch (biasedDirection)
            {
                case Direction.North:
                    nextY = Mathf.Clamp(currentY + 1, 0, mapHeight - 1);
                    break;
                case Direction.East:
                    nextX = Mathf.Clamp(currentX + 1, 0, mapWidth - 1);
                    break;
                case Direction.South:
                    nextY = Mathf.Clamp(currentY - 1, 0, mapHeight - 1);
                    break;
                case Direction.West:
                    nextX = Mathf.Clamp(currentX - 1, 0, mapWidth - 1);
                    break;
            }


            if (!roomLocations.Contains(new Vector2Int(nextX, nextY))) // *TODO ???: !forkRoomPositions.Contains(position)
            {
                roomLocations.Add(new Vector2Int(nextX, nextY));
                roomPathData.Add(new RoomData(new Vector2Int(nextX, nextY), false));
                currentX = nextX;
                currentY = nextY;
            }
            else
            {
                // Reset the X and Y to the previous values
                forkLength++;
            }

            // Add a bias towards the randomly chosen perpendicular direction
            biasedDirection = Random.Range(0, 100) < forkDirectionBias ? perpendicularDirection : (Direction)Random.Range(0, 4);
        }

        // Make a copy of the last forkRoomLocation added
        RoomData lastForkPathRoomData = roomPathData.Last();

        // Remove the last forkRoomLocation added because it has it's bool set to 'false' for being the last room in it's path
        roomPathData.RemoveAt(roomPathData.Count - 1);

        // Get the previous room location and work out the location of its door that will lead back to the room wence they came from
        Vector2Int previousRoomLocation = roomPathData.Last().Location;
        Direction oneDoorDirection = CalculateLastRoomDoorDirection(lastForkPathRoomData.Location, previousRoomLocation);

        // Add the location and previous location of copy above and make the bool 'true' for being the last room in the fork path
        roomPathData.Add(new RoomData(lastForkPathRoomData.Location, previousRoomLocation, true, oneDoorDirection));
    }


    private bool IsStraightPathX(int x1, int y1, int x2, int y2)
    {
        return (x1 != x2 && y1 == y2);
    }

    private bool IsStraightPathY(int x1, int y1, int x2, int y2)
    {
        return (x1 == x2 && y1 != y2);
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