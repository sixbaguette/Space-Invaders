using UnityEngine;

public class UFOPool : MonoBehaviour
{
    [SerializeField] private GameObject ufoPrefab;
    private GameObject ufoInstance;

    private void Awake()
    {
        if (ufoPrefab == null)
        {
            Debug.LogError("ufoPrefab pas assigné");
            return;
        }

        ufoInstance = Instantiate(ufoPrefab);
        ufoInstance.SetActive(false);
    }

    public GameObject SpawnUFO(Vector3 position)
    {
        if (ufoInstance == null) return null;
        if (ufoInstance.activeSelf) return null;

        ufoInstance.transform.position = position;

        UFOController controller = ufoInstance.GetComponent<UFOController>();
        controller.Initialize(PlayerShotCounter.Instance != null ? PlayerShotCounter.Instance.TotalShots : 0);

        ufoInstance.SetActive(true);
        return ufoInstance;
    }

    public void DespawnUFO()
    {
        if (ufoInstance != null)
            ufoInstance.SetActive(false);
    }
}