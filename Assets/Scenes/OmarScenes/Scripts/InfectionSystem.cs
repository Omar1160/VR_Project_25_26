using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
	public class InfectionSystem : MonoBehaviour
	{ 
		public bool isInfected = false;

		Renderer rend;

		private void Start()
		{
			rend = GetComponentInChildren<Renderer>();
			UpdateColor();
		}

		public void Infect()
		{
			isInfected = true;
			gameObject.tag = "Infected";
			UpdateColor();
		}

		void UpdateColor()
		{
			if(isInfected)
			{
				rend.material.color = Color.red;

			}
			else
			{
				rend.material.color = Color.white;
			}

		}

		private void OnCollisionEnter(Collision collision)
		{
			InfectionSystem other = collision.gameObject.GetComponent<InfectionSystem>();

			if(other != null)
			{
				if(isInfected && !other.isInfected)
				{
					other.Infect();
				}
			}
		}
	}
}
