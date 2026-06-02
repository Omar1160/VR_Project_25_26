using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Omar.Scripts
{
	public class SpawnArea : MonoBehaviour
	{
		public Vector3 customAreaSize = new Vector3(10f, 0f, 10f);
		private BoxCollider areaCollider;
		public Vector3 GetRandomPosition()
		{
			Vector3 center = transform.position;

			float x = Random.Range(center.x - customAreaSize.x / 2, center.x + customAreaSize.x / 2);
			float z = Random.Range(center.z - customAreaSize.z / 2, center.z + customAreaSize.z / 2);

			return new Vector3(x, 1.2f, z);
		}

		private void OnDrawGizmos()
		{

			Gizmos.color = Color.yellow;

			// Teken de kubus op basis van de positie van dit object en de custom grootte
			Gizmos.DrawWireCube(transform.position, customAreaSize);
		}


	}
}
