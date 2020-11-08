using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadricGameManager : GameManager
{
    public const int FIELD_SIZE_X = 7;
    public const int FIELD_SIZE_Y = 7;

    protected override void Update()
    {
        if (bombs.Count > 0)
        {
            var forExplode = bombs.FindAll(n => n.IsReady());
            forExplode.ForEach(n => ExplodeBomb(n));
        }
    }

    #region Init

    public override void Init()
    {
        InitGround();

        InitBlocks();

        InitPlayers();

        InitBricks();

        InitPowerUps();

        InitBombs();

        InitCamera();

        onInitDone?.Invoke();
    }

    protected override void InitGround()
    {
        grounds = new Ground[FIELD_SIZE_X][];
        for (int i = 0; i < grounds.Length; i++)
            grounds[i] = new Ground[FIELD_SIZE_Y];


        var groundData = Database.instance.grounds.First();

        for(int x = 0; x < FIELD_SIZE_X; x++)
        {
            for(int y = 0; y < FIELD_SIZE_Y; y++)
            {
                var ground = new Ground(groundData, new Vector2Int(x, y), new Vector2(x, y));
                grounds[x][y] = ground;
            }
        }
    }

    protected override void InitBlocks()
    {
        blocks = new List<Block>();

        var blockData = Database.instance.blocks.Last();

        for (int y = 0; y < FIELD_SIZE_Y; y++)
        {
            for (int x = 0; x < FIELD_SIZE_X; x++)
            {
                if (y == 0 || y == FIELD_SIZE_Y - 1)
                {
                    blocks.Add(new Block(blockData, new Vector2Int(x, y), new Vector2(x, y)));
                    continue;
                }

                if ((y % 2) != 0)
                {
                    if (x == 0 || x == FIELD_SIZE_X - 1)
                    {
                        blocks.Add(new Block(blockData, new Vector2Int(x, y), new Vector2(x, y)));
                    }
                }
                else
                {
                    if ((x % 2) == 0)
                    {
                        blocks.Add(new Block(blockData, new Vector2Int(x, y), new Vector2(x, y)));
                    }
                }
            }
        }
    }

    protected override void InitBricks()
    {
        bricks = new List<Brick>();

        int count = Mathf.FloorToInt(FIELD_SIZE_X * FIELD_SIZE_Y - (blocks.Count + players.Count * (Constants.FIELD_CELL_SIZE * 3)));
        int bricksCount = Mathf.RoundToInt(count * (BRICKS_DENSITY / 100.0f));

        var brickData = Database.instance.bricks.First();

        while (bricksCount > 0)
        {
            int x = Random.Range(0, FIELD_SIZE_X);
            int y = Random.Range(0, FIELD_SIZE_Y);

            var pos = new Vector2(x, y);
            var coords = new Vector2Int(x, y);

            var cell = GetCellType(coords);
            if (cell != CellType.Empty)
                continue;

            if (players.Any(n => Vector2.Distance(n.pos, pos) < Constants.FIELD_CELL_SIZE * 2))
                continue;

            bricks.Add(new Brick(brickData, new Vector2Int(x, y), pos));
            bricksCount--;
        }
    }

    protected override void InitPowerUps()
    {
        powerUPs = new List<PowerUP>();

        int powerUpCount = Mathf.RoundToInt(bricks.Count * (POWERUP_DENSITY / 100.0f));

        var emptyBricks = new List<Brick>(bricks);

        while (powerUpCount > 0)
        {
            var brick = emptyBricks.Random();
            emptyBricks.Remove(brick);

            int powerUpNum = UnityEngine.Random.Range(0, Database.instance.powerUps.Count);
            var powerUpData = Database.instance.powerUps[powerUpNum];

            powerUPs.Add(new PowerUP(powerUpData, brick.coords, brick.pos));
            powerUpCount--;
        }
    }

    protected override void InitPlayers()
    {
        players = new List<Player>();

        var firstPlayerData = Database.instance.players[0];
        players.Add(new Player(0, firstPlayerData, new Vector2(FIELD_SIZE_X - 2, 1), false));

        var secondPlayerData = Database.instance.players[1];
        players.Add(new Player(2, secondPlayerData, new Vector2(FIELD_SIZE_X - 2, FIELD_SIZE_Y - 2), true));

        var thirdPlayerData = Database.instance.players[2];
        players.Add(new Player(1, thirdPlayerData, new Vector2(1, FIELD_SIZE_Y - 2), false));

        var fourthPlayerData = Database.instance.players[1];
        players.Add(new Player(3, fourthPlayerData, new Vector2(1, 1), true));
    }

    protected override void InitBombs()
    {
        bombs = new List<Bomb>();
    }

    protected override void InitCamera()
    {
        float x = FIELD_SIZE_X / 2 * -1;
        float z = FIELD_SIZE_Y / 2;
        float y = Mathf.Abs(x) + Mathf.Abs(z);

        Camera.main.transform.position = new Vector3(x, y, z);
    }

    #endregion

    #region Actions

    public override void MovePlayer(int id, Vector2 dir)
    {
        var player = players.Find(n => n.id.Equals(id));
        var nextPos = player.pos + (dir * player.moveSpeed * Time.deltaTime);

        //Works for spheres
        foreach (var block in blocks)
        {
            if (Vector3.Distance(block.pos, nextPos) < Constants.MOVE_COLLISION_DIST)
            {
                nextPos = block.pos + (nextPos - block.pos).normalized * Constants.MOVE_COLLISION_DIST;
            }
        }

        foreach (var brick in bricks)
        {
            if (Vector3.Distance(brick.pos, nextPos) < Constants.MOVE_COLLISION_DIST)
            {
                nextPos = brick.pos + (nextPos - brick.pos).normalized * Constants.MOVE_COLLISION_DIST;
            }
        }

        foreach (var bomb in bombs)
        {
            if (Vector3.Distance(bomb.pos, nextPos) < Constants.MOVE_COLLISION_DIST)
            {
                if (Vector3.Distance(bomb.pos, player.pos) <= Constants.MOVE_ON_BOMB_THRESHOLD)
                    continue;

                nextPos = bomb.pos + (nextPos - bomb.pos).normalized * Constants.MOVE_COLLISION_DIST;
            }
        }

        var powers = powerUPs.FindAll(n => Vector3.Distance(n.pos, nextPos) < Constants.PICK_UP_DIST);
        for (int i = 0; i < powers.Count; i++)
        {
            player.PickUpPowerUp(powers[i].data.type);
            powerUPs.Remove(powers[i]);

            Debug.Log($"Pick up {powers[i].data.type}");
            onPowerUPPicked?.Invoke(powers[i]);
        }

        player.pos = nextPos;
        //player.coords = nextPos.ToRoundInt();
        onMovePlayer?.Invoke(player);
    }

    public override void DeathPlayer(Player player)
    {
        players.Remove(player);
        onDeathPlayer?.Invoke(player);
    }

    public override void SpawnBomb(DBBomb dbBomb, Player owner, Vector2 pos)
    {
        int spawnedBombs = bombs.Count(n => n.owner.Equals(owner));
        if (spawnedBombs >= owner.bombsLimit)
        {
            return;
        }

        var cellType = GetCellType(pos.ToRoundInt());
        if (cellType != CellType.Empty && cellType != CellType.Player)
        {
            return;
        }

        var bomb = new Bomb(dbBomb, owner, pos.ToRoundInt(), pos.ToRoundInt()); //TODO: Get the nearest cell for bomb
        bombs.Add(bomb);
        onBombSpawned?.Invoke(bomb);
    }

    public override void RemoveBomb(Bomb bomb)
    {
        onBombRemoved?.Invoke(bomb);
    }

    public override void ExplodeBomb(Bomb bomb)
    {
        bombs.Remove(bomb);
        onBombRemoved?.Invoke(bomb);


        //Center
        onExplosion?.Invoke(ExplosionType.Basic, bomb.coords);
        HandleDestruction(bomb.coords);


        //Left
        for (int x = bomb.coords.x - 1; x > bomb.coords.x - bomb.power; x--)
        {
            var coords = new Vector2Int(x, bomb.coords.y);
            if (HandleDestruction(coords))
                break;
        }

        //Right
        for (int x = bomb.coords.x + 1; x < bomb.coords.x + bomb.power; x++)
        {
            var coords = new Vector2Int(x, bomb.coords.y);
            if (HandleDestruction(coords))
                break;
        }

        //Top
        for (int y = bomb.coords.y + 1; y < bomb.coords.y + bomb.power; y++)
        {
            var coords = new Vector2Int(bomb.coords.x, y);
            if (HandleDestruction(coords))
                break;
        }

        //Bot
        for (int y = bomb.coords.y - 1; y > bomb.coords.y - bomb.power; y--)
        {
            var coords = new Vector2Int(bomb.coords.x, y);
            if (HandleDestruction(coords))
                break;
        }
    }

    #endregion

    #region Tools

    public override CellType GetCellType(Vector2Int coords)
    {
        if (coords.x < 0 || coords.x >= FIELD_SIZE_X ||
            coords.y < 0 || coords.y >= FIELD_SIZE_Y)
        {
            return CellType.Outside;
        }

        if (bombs.Any(n => n.coords.Equals(coords)))
            return CellType.Bomb;

        if (players.Any(n => Vector2.Distance(n.pos, coords) < Constants.EXPLOSION_AFFECT_DIST))
            return CellType.Player;

        if (blocks.Any(n => n.coords.Equals(coords)))
            return CellType.Block;

        if (bricks.Any(n => n.coords.Equals(coords)))
            return CellType.Brick;

        if (powerUPs.Any(n => n.coords.Equals(coords)))
            return CellType.PowerUp;

        return CellType.Empty;
    }

    public override bool HandleDestruction(Vector2Int coords)
    {
        var type = GetCellType(coords);

        switch (type)
        {
            case CellType.Bomb:
                var bomb = bombs.Find(n => n.pos.Equals(coords));
                bomb.SetReady();
                return true;

            case CellType.Brick:
                var brick = bricks.Find(n => n.pos.Equals(coords));
                bricks.Remove(brick);
                onBrickDestroyed?.Invoke(brick);
                return true;

            case CellType.Player:
                var player = players.Find(n => Vector2.Distance(n.pos, coords) < Constants.EXPLOSION_AFFECT_DIST);
                players.Remove(player);
                onDeathPlayer?.Invoke(player);
                onExplosion?.Invoke(ExplosionType.Basic, coords);
                return false;

            case CellType.PowerUp:
                var powerUp = powerUPs.Find(n => n.pos.Equals(coords));
                powerUPs.Remove(powerUp);
                onPowerUPDestroyed?.Invoke(powerUp);
                return false;

            case CellType.Empty:
                onExplosion?.Invoke(ExplosionType.Basic, coords);
                return false;

            default:

                return true;
        }
    }

    #endregion
}
