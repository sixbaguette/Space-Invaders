using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuPause : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;

    private InputSystem_Actions controls;
    public static bool IsPaused { get; private set; }

    [SerializeField] private string sceneNameToLoad;

    private bool restart = false;

    private void Awake()
    {
        controls = new InputSystem_Actions();
        controls.UI.Pause.performed += _ => TogglePause();
    }

    private void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void TogglePause()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }

    private void Pause()
    {
        IsPaused = true;

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void Resume()
    {
        IsPaused = false;

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(sceneNameToLoad);
        restart = true;
        if (restart)
        {
            IsPaused = false;
            restart = false;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
