using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.MLAgents;
using UnityEngine;

namespace Assets.Scripts
{
	public class BombSpawner : MonoBehaviour
	{
		public GameObject bombPrefab;
		public Transform arena;
		public float spawnHeight = 50f;
		public Vector3 arenaSize;
		private bool isBombActive = false;

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;

			Vector3 center = transform.position;


			Vector3 size = new Vector3(arenaSize.x, 0.1f, arenaSize.y);

			Gizmos.DrawWireCube(center, size);
		}

		public void DropBomb()
		{
			if (isBombActive) return;
			isBombActive = true;

			var allAgents = FindObjectsByType<NPCAgent>(FindObjectsSortMode.None).Where(a => a.gameObject.activeSelf).ToList();
			int targetCount = Mathf.CeilToInt(allAgents.Count * 0.2f);
			var targets = allAgents.Where(a => !a.isInfected).OrderBy(x => Random.value).Take(targetCount).ToList();

			if(targets.Count == 0) { isBombActive = false; return; }

			Vector3 spawnPos = arena.position + new Vector3(
				Random.Range(-arenaSize.x / 2, arenaSize.x / 2),
				spawnHeight, // GEBRUIK DEZE VARIABELE
				Random.Range(-arenaSize.z / 2, arenaSize.z / 2)
			); GameObject bombObj = Instantiate(bombPrefab, spawnPos, Quaternion.identity);
			bombObj.GetComponent<Bomb>().targetsToInfect = targets;
			bombObj.GetComponent<Bomb>().spawner = this;
		}

		public void ResetSpawner()
		{
			isBombActive = false;
		}

	}
}
