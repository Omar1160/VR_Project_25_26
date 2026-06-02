using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Omar.Scripts
{
	public class Checkpoint: MonoBehaviour
	{
		private Renderer rend;
		public Color activeColor = Color.green;
		public Color inActiveColor = Color.gray;

		 void Awake()
		{
			rend = GetComponent<Renderer>();
			SetState(true);
		}

		public void SetState(bool isActive)
		{
			rend.material.color = isActive ? activeColor : inActiveColor;
		}
	}
}
