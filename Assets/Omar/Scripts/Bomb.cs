using Assets.Omar.Scripts;
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
		public List<NPCAgent> targetsToInfect = new List<NPCAgent>();
		public BombSpawner spawner;

		public void Start()
		{
			Invoke("Explode", 2.0f);
		}
		
		public void Explode()
		{
			foreach (var target in targetsToInfect)
			{
				if(target != null && !target.isInfected)
				{
					Debug.Log($"<color=red>Bom infecteert: {target.name}</color>");
					target.BecomeInfected();
				}
			}

			if(spawner != null)
			{
				spawner.ResetSpawner();
			}

			// Verwijder de bom
			Destroy(gameObject);
		}


	}


}
 