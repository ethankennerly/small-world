using UnityEngine;

namespace FineGameDesign.Utils
{
	public sealed class CountTrigger : MonoBehaviour
	{
		public static int collisionLayer = 8;
		public int count = 0;

		void OnTriggerEnter(Collider other)
		{
			if (collisionLayer == other.gameObject.layer)
			{
				count++;
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (collisionLayer == other.gameObject.layer && 1 <= count)
			{
				count--;
			}
		}
	}
}
