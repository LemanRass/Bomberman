using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexagonalGameManager : GameManager
{
    public const int maxSize = 11;
    public const int minSize = 6;

    public const float verticalOffset = 0.78f;
    public const float horizontalOffset = 1.0f;


    

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

        InitPlayers();

        InitBricks();


        /*InitBlocks();

        InitPowerUps();

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
        players.Add(new Player(0, firstPlayerData, grounds[0][0].pos, false));

        var secondPlayerData = Database.instance.players[1];
        players.Add(new Player(2, secondPlayerData, grounds[grounds.Length - 1][0].pos, true));

        var thirdPlayerData = Database.instance.players[2];
        players.Add(new Player(1, thirdPlayerData, grounds[0][grounds[0].Length - 1].pos, false));

        var fourthPlayerData = Database.instance.players[1];
        players.Add(new Player(3, fourthPlayerData, grounds[grounds.Length - 1][grounds[grounds.Length - 1].Length - 1].pos, true));
    }

    protected override void InitBombs() { }

    protected override void InitCamera() { }

    #endregion



    #region Actions

    public override void MovePlayer(int id, Vector2 dir) { }

    public override void DeathPlayer(Player player) { }

    public override void SpawnBomb(DBBomb dbBomb, Player owner, Vector2 pos) { }

    public override void RemoveBomb(Bomb bomb) { }

    public override void ExplodeBomb(Bomb bomb) { }

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
