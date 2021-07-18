using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public GameObject room;

    public Tilemap roomMap;
    public Tilemap roomBG;
    public Tilemap foreGround;

    private RoomTile[] roomTiles;

    public Room GenerateRoom(GameObject mapParent, GameObject roomPrefab, TileBase[] additionalTiles, float chanceToSpawnAdditionalTiles, Material tilemapMaterial)
    {
        foreGround = CreateTilemap("RoomForeground", mapParent, false, new Vector3(0, 0, 0), tilemapMaterial);
        roomMap = CreateTilemap("Room", mapParent, true, new Vector3(0, 0, 0), tilemapMaterial);

        roomMap.gameObject.layer = 11;

        roomMap.gameObject.tag = "Ground";

        roomBG = CreateTilemap("RoomBG", mapParent, false, new Vector3(0, 0, 20), tilemapMaterial);

        room = Instantiate(roomPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        roomTiles = room.GetComponentsInChildren<RoomTile>();

        foreach (RoomTile roomTile in roomTiles)
        {
            SpriteRenderer renderer = roomTile.gameObject.GetComponent<SpriteRenderer>();

            Destroy(renderer);

            (roomMap, foreGround) = roomTile.Create(roomMap, foreGround, additionalTiles, chanceToSpawnAdditionalTiles);
            roomBG = roomTile.CreateBackground(roomBG);
        }

        Destroy(room.gameObject);

        return this;
    }

    private Tilemap CreateTilemap(string tilemapName, GameObject mapParent, bool usesCollider, Vector3 position, Material tilemapMaterial)
    {
        GameObject tilemapObject = new GameObject(tilemapName);

        tilemapObject.transform.position = position;

        Tilemap tilemap = tilemapObject.AddComponent<Tilemap>();
        TilemapRenderer tilemapRenderer = tilemapObject.AddComponent<TilemapRenderer>();

        if (usesCollider)
        {
            TilemapCollider2D collider2D = tilemapObject.AddComponent<TilemapCollider2D>();
        }

        tilemap.tileAnchor = new Vector3(0, 0, 0);
        tilemapObject.transform.SetParent(mapParent.transform);

        tilemapRenderer.material = tilemapMaterial;

        return tilemap;
    }
}
