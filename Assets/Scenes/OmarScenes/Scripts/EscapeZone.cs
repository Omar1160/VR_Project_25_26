using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
	public class EscapeZone : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			PreyAgent prey = other.GetComponent<PreyAgent>();

			if(prey != null)
			{
				prey.AddReward(10f);

				Debug.Log("Prey entered Zone!");

				prey.EndEpisode();
			}
		}
	}
}
