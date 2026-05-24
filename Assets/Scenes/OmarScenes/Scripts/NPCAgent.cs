using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


namespace Assets.Scripts
{

	
	public class NPCAgent : Agent
	{
		public bool isInfected = false;

		public float rotationSpeed = 150f;

		private float previousDistanceToPrey;

		public Transform preyTarget;

		public float healthySpeed = 5f;

		public float infectedSpeed = 12f;

		private float currentSpeed;

		public Vector2 size = new Vector2(20, 20);

		public float height = 0.5f;

		public Transform areaCenter;

		public float areaRadius = 8f;

		Rigidbody rb;

		private Renderer rend;

		private RobotAnimator robotAnimator;

		private PreyAgent prey;
		public override void Initialize()
		{
			rb = GetComponent<Rigidbody>();
			rend = GetComponentInChildren<Renderer>();
			prey = FindAnyObjectByType<PreyAgent>();
			robotAnimator = GetComponent<RobotAnimator>();

		}

		public override void OnEpisodeBegin()
		{
			rb.linearVelocity = Vector3.zero;

			// 1. Zoek de instance
			if (BombArea.Instance == null)
			{
				BombArea.Instance = Object.FindAnyObjectByType<BombArea>();
			}

			// 2. Veiligheidscheck: bestaat hij nu wel?
			if (BombArea.Instance != null)
			{
				transform.position = BombArea.Instance.GetRandomPoint();
			}
			

			isInfected = false;

			previousDistanceToPrey = Vector3.Distance(transform.position, prey.transform.position);

			currentSpeed = healthySpeed;

			rend.material.color = Color.green;

		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;

			Vector3 center = transform.position;

			Vector3 cubeSize = new Vector3(
				size.x,
				0.1f,
				size.y
			);

			Gizmos.DrawWireCube(center, cubeSize);
		}

		public override void CollectObservations(VectorSensor sensor)
		{
			sensor.AddObservation(transform.localPosition);
			sensor.AddObservation(rb.linearVelocity);
			sensor.AddObservation(isInfected ? 1.0f : 0.0f);

		}

		public override void OnActionReceived(ActionBuffers actions)
		{
			rb.linearVelocity = Vector3.zero;

			float moveX = actions.ContinuousActions[0];
			float moveZ = actions.ContinuousActions[1];

			rb.linearVelocity = new Vector3(moveX * currentSpeed, 0, moveZ * currentSpeed);

			// healthy reward
			if (!isInfected)
			{
				// survival
				AddReward(0.0001f);
			}

			// infected chase reward
			if (isInfected)
			{
				float currentDistance = Vector3.Distance(transform.position, prey.transform.position);

				float reward = previousDistanceToPrey - currentDistance;

				AddReward(reward * 0.02f);

				previousDistanceToPrey = currentDistance;
			}

			// blijf binnen area

			Vector3 toCenter = areaCenter.position - transform.position;

			float distanceFromCenter = toCenter.magnitude;

			
			if(distanceFromCenter > areaRadius)
				{
					// penalty buiten zone
				AddReward(-0.02f);

			}

			// clamp inside arena

		if (BombArea.Instance != null)
            {
                transform.position = BombArea.Instance.ClampToArea(transform.position);
            }

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

			// Infecteed NPC raakt healhty NPC

			if (other.CompareTag("NPC") && isInfected)
			{
				NPCAgent npc = other.GetComponent<NPCAgent>();

				npc.BecomeInfected();

				Debug.Log("NPC infected: " + npc.name);

				AddReward(5f);
			}

			// Infect NPC raakt prey

			if (other.CompareTag("Prey") && isInfected)
			{
				PreyAgent prey = other.GetComponent<PreyAgent>();

				Debug.Log("NPC infected Prey");

				prey.GameOver();

				AddReward(10f);
			}

		}

	

		private void OnCollisionEnter(Collision collision)
		{


			if (collision.gameObject.CompareTag("Wall"))
			{
				Debug.Log("Hit wall");

				AddReward(-0.01f);
			}
		}

		public void BecomeInfected()
		{
			if (isInfected)
				return;

			isInfected = true;

			BombSpawner spawner = FindAnyObjectByType<BombSpawner>();
			if(spawner != null)
			{
				spawner.SetDifficulty(true);
			}
			currentSpeed = infectedSpeed;

			rend.material.color = Color.red;

			gameObject.tag = "Enemy";


		}
	}
}
