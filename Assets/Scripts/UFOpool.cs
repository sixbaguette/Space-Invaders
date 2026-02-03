using UnityEngine;

public class UFOpool : MonoBehaviour
{
    [SerializeField]
    private GameObject UFOPrefab;

    public int poolSize = 1;
    private GameObject UFOPool;

    private void Start()
    {
        UFOPool = Instantiate(UFOPrefab);
        UFOPool.SetActive(false);
    }

    private void OnUFOSpawn()
    {
        if (MenuPause.IsPaused) return;
        if (!GameManager.Instance.IsPaused && !GameManager.Instance.isExploding)
        {
            if (!UFOPool.activeSelf)
            {
                UFOPool.SetActive(true);

                return;
            }
        }
    }
}
