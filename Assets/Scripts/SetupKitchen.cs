using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupKitchen : MonoBehaviour {

    //Public variables to set tile prefabs
    public GameObject tileSprite1;
    public GameObject tileSprite2;
    public GameObject tileSprite3;
    public GameObject tileSprite4;
    public GameObject tileSprite5;
    public GameObject tileSprite6;
    public GameObject tileSprite7;
    public GameObject tileSprite8;
    public GameObject tileSprite9;
    public GameObject tileSprite10;
    public GameObject tileSprite11;
    public GameObject TopCounterObject;
    public GameObject TheWallObject;
    public GameObject OvenObject;
    public GameObject ChoppingObject;
    public GameObject BlenderObject;
    public GameObject ServeWindowCObject;
    public GameObject ServeWindowOObject;
    public GameObject TicketWindowCObject;
    public GameObject TicketWindowOObject;
    public GameObject CoverObject;
    public GameObject ClutterObject;

    public GameObject CubeObject;
    
    
    float tileWidth;
    float tileHeight;

    int cameraWidth = 10;
    int cameraHeight = 5;
    int cameraOffsetX = 13;
    int cameraOffsetY = 6;

    const int TILECOUNTX = 15;
    const int TILECOUNTY = 7;

    Grid tileGrid;

    Vector2[,] gridPositions;
    GameObject[,] tiles;
    int[,] tileLayout;
    Vector2 topCounterPos;
    Vector2 theWallPos;
    Vector2 ovenPos;
    Vector2 choppingPos;
    Vector2 blenderPos;
    Vector2 serveWindowCPos;
    Vector2 serveWindowOPos;
    Vector2 ticketWindowCPos;
    Vector2 ticketWindowOPos;
    Vector2 coverPos;
    Vector2 cubePos;

    // Use this for initialization
    void Start ()
    {

    }

    public void Initialize()
    {
        //set initial position values
        tileWidth = tileSprite1.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        tileHeight = tileSprite2.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        gridPositions = new Vector2[TILECOUNTX, TILECOUNTY];
        tiles = new GameObject[TILECOUNTX, TILECOUNTY];
        topCounterPos = new Vector2(0.0f, 0.0f);
        theWallPos = new Vector2(0.0f, 0.0f);
        ovenPos = new Vector2(0.0f, 1.215f);
        choppingPos = new Vector2(9.16f, -3.76f);
        blenderPos = new Vector2(-9.0f, -3.77f);
        serveWindowCPos = new Vector2(8.14f, 4.5f);
        serveWindowOPos = new Vector2(8.14f, 4.5f);
        ticketWindowCPos = new Vector2(-8.14f, 4.5f);
        ticketWindowOPos = new Vector2(-8.14f, 4.5f);
        coverPos = new Vector2(0.0f, 0.0f);
        cubePos = new Vector2(0.0f, -5.0f);

        //instantiate each preparation station
        SpawnStations();
    }

    void CreateGrid()
    {
        for (int i = 0; i < TILECOUNTX; i++)
        {
            for (int j = 0; j < TILECOUNTY; j++)
            {
                gridPositions[i, j] = new Vector2(i * tileWidth - cameraOffsetX , -j * tileHeight + cameraOffsetY - (tileHeight / 2));
            }
        }
    }
    
    void PositionTiles()
    {
        for (int i = 0; i < TILECOUNTX; i++)
        {
            for (int j = 0; j < TILECOUNTY; j++)
            {
                if (tiles[i, j] != null)
                {
                    tiles[i, j].transform.SetPositionAndRotation(gridPositions[i, j], Quaternion.identity);
                }
            }
        }
    }

    void SetLayout()
    {
        tileLayout = new int[TILECOUNTY, TILECOUNTX]
                {{00, 00, 00, 00, 00, 00, 00, 00 ,00 ,00 ,00 ,00, 00, 00, 00},
                 {05, 03, 03, 03, 03, 03, 03, 03 ,03 ,03 ,03 ,03, 03, 03, 08},
                 {06, 04, 04, 04, 04, 04, 04, 04 ,04 ,04 ,04 ,04, 04, 04, 09},
                 {06, 01, 01, 01, 01, 01, 01, 01 ,01 ,01 ,01 ,01, 01, 01, 09},
                 {06, 01, 01, 01, 01, 01, 01, 01 ,01 ,01 ,01 ,01, 01, 01, 09},
                 {06, 01, 01, 01, 01, 01, 01, 01 ,01 ,01 ,01 ,01, 01, 01, 09},
                 {07, 11, 11, 11, 11, 11, 11, 11 ,11 ,11 ,11 ,11, 11, 11, 10}};
    }

    void SpawnTiles()
    {
        for (int i = 0; i < TILECOUNTX; i++)
        {
            for (int j = 0; j < TILECOUNTY; j++)
            {
                switch (tileLayout[j, i])
                {
                    case 0:
                        tiles[i, j] = null;
                        break;

                    case 1:
                        tiles[i, j] = Instantiate(tileSprite1);
                        break;

                    case 2:
                        tiles[i, j] = Instantiate(tileSprite2);
                        break;

                    case 3:
                        tiles[i, j] = Instantiate(tileSprite3);
                        break;

                    case 4:
                        tiles[i, j] = Instantiate(tileSprite4);
                        break;

                    case 5:
                        tiles[i, j] = Instantiate(tileSprite5);
                        break;

                    case 6:
                        tiles[i, j] = Instantiate(tileSprite6);
                        break;

                    case 7:
                        tiles[i, j] = Instantiate(tileSprite7);
                        break;

                    case 8:
                        tiles[i, j] = Instantiate(tileSprite8);
                        break;

                    case 9:
                        tiles[i, j] = Instantiate(tileSprite9);
                        break;

                    case 10:
                        tiles[i, j] = Instantiate(tileSprite10);
                        break;

                    case 11:
                        tiles[i, j] = Instantiate(tileSprite11);
                        break;

                    default:
                        tiles[i, j] = null;
                        break;
                }

            }
        }
    }

    void SpawnStations()
    {
        TopCounterObject.transform.SetPositionAndRotation(topCounterPos, Quaternion.identity);
        Instantiate<GameObject>(TopCounterObject);

        TheWallObject.transform.SetPositionAndRotation(theWallPos, Quaternion.identity);
        Instantiate<GameObject>(TheWallObject);

        OvenObject.transform.SetPositionAndRotation(ovenPos, Quaternion.identity);
        Instantiate<GameObject>(OvenObject);

        ChoppingObject.transform.SetPositionAndRotation(choppingPos, Quaternion.identity);
        Instantiate<GameObject>(ChoppingObject);

        BlenderObject.transform.SetPositionAndRotation(blenderPos, Quaternion.identity);
        Instantiate<GameObject>(BlenderObject);

        ServeWindowOObject.transform.SetPositionAndRotation(serveWindowOPos, Quaternion.identity);
        Instantiate<GameObject>(ServeWindowOObject);

        ServeWindowCObject.transform.SetPositionAndRotation(serveWindowCPos, Quaternion.identity);
        Instantiate<GameObject>(ServeWindowCObject);

        TicketWindowOObject.transform.SetPositionAndRotation(ticketWindowOPos, Quaternion.identity);
        Instantiate<GameObject>(TicketWindowOObject);

        TicketWindowCObject.transform.SetPositionAndRotation(ticketWindowCPos, Quaternion.identity);
        Instantiate<GameObject>(TicketWindowCObject);

        CoverObject.transform.SetPositionAndRotation(coverPos, Quaternion.identity);
        Instantiate<GameObject>(CoverObject);

        CubeObject.transform.SetPositionAndRotation(cubePos, Quaternion.identity);
        Instantiate<GameObject>(CubeObject);

        ClutterObject.transform.SetPositionAndRotation(Vector2.zero, Quaternion.identity);
        Instantiate<GameObject>(ClutterObject);
    }

    public void DestroyObject(GameObject toDestroy)
    {
        Destroy(toDestroy);
        toDestroy = null;
    }
}
