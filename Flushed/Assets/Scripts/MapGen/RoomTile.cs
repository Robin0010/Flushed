using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomTile : MonoBehaviour
{
    public TileBase tile;
    public TileBase tileBG;

    [SerializeField] bool isWall;
    [SerializeField] bool overwriteChance;
    [SerializeField] float spawnChance;
    [SerializeField] GameObject tilePrefab;

    private void OnValidate()
    {
        if (overwriteChance)
        {
            spawnChance = 100;
        }
    }

    public (Tilemap, Tilemap) Create(Tilemap roomMap, Tilemap foreGround, TileBase[] additionalTiles, float chanceToSpawnAdditionalTiles)
    {
        if (tile != null)
        {
            float rnd = Random.Range(0, 100);

            if (rnd < spawnChance)
            {
                Vector3Int position = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);

                position = roomMap.WorldToCell(position);

                roomMap.SetTile(position, tile);
            }

            if (rnd < chanceToSpawnAdditionalTiles)
            {
                int rndTileIndex = Random.Range(0, additionalTiles.Length);

                Vector3Int position = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);

                position = foreGround.WorldToCell(position);

                foreGround.SetTile(position, additionalTiles[rndTileIndex]);
            }
        }

        return (roomMap, foreGround);
    }

    public Tilemap CreateBackground(Tilemap roomBG)
    {
        Vector3Int position = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);

        position = roomBG.WorldToCell(position);

        roomBG.SetTile(position, tileBG);

        return roomBG;
    }
}
