using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    #region Instance
    private static UIManager m_Instance;
    public static UIManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<UIManager>();
                if (m_Instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(UIManager).Name;
                    m_Instance = obj.AddComponent<UIManager>();
                }
            }
            return m_Instance;
        }
    }
    #endregion

    #region PublicVariables
    public GameObject startPanel;
    public GameObject gameOverPanel;
    public GameObject hudPanel;
    public TextMeshProUGUI scoreWidget;
    public TextMeshProUGUI highScoreWidget;
    public TextMeshProUGUI coinCountWidget;
    #endregion

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

    public void UpdateScore(int score)
    {
        scoreWidget.text = score.ToString("D7");
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        hudPanel.SetActive(false);
    }

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        hudPanel.SetActive(false);
    }

    public void HideGameOverPanel()
    {
        gameOverPanel.SetActive(false);
        hudPanel.SetActive(true);
    }

    public void HideStartPanel()
    {
        startPanel.SetActive(false);
        hudPanel.SetActive(true);
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

    public void UpdateHighScore(int score)
    {
        highScoreWidget.text = score.ToString("D7");
    }

    public void UpdateCoinCount(int count)
    {
        coinCountWidget.text = count.ToString("D7");
    }
    
    public void OnStartButtonClick()
    {
        GameManager.Instance.StartGame();
    }
}
