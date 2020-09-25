using UnityEngine;

public class GroundView : MonoBehaviour
{
    public Ground ground;

    public void Init(Ground ground)
    {
        this.ground = ground;

        float scaleX = Constants.FIELD_SIZE_X;
        float scaleZ = Constants.FIELD_SIZE_Y;
        transform.localScale = new Vector3(scaleX, 0.1f, scaleZ);

        float posX = (scaleX - 1.0f) / 2 * -1.0f;
        float posZ = (scaleZ - 1.0f) / 2;
        transform.localPosition = new Vector3(posX, 0, posZ);
    }
}
