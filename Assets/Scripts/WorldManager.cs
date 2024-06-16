using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
	#region Instance
	private static WorldManager m_Instance;
	public static WorldManager Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = FindObjectOfType<WorldManager>();
				if (m_Instance == null)
				{
					GameObject obj = new GameObject();
					obj.name = typeof(WorldManager).Name;
					m_Instance = obj.AddComponent<WorldManager>();
				}
			}
			return m_Instance;
		}
	}
	#endregion

	[SerializeField]
	private float m_ChunkLength = 100.0f;
	[SerializeField]
	private int m_ChunksToSpawn = 5;
	[SerializeField]
	private GameObject StartChunkInstance;
	[SerializeField]
	private GameObject StartChunkPrefab;
	[SerializeField]
	private GameObject[] m_ChunkPrefabs;

	private Vector3 m_StartChunkPosition;
	private int m_CurrentChunkCount = 1;
	private List<GameObject> m_CurrentChunks = new List<GameObject>();
	private GameObject m_Player;
	private float m_SpawnNewChunkDistance = 0.0f;

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

		m_StartChunkPosition = StartChunkInstance.transform.position;
	}

	void Start()
	{
		m_Player = GameObject.FindGameObjectWithTag("Player");
		m_CurrentChunks.Add(StartChunkInstance);

		if (m_ChunkPrefabs.Length > 0)
		{
			for (int i = 0; i < m_ChunksToSpawn - 1; i++)
			{
				CreateChunk();
			}
		}
	}

	void Update()
	{
		if (m_Player.transform.position.z >= m_SpawnNewChunkDistance)
		{
			CreateChunk();
		}
	}

	public void CreateChunk()
	{
		Vector3 instantiationPosition = m_StartChunkPosition + Vector3.forward * m_ChunkLength * m_CurrentChunkCount;
		m_CurrentChunks.Add(Instantiate(m_ChunkPrefabs[Random.Range(0, m_ChunkPrefabs.Length)], instantiationPosition, Quaternion.identity));
		m_CurrentChunkCount++;
		m_SpawnNewChunkDistance = instantiationPosition.z - m_ChunkLength * (m_ChunksToSpawn - 2);

		if (m_CurrentChunks.Count > m_ChunksToSpawn)
		{
			DestroyOldestChunk();
		}
	}

	public void DestroyOldestChunk()
	{
		Destroy(m_CurrentChunks[0]);
		m_CurrentChunks.RemoveAt(0);
	}

	public void ResetWorld()
	{
		foreach (GameObject chunk in m_CurrentChunks)
		{
			Destroy(chunk);
		}

		StartChunkInstance = Instantiate(StartChunkPrefab, m_StartChunkPosition, Quaternion.identity);

		m_CurrentChunks.Clear();
		m_CurrentChunkCount = 1;
		m_SpawnNewChunkDistance = 0.0f;
		m_CurrentChunks.Add(StartChunkInstance);

		ResetCoins();
	}

	public void ResetCoins()
	{
		foreach (Coin coin in FindObjectsOfType<Coin>())
		{
			coin.SetEnabled(true);
		}
	}
}
