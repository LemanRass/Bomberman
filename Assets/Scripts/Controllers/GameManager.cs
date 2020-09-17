using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum CellType
{
    Empty,
    Player,
    PowerUp,
    Block,
    Brick,
    Bomb,
    Outside
}

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
    public List<Bomb> bombs;

    public Action onInitDone = null;
    public Action<Player> onPlayerMoved = null;
    public Action<Bomb> onBombSpawned = null;
    public Action<Bomb> onBombRemoved = null;
    public Action<ExplosionType, Vector2> onExplosion = null;

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

        //InitPowerUps();

        InitBombs();

        InitCamera();
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

        var brickData = Database.instance.bricks.Last();

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

            bricks.Add(new Brick(brickData, pos));
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

            int powerUpNum = UnityEngine.Random.Range(0, Database.instance.powerUps.Count);
            var powerUpData = Database.instance.powerUps[powerUpNum];

            powerUps.Add(new PowerUp(powerUpData.type, brick.pos));
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

    private void InitBombs()
    {
        bombs = new List<Bomb>();
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

    public void SpawnBomb(DBBomb dbBomb, int power, Vector2 pos)
    {
        var bomb = new Bomb(dbBomb, power, pos);
        bombs.Add(bomb);
        onBombSpawned?.Invoke(bomb);
    }

    public void RemoveBomb(Bomb bomb)
    {
        onBombRemoved?.Invoke(bomb);
    }

    public void ExplodeBomb(Bomb bomb)
    {
        bombs.Remove(bomb);
        onBombRemoved?.Invoke(bomb);


        //Center
        onExplosion?.Invoke(ExplosionType.Basic, bomb.pos);


        //Left
        for(int x = bomb.pos.ToRoundInt().x - 1; x > bomb.pos.ToRoundInt().x - bomb.power; x--)
        {
            var pos = new Vector2(x, bomb.pos.y);
            var type = GetCellType(pos);

            if(type == CellType.Empty)
            {
                onExplosion?.Invoke(ExplosionType.Basic, pos);
            }
            else
            {
                break;
            }
        }

        //Right
        for(int x = bomb.pos.ToRoundInt().x + 1; x < bomb.pos.ToRoundInt().x + bomb.power; x++)
        {
            var pos = new Vector2(x, bomb.pos.y);
            var type = GetCellType(pos);

            if(type == CellType.Empty)
            {
                onExplosion?.Invoke(ExplosionType.Basic, pos);
            }
            else
            {
                break;
            }
        }


        //Top
        for(int y = bomb.pos.ToRoundInt().y + 1; y < bomb.pos.ToRoundInt().y + bomb.power; y++)
        {
            var pos = new Vector2(bomb.pos.x, y);
            var type = GetCellType(pos);

            if (type == CellType.Empty)
            {
                onExplosion?.Invoke(ExplosionType.Basic, pos);
            }
            else
            {
                break;
            }
        }

        //Bot
        for(int y = bomb.pos.ToRoundInt().y - 1; y > bomb.pos.ToRoundInt().y - bomb.power; y--)
        {
            var pos = new Vector2(bomb.pos.x, y);
            var type = GetCellType(pos);

            if (type == CellType.Empty)
            {
                onExplosion?.Invoke(ExplosionType.Basic, pos);
            }
            else
            {
                break;
            }
        }
    }

    private void Update()
    {
        if(bombs.Count > 0)
        {
            var forExplode = bombs.FindAll(n => n.IsReady());
            forExplode.ForEach(n => ExplodeBomb(n));
        }
    }

    public void InitCamera()
    {
        float x = FIELD_SIZE.x / 2 * -1;
        float z = FIELD_SIZE.y / 2;
        float y = Mathf.Abs(x) + Mathf.Abs(z);

        Camera.main.transform.position = new Vector3(x, y, z);
    }

    public CellType GetCellType(Vector2 pos)
    {
        var rounded = pos.ToRound();

        if (pos.x < 0 || pos.x >= FIELD_SIZE.x ||
            pos.y < 0 || pos.y >= FIELD_SIZE.y)
        {
            return CellType.Outside;
        }

        if (players.Any(n => n.pos.ToRound().Equals(rounded)))
            return CellType.Player;

        /*if (powerUps.Any(n => n.pos.ToRound().Equals(rounded)))
            return CellType.PowerUp;*/

        if (blocks.Any(n => n.pos.ToRound().Equals(rounded)))
            return CellType.Block;

        if (bricks.Any(n => n.pos.ToRound().Equals(rounded)))
            return CellType.Brick;

        if (bombs.Any(n => n.pos.ToRound().Equals(rounded)))
            return CellType.Bomb;

        return CellType.Empty;
    }

    
}
