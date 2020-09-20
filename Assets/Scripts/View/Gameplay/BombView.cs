using UnityEngine;

public class BombView : MonoBehaviour
{
    [HideInInspector]
    public Bomb bomb;

    public float minScale;
    public float maxScale;

    public float pulseSpeed = 5.0f;

    public void Init(Bomb bomb)
    {
        this.bomb = bomb;
        transform.localPosition = new Vector3(-bomb.pos.x, 0, bomb.pos.y);
    }

    private void Update()
    {
        float scale = minScale + Mathf.PingPong(Time.time, maxScale - minScale);
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
