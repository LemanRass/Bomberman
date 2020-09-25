using UnityEngine;

public class Player
{
    public int id;
    public DBPlayer data;
    public Vector2 pos;
    public bool isAI;

    public Player(int id, DBPlayer data, Vector2 pos, bool isAI)
    {
        this.id = id;
        this.data = data;
        this.pos = pos;
        this.isAI = isAI;
    }
}
