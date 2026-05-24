using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
	public class RobotAnimator : MonoBehaviour
	{
		public Animator animator;
		public Rigidbody rb;


		public void Update()
		{
			float speed = rb.linearVelocity.magnitude;

			animator.SetFloat("Speed", speed);
		}

		public void Attack()
		{
			animator.SetTrigger("Attack");
		}
	}
}
