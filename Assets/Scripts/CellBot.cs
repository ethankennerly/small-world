using UnityEngine;
using System.Collections.Generic;

namespace Finegamedesign.SmallWorld
{
	[RequireComponent(typeof(Collider2D))]
	public sealed class CellBot : MonoBehaviour
	{
		public float chaseRelativeScale = 0.875f;
		public float fleeRelativeScale = 1.25f;

		private GameObject player;
		public PhotonBody playerBehaviour;
		private GameObject chaseTo;
		private GameObject fleeFrom;

		private List<GameObject> visibleCells = new List<GameObject>();

		void Awake()
		{
			player = transform.Find("PlayerCell").gameObject;
			playerBehaviour = player.GetComponent<PhotonBody>();
		}

		void FixedUpdate()
		{
			if (null == player)
			{
				return;
			}
			transform.position = player.transform.position;
			UpdateDirection();
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.layer != player.layer)
			{
				return;
			}
			GameObject otherObject = other.gameObject;
			if (visibleCells.IndexOf(otherObject) <= -1)
			{
				visibleCells.Add(otherObject);
			}
		}

		void OnTriggerExit2D(Collider2D other)
		{
			if (other.gameObject.layer != player.layer)
			{
				return;
			}
			GameObject otherObject = other.gameObject;
			if (0 <= visibleCells.IndexOf(otherObject))
			{
				visibleCells.Remove(otherObject);
			}
		}

		private void UpdateDirection()
		{
			if (!player.activeSelf)
			{
				return;
			}
			SizeReferee.ReverseByScaleX(visibleCells);
			UpdateFleeOrChase();
			if (null == fleeFrom && null == chaseTo)
			{
				UpdateRandomDirection();
			}
		}

		private void UpdateFleeOrChase()
		{
			fleeFrom = null;
			chaseTo = null;
			float scale = player.transform.localScale.x;
			for (int index = 0; index < visibleCells.Count; index++)
			{
				GameObject interest = visibleCells[index];
				if (!interest.activeSelf)
				{
					continue;
				}
				float interestScale = interest.transform.localScale.x;
				float relativeScale = interestScale / scale;
				if (relativeScale <= fleeRelativeScale)
				{
					fleeFrom = interest;
					playerBehaviour.MoveAwayFrom(fleeFrom.transform.position);
					break;
				}
				else if (chaseRelativeScale <= relativeScale)
				{
					chaseTo = interest;
					playerBehaviour.MoveToward(chaseTo.transform.position);
					break;
				}
			}
		}

		// TODO
		private void UpdateRandomDirection()
		{
		}
	}
}
