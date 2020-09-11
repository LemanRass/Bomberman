using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cell
{
    public CellType type;
    public object data;
}

public enum CellType
{
    Empty,
    Block,
    Brick,
    PowerUp,
    Bomb
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    private void Awake() => instance = this;

    public Vector2Int FIELD_SIZE;

    public int BRICKS_COUNT = 30;
    public int POWERUP_COUNT = 5;
    public float PLAYER_SPEED = 2.0f;

    public Cell[,] field = new Cell[9, 9];

    public List<Block> blocks;
    public List<Brick> bricks;
    public List<PowerUp> powerUps;
    public List<Player> players;

    public List<Vector2> playerPoints;

    public Action onInitDone = null;
    public Action<Player> onPlayerMoved = null;

    private void Start()
    {
        Init();
        onInitDone?.Invoke();
    }

    public void Init()
    {
        InitBlocks2();

        //InitBricks();

        //InitPowerUps();

        InitPlayers();
    }

    private void InitBlocks2()
    {
        for (int y = 0; y < field.GetLength(1); y++)
        {
            for (int x = 0; x < field.GetLength(0); x++)
            {
                if (y == 0 || y == field.GetLength(1) - 1)
                {
                    field[x, y] = new Cell()
                    {
                        type = CellType.Block,
                        data = new Block(x, y)
                    };
                    continue;
                }

                if ((y % 2) != 0)
                {
                    if (x == 0 || x == field.GetLength(0) - 1)
                    {
                        field[x, y] = new Cell()
                        {
                            type = CellType.Block,
                            data = new Block(x, y)
                        };
                    }
                }
                else
                {
                    if ((x % 2) == 0)
                    {
                        field[x, y] = new Cell()
                        {
                            type = CellType.Block,
                            data = new Block(x, y)
                        };
                    }
                }
            }
        }
    }


    private void InitBlocks()
    {
        blocks = new List<Block>();

        for (int y = 0; y < FIELD_SIZE.y; y++)
        {
            for (int x = 0; x < FIELD_SIZE.x; x++)
            {
                if (y == 0 || y == FIELD_SIZE.y - 1)
                {
                    blocks.Add(new Block(x, y));
                    continue;
                }

                if ((y % 2) != 0)
                {
                    if (x == 0 || x == FIELD_SIZE.x - 1)
                    {
                        blocks.Add(new Block(x, y));
                    }
                }
                else
                {
                    if ((x % 2) == 0)
                    {
                        blocks.Add(new Block(x, y));
                    }
                }
            }
        }
    }

    private void InitBricks()
    {
        bricks = new List<Brick>();

        int bricksCount = BRICKS_COUNT;

        while (bricksCount > 0)
        {
            int x = UnityEngine.Random.Range(0, FIELD_SIZE.x);
            int y = UnityEngine.Random.Range(0, FIELD_SIZE.y);
            var pos = new Vector2(x, y);

            if (blocks.Any(n => n.pos.Equals(pos)))
                continue;

            if (bricks.Any(n => n.pos.Equals(pos)))
                continue;

            if (playerPoints.Any(n => Vector2.Distance(n, pos) < 2))
                continue;

            bricks.Add(new Brick(pos));
            bricksCount--;
        }
    }

    private void InitPowerUps()
    {
        powerUps = new List<PowerUp>();
        
        while(powerUps.Count < POWERUP_COUNT)
        {
            int wallIdx = UnityEngine.Random.Range(0, bricks.Count);
            var wall = bricks[wallIdx];
            if (powerUps.Any(n => n.pos.Equals(wall.pos)))
                continue;

            int powerUpId = UnityEngine.Random.Range(0, Database.instance.powerUps.Count);
            var powerUpData = Database.instance.powerUps.ElementAt(powerUpId);
            powerUps.Add(new PowerUp(powerUpData.Key, wall.pos));
        }
    }

    private void InitPlayers()
    {
        players = new List<Player>();

        for(int i = 0; i < playerPoints.Count; i++)
        {
            players.Add(new Player(i, playerPoints[i], i == 0));
        }
    }

    public void MovePlayer(int id, Vector2 dir)
    {
        var player = players.Find(n => n.id.Equals(id));
        var nextPos = player.pos + (dir * PLAYER_SPEED * Time.deltaTime);

        /*if(nextPos.x > FIELD_SIZE.x ||
            nextPos.x < 0 ||
            nextPos.y > FIELD_SIZE.y ||
            nextPos.y < 0)*/


        for(int x = 0; x < field.GetLength(0); x++)
        {
            for(int y = 0; y < field.GetLength(1); y++)
            {
                if(field[x, y].type == CellType.Block)
                {
                    var block = field[x, y].data as Block;

                    if (Vector3.Distance(block.pos, nextPos) < 0.98f)
                    {
                        nextPos = block.pos + (nextPos - block.pos).normalized * 0.98f;
                    }
                }
            }
        }

        /*
        //Works for spheres
        foreach(var block in blocks)
        {
            if (Vector3.Distance(block.pos, nextPos) < 0.98f)
            {
                nextPos = block.pos + (nextPos - block.pos).normalized * 0.98f;
            }
        }
        */

        player.pos = nextPos;
        onPlayerMoved?.Invoke(player);
    }
}
