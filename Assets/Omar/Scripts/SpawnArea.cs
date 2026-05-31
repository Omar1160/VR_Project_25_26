using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Omar.Scripts
{
	public class SpawnArea : MonoBehaviour
	{
		private BoxCollider areaCollider;

		private void Awake()
		{
			areaCollider = GetComponent<BoxCollider>();
		}

		public Vector3 GetRandomPosition()
		{
			Vector3 center = areaCollider.bounds.center;
			Vector3 size = areaCollider.bounds.size;

			float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
			float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

			return new Vector3(x, 1.2f, z);
		}
	}
}
