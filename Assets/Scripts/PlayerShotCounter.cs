using UnityEngine;

public class PlayerShotCounter : MonoBehaviour
{
    public static PlayerShotCounter Instance;

    public int TotalShots { get; private set; }

    public event System.Action<int> RegisterShotEvent;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterShot()
    {
        TotalShots++;

        RegisterShotEvent?.Invoke(TotalShots);
    }
}