using UnityEngine;
using UnityEngine.InputSystem;

public class MissileManager : MonoBehaviour
{
    [SerializeField]
    private GameObject missilePrefab;

    [SerializeField]
    private Transform firePoint;

    public int poolSize = 1;
    private GameObject[] missilePool;
    private int currentMissileIndex = 0;

    private InputSystem_Actions controls;

    [SerializeField]
    private float fireCooldown = 1f;
    private float lastFireTime = 0f;

    private void Awake()
    {
        controls = new InputSystem_Actions();

        controls.Player.Fire.performed += ctx => OnFire(ctx);
    }

    private void Start()
    {
        missilePool = new GameObject[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            missilePool[i] = Instantiate(missilePrefab);
            missilePool[i].SetActive(false);
        }
    }

    private void OnFire(InputAction.CallbackContext ctx)
{
    if (MenuPause.IsPaused) return;
    if (ctx.performed && !GameManager.Instance.IsPaused && !GameManager.Instance.isExploding)
    {
        if(Time.time - lastFireTime < fireCooldown) return;
        lastFireTime = Time.time;

        for (int i = 0; i < poolSize; i++)
        {
            int index = (currentMissileIndex + i) % poolSize;

            if (!missilePool[index].activeSelf)
            {
                missilePool[index].transform.position = firePoint.position;
                missilePool[index].transform.rotation = firePoint.rotation;
                missilePool[index].SetActive(true);

                PlayerShotCounter.Instance.RegisterShot();

                currentMissileIndex = (index + 1) % poolSize;
                return;
            }
        }

        Debug.Log("Aucun missile disponible !");
    }
}

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }
}
