using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuadricGameManager : GameManager
{
    protected override void Start()
    {
        Debug.Log($"HEllo");

        Init();
        onInitDone?.Invoke();
    }

    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        if (bombs.Count > 0)
        {
            var forExplode = bombs.FindAll(n => n.IsReady());
            forExplode.ForEach(n => ExplodeBomb(n));
        }
    }

    #region Init

    protected override void Init()
    {
        InitGround();

        InitBlocks();

        InitPlayers();

        InitBricks();

        InitPowerUps();

        InitBombs();

        InitCamera();
    }

    protected override void InitGround()
    {
        grounds = new List<Ground>();

        var groundData = Database.instance.grounds.Last();

        for(int x = 0; x < Constants.FIELD_SIZE_X; x++)
        {
            for(int y = 0; y < Constants.FIELD_SIZE_Y; y++)
            {
                grounds.Add(new Ground(groundData, new Vector2(x, y)));
            }
        }

        Debug.Log("Init ground done!");
    }

    protected override void InitBlocks()
    {
        blocks = new List<Block>();

        var blockData = Database.instance.blocks.Last();

        for (int y = 0; y < Constants.FIELD_SIZE_Y; y++)
        {
            for (int x = 0; x < Constants.FIELD_SIZE_X; x++)
            {
                if (y == 0 || y == Constants.FIELD_SIZE_Y - 1)
                {
                    blocks.Add(new Block(blockData, new Vector2(x, y)));
                    continue;
                }

                if ((y % 2) != 0)
                {
                    if (x == 0 || x == Constants.FIELD_SIZE_X - 1)
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

    protected override void InitBricks()
    {
        bricks = new List<Brick>();

        int count = Mathf.FloorToInt(Constants.FIELD_SIZE_X * Constants.FIELD_SIZE_Y - (blocks.Count + players.Count * (Constants.FIELD_CELL_SIZE * 3)));
        int bricksCount = Mathf.RoundToInt(count * (BRICKS_DENSITY / 100.0f));

        var brickData = Database.instance.bricks.First();

        while (bricksCount > 0)
        {
            int x = UnityEngine.Random.Range(0, Constants.FIELD_SIZE_X);
            int y = UnityEngine.Random.Range(0, Constants.FIELD_SIZE_Y);
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

            powerUPs.Add(new PowerUP(powerUpData, brick.pos));
            powerUpCount--;
        }
    }

    protected override void InitPlayers()
    {
        players = new List<Player>();

        var firstPlayerData = Database.instance.players[0];
        players.Add(new Player(0, firstPlayerData, Constants.botLeftSpawnPoint, false));

        var secondPlayerData = Database.instance.players[1];
        players.Add(new Player(2, secondPlayerData, Constants.topLeftSpawnPoint, true));

        var thirdPlayerData = Database.instance.players[2];
        players.Add(new Player(1, thirdPlayerData, Constants.topRightSpawnPoint, false));

        var fourthPlayerData = Database.instance.players[1];
        players.Add(new Player(3, fourthPlayerData, Constants.botRightSpawnPoint, true));
    }

    protected override void InitBombs()
    {
        bombs = new List<Bomb>();
    }

    protected override void InitCamera()
    {
        float x = Constants.FIELD_SIZE_X / 2 * -1;
        float z = Constants.FIELD_SIZE_Y / 2;
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

        var cellType = GetCellType(pos);
        if (cellType != CellType.Empty && cellType != CellType.Player)
        {
            return;
        }

        var bomb = new Bomb(dbBomb, owner, pos);
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
        onExplosion?.Invoke(ExplosionType.Basic, bomb.pos);
        HandleDestruction(bomb.pos);


        //Left
        for (int x = bomb.pos.ToRoundInt().x - 1; x > bomb.pos.ToRoundInt().x - bomb.power; x--)
        {
            var pos = new Vector2(x, bomb.pos.y);
            if (HandleDestruction(pos))
                break;
        }

        //Right
        for (int x = bomb.pos.ToRoundInt().x + 1; x < bomb.pos.ToRoundInt().x + bomb.power; x++)
        {
            var pos = new Vector2(x, bomb.pos.y);
            if (HandleDestruction(pos))
                break;
        }

        //Top
        for (int y = bomb.pos.ToRoundInt().y + 1; y < bomb.pos.ToRoundInt().y + bomb.power; y++)
        {
            var pos = new Vector2(bomb.pos.x, y);
            if (HandleDestruction(pos))
                break;
        }

        //Bot
        for (int y = bomb.pos.ToRoundInt().y - 1; y > bomb.pos.ToRoundInt().y - bomb.power; y--)
        {
            var pos = new Vector2(bomb.pos.x, y);
            if (HandleDestruction(pos))
                break;
        }
    }

    #endregion

    #region Tools

    public override CellType GetCellType(Vector2 pos)
    {
        var rounded = pos.ToRound();

        if (pos.x < 0 || pos.x >= Constants.FIELD_SIZE_X ||
            pos.y < 0 || pos.y >= Constants.FIELD_SIZE_Y)
        {
            return CellType.Outside;
        }

        if (bombs.Any(n => n.pos.ToRound().Equals(rounded)))
            return CellType.Bomb;

        if (players.Any(n => Vector2.Distance(n.pos, pos) < Constants.EXPLOSION_AFFECT_DIST))
            return CellType.Player;

        if (blocks.Any(n => n.pos.ToRound().Equals(rounded)))
            return CellType.Block;

        if (bricks.Any(n => n.pos.ToRound().Equals(rounded)))
            return CellType.Brick;

        if (powerUPs.Any(n => n.pos.ToRound().Equals(rounded)))
            return CellType.PowerUp;

        return CellType.Empty;
    }

    public override bool HandleDestruction(Vector2 pos)
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
                var powerUp = powerUPs.Find(n => n.pos.Equals(pos));
                powerUPs.Remove(powerUp);
                onPowerUPDestroyed?.Invoke(powerUp);
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
