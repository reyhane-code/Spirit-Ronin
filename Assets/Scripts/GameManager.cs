using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI[] comboMessages;
    [SerializeField] private GameObject[] lifeIcons;

    [Header("Game Settings")]
    [SerializeField] private int startLives = 3;
    [SerializeField] private float comboTimeWindow = 2f;
    [SerializeField] private float comboBonus = 0.2f;

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

    // ================= SCORE =================

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

    // ================= LIFE =================

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
        Debug.Log($"GAME OVER | Final Score: {score}");
        // TODO: show game over panel
    }

    // ================= DEBUG =================

    public void ResetGame()
    {
        InitializeGame();
    }


}
