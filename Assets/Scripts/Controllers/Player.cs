using UnityEngine;

public class Player
{
    public int id;
    public Vector2 pos;
    public bool isLocal;

    public Player(int id, Vector2 pos, bool isLocal)
    {
        this.id = id;
        this.pos = pos;
        this.isLocal = isLocal;
    }
}
