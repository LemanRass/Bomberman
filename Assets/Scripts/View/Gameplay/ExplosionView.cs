using UnityEngine;

public class ExplosionView : MonoBehaviour
{
    public GameObject prefab;
    public float duration;

    private GameObject explosion;

    public void Explode(Vector2 pos)
    {
        transform.localPosition = new Vector3(-pos.x, 0, pos.y);
        explosion = Instantiate<GameObject>(prefab, transform);
        Invoke("SelfDestroy", duration);
    }

    private void SelfDestroy()
    {
        Destroy(explosion);
        Destroy(gameObject);
    }
}
