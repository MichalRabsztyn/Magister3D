using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    #region Instance
    private static ScoreManager m_Instance;
    public static ScoreManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<ScoreManager>();
                if (m_Instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(ScoreManager).Name;
                    m_Instance = obj.AddComponent<ScoreManager>();
                }
            }
            return m_Instance;
        }
    }
    #endregion

    #region PublicVariables
    public int score = 0;
    public int highScore = 0;
    public int pointsPerFrame = 0;
    public int coinCount = 0;
    public int maxCoins = 0;
    public int attempts = 0;
    #endregion

    #region ScoreManagerMethods
    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UIManager.Instance.UpdateHighScore(highScore);
        score = 0;

        attempts = FileWriter.Instance.GetLastAttemptIndex();
    }

    private void FixedUpdate()
    {
        if (!PlayerStateManager.Instance.HasBegunPlay() || PlayerStateManager.Instance.IsDead())
        {
            return;
        }
       
        AddScore(pointsPerFrame);
    }

    public void AddScore(int value)
    {
        score += value;

        UpdateScoreWidget();
    }

    public void SetScore(int value)
    {
        score = value;

        if (score > highScore)
        {
            highScore = score;
            SaveHighScore();
        }

        UpdateScoreWidget();
    }

    public void UpdateScoreWidget()
    {
        UIManager.Instance.UpdateScore(score);
    }

    public void ResetScore()
    {
        score = 0;

        UpdateScoreWidget();
    }

    public void SaveHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            UIManager.Instance.UpdateHighScore(highScore);
        }
    }

    public void AddCoin()
    {
        coinCount++;
        UIManager.Instance.UpdateCoinCount(coinCount);
    }

    public void ResetCoinCount()
    {
        coinCount = 0;
        UIManager.Instance.UpdateCoinCount(coinCount);
    }

    public void SaveMaxCoins()
    {
        if (coinCount > maxCoins)
        {
            maxCoins = coinCount;
            PlayerPrefs.SetInt("MaxCoins", maxCoins);
        }
    }

    public void IncrementAttempts()
    {
        attempts++;
    }

    public void SaveAttempt()
    {
        FileWriter.Instance.WriteValuesToFile(attempts, score, maxCoins);
    }

    #endregion
}
