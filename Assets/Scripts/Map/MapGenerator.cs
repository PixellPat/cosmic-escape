using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject roomPrefab;
    public GameObject forkRoomPrefab;
    public GameObject startRoomPrefab;
    public GameObject finishRoomPrefab;

    public int mainDirectionBias = 60; // Bias for the main path direction
    public int forkDirectionBias = 30; // Bias for the fork path direction
    public int numberOfSteps = 50; // Number of steps
    public int startX = 20; // Starting X Position
    public int startY = 20; // Starting Y Position
    public int mapWidth = 50; // Number of rooms horizontally
    public int mapHeight = 50; // Number of rooms vertically
    public float roomWidth = 11f; // Width of a single room in units
    public float roomHeight = 9f; // Height of a single room in units

    private HashSet<Vector2Int> visitedRooms;
    private HashSet<Vector2Int> forkRoomPositions;

    private enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    private class Room
    {
        public int x;
        public int y;
        public GameObject roomPrefab;

        public Room(int x, int y, GameObject roomPrefab)
        {
            this.x = x;
            this.y = y;
            this.roomPrefab = roomPrefab;
        }
    }

    private void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        // Initialize the HashSets
        visitedRooms = new HashSet<Vector2Int>();
        forkRoomPositions = new HashSet<Vector2Int>();

        // Add the starting room location
        visitedRooms.Add(new Vector2Int(startX, startY));

        int currentX = startX;
        int currentY = startY;
        int straightCountX = 0;
        int straightCountY = 0;

        // Randomly select a starting preferred direction (0: Up, 1: Right, 2: Down, 3: Left)
        Direction preferredDirection = (Direction)Random.Range(0, 4);

        // Let the biased direction equal the preferred direction for the first step 
        Direction biasedDirection = preferredDirection;

        for (int i = 0; i < numberOfSteps - 1; i++)
        {
            // Fail safe in case of infinite loops
            if (numberOfSteps >= 100 || numberOfSteps < 0)
            {
                Debug.LogWarning("ERROR: numberOfSteps = " + numberOfSteps + " ===============================");
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
            if (!visitedRooms.Contains(new Vector2Int(nextX, nextY))) // && (biasedDirection == Direction.Up || biasedDirection == Direction.Right || biasedDirection == Direction.Down || biasedDirection == Direction.Left))
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
                visitedRooms.Add(new Vector2Int(currentX, currentY));

            }
            else
            {
                numberOfSteps++;
            }

            // Add a bias towards the randomly chosen preferred direction
            biasedDirection = Random.Range(0, 100) < mainDirectionBias ? preferredDirection : (Direction)Random.Range(0, 4);
        }

        DrawMap();
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
                Debug.LogWarning("ERROR: forkLength = " + forkLength + " ===============================");
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


            if (!visitedRooms.Contains(new Vector2Int(nextX, nextY))) // *TODO ???: !forkRoomPositions.Contains(position)
            {
                visitedRooms.Add(new Vector2Int(nextX, nextY));
                forkRoomPositions.Add(new Vector2Int(nextX, nextY));
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

    private void DrawMap()
    {
        StringBuilder logBuilder = new StringBuilder();

        foreach (Vector2Int position in visitedRooms)
        {
            GameObject roomPrefabToInstantiate;

            if (position == visitedRooms.First()) // Check if it's the first room
            {
                roomPrefabToInstantiate = startRoomPrefab;
            }
            else if (position == visitedRooms.Last()) // Check if it's the last room
            {
                roomPrefabToInstantiate = finishRoomPrefab;
            }
            else if (forkRoomPositions.Contains(position)) // Check if it's a fork path room
            {
                roomPrefabToInstantiate = forkRoomPrefab;
            }
            else // Otherwise just instantiate a normal room prefab
            {
                roomPrefabToInstantiate = roomPrefab;
            }

            Vector3 spawnPosition = new Vector3(position.x * roomWidth, position.y * roomHeight, 0f);
            Instantiate(roomPrefabToInstantiate, spawnPosition, Quaternion.identity);

            logBuilder.Append("Room Coords: (").Append(position.x).Append(", ").Append(position.y).Append(")").Append(System.Environment.NewLine);
        }

        Debug.LogWarning(logBuilder.ToString());
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