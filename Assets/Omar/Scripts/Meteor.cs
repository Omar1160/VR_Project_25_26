using UnityEngine;

using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Omar.Scripts
{
	public class Meteor : MonoBehaviour
	{
		public float infectRadius = 10f;

		private void OnTriggerEnter(Collider other)
		{
			if(other.gameObject.CompareTag("Bombarea"))
			{
				Debug.Log("Meteoriet inslag op bombarea!");
				ExplodeAndInfect();
			}

		}

		private void ExplodeAndInfect()
		{
			var allAgents = FindObjectsByType<NPCAgent>(FindObjectsSortMode.None)
							.Where(a => Vector3.Distance(transform.position, a.transform.position) < infectRadius)
							.Where(a => !a.isInfected)
							.ToList();
			Debug.Log($"<color=yellow>Meteoriet inslag! Agenten in buurt: {allAgents.Count}</color>");

			int targetCount = Mathf.CeilToInt(allAgents.Count * 0.2f);
			var targetsToInfect = allAgents.OrderBy(x => Random.value).Take(targetCount).ToList();

			Debug.Log($"<color=yellow>Totaal te infecteren (20%): {targetsToInfect.Count}</color>");
			foreach (var target in targetsToInfect)
			{
				Debug.Log($"<color=red>Meteoriet infecteert: {target.name} op positie {target.transform.position}</color>"); 
				target.BecomeInfected();

			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, infectRadius);
		}
	}
}
