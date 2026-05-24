using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
	public class Bomb : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			NPCAgent npc = other.GetComponent<NPCAgent>();
			PreyAgent prey = other.GetComponent<PreyAgent>();

			if(npc != null)
			{
				npc.BecomeInfected();
			}

			if(prey != null)
			{
				Debug.Log("Bomb hit Prey!");

				prey.GameOver();
			}
			Destroy(gameObject);
		}
	}
}
