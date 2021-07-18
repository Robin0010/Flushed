using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] int levelSize;
    [SerializeField] int borderSize;
    [SerializeField] float chanceToSpawnAdditionalTile;
    [SerializeField] GameObject[] roomTemplates;
    [SerializeField] TileBase[] additionalTiles;
    [SerializeField] Material spriteLit;

    [SerializeField] GameObject mapParent;

    [SerializeField] TileBase borderTile;

    private int xBorderOffset = 16;
    private int yBorderOffset = 16;

    private List<Room> rooms = new List<Room>();

    private void Start()
    {
        GenerateRooms();

        DrawBorder();
    }

    private void GenerateRooms()
    {
        for (int i = 0; i < levelSize; i++)
        {
            for (int j = 0; j < levelSize; j++)
            {
                Room room = new Room();

                int rndRoom = Random.Range(0, roomTemplates.Length);

                rooms.Add(room.GenerateRoom(mapParent, roomTemplates[rndRoom], additionalTiles, chanceToSpawnAdditionalTile, spriteLit));
            }
        }

        int index = 0;

        for (int y = 0; y < levelSize; y++)
        {
            for (int x = 0; x < levelSize; x++)
            {
                rooms[index].roomMap.transform.position = new Vector2(x * 33, y * 33);
                rooms[index].roomBG.transform.position = new Vector3(x * 33, y * 33, 20);
                rooms[index].foreGround.transform.position = new Vector3(x * 33, y * 33, -1);

                index++;
            }
        }
    }

    private void DrawBorder()
    {
        Tilemap border = CreateTilemap("Border", mapParent, true, new Vector3(0, 0, 0));

        border.gameObject.layer = 11;

        border.gameObject.tag = "Ground";

        for (int y = 0; y < levelSize * 33; y++)
        {
            for (int x = 0; x < levelSize * 33; x++)
            {
                if (x == 0)
                {
                    for (int i = 1; i < borderSize + 1; i++)
                    {
                        Vector3Int position = new Vector3Int(x - i - xBorderOffset, y - yBorderOffset, 0);

                        border.SetTile(position, borderTile);
                    }
                }

                if (x == levelSize * 33 - 1)
                {
                    for (int i = 1; i < borderSize + 1; i++)
                    {
                        Vector3Int position = new Vector3Int(x + i - xBorderOffset, y - yBorderOffset, 0);

                        border.SetTile(position, borderTile);
                    }
                }

                if (y == 0)
                {
                    for (int i = 1; i < borderSize + 1; i++)
                    {
                        Vector3Int position = new Vector3Int(x - xBorderOffset, y - i - yBorderOffset, 0);

                        border.SetTile(position, borderTile);
                    } 
                }

                if (y == levelSize * 33 - 1)
                {
                    for (int i = 1; i < borderSize + 1; i++)
                    {
                        Vector3Int position = new Vector3Int(x - xBorderOffset, y + i - yBorderOffset, 0);

                        border.SetTile(position, borderTile);
                    } 
                }
            }
        }
    }

    private Tilemap CreateTilemap(string tilemapName, GameObject mapParent, bool usesCollider, Vector3 position)
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

        tilemapRenderer.material = spriteLit;

        return tilemap;
    }
}
