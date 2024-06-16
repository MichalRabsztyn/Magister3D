using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    private static PlayerStateManager m_Instance;

    [SerializeField]
    private bool m_InstantRespawn = false;
    [SerializeField]
    public bool m_InstantStart = false;

    private bool m_HasBegunPlay = false;
    private bool m_bIsDead = false;

    public delegate void BeginPlayHandler();
    public event BeginPlayHandler OnBeginPlay;

    public static PlayerStateManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<PlayerStateManager>();
                if (m_Instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(PlayerStateManager).Name;
                    m_Instance = obj.AddComponent<PlayerStateManager>();
                }
            }
            return m_Instance;
        }
    }

    void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void BeginPlay()
    { 
        m_HasBegunPlay = true;
        m_bIsDead = false;
        OnBeginPlay?.Invoke();
    }

    public void EndPlay() { m_HasBegunPlay = false; }

    public void KillPlayer() { m_bIsDead = true; }

    public void RevivePlayer() { m_bIsDead = false; }

    public bool HasBegunPlay() { return m_HasBegunPlay; }

    public bool IsDead() { return m_bIsDead; }

    public void Die()
    {
        EndPlay();
        KillPlayer();
        if (m_InstantRespawn)
        {
            BeginPlay();
        }
        else
        {
            UIManager.Instance.ShowGameOverPanel();
        }
        
        ScoreManager.Instance.SaveHighScore();
        ScoreManager.Instance.SaveMaxCoins();
        ScoreManager.Instance.SaveAttempt();
        WorldManager.Instance.ResetWorld();
    }
}
