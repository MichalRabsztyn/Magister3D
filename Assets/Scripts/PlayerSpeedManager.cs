using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeedManager : MonoBehaviour
{
    #region Instance
    private static PlayerSpeedManager m_Instance;
    public static PlayerSpeedManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<PlayerSpeedManager>();
                if (m_Instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(PlayerSpeedManager).Name;
                    m_Instance = obj.AddComponent<PlayerSpeedManager>();
                }
            }
            return m_Instance;
        }
    }
    #endregion

    public float currentForwardMoveSpeed = 0.0f;
    public float accelerationPerFrame = 0.1f;
    public float startingSpeed = 0.0f;

    private Character m_Player;

    void Awake()
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

        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
        m_Player.forwardMoveSpeed = currentForwardMoveSpeed;

        PlayerStateManager.Instance.OnBeginPlay += SetStartingSpeed;
    }

    void Update()
    {
        if (!PlayerStateManager.Instance.HasBegunPlay() || PlayerStateManager.Instance.IsDead())
        {
            return;
        }

        if (m_Player.forwardMoveSpeed > 0.0f)
        {
            currentForwardMoveSpeed += accelerationPerFrame * Time.deltaTime;
            m_Player.forwardMoveSpeed = currentForwardMoveSpeed;
        }
    }

    public void ResetSpeed()
    {
        m_Player.forwardMoveSpeed = 0.0f;
    }

    public void SetStartingSpeed()
    {
        SetSpeed(startingSpeed);
    }

    public void SetSpeed(float speed)
    {
        currentForwardMoveSpeed = speed;
        m_Player.forwardMoveSpeed = speed;
    }
}
