using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    #region Instance
    private static GameManager m_Instance;

    public static GameManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<GameManager>();
                if (m_Instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(GameManager).Name;
                    m_Instance = obj.AddComponent<GameManager>();
                }
            }
            return m_Instance;
        }
    }
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

    private void Start()
    {
        if(PlayerStateManager.Instance.m_InstantStart)
        {
            StartGame();
        }
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        UIManager.Instance.HideStartPanel();
        UIManager.Instance.HideGameOverPanel();
        ScoreManager.Instance.ResetScore();
        ScoreManager.Instance.ResetCoinCount();
        ScoreManager.Instance.IncrementAttempts();
        PlayerStateManager.Instance.BeginPlay();
    }
}
