using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.MLAgents;
using TMPro;


namespace Assets.Scripts
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance;

		[Header("Timer")]
		public float gameDuration = 60f;

		private float timer;

		[Header("UI")]
		public GameObject winPanel;
		public GameObject losePanel;
		public GameObject pausePanel;
		public GameObject hudPanel;
		public GameObject mainMenuPanel;

		public TextMeshProUGUI timerText;

		bool gameEnded = false;

		[Header("Agents")]
		public List<Agent> healthyAgents;
		public List<Agent> infectedAgents;

		[Header("Bomb")]
		public GameObject bombPrefab;
		private GameObject currentBomb;
		[Header("Arena")]
		public Vector2 arenaSize = new Vector2(20f, 20f);

	
		private void Update()
		{

			if (gameEnded)
				return;

			timer -= Time.deltaTime;

			timerText.text = "Time: " + Mathf.Ceil(timer);

			if(timer <= 0)
			{
				Debug.Log("Healthy survived!");
				WinGame();
			}
		
		}

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{

			timer = gameDuration;

			Time.timeScale = 1f;

			winPanel.SetActive(false);
			losePanel.SetActive(false);
			pausePanel.SetActive(false);

			gameEnded = false;

			ResetEnvironment();
		}

		public void ResetEnvironment()
		{
			ResetAgents();
			ResetBomb();
		}


		public void ResetAgents()
		{

			foreach (Agent a in healthyAgents)
			{
				if (a == null)
					continue;

				
				a.transform.localPosition = GetRandomPosition();

				a.EndEpisode();
			
			}
			foreach (Agent a in infectedAgents)
			{
				if (a == null)
					continue;


				a.transform.localPosition = GetRandomPosition();

				a.EndEpisode();
			}
		}

		public void WinGame()
		{
			gameEnded = true;

			winPanel.SetActive(true);

			Time.timeScale = 0f;
		}

		public void LoseGame()
		{
			gameEnded = true;

			losePanel.SetActive(true);

			Time.timeScale = 0f;
		}

		public void PauseGame()
		{
			pausePanel.SetActive(true);

			Time.timeScale = 0f;
		}

		public void ResumeGame()
		{
			pausePanel.SetActive(false);

			Time.timeScale = 1f;
		}

		public void RestartGame()
		{
			Time.timeScale = 1f;

			UnityEngine.SceneManagement.SceneManager.LoadScene(0);
		}

		public void QuitGame()
		{
			Application.Quit();
		}

		public void ResetBomb()
		{
			if(currentBomb != null)
			{
				Destroy(currentBomb);
			}

			Vector3 pos = GetRandomPosition();
			currentBomb = Instantiate(bombPrefab, pos, Quaternion.identity);
		}


		public Vector3 GetRandomPosition()
		{
			return new Vector3(Random.Range(-arenaSize.x / 2, arenaSize.x / 2), 0.5f, Random.Range(-arenaSize.y / 2, arenaSize.y / 2)); 
		}

		public void CheckWinCondition()
		{
			bool allInfected = true;

			foreach (Agent a in healthyAgents)
			{
				InfectionSystem inf = a.GetComponent<InfectionSystem>();
				if(inf != null && !inf.isInfected)
				{
					allInfected = false;
					break;
				}
			}

			if(allInfected)
			{
				Debug.Log("All agents infected -> Episode reset");
				ResetEnvironment();
			}

		}
	}
}
