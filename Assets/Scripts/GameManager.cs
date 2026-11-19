using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Основные ссылки")] public AudioSource theMusic;
    public BeatScroller theBS;
    public bool startPlaying;

    [Header("Очки и множитель")] public int currentScore;
    public int scorePerNote = 10;
    public int currentMultiplier = 1;
    public int multiplierTracker;
    public int[] multiplierThresholds = { 4, 8, 16 };

    [Header("Шкала Spirit")] public Slider spiritSlider;
    public float maxSpirit = 100f;
    public float currentSpirit = 50f;
    public float spiritGain = 5f;
    public float spiritLoss = 10f;

    [Header("UI во время игры")] public TMP_Text scoreText;
    public TMP_Text multiText;
    public TMP_Text timerText;

    [Header("Экран результатов (Видео #5)")]
    public GameObject gameOverScreen;

    public GameObject resultsScreen;
    public TMP_Text hitsText;
    public TMP_Text missesText;
    public TMP_Text finalScoreText;

    private int totalHits;
    private int totalMisses;
    private bool songEnded;
    public float songDuration = 71f;
    private float songTimer;

    void Start()
    {
        instance = this;
        currentMultiplier = 1;

        if (scoreText != null) scoreText.text = "Score: 0";
        if (multiText != null) multiText.text = "Multiplier: x1";

        if (spiritSlider != null)
        {
            spiritSlider.maxValue = maxSpirit;
            spiritSlider.value = currentSpirit;
        }

        if (resultsScreen != null) resultsScreen.SetActive(false);
    }

    void Update()
    {
        if (!startPlaying)
        {
            if (Input.anyKeyDown)
            {
                startPlaying = true;
                theBS.hasStarted = true;
                theMusic.Play();
            }
        }
        else if (!songEnded && !theMusic.isPlaying)
        {
            ShowResults();
        }
    }

    public void NoteHit()
    {
        totalHits++;

        if (currentMultiplier - 1 < multiplierThresholds.Length)
        {
            multiplierTracker++;
            if (multiplierThresholds[currentMultiplier - 1] <= multiplierTracker)
            {
                multiplierTracker = 0;
                currentMultiplier++;
            }
        }

        currentScore += scorePerNote * currentMultiplier;
        currentSpirit = Mathf.Clamp(currentSpirit + spiritGain, 0f, maxSpirit);

        UpdateUI();
    }
    public void NoteMissed()
    {
        totalMisses++;
        currentMultiplier = 1;
        multiplierTracker = 0;

        currentScore = Mathf.Max(currentScore - 30, -200); 
        currentSpirit = Mathf.Clamp(currentSpirit - spiritLoss, 0f, maxSpirit);

        UpdateUI();

        if (currentScore <= -200)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        songEnded = true;
        theMusic.Stop();
        theBS.hasStarted = false;

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
        else
        {
            ShowResults();
        }
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + currentScore;
        if (multiText != null) multiText.text = "Multiplier: x" + currentMultiplier;
    }

    void ShowResults()
    {
        void Update()
        {
            if (!startPlaying)
            {
                if (Input.anyKeyDown)
                {
                    startPlaying = true;
                    theBS.hasStarted = true;
                    theMusic.Play();
                }
            }
            else if (!songEnded)
            {
                songTimer += Time.deltaTime;
                float timeLeft = Mathf.Max(0, songDuration - songTimer);
                if (timerText != null)
                {
                    int minutes = Mathf.FloorToInt(timeLeft / 60);
                    int seconds = Mathf.FloorToInt(timeLeft % 60);
                    timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                }

                if (!theMusic.isPlaying || (songDuration > 0 && songTimer >= songDuration))
                {
                    ShowResults();
                }
            }
        }
    }
}