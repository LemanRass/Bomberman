using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings instance { get; private set; }
    private void Awake() => instance = this;

    public KeyMapSetting[] playersKeyMap;
}
