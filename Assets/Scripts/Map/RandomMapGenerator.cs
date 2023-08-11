using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomMapGenerator
{
    private const int maxMainPathIterations = 100;
    private const int maxForkPathIterations = 30;

    private int mapWidth;
    private int mapHeight;
    private int numRooms;

    private int startX = 20; // Starting X Position
    private int startY = 20; // Starting Y Position
    private int mainDirectionBias = 60; // Bias for the main path direction
    private int forkDirectionBias = 30; // Bias for the fork path direction

    private List<Vector2Int> mainRoomLocations;
    private List<Vector2Int> forkRoomLocations;
    private List<RoomData> roomPathData;


    private enum Direction 
    {
        Up,
        Right,
        Down,
        Left
    }

    public struct RoomData
    {
        public Vector2Int Location;
        public bool IsLastRoom;

        public RoomData(Vector2Int location, bool isLastRoom)
        {
            Location = location;
            IsLastRoom = isLastRoom;
        }
    }


    public RandomMapGenerator(int width, int height, int numRooms)
    {
        mapWidth = width; // Number of rooms horizontally
        mapHeight = height; // Number of rooms vertically
        this.numRooms = numRooms;        
    }

    public List<RoomData> GenerateMap()
    {
        int currentX = startX;
        int currentY = startY;
        int straightCountX = 0;
        int straightCountY = 0;
        int originalNumRooms = numRooms;

        // Initialize the Lists
        mainRoomLocations = new List<Vector2Int>();
        forkRoomLocations = new List<Vector2Int>();
        roomPathData = new List<RoomData>();

        // Add the starting room locations
        mainRoomLocations.Add(new Vector2Int(startX, startY));
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
                mainRoomLocations.Clear(); // Clear mainRoomLocations list
                forkRoomLocations.Clear(); // Clear forkRoomLocations list
                roomPathData.Clear(); // Clear roomPathData list

                // Add the starting room locations
                mainRoomLocations.Add(new Vector2Int(startX, startY));
                roomPathData.Add(new RoomData(new Vector2Int(startX, startY), false));

                continue; // Restart the loop from the beginning
            }

            int nextX = currentX;
            int nextY = currentY;

            switch (biasedDirection)
            {
                case Direction.Up:
                    nextY = Mathf.Clamp(currentY + 1, 0, mapHeight - 1);
                    break;
                case Direction.Right:
                    nextX = Mathf.Clamp(currentX + 1, 0, mapWidth - 1);
                    break;
                case Direction.Down:
                    nextY = Mathf.Clamp(currentY - 1, 0, mapHeight - 1);
                    break;
                case Direction.Left:
                    nextX = Mathf.Clamp(currentX - 1, 0, mapWidth - 1);
                    break;
            }

            // Check if the next room has already been visited and if there are rooms directly up, right, down and left of the current room
            if (!mainRoomLocations.Contains(new Vector2Int(nextX, nextY))) 
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
                mainRoomLocations.Add(new Vector2Int(currentX, currentY));
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

        // Add the location of copy above and make the bool 'true' for being the last room in the main path
        roomPathData.Add(new RoomData(lastMainPathRoomData.Location, true));

        return roomPathData;
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
                case Direction.Up:
                    nextY = Mathf.Clamp(currentY + 1, 0, mapHeight - 1);
                    break;
                case Direction.Right:
                    nextX = Mathf.Clamp(currentX + 1, 0, mapWidth - 1);
                    break;
                case Direction.Down:
                    nextY = Mathf.Clamp(currentY - 1, 0, mapHeight - 1);
                    break;
                case Direction.Left:
                    nextX = Mathf.Clamp(currentX - 1, 0, mapWidth - 1);
                    break;
            }


            if (!mainRoomLocations.Contains(new Vector2Int(nextX, nextY))) // *TODO ???: !forkRoomPositions.Contains(position)
            {
                mainRoomLocations.Add(new Vector2Int(nextX, nextY));
                forkRoomLocations.Add(new Vector2Int(nextX, nextY));
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

        foreach (Vector2Int forkLocation in forkRoomLocations)
        {
            bool isLastRoom = forkLocation == forkRoomLocations.Last();
            roomPathData.Add(new RoomData(forkLocation, isLastRoom));
        }
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

        if (mainDirection == Direction.Up || mainDirection == Direction.Down)
        {
            return randomValue == 0 ? Direction.Left : Direction.Right;
        }
        else // mainDirection is Right or Left
        {
            return randomValue == 0 ? Direction.Up : Direction.Down;
        }
    }
}