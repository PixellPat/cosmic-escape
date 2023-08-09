using System.Collections.Generic;
using UnityEngine;

public class RandomMapGenerator
{
    private int mapWidth;
    private int mapHeight;
    private int numRooms;
    //private int roomWidth = 11; // Width of a single room in units
    //private int roomHeight = 9; // Height of a single room in units

    private int startX = 20; // Starting X Position
    private int startY = 20; // Starting Y Position
    private int mainDirectionBias = 60; // Bias for the main path direction
    private int forkDirectionBias = 30; // Bias for the fork path direction

    private HashSet<Vector2Int> roomLocations;
    private HashSet<Vector2Int> forkRoomLocations;

    private enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }


    public RandomMapGenerator(int width, int height, int numRooms)
    {
        mapWidth = width; // Number of rooms horizontally
        mapHeight = height; // Number of rooms vertically
        this.numRooms = numRooms;        
    }

    public HashSet<Vector2Int> GenerateMap()
    {
        // Initialize the HashSets
        roomLocations = new HashSet<Vector2Int>();
        forkRoomLocations = new HashSet<Vector2Int>();

        // Add the starting room location
        roomLocations.Add(new Vector2Int(startX, startY));

        int currentX = startX;
        int currentY = startY;
        int straightCountX = 0;
        int straightCountY = 0;

        // Randomly select a starting preferred direction (0: Up, 1: Right, 2: Down, 3: Left)
        Direction preferredDirection = (Direction)Random.Range(0, 4);

        // Let the biased direction equal the preferred direction for the first step 
        Direction biasedDirection = preferredDirection;

        for (int i = 0; i < numRooms - 1; i++)
        {
            // Fail safe in case of infinite loops
            if (numRooms >= 100 || numRooms < 0)
            {
                Debug.Log("ERROR: numberOfSteps = " + numRooms + " ===============================");
                break;
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
            if (!roomLocations.Contains(new Vector2Int(nextX, nextY))) // && (biasedDirection == Direction.Up || biasedDirection == Direction.Right || biasedDirection == Direction.Down || biasedDirection == Direction.Left))
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

            }
            else
            {
                numRooms++;
            }

            // Add a bias towards the randomly chosen preferred direction
            biasedDirection = Random.Range(0, 100) < mainDirectionBias ? preferredDirection : (Direction)Random.Range(0, 4);
        }

        // Add generated room locations to the list
        
        return roomLocations;
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
            if (forkLength >= 30 || forkLength < 0)
            {
                Debug.Log("ERROR: forkLength = " + forkLength + " ===============================");
                break;
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


            if (!roomLocations.Contains(new Vector2Int(nextX, nextY))) // *TODO ???: !forkRoomPositions.Contains(position)
            {
                roomLocations.Add(new Vector2Int(nextX, nextY));
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
