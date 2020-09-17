using UnityEngine;

public class BombView : MonoBehaviour
{
    [HideInInspector]
    public Bomb bomb;

    public void Init(Bomb bomb)
    {
        this.bomb = bomb;
    }
}
