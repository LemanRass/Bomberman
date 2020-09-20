using System.Collections.Generic;
using UnityEngine;

public class BombView : MonoBehaviour
{
    [HideInInspector]
    public Bomb bomb;

    public float minScale;
    public float maxScale;


    public GameObject firePrefab;
    private GameObject fireObject;
    public List<GameObject> bubochki = new List<GameObject>();

    public float pulseSpeed = 5.0f;
    private int lastIndex = -1;

    public void Init(Bomb bomb)
    {
        this.bomb = bomb;
        transform.localPosition = new Vector3(-bomb.pos.x, 0.1f, bomb.pos.y);

        //GFX
        fireObject = Instantiate<GameObject>(firePrefab);
        fireObject.transform.SetParent(bubochki[0].transform);
    }

    private void Update()
    {
        //[Pulse scale]
        float scale = minScale + Mathf.PingPong(Time.time * pulseSpeed, maxScale - minScale);
        transform.localScale = new Vector3(scale, scale, scale);

        //[0 -> 1]
        float progress = Mathf.Clamp01((Time.time - bomb.spawnTimestamp) / (bomb.explosionTimestamp - bomb.spawnTimestamp));

        //Bubochka index
        int index = (bubochki.Count - 1) - Mathf.RoundToInt(progress * (bubochki.Count - 1));

        if(lastIndex != index)
        {
            bubochki[index].gameObject.SetActive(false);
            lastIndex = index;
            if (index != 0)
            {
                fireObject.transform.SetParent(bubochki[index - 1].transform);
                fireObject.transform.Reset();
            }
        }
    }
}
