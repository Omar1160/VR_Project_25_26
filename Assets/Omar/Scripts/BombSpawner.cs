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

		private void Start()
		{
			isBombActive = false;
			Debug.Log("BombSpawner: isBombActive gereset naar false bij start.");
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;

			Vector3 center = transform.position;


			Vector3 size = new Vector3(arenaSize.x, 0.1f, arenaSize.y);

			Gizmos.DrawWireCube(center, size);
		}

		public void DropBomb()
		{
			Debug.Log($"<color=blue>DropBomb gestart. isBombActive: {isBombActive}</color>");

			if (isBombActive)
			{
				Debug.LogWarning("DropBomb genegeerd: er is al een bom actief.");
				return;
			}
			if (bombPrefab == null)
			{
				Debug.LogError("BombSpawner: Bomb Prefab is NIET gekoppeld in de Inspector!");
				return;
			}
			if (arena == null)
			{
				Debug.LogError("BombSpawner: Arena Transform is NIET gekoppeld in de Inspector!");
				return;
			}

			isBombActive = true;

			var allAgents = FindObjectsByType<NPCAgent>(FindObjectsSortMode.None).Where(a => a.gameObject.activeSelf).ToList();
			Debug.Log("Aantal gevonden agenten: " + allAgents.Count);
			int targetCount = Mathf.CeilToInt(allAgents.Count * 0.2f);
			var targets = allAgents.Where(a => !a.isInfected).OrderBy(x => Random.value).Take(targetCount).ToList();
			Debug.Log($"Agenten gevonden: {allAgents.Count}, Targets geselecteerd: {targets.Count}");
			if (targets.Count == 0) {
				Debug.LogWarning("Geen gezonde agenten gevonden om te infecteren.");
				isBombActive = false; return; }

			Vector3 spawnPos = arena.position + new Vector3(
				Random.Range(-arenaSize.x / 2, arenaSize.x / 2),
				spawnHeight, // GEBRUIK DEZE VARIABELE
				Random.Range(-arenaSize.z / 2, arenaSize.z / 2)
			);
			Debug.Log($"<color=cyan>BombSpawner: Spawnt bom op positie: {spawnPos}</color>");
			GameObject bombObj = Instantiate(bombPrefab, spawnPos, Quaternion.identity);
			bombObj.GetComponent<Bomb>().targetsToInfect = targets;
			bombObj.GetComponent<Bomb>().spawner = this;
		}

		public void ResetSpawner()
		{
			isBombActive = false;
		}

	}
}
