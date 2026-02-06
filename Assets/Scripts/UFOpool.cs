using UnityEngine;

public class UFOPool : MonoBehaviour
{
    [SerializeField] private GameObject ufoPrefab;

    private GameObject ufoInstance;

    private void Awake()
    {
        if (ufoPrefab == null)
        {
            Debug.LogError("UFOPool : ufoPrefab n'est pas assigné !");
            return;
        }

        ufoInstance = Instantiate(ufoPrefab);
        ufoInstance.SetActive(false);

        UFOController controller = ufoInstance.GetComponent<UFOController>();
        if (controller != null && controller.ufoSound == null)
        {
            AudioSource source = ufoInstance.GetComponent<AudioSource>();
            if (source != null)
                controller.ufoSound = source;
            else
                Debug.LogWarning("UFOPool : aucun AudioSource trouvé pour le UFO !");
        }
    }

    public GameObject SpawnUFO(Vector3 position)
    {
        if (ufoInstance == null)
        {
            Debug.LogWarning("UFOPool : ufoInstance est null !");
            return null;
        }

        if (ufoInstance.activeSelf)
            return null;

        ufoInstance.transform.position = position;
        ufoInstance.SetActive(true);
        return ufoInstance;
    }

    public void DespawnUFO()
    {
        if (ufoInstance != null)
            ufoInstance.SetActive(false);
    }
}