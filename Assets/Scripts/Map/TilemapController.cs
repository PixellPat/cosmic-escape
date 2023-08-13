using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Tilemap doorTilemap;

    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase doorTile;

    public void PlaceTiles(RoomSO room)
    {
        ClearTilemaps();
        //floorTilemap.SetTile(new Vector3Int(0, 0, 0), floorTile);

        foreach (Vector2Int tilePosition in room.floorTileLocations)
        {
            floorTilemap.SetTile(new Vector3Int(tilePosition.x, tilePosition.y, 0), floorTile);
        }

        foreach (Vector2Int wallTilePos in room.wallTileLocations)
        {
            wallTilemap.SetTile(new Vector3Int(wallTilePos.x, wallTilePos.y, 0), wallTile);
        }

        foreach (Vector2Int doorTilePos in room.doorTileLocations)
        {
            doorTilemap.SetTile(new Vector3Int(doorTilePos.x, doorTilePos.y, 0), doorTile);
        }
    }

    private void ClearTilemaps()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        doorTilemap.ClearAllTiles();
    }
}