using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Assets.Scripts
{
	public class BombSpawner : MonoBehaviour
	{
		public GameObject bombPrefab;

		
		public float spawnHeight = 15f;

		public Vector2 arenaSize = new Vector2(20, 20);

		public float defaultInterval = 10f;

		public float infectedInterval = 20f;

		public float currentInterval = 20f;
		private void Start()
		{
			StartCoroutine(SpawnLoop());
		}

		IEnumerator SpawnLoop()
		{
			while (true)
			{
				SpawnBomb();
				yield return new WaitForSeconds(currentInterval);

			}



		}

		public void SetDifficulty(bool isInfected)
		{
			currentInterval = isInfected ? infectedInterval : defaultInterval;
			Debug.Log("Spawn interval aangepast naar: " + currentInterval);
		}

		void SpawnBomb()
		{
			Vector3 randomPos = new Vector3(
				Random.Range(-arenaSize.x / 2, arenaSize.x / 2 ), spawnHeight, Random.Range(-arenaSize.y / 2 , arenaSize.y / 2));

			Instantiate(bombPrefab, randomPos, Quaternion.identity);

			Debug.DrawRay(randomPos, Vector3.down * 5, Color.red, 2f);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;

			Vector3 center = transform.position;

			Vector3 size = new Vector3(arenaSize.x, 0.1f, arenaSize.y);

			Gizmos.DrawWireCube(center, size);
		}
	}
}
