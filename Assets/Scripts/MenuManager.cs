using UnityEditor.SearchService;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;
    public string sceneNameToLoad;

    private void Start()
    {
        startButton.onClick.AddListener(ChangeScene);
        DontDestroyOnLoad(gameObject);
    }

    private void QuitGame()
    {
        if (quitButton)
        {
            Application.Quit();
        }
    }

    private void ChangeScene()
    {
        if (startButton)
        {
            SceneManager.LoadScene(sceneNameToLoad);
        }
    }
}
