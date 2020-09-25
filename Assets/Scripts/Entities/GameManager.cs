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

    public Ground ground;
    public List<Block> blocks = new List<Block>();
    public List<Brick> bricks = new List<Brick>();
    public List<PowerUp> powerUps = new List<PowerUp>();
    public List<Player> players = new List<Player>();
    public List<Bomb> bombs = new List<Bomb>();

    public Action onInitDone = null;
    public Action<Player> onMovePlayer = null;
    public Action<Player> onDeathPlayer = null;
    public Action<Bomb> onBombSpawned = null;
    public Action<Bomb> onBombRemoved = null;
    public Action<Brick> onBrickDestroyed = null;
    public Action<ExplosionType, Vector2> onExplosion = null;

    private void Start()
    {
        Init();
        onInitDone?.Invoke();
    }

    private void Update()
    {
        if (bombs.Count > 0)
        {
            var forExplode = bombs.FindAll(n => n.IsReady());
            forExplode.ForEach(n => ExplodeBomb(n));
        }
    }

    #region Initialization

    public void Init()
    {
        InitGround();

        InitBlocks();

        InitPlayers();

        InitBricks();

        InitPowerUps();

        InitBombs();

        InitCamera();
    }

    private void InitGround()
    {
        var groundData = Database.instance.grounds.First();
        ground = new Ground(groundData);
    }

    private void InitBlocks()
    {
        blocks = new List<Block>();

        var blockData = Database.instance.blocks.First();

        for (int y = 0; y < FIELD_SIZE.y; y++)
        {
            for (int x = 0; x < FIELD_SIZE.x; x++)
            {
                if (y == 0 || y == FIELD_SIZE.y - 1)
                {
                    blocks.Add(new Block(blockData, new Vector2(x, y)));
                    continue;
                }

                if ((y % 2) != 0)
                {
                    if (x == 0 || x == FIELD_SIZE.x - 1)
                    {
                        blocks.Add(new Block(blockData, new Vector2(x, y)));
                    }
                }
                else
                {
                    if ((x % 2) == 0)
                    {
                        blocks.Add(new Block(blockData, new Vector2(x, y)));
                    }
                }
            }
        }
    }

    private void InitBricks()
    {
        bricks = new List<Brick>();

        int count = Mathf.FloorToInt(FIELD_SIZE.x * FIELD_SIZE.y - (blocks.Count + players.Count * (Constants.FIELD_CELL_SIZE * 3)));
        int bricksCount = Mathf.RoundToInt(count * (BRICKS_DENSITY / 100.0f));

        var brickData = Database.instance.bricks.First();

        while (bricksCount > 0)
        {
            int x = UnityEngine.Random.Range(0, FIELD_SIZE.x);
            int y = UnityEngine.Random.Range(0, FIELD_SIZE.y);
            var pos = new Vector2(x, y);

            var cell = GetCellType(pos);
            if (cell != CellType.Empty)
                continue;

            if (players.Any(n => Vector2.Distance(n.pos, pos) < Constants.FIELD_CELL_SIZE * 2))
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

        /*while(powerUpCount > 0)
        {
            var brick = bricks.Random();

            if (powerUps.Any(n => n.pos.Equals(brick.pos)))
                continue;

            int powerUpNum = UnityEngine.Random.Range(0, Database.instance.powerUps.Count);
            var powerUpData = Database.instance.powerUps[powerUpNum];

            powerUps.Add(new PowerUp(powerUpData.type, brick.pos));
            powerUpCount--;
        }*/
    }

    private void InitPlayers()
    {
        players = new List<Player>();

        var firstPlayerData = Database.instance.players.First();
        players.Add(new Player(0, firstPlayerData, new Vector2(1, 1), false));

        var secondPlayerData = Database.instance.players.Last();
        players.Add(new Player(1, secondPlayerData, new Vector2(1, FIELD_SIZE.y - 2), false));

        var thirdPlayerData = Database.instance.players.Last();
        players.Add(new Player(2, thirdPlayerData, new Vector2(FIELD_SIZE.x - 2, 1), true));

        var fourthPlayerData = Database.instance.players.Last();
        players.Add(new Player(3, fourthPlayerData, new Vector2(FIELD_SIZE.x - 2, FIELD_SIZE.y - 2), true));
    }

    private void InitBombs()
    {
        bombs = new List<Bomb>();
    }

    public void InitCamera()
    {
        float x = FIELD_SIZE.x / 2 * -1;
        float z = FIELD_SIZE.y / 2;
        float y = Mathf.Abs(x) + Mathf.Abs(z);

        Camera.main.transform.position = new Vector3(x, y, z);
    }

    #endregion


    #region Actions

    public void MovePlayer(int id, Vector2 dir)
    {
        var player = players.Find(n => n.id.Equals(id));
        var nextPos = player.pos + (dir * PLAYER_SPEED * Time.deltaTime);
        
        //Works for spheres
        foreach(var block in blocks)
        {
            if (Vector3.Distance(block.pos, nextPos) < Constants.MOVE_COLLISION_DIST)
            {
                nextPos = block.pos + (nextPos - block.pos).normalized * Constants.MOVE_COLLISION_DIST;
            }
        }

        //Works for spheres
        foreach (var brick in bricks)
        {
            if (Vector3.Distance(brick.pos, nextPos) < Constants.MOVE_COLLISION_DIST)
            {
                nextPos = brick.pos + (nextPos - brick.pos).normalized * Constants.MOVE_COLLISION_DIST;
            }
        }

        //Works for spheres
        foreach (var bomb in bombs)
        {
            if (Vector3.Distance(bomb.pos, nextPos) < Constants.MOVE_COLLISION_DIST)
            {
                if (Vector3.Distance(bomb.pos, player.pos) <= Constants.MOVE_ON_BOMB_THRESHOLD)
                    continue;

                nextPos = bomb.pos + (nextPos - bomb.pos).normalized * Constants.MOVE_COLLISION_DIST;
            }
        }

        player.pos = nextPos;
        onMovePlayer?.Invoke(player);
    }

    public void DeathPlayer(Player player)
    {
        players.Remove(player);
        onDeathPlayer?.Invoke(player);
    }

    public void SpawnBomb(DBBomb dbBomb, Player owner, Vector2 pos)
    {
        int spawnedBombs = bombs.Count(n => n.owner.Equals(owner));
        if(spawnedBombs >= owner.bombsLimit)
        {
            Debug.LogError($"Reached bombs limit ({spawnedBombs}/{owner.bombsLimit}).");
            return;
        }

        var bomb = new Bomb(dbBomb, owner, pos);
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
        HandleDestruction(bomb.pos);


        //Left
        for(int x = bomb.pos.ToRoundInt().x - 1; x > bomb.pos.ToRoundInt().x - bomb.power; x--)
        {
            var pos = new Vector2(x, bomb.pos.y);
            if (HandleDestruction(pos))
                break;
        }

        //Right
        for(int x = bomb.pos.ToRoundInt().x + 1; x < bomb.pos.ToRoundInt().x + bomb.power; x++)
        {
            var pos = new Vector2(x, bomb.pos.y);
            if (HandleDestruction(pos))
                break;
        }

        //Top
        for(int y = bomb.pos.ToRoundInt().y + 1; y < bomb.pos.ToRoundInt().y + bomb.power; y++)
        {
            var pos = new Vector2(bomb.pos.x, y);
            if (HandleDestruction(pos))
                break;
        }

        //Bot
        for(int y = bomb.pos.ToRoundInt().y - 1; y > bomb.pos.ToRoundInt().y - bomb.power; y--)
        {
            var pos = new Vector2(bomb.pos.x, y);
            if (HandleDestruction(pos))
                break;
        }
    }

    #endregion



    #region Tools

    public CellType GetCellType(Vector2 pos)
    {
        var rounded = pos.ToRound();

        if (pos.x < 0 || pos.x >= FIELD_SIZE.x ||
            pos.y < 0 || pos.y >= FIELD_SIZE.y)
        {
            return CellType.Outside;
        }

        if (players.Any(n => Vector2.Distance(n.pos, pos) < Constants.EXPLOSION_AFFECT_DIST))
            return CellType.Player;

        if (powerUps.Any(n => n.pos.ToRound().Equals(rounded)))
            return CellType.PowerUp;

        if (blocks.Any(n => n.pos.ToRound().Equals(rounded)))
            return CellType.Block;

        if (bricks.Any(n => n.pos.ToRound().Equals(rounded)))
            return CellType.Brick;

        if (bombs.Any(n => n.pos.ToRound().Equals(rounded)))
            return CellType.Bomb;

        return CellType.Empty;
    }

    public bool HandleDestruction(Vector2 pos)
    {
        var type = GetCellType(pos);

        switch (type)
        {
            case CellType.Bomb:
                var bomb = bombs.Find(n => n.pos.Equals(pos));
                bomb.SetReady();
                return true;

            case CellType.Brick:
                var brick = bricks.Find(n => n.pos.Equals(pos));
                bricks.Remove(brick);
                onBrickDestroyed?.Invoke(brick);
                return true;

            case CellType.Player:
                var player = players.Find(n => Vector2.Distance(n.pos, pos) < Constants.EXPLOSION_AFFECT_DIST);
                players.Remove(player);
                onDeathPlayer?.Invoke(player);
                onExplosion?.Invoke(ExplosionType.Basic, pos);
                return false;

            case CellType.PowerUp:
                //var powerUp ...
                //Destroy powerUp
                return false;

            case CellType.Empty:
                onExplosion?.Invoke(ExplosionType.Basic, pos);
                return false;

            default:

                return true;
        }
    }

    #endregion
}
