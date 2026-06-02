 using UnityEngine;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
	public class BombArea : MonoBehaviour
	{
		public static BombArea Instance;

		public Vector3 center;

		public Vector3 size = new Vector3(16, 0, 16);

		void Awake()
		{
			Instance = this;
			center = transform.position;
		}

		public Vector3 GetRandomPoint()
		{
			return new Vector3(Random.Range(center.x - size.x / 2, center.x + size.x / 2), 0.5f, Random.Range(center.z - size.z / 2, center.z + size.z / 2));
		}

		public Vector3 ClampToArea(Vector3 pos)
		{
			return new Vector3(
				Mathf.Clamp(pos.x, center.x - size.x / 2, center.x + size.x / 2),
				pos.y,
				Mathf.Clamp(pos.z, center.z - size.z / 2, center.z + size.z / 2) // Hier aangepast
			);
		}
	}
}
