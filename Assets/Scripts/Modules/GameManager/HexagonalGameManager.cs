﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexagonalGameManager : GameManager
{
    public const int maxSize = 11;
    public const int minSize = 6;

    public const float verticalOffset = 0.78f;
    public const float horizontalOffset = 1.0f;


    public const float MOVE_COLLISION_DIST = 0.8f;

    protected override void Update()
    {
        if (bombs.Count > 0)
        {
            var forExplode = bombs.FindAll(n => n.IsReady());
            forExplode.ForEach(n => ExplodeBomb(n));
        }
    }

    #region Initialization

    public override void Init()
    {
        grounds = new Ground[(maxSize - minSize) * 2 + 1][];

        InitGround();

        InitBlocks();

        InitPlayers();

        InitBricks();



        /*InitPowerUps();

        InitBombs();

        InitCamera();*/

        onInitDone?.Invoke();
    }

    protected override void InitGround()
    {
        var groundData = Database.instance.grounds.Last();

        int rowsCount = (maxSize - minSize) * 2 + 1;

        int count = minSize;
        
        float oy = verticalOffset * (rowsCount / 2);

        for(int i = 0; i < rowsCount; i++)
        {
            float ox = (count - 1) * horizontalOffset / 2;

            grounds[i] = new Ground[count];

            for (int j = 0; j < count; j++)
            {
                var ground = new Ground(groundData, new Vector2Int(j, i), new Vector2(-j + ox, oy));
                grounds[i][j] = ground;
            }

            if (i >= rowsCount / 2)
            {
                count--;
                ox -= horizontalOffset / 2;

            }
            else
            {
                count++;
                ox += horizontalOffset / 2;

            }

            oy -= verticalOffset;
            //return;
        }

        for(int i = 0; i < grounds.Length; i++)
        {
            Debug.Log($"{grounds[i].Length}");
        }
    }

    protected override void InitBlocks()
    {
        blocks = new List<Block>();

        var blockData = Database.instance.blocks.Last();
        for(int x = 0; x < grounds.Length; x++)
        {
            for(int y = 0; y < grounds[x].Length; y++)
            {
                if(x == 0 || x == grounds.Length - 1)
                {
                    blocks.Add(new Block(blockData, new Vector2Int(x, y), grounds[x][y].pos));
                }
                else
                {
                    if (y == 0 || y == grounds[x].Length - 1)
                    {
                        blocks.Add(new Block(blockData, new Vector2Int(x, y), grounds[x][y].pos));
                    }
                }
            }
        }
    }

    protected override void InitBricks()
    {
        var brickData = Database.instance.bricks.Last();

        int count = grounds.Count();
        int bricksCount = Mathf.RoundToInt(count * (BRICKS_DENSITY / 100.0f));

        while (bricksCount > 0)
        {
            var randomRow = grounds.Random();
            var ground = randomRow.Random();

            var cell = GetCellType(ground.coords);
            if (cell != CellType.Empty)
                continue;

            if (players.Any(n => Vector2.Distance(n.pos, ground.pos) <= 1.0f))
                continue;

            bricks.Add(new Brick(brickData, ground.coords, ground.pos));
            bricksCount--;
        }
    }

    protected override void InitPowerUps() { }

    protected override void InitPlayers()
    {
        players = new List<Player>();

        var firstPlayerData = Database.instance.players[0];
        players.Add(new Player(0, firstPlayerData, grounds[1][1].pos, false));

        var secondPlayerData = Database.instance.players[1];
        players.Add(new Player(2, secondPlayerData, grounds[grounds.Length - 2][1].pos, true));

        var thirdPlayerData = Database.instance.players[2];
        players.Add(new Player(1, thirdPlayerData, grounds[1][grounds[1].Length - 2].pos, false));

        var fourthPlayerData = Database.instance.players[1];
        players.Add(new Player(3, fourthPlayerData, grounds[grounds.Length - 2][grounds[grounds.Length - 2].Length - 2].pos, true));
    }

    protected override void InitBombs()
    {
        bombs = new List<Bomb>();
    }

    protected override void InitCamera() { }

    #endregion



    #region Actions

    public override void MovePlayer(int id, Vector2 dir)
    {
        var player = players.Find(n => n.id.Equals(id));
        var nextPos = player.pos + (dir * player.moveSpeed * Time.deltaTime);

        foreach (var block in blocks)
        {
            if (Vector3.Distance(block.pos, nextPos) < MOVE_COLLISION_DIST)
            {
                nextPos = block.pos + (nextPos - block.pos).normalized * MOVE_COLLISION_DIST;
            }
        }

        foreach (var brick in bricks)
        {
            if (Vector3.Distance(brick.pos, nextPos) < MOVE_COLLISION_DIST)
            {
                nextPos = brick.pos + (nextPos - brick.pos).normalized * MOVE_COLLISION_DIST;
            }
        }

        player.pos = nextPos;
        onMovePlayer?.Invoke(player);
    }

    public override void DeathPlayer(Player player) { }

    public override void SpawnBomb(DBBomb dbBomb, Player owner, Vector2 pos)
    {
        int spawnedBombs = bombs.Count(n => n.owner.Equals(owner));
        if (spawnedBombs >= owner.bombsLimit)
        {
            return;
        }

        //var cellType = GetCellType(pos.ToRoundInt());
        //if (cellType != CellType.Empty && cellType != CellType.Player)
        //{
        //    return;
        //}

        var bomb = new Bomb(dbBomb, owner, pos.ToRoundInt(), pos); //TODO: Get the nearest cell for bomb
        bombs.Add(bomb);
        onBombSpawned?.Invoke(bomb);
    }

    public override void RemoveBomb(Bomb bomb)
    {
        bombs.Remove(bomb);
        onBombRemoved?.Invoke(bomb);
    }

    public override void ExplodeBomb(Bomb bomb)
    {
        RemoveBomb(bomb);
    }

    #endregion



    #region Tools

    public override CellType GetCellType(Vector2Int coords)
    {
        

        if (bricks.Any(n => n.coords.Equals(coords)))
            return CellType.Brick;

        if (players.Any(n => Vector2.Distance(n.pos, coords) < Constants.EXPLOSION_AFFECT_DIST))
            return CellType.Player;



        return CellType.Empty;
    }

    public override bool HandleDestruction(Vector2Int coords)
    {
        return false;
    }

    #endregion
}