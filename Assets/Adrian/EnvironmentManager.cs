using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public GameObject hunterPrefab;
    public GameObject civilianPrefab;
    public Transform arenaFloor;

    public int civilianCount = 4;

    private List<GameObject> spawnedCivilians = new List<GameObject>();
    private float spawnRadius;

    // Changing this to public so the hunter can see and modify it directly
    [HideInInspector] public int activeCivilians;


    public void ResetRound(GameObject hunterInstance)
    {
        // Calculate radius using the scale of the Ground child object directly
        spawnRadius = (arenaFloor.localScale.x * 10f) / 2f - 2f;

        activeCivilians = civilianCount;

        // 1. Completely destroy old civilian instances
        for (int i = spawnedCivilians.Count - 1; i >= 0; i--)
        {
            if (spawnedCivilians[i] != null)
            {
                Destroy(spawnedCivilians[i]);
            }
        }
        spawnedCivilians.Clear();

        // 2. Relocate the hunter to its random starting position
        Vector3 hunterPos = GetRandomPosition();
        hunterInstance.transform.position = hunterPos;

        Debug.Log($"[SPAWN] Hunter position updated to: {hunterPos}");

        // 3. Spawn fresh civilians sequentially
        for (int i = 0; i < civilianCount; i++)
        {
            Vector3 civPos = GetRandomPosition();
            int safetyCheck = 0;

            // Simple boundary loop to prevent overlapping with the hunter
            while (Vector3.Distance(civPos, hunterPos) < 4f && safetyCheck < 50)
            {
                civPos = GetRandomPosition();
                safetyCheck++;
            }

            // Explicitly pass this.transform to ground them to the Arena coordinate system
            GameObject civ = Instantiate(civilianPrefab, civPos, Quaternion.identity, this.transform);
            spawnedCivilians.Add(civ);

            // This log MUST print if Instantiate succeeds!
            Debug.Log($"[SPAWN] Civilian {i} successfully initialized at: {civPos}");
        }
    }

    private Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(-spawnRadius, spawnRadius);
        float randomZ = Random.Range(-spawnRadius, spawnRadius);
        return new Vector3(arenaFloor.position.x + randomX, arenaFloor.position.y + 0.5f, arenaFloor.position.z + randomZ);
    }
}