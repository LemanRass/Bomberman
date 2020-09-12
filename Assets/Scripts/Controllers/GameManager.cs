using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    private void Awake() => instance = this;

    public Vector2Int FIELD_SIZE;

    [Range(0, 100)]
    public int BRICKS_DENSITY = 90;
    [Range(0, 100)]
    public int POWERUP_DENSITY = 50;
    public float PLAYER_SPEED = 2.0f;

    public List<Block> blocks;
    public List<Brick> bricks;
    public List<PowerUp> powerUps;
    public List<Player> players;

    public Action onInitDone = null;
    public Action<Player> onPlayerMoved = null;

    private void Start()
    {
        Init();
        onInitDone?.Invoke();
    }

    public void Init()
    {
        InitBlocks();

        InitPlayers();

        InitBricks();

        InitPowerUps();

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

        int count = FIELD_SIZE.x * FIELD_SIZE.y - (blocks.Count + players.Count * 3);
        int bricksCount = Mathf.RoundToInt(count * (BRICKS_DENSITY / 100.0f));

        while (bricksCount > 0)
        {
            int x = UnityEngine.Random.Range(0, FIELD_SIZE.x);
            int y = UnityEngine.Random.Range(0, FIELD_SIZE.y);
            var pos = new Vector2(x, y);

            if (blocks.Any(n => n.pos.Equals(pos)))
                continue;

            if (bricks.Any(n => n.pos.Equals(pos)))
                continue;

            if (players.Any(n => Vector2.Distance(n.pos, pos) < 2.0f))
                continue;

            bricks.Add(new Brick(pos));
            bricksCount--;
        }
    }

    //Execute only after bricks init
    private void InitPowerUps()
    {
        powerUps = new List<PowerUp>();

        int powerUpCount = Mathf.RoundToInt(bricks.Count * (POWERUP_DENSITY / 100.0f));
        Debug.Log($"Power Ups Count: {powerUpCount}");

        while(powerUpCount > 0)
        {
            var brick = bricks.Random();

            if (powerUps.Any(n => n.pos.Equals(brick.pos)))
                continue;

            int powerUpId = UnityEngine.Random.Range(0, Database.instance.powerUps.Count);
            var powerUpData = Database.instance.powerUps.ElementAt(powerUpId);

            powerUps.Add(new PowerUp(powerUpData.Key, brick.pos));
            powerUpCount--;
        }
    }

    private void InitPlayers()
    {
        players = new List<Player>();

        players.Add(new Player(1, new Vector2(1, 1), true));
        players.Add(new Player(2, new Vector2(1, FIELD_SIZE.y - 2), false));
        players.Add(new Player(3, new Vector2(FIELD_SIZE.x - 2, 1), false));
        players.Add(new Player(4, new Vector2(FIELD_SIZE.x - 2, FIELD_SIZE.y - 2), false));
    }

    public void MovePlayer(int id, Vector2 dir)
    {
        var player = players.Find(n => n.id.Equals(id));
        var nextPos = player.pos + (dir * PLAYER_SPEED * Time.deltaTime);
        
        //Works for spheres
        foreach(var block in blocks)
        {
            if (Vector3.Distance(block.pos, nextPos) < 0.98f)
            {
                nextPos = block.pos + (nextPos - block.pos).normalized * 0.98f;
            }
        }
        
        player.pos = nextPos;
        onPlayerMoved?.Invoke(player);
    }
}
