using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Blade blade;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timerText;
    [SerializeField] private GameObject startPanel; // Start panel for play button
    [SerializeField] private GameObject menuPanel;   // End panel with final score
    [SerializeField] private Text finalScoreText;
    [SerializeField] private Text highScoreText;     // High score display in end panel
    [SerializeField] private Image fadeImage;

    [SerializeField] private AudioClip menusound; // Sound when fruit is sliced
    private AudioSource audioSource; // AudioSource to play the slice sound

    public int score { get; private set; } = 0;
    public float gameDuration = 60f; // Total game time in seconds

    private float timer;
    private bool gameStarted = false;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        timer = gameDuration;
        spawner.enabled = false;       // Disable spawner until game starts
        startPanel.SetActive(true);  
        scoreText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
           // Show start panel at beginning
        menuPanel.SetActive(false);     // Hide menu panel at start
        
        PlayerPrefs.DeleteKey("hiscore"); //New high score kept for each session. If removed permenantly saved. 
        UpdateHighScore();
    }

    private void Update()
    {
        if (gameStarted && timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.RoundToInt(timer).ToString();

            if (timer <= 0)
            {
                EndGame();
            }
        }
    }

    private void StartGame()
    {
        startPanel.SetActive(false); 
        menuPanel.SetActive(false);
        scoreText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true); // Hide start panel
        spawner.enabled = true;       // Enable spawner for game objects
        gameStarted = true;
        NewGame();
    }

    private void NewGame()
    {
        Time.timeScale = 1f;
        ClearScene();

        blade.enabled = true;
        spawner.enabled = true;

        score = 0;
        scoreText.text = score.ToString();
        timer = gameDuration; // Reset timer
    }

    private void ClearScene()
    {
        // Destroy all fruits and bombs
        foreach (Fruit fruit in FindObjectsOfType<Fruit>())
        {
            Destroy(fruit.gameObject);
        }
        foreach (Bomb bomb in FindObjectsOfType<Bomb>())
        {
            Destroy(bomb.gameObject);
        }
    }

    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();

        // Update high score if current score exceeds previous high score
        float highScore = PlayerPrefs.GetFloat("hiscore", 0);

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetFloat("hiscore", highScore);
        }
    }

    public void Explode()
    {
        blade.enabled = false;
        spawner.enabled = false;
        StartCoroutine(ExplodeSequence());
    }

    private IEnumerator ExplodeSequence()
{
    float elapsed = 0f;
    float duration = 0.5f;

    // Fade to white
    while (elapsed < duration)
    {
        float t = Mathf.Clamp01(elapsed / duration);
        fadeImage.color = Color.Lerp(Color.clear, Color.white, t);
        Time.timeScale = 1f - t;
        elapsed += Time.unscaledDeltaTime;
        yield return null;
    }

    yield return new WaitForSecondsRealtime(1f); // Pause briefly while fully white

    elapsed = 0f;
    // Fade back to clear
    while (elapsed < duration)
    {
        float t = Mathf.Clamp01(elapsed / duration);
        fadeImage.color = Color.Lerp(Color.white, Color.clear, t);
        elapsed += Time.unscaledDeltaTime;
        yield return null;
    }

    EndGame();
}


    private void EndGame()
    {
        audioSource.PlayOneShot(menusound);
        Time.timeScale = 0f;
        gameStarted = false;
        blade.enabled = false;
        spawner.enabled = false;

        finalScoreText.text = "Score: " + score.ToString();
        highScoreText.text = "High Score: " + PlayerPrefs.GetFloat("hiscore", 0).ToString();
        menuPanel.SetActive(true);  // Show menu panel on game end
    }

    private void UpdateHighScore()
    {
        highScoreText.text = "High Score: " + PlayerPrefs.GetFloat("hiscore", 0).ToString();
    }

    // Button methods
    public void RestartGame()
    {
        Time.timeScale = 1f;
        StartGame(); // Start new Game();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // Called by Play button on start panel
    public void OnPlayButtonPressed()
    {
        StartGame();
    }
}
