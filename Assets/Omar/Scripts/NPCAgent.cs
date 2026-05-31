using Assets.Omar.Scripts;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;


namespace Assets.Scripts
{

	
	public class NPCAgent : Agent
	{

		public bool isInfected = false;

		public float rotationSpeed = 30f;

		public float healthySpeed;

		public float infectedSpeed;

		public Transform preyTarget;

		public Transform escapeZoneTransform;

		public float maxArenaDistance;

		public BombSpawner spawner;

		public GameObject bombPrefab;

		public float height = 0.5f;

		Rigidbody rb;

		private Renderer rend;

		public SpawnArea spawnArea;

		private float previousDistanceToPrey;

		private float lastDistanceToZone;

		private bool reachedGoal = false;

		public override void Initialize()
		{
			rb = GetComponent<Rigidbody>();
			rend = GetComponentInChildren<Renderer>();
			ArenaManager arenamanager =FindAnyObjectByType<ArenaManager>();

			if(arenamanager != null)
			{
				maxArenaDistance = Mathf.Sqrt(Mathf.Pow(arenamanager.arenaSize.x, 2) + Mathf.Pow(arenamanager.arenaSize.z, 2));
				Debug.Log("Arena grootte geladen: " + arenamanager.arenaSize + ". Max afstand: " + maxArenaDistance);
			}

			else
			{
				Debug.LogError("ArenaManager niet gevonden in de scene!");
			}
		}

		public override void OnEpisodeBegin()
		{
			isInfected = false;
			gameObject.tag = "Prey";
			rend.material.color = Color.green;
			rb.isKinematic = false;
			rb.linearVelocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero; // draaisnelheid ook op nul
		
			// Zorg dat de stats gereset worden
			var manager = FindAnyObjectByType<ArenaManager>();
			Debug.Log("Manager gevonden: " + (manager != null));
			if (manager != null)
			{
				// We resetten de stats alleen als we de eerste agent in de array zijn 
				// zodat we niet 20x per episode resetten

				manager.bombSpawnedThisEpisode = false;
				manager.ResetStats();
			
			}
		
			if(manager != null && !manager.bombSpawnedThisEpisode)
			{
				Debug.Log("Bom wordt aangeroepen"); 
				manager.bombSpawnedThisEpisode = true;
				Invoke(nameof(DelayedBombSpawn), 0.2f);
			}
			
			if(spawnArea != null)
			{
				transform.position = spawnArea.GetRandomPosition();
			} else
			{
				Debug.Log("Geen SpawnAreea gekoppeld!");
			}

				// Reset afstand-tracking
				lastDistanceToZone = GetDistanceToEscapeZone();
				previousDistanceToPrey = GetNearestPreyDistance();
			
		}


		private void DelayedBombSpawn()
		{
			if (spawner != null)
			{
				Debug.Log("Agent probeert bom te spawnen!");
				spawner.DropBomb();
			}
			else
			{
				Debug.LogError("Spawner is NIET gekoppeld in de Inspector!");
			}
		}

		public override void CollectObservations(VectorSensor sensor)
		{
		
			//1. Status van de agent (Geinfecterd of niet?)
			sensor.AddObservation(isInfected ? 1.0f : 0.0f);
			//2. Afstand naar de EscapeZone (gebruik je nieuwe, snelle methode)
			sensor.AddObservation(GetDistanceToEscapeZone());
			// 3. Afstand naar de dichtsbijzijnde Prey
			sensor.AddObservation(GetNearestPreyDistance() / maxArenaDistance);

		}

		private float GetNearestPreyDistance()
		{
			var preys = GameObject.FindGameObjectsWithTag("Prey");

			// Als er geen prey is, is de afstand "maximaalé (1.0 in genormaliserde termen)
			if (preys.Length == 0) return 1.0f;

			// Bereken de afstand naar de dichtsbijzijnde prey
			float minDistance = preys.Select(p => Vector3.Distance(transform.position, p.transform.position)).Min();

			// Normaliseer direct (deel door de max afstand)
			return minDistance / maxArenaDistance;
		}

		private float GetDistanceToEscapeZone()
		{
			if(escapeZoneTransform == null) return 1.0f;
		
			float distance = Vector3.Distance(transform.position, escapeZoneTransform.position);

			// Deeel door de berekende diagonaal
			return distance / maxArenaDistance;
		}
		
		public override void OnActionReceived(ActionBuffers actions)
		{
			// 1. Bewegingslogica (voorbeeld)

			float moveX = actions.ContinuousActions[0];
			float moveZ = actions.ContinuousActions[1];
			float turn = actions.ContinuousActions[2]; // Nieuwe actie voor draaien

			float speed = isInfected ? infectedSpeed : healthySpeed;
			rb.linearVelocity = new Vector3(moveX * speed, 0, moveZ * speed);
			rb.angularVelocity = new Vector3(0, turn * rotationSpeed, 0);
			// dit is de rem: Limiter de hoeksnelheid
		

			if(isInfected)
			{
				float currentDist = GetNearestPreyDistance();
				AddReward((previousDistanceToPrey - currentDist) * 0.005f);
				previousDistanceToPrey = currentDist;
			} else
			{
				float dist = GetDistanceToEscapeZone();
				if (dist < lastDistanceToZone) AddReward(0.001f);
				else AddReward(-0.0005f);
				lastDistanceToZone = dist;
			}

			//  Kleine straf per stap om snelheid te forceren
			AddReward(-0.0001f);


		}
		public override void Heuristic(in ActionBuffers actionsOut)
		{
			var actions = actionsOut.ContinuousActions;

			float moveX = 0;
			float moveZ = 0;

			// WASD
			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
				moveZ = 1;

			if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
				moveZ = -1;

			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
				moveX = -1;

			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
				moveX = 1;

			actions[0] = moveX;
			actions[1] = moveZ;

			float turn = 0;

			if (Input.GetKey(KeyCode.Q))
				turn = -1;

			if (Input.GetKey(KeyCode.E))
				turn = 1;

			transform.Rotate(0, turn * rotationSpeed * Time.deltaTime, 0);
		}
		private void OnTriggerEnter(Collider other)
		{
			if(gameObject.CompareTag("Hunter") && other.CompareTag("Prey"))
			{
				var prey = other.GetComponent<NPCAgent>();
				if (prey != null) prey.BecomeInfected();
				AddReward(1.0f);
				Debug.Log($"<color=red>Hunter heeft Prey geïnfecteerd! +1.0 Reward</color>");
			}

			// ontsnapping-logica
			if(gameObject.CompareTag("Prey") && other.CompareTag("Escapezone"))
			{
				gameObject.tag = "Escaped";

				GetComponentInChildren<Renderer>().material.color = Color.blue;

				//3. bevriezen
				rb.linearVelocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				rb.isKinematic = true; // Zorgt dat physics hem niet meer beweegt

				//4. Rapportage naar Managr
				var manager = FindAnyObjectByType<ArenaManager>();
				if (manager != null) manager.ReportPreyEscaped();

				AddReward(1.0f);
				Debug.Log($"<color=green>Prey is ontsnapt! +1.0 Reward</color>");
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.CompareTag("Wall"))
			{
				Debug.Log("Agent Hit wall");

				AddReward(-0.01f);
			}
		}

		public void BecomeInfected()
		{
			isInfected = true;
			gameObject.tag = "Hunter";
			rend.material.color = Color.red;

			var manager = FindAnyObjectByType<ArenaManager>();
			if (manager != null) manager.UpdateUI();

		}
	}
}
