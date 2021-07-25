using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject linePrefab;
    public Sprite xSprite;
    public Sprite oSprite;
    public Text infoText;
    public AudioClip xSound;
    public AudioClip oSound;

    private Vector3 linePosition;
    private Quaternion lineRotation;
    private Vector3 lineScale;
    private bool isXTurn;
    private bool gameOver;
    private GameObject[,] tiles;
    private GameObject line;
    private AudioSource audioSource;

    private readonly int rows = 3;
    private readonly int cols = 3;
    private readonly float tileOffset = 2f;

    void Start()
    {
        tiles = new GameObject[rows, cols];
        int row = 0;
        for (int y = 1; y >= -1; y -= 1)
        {
            int col = 0;
            for (int x = -1; x <= 1; x += 1)
            {                
                tiles[row, col] = Instantiate(tilePrefab, new Vector3(x * tileOffset, y * tileOffset, 0), Quaternion.identity);
                col += 1;
            }
            row += 1;
        }

        isXTurn = true;
        gameOver = false;
        linePosition = Vector3.zero;
        lineRotation = Quaternion.identity;
        lineScale = new Vector3(0.2f, 6f, 1f);
        infoText.text = "";

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!gameOver && Input.GetMouseButtonDown(0)) // If we detect a mouse left click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray, 1500f);

            // Check if we clicked the collision box of a tile
            foreach (RaycastHit2D hit in hits)
            {
                foreach (GameObject tile in tiles)
                {
                    if (hit.collider == tile.GetComponent<BoxCollider2D>())
                    {
                        Tile tileScript = tile.GetComponent<Tile>();
                        if (isXTurn)
                        {
                            tileScript.SetValue(1);
                            tileScript.SetSprite(xSprite);
                            audioSource.clip = xSound;
                        }
                        else
                        {
                            tileScript.SetValue(2);
                            tileScript.SetSprite(oSprite);
                            audioSource.clip = oSound;
                        }
                        audioSource.Play();
                        CheckWinner();
                        CheckDraw();
                        if (!gameOver)
                        {
                            FlipTurn();
                        }
                    }
                }
            }
        }

        if (gameOver && Input.GetKeyUp(KeyCode.R))
        {
            foreach (GameObject tile in tiles)
            {
                Destroy(tile);
            }
            Destroy(line);
            Start();
        }
    }

    private void FlipTurn()
    {
        isXTurn = !isXTurn;
    }

    private void CheckWinner()
    {
        if (CheckCols() || CheckRows() || CheckLeftDiagonal() || CheckRightDiagonal())
        {
            gameOver = true;
        }

        if (gameOver)
        {
            line = Instantiate(linePrefab, linePosition, lineRotation);
            line.transform.localScale = lineScale;
            line.GetComponent<SpriteRenderer>().color = Color.red;
            string winner = isXTurn ? "X" : "O";
            infoText.text = "The winner is " + winner + "\nPress R to start a new game";
        }
    }

    private void CheckDraw()
    {
        bool allTilesSet = true;
        foreach (GameObject tile in tiles)
        {
            if (tile.GetComponent<Tile>().GetValue() == -1)
            {
                allTilesSet = false;
                break;
            }
        }

        if (allTilesSet)
        {
            gameOver = true;
            infoText.text = "Draw\nPress R to start a new game";
        }
    }

    private bool CheckCols()
    {
        for (int i = 0; i < 3; i += 1)
        {
            if (CheckCol(i))
            {
                linePosition = new Vector3((i * 2) - 2, 0, -1);
                lineRotation = Quaternion.identity;
                return true;
            }
        }
        return false;
    }

    private bool CheckCol(int col)
    {
        List<Tile> tileScripts = new List<Tile>();
        for (int i = 0; i < 3; i += 1)
        {
            tileScripts.Add(tiles[i, col].GetComponent<Tile>());
        }
        return AllTilesHaveSameValue(tileScripts);
    }

    private bool CheckRows()
    {
        for (int i = 0; i < 3; i += 1)
        {
            if (CheckRow(i))
            {
                linePosition = new Vector3(0, (i * -2) + 2, -1);
                lineRotation = Quaternion.Euler(0, 0, 90);
                return true;
            }
        }
        return false;
    }

    private bool CheckRow(int row)
    {
        List<Tile> tileScripts = new List<Tile>();
        for (int i = 0; i < 3; i += 1)
        {
            tileScripts.Add(tiles[row, i].GetComponent<Tile>());
        }
        return AllTilesHaveSameValue(tileScripts);
    }

    private bool CheckLeftDiagonal()
    {
        List<Tile> tileScripts = new List<Tile>
        {
            tiles[0, 0].GetComponent<Tile>(),
            tiles[1, 1].GetComponent<Tile>(),
            tiles[2, 2].GetComponent<Tile>(),
        };
        bool result = AllTilesHaveSameValue(tileScripts);
        if (result)
        {
            linePosition = new Vector3(0, 0, -1);
            lineRotation = Quaternion.Euler(0, 0, 45);
            lineScale = new Vector3(0.2f, 8.5f, 1f);
        }
        return result;
    }

    private bool CheckRightDiagonal()
    {
        List<Tile> tileScripts = new List<Tile>()
        {
            tiles[0, 2].GetComponent<Tile>(),
            tiles[1, 1].GetComponent<Tile>(),
            tiles[2, 0].GetComponent<Tile>(),
        };
        bool result = AllTilesHaveSameValue(tileScripts);
        if (result)
        {
            linePosition = new Vector3(0, 0, -1);
            lineRotation = Quaternion.Euler(0, 0, 135);
            lineScale = new Vector3(0.2f, 8.5f, 1f);
        }
        return result;
    }

    private bool AllTilesHaveSameValue(List<Tile> tiles)
    {
        return tiles[0].GetValue() == tiles[1].GetValue()
            && tiles[0].GetValue() == tiles[2].GetValue()
            && tiles[0].GetValue() != -1;
    }
}
