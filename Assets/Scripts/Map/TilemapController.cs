using MyEnums;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{
    // Tilemaps
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Tilemap doorTilemap;

    // Wall Tiles
    public TileBase northWallTile;
    public TileBase eastWallTile;
    public TileBase southWallTile;
    public TileBase westWallTile;

    // Corner Wall Tiles
    public TileBase cornerNWTile;
    public TileBase cornerNETile;
    public TileBase cornerSWTile;
    public TileBase cornerSETile;

    // Floor Tiles
    public TileBase[] floorTiles;
    //public TileBase floorTile2;
    //public TileBase floorTile3;
    //public TileBase floorTile4;

    // Door Tiles
    public TileBase eastDoorTile;
    public TileBase westDoorTile;
    public TileBase northDoorTile;
    public TileBase southDoorTile;


    public enum CornerOrientation
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        None
    }



    public void PlaceTiles(RoomSO room)
    {
        foreach (Vector2Int tilePosition in room.floorTileLocations)
        {
            floorTilemap.SetTile(new Vector3Int(tilePosition.x, tilePosition.y, 0), floorTiles[Random.Range(0, floorTiles.Length)]);
        }

        foreach (Vector2Int wallTilePos in room.wallTileLocations)
        {
            // Calculate the corner's position within the room
            Vector2Int relativeCornerPos = wallTilePos - room.roomPosition;

            // Determine the wall orientation based on its position relative to the room
            Direction wallOrientation = DetermineWallOrientation(relativeCornerPos, room.roomWidth, room.roomHeight);

            // Get the appropriate wall tile based on the orientation
            TileBase wallTile = GetWallTileForOrientation(wallOrientation);

            // Place the wall tile
            if (wallTile != null)
            {
                wallTilemap.SetTile(new Vector3Int(wallTilePos.x, wallTilePos.y, 0), wallTile);
            }
        }

        foreach (Vector2Int cornerTilePos in room.cornerWallTileLocations)
        {
            // Calculate the corner's position within the room
            Vector2Int relativeCornerPos = cornerTilePos - room.roomPosition;

            // Determine which corner tile to use based on the corner orientation
            CornerOrientation cornerOrientation = DetermineCornerOrientation(relativeCornerPos, room.roomWidth, room.roomHeight);
            TileBase cornerTile = GetCornerTileForOrientation(cornerOrientation);

            // Place the chosen corner tile at the cornerTilePos
            if (cornerTile != null)
            {
                wallTilemap.SetTile(new Vector3Int(cornerTilePos.x, cornerTilePos.y, 0), cornerTile);
            }
        }

        foreach (Vector2Int doorTilePos in room.doorTileLocations)
        {
            // Calculate the corner's position within the room
            Vector2Int relativeCornerPos = doorTilePos - room.roomPosition;

            // Determine the wall orientation based on its position relative to the room
            Direction wallOrientation = DetermineWallOrientation(relativeCornerPos, room.roomWidth, room.roomHeight);

            TileBase doorTile = GetDoorTileForDirection(wallOrientation);

            doorTilemap.SetTile(new Vector3Int(doorTilePos.x, doorTilePos.y, 0), doorTile);
        }
    }

    private TileBase GetWallTileForOrientation(Direction wallOrientation)
    {
        switch (wallOrientation)
        {
            case Direction.North:
                return northWallTile;
            case Direction.East:
                return eastWallTile;
            case Direction.South:
                return southWallTile;
            case Direction.West:
                return westWallTile;
            default:
                return null; // Handle default case appropriately
        }
    }

    private Direction DetermineWallOrientation(Vector2Int wallTilePos, int roomWidth, int roomHeight)
    {
        int x = wallTilePos.x;
        int y = wallTilePos.y;
        int maxX = roomWidth - 1;
        int maxY = roomHeight - 1;

        if (y == maxY)
        {
            return Direction.North;
        }
        else if (x == maxX)
        {
            return Direction.East;
        }
        else if (y == 0)
        {
            return Direction.South;
        }
        else if (x == 0)
        {
            return Direction.West;
        }
        else
        {
            return Direction.None; // This should not happen if wallTilePos is valid
        }
    }

    private CornerOrientation DetermineCornerOrientation(Vector2Int cornerPosition, int roomWidth, int roomHeight)
    {
        if (cornerPosition == Vector2Int.zero) // Bottom-left corner
        {
            return CornerOrientation.BottomLeft;
        }
        else if (cornerPosition == new Vector2Int(roomWidth - 1, 0)) // Bottom-right corner
        {
            return CornerOrientation.BottomRight;
        }
        else if (cornerPosition == new Vector2Int(0, roomHeight - 1)) // Top-left corner
        {
            return CornerOrientation.TopLeft;
        }
        else if (cornerPosition == new Vector2Int(roomWidth - 1, roomHeight - 1)) // Top-right corner
        {
            return CornerOrientation.TopRight;
        }
        else
        {
            return CornerOrientation.None;
        }
    }

    private TileBase GetCornerTileForOrientation(CornerOrientation cornerOrientation)
    {
        switch (cornerOrientation)
        {
            case CornerOrientation.TopLeft:
                return cornerNWTile;
            case CornerOrientation.TopRight:
                return cornerNETile;
            case CornerOrientation.BottomLeft:
                return cornerSWTile;
            case CornerOrientation.BottomRight:
                return cornerSETile;
            default:
                return null; // Return null for unknown corner orientations
        }
    }

    private TileBase GetDoorTileForDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return northDoorTile;
            case Direction.East:
                return eastDoorTile;
            case Direction.South:
                return southDoorTile;
            case Direction.West:
                return westDoorTile;
            default:
                return null; // Handle default case appropriately
        }
    }

    private TileBase FlipTile180Degrees(TileBase tile)
    {
        if (tile is Tile)
        {
            Tile tileData = (Tile)tile;
            Matrix4x4 matrix = tileData.transform;
            matrix = matrix * Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180));
            tileData.transform = matrix;
            return tileData;
        }
        return tile; // Return the original tile if it's not a Tile type
    }
}