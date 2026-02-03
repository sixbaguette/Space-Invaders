using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject Player;

    public bool IsPaused = false;
    public bool isExploding = false;

    private InputSystem_Actions controls;

    [SerializeField]
    private int score = 0;

    private int lives = 3;

    public int highScore = 0;

    [SerializeField]
    private TextMeshProUGUI scoreUI;
    [SerializeField]
    private TextMeshProUGUI hightScoreUI;

    [SerializeField]
    private GameObject[] livesSprite;

    [SerializeField]
    private TextMeshProUGUI livesCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GetHighScore();
        //ResetScore();
        livesCount.text = lives.ToString();
    }

    public void AddScore(int points)
    {
        score += points;
        if (score.ToString().Length <= 2)
        {
            scoreUI.text = "00" + score.ToString();
            return;
        }

        if (score.ToString().Length == 3)
        {
            scoreUI.text = "0" + score.ToString();
            return;
        }

        scoreUI.text = score.ToString();
    }


    public void LoseLives()
    {
        lives--;
        if (lives == 0)
        {
            StartCoroutine(GameObject.Find("Player").GetComponent<PlayerScript>().PlayDeathAnimation());

            if (GameObject.Find("Player").GetComponent<PlayerScript>().IsPlaying == false)
            {
                GameOver();
            }
        }

        if (lives == 3)
        {
            livesSprite[0].SetActive(true);
            livesSprite[1].SetActive(true);
        }
        else if (lives == 2)
        {
            livesSprite[1].SetActive(false);
            StartCoroutine(GameObject.Find("Player").GetComponent<PlayerScript>().PlayDeathAnimation());
        }
        else if (lives == 1)
        {
            livesSprite[0].SetActive(false);
            StartCoroutine(GameObject.Find("Player").GetComponent<PlayerScript>().PlayDeathAnimation());
        }

        livesCount.text = lives.ToString();
    }

    public void GameOver()
    {
        SaveScore();
        Destroy(Player);
        Debug.Break();
    }

    public void CompletedLevel()
    {

    }

    private void SaveScore()
    {
        if (score < highScore) return;

        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save();
    }

    private void ResetScore()
    {
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.Save();
    }

    private void GetHighScore()
    {
        highScore = PlayerPrefs.GetInt("Score", score);

        if (highScore.ToString().Length <= 1)
        {
            hightScoreUI.text = "000" + highScore.ToString();
            return;
        }

        if (highScore.ToString().Length <= 2)
        {
            hightScoreUI.text = "00" + highScore.ToString();
            return;
        }

        if (highScore.ToString().Length == 3)
        {
            hightScoreUI.text = "0" + highScore.ToString();

            return;
        }
        hightScoreUI.text = highScore.ToString();
    }
}