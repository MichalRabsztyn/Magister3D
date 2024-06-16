using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
	private void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			ScoreManager.Instance.AddCoin();
			GetComponent<ParticleSystem>()?.Play();

			SetEnabled(false);
		}
	}

	private void Update()
	{
		transform.Rotate(Vector3.up, 100 * Time.deltaTime);
	}

	private void OnEnable()
	{
		transform.Rotate(Vector3.up, Random.Range(0, 360));
	}

	private void OnDisable()
	{
		transform.rotation = Quaternion.identity;
	}

	public void SetEnabled(bool state)
	{
		GetComponentInChildren<MeshRenderer>().enabled = state;
	}
}
