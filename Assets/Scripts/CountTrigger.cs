using UnityEngine;

namespace Finegamedesign.Utils
{
	public sealed class CountTrigger : MonoBehaviour
	{
		public static int collisionLayer = 8;
		public int count = 0;
		public bool isVerbose = false;

		void OnTriggerEnter2D(Collider2D other)
		{
			if (isVerbose)
			{
				Debug.Log("CountTrigger.OnTriggerEnter: layer "
					+ other.gameObject.layer);
			}
			if (collisionLayer == other.gameObject.layer)
			{
				count++;
			}
		}

		void OnTriggerExit2D(Collider2D other)
		{
			if (collisionLayer == other.gameObject.layer && 1 <= count)
			{
				count--;
			}
		}
	}
}
