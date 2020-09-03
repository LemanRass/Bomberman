using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    private void Awake() => instance = this;

    public Vector2Int FIELD_SIZE;

    public int BRICKS_COUNT = 30;
    public int POWERUP_COUNT = 5;
    public float PLAYER_SPEED = 2.0f;

    public List<Block> blocks;
    public List<Brick> bricks;
    public List<PowerUp> powerUps;
    public List<Player> players;

    public List<Vector3> playerPoints;

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

        InitBricks();

        InitPowerUps();

        InitPlayers();
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
            var pos = new Vector2Int(x, y);

            if (blocks.Any(n => n.pos.Equals(pos)))
                continue;

            if (bricks.Any(n => n.pos.Equals(pos)))
                continue;

            var pos3 = new Vector3(pos.x, 0, pos.y);

            if (playerPoints.Any(n => Vector3.Distance(n, pos3) < 2))
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

    public void MovePlayer(int id, Vector3 dir)
    {
        var player = players.Find(n => n.id.Equals(id));
        var nextPos = player.pos + (dir * PLAYER_SPEED * Time.deltaTime);

        /*if(nextPos.x > FIELD_SIZE.x ||
            nextPos.x < 0 ||
            nextPos.y > FIELD_SIZE.y ||
            nextPos.y < 0)*/

        foreach(var block in blocks)
        {
            //if within horizontal
            if(nextPos.x > block.pos3.x - 0.5f && nextPos.x < block.pos3.x + 0.5f )

            if (Vector3.Distance(block.pos3, nextPos) < 0.98f)
            {
                nextPos = block.pos3 + Vector3.Normalize(nextPos - block.pos3) * 0.98f;
            }
        }

        player.pos = nextPos;
        onPlayerMoved?.Invoke(player);
    }
}
