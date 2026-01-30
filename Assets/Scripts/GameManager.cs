using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI[] comboMessages;
    public GameObject[] lifeIcons;
    public int startLives = 3;
    public float comboTimeWindow = 2f;
    public float comboBonus = 0.2f;
    public GameObject gameOverPanel;
    public float slowMotionScale = 0.15f;
    public float slowMotionDuration = 0.5f;
    private int score;
    private int lives;
    private int comboCount;
    private float lastScoreTime;
    private Coroutine comboCoroutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeGame();
    }

    private void InitializeGame()
    {
        score = 0;
        lives = startLives;
        comboCount = 0;
        lastScoreTime = -comboTimeWindow;

        UpdateScoreUI();
        UpdateLivesUI();
        HideCombo();
    }

    public void AddScore(int basePoints)
    {
        float timeDiff = Time.time - lastScoreTime;

        if (timeDiff <= comboTimeWindow)
            comboCount++;
        else
            comboCount = 0;

        lastScoreTime = Time.time;

        int finalPoints = CalculateComboScore(basePoints);
        score += finalPoints;

        UpdateScoreUI();
        ShowCombo(comboCount);
    }

    private int CalculateComboScore(int basePoints)
    {
        float multiplier = 1f + (comboCount * comboBonus);
        return Mathf.RoundToInt(basePoints * multiplier);
    }

    // ================= UI =================

    private void UpdateScoreUI()
    {
        if (scoreText == null) return;

        scoreText.text = score.ToString();

    }

    private void UpdateLivesUI()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
            lifeIcons[i].SetActive(i < lives);
    }

    private void ShowCombo(int combo)
    {
        if (combo < 1 || comboText == null) return;

        comboText.gameObject.SetActive(true);

        string comboNumber = combo.ToString();

        if (combo < comboMessages.Length)
            comboText.text = comboMessages[combo].text;
        else
            comboText.text = $"{comboNumber}X !";

        if (comboCoroutine != null)
            StopCoroutine(comboCoroutine);

        comboCoroutine = StartCoroutine(HideComboDelayed());
    }


    private IEnumerator HideComboDelayed()
    {
        yield return new WaitForSeconds(1f);
        HideCombo();
    }

    private void HideCombo()
    {
        if (comboText != null)
            comboText.gameObject.SetActive(false);
    }

    public void LoseLife()
    {
        lives--;
        comboCount = 0;
        HideCombo();
        UpdateLivesUI();

        if (lives <= 0)
            GameOver();
    }

    private void GameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // 1️⃣ Slow motion
        Time.timeScale = slowMotionScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(slowMotionDuration);

        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        gameOverPanel.SetActive(false);
        SceneManager.LoadScene("MainScene");

    }



}
