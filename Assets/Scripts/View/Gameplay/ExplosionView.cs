using UnityEngine;

public class ExplosionView : MonoBehaviour
{
    public GameObject prefab;
    public float duration;

    private GameObject explosion;

    public void Explode(int power)
    {
        explosion = Instantiate<GameObject>(prefab, transform);
        Invoke("SelfDestroy", duration);
    }

    private void SelfDestroy()
    {
        Destroy(explosion);
        Destroy(gameObject);
    }
}
