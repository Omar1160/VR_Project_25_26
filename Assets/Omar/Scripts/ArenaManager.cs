using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.MLAgents;
using UnityEngine;

namespace Assets.Omar.Scripts
{
	public class ArenaManager : MonoBehaviour
	{
		public NPCAgent[] allAgents;
		public TextMeshProUGUI txtEscaped;
		public TextMeshProUGUI txtHunters;
		public TextMeshProUGUI txtPreys;


		public int totalPreyEscaped = 0;
		public BombSpawner bombSpawner;
		public Vector3 arenaSize = new Vector3(80, 0.1f, 70);
		public Transform arena;

		private bool isEnding = false;

		// Voeg deze tellers toe
		public int preyWins = 0;
		public int hunterWins = 0;

		public bool isGlobalHuntMode = false;
		public float hunterThreshold = 0.60f;
		public bool bombSpawnedThisEpisode = false;

		private void Start()
		{
			// Forceer een schone start bij het laden van de scene
			ResetStats();
		}
		public void ResetStats()

		{
			Debug.Log("<color=magenta>ArenaManager: Stats aan het resetten!</color>"); // ZIE JE DIT?

			totalPreyEscaped = 0;
			bombSpawnedThisEpisode = false;
			UpdateUI();
		}
		public void UpdateUI()
		{
			// Cijfers Ophalen
			int totalHunters = allAgents.Count(a => a.CompareTag("Hunter") && a.gameObject.activeSelf == true);
			int totalPrey = allAgents.Count(a => a.CompareTag("Prey") && a.gameObject.activeSelf == true);

			
			Debug.Log("totalHunters:" + totalHunters);
			Debug.Log("totalAgents:" + totalPrey);

			txtEscaped.text = "Escaped: " + totalPreyEscaped;
			txtHunters.text = "Hunters: " + totalHunters;
			txtPreys.text = "Preys: " + totalPrey;

			// Na de UI update, checken we of het spel voorbij is
			if (!isEnding) CheckGameStatus(totalHunters);

		}

	
	
		public void CheckGameStatus(int totalHunters)
		{
			int totalAgents = allAgents.Length;
			if (totalAgents == 0) return;

			Debug.Log("totalAgents:" + totalAgents);
			Debug.Log("totalHunters:" + totalHunters);
			Debug.Log("totalPreyEscaped:" + totalPreyEscaped);


			float hunterPercentage = (float)totalHunters / totalAgents;
			float escapePercentage = (float)totalPreyEscaped / totalAgents;

			Debug.Log("hunterPercentage:" + hunterPercentage);
			Debug.Log("escapePercentage:" + escapePercentage);
			String reason = "";
			if (hunterPercentage >= 0.60f)
			{
				hunterWins++;
				reason = "Hunter Wint!";
				EndGame();

			}
			else if (escapePercentage >= 0.60f)
			{
				preyWins++;

				reason = "Prey Wint!";
				EndGame();
			}

			else if(hunterPercentage >= 0.50f && escapePercentage >= 0.50f)
			{
				Debug.Log("<color=yellow>Gelijkspel (50/50)! Episode eindigt.</color>");
				EndGame();
			}
				Debug.Log($"{reason} | Score: Prey {preyWins} - Hunters {hunterWins}");

		}

		public void EndGame( )
		{
			if (isEnding) return;
			isEnding = true; // Zet een slot op de deur

			Invoke(nameof(ForceEndEpisodes), 0.3f);

		}
	
		private void ForceEndEpisodes()
		{
			foreach (var agent in allAgents)
			{
				agent.EndEpisode();
			}

			if (bombSpawner != null)
			{
				bombSpawner.ResetSpawner();
			}

			isEnding = false;
		}
		public void ReportPreyEscaped()
		{
			totalPreyEscaped++;
			UpdateUI();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;

			Vector3 center = arena.position;

			Gizmos.DrawWireCube(center, arenaSize);
		}

	}
}
