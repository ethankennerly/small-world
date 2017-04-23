using UnityEngine;
using System.Collections.Generic;

namespace Finegamedesign.SmallWorld
{
	[RequireComponent(typeof(Collider2D))]
	public sealed class CellBot : MonoBehaviour
	{
		public float chaseRelativeScale = 0.875f;
		public float fleeRelativeScale = 1.25f;

		public GameObject player;
		public PhotonBody playerBehaviour;
		private GameObject chaseTo;
		private GameObject fleeFrom;
		private Collider2D vision;

		private List<GameObject> visibleCells = new List<GameObject>();

		void Awake()
		{
			player = transform.Find("PlayerCell").gameObject;
			vision = GetComponent<Collider2D>();
			playerBehaviour = player.GetComponent<PhotonBody>();
		}

		void Update()
		{
			if (null == player)
			{
				return;
			}
			VisionFollowPlayer();
			UpdateDirection();
		}

		private void VisionFollowPlayer()
		{
			Vector3 position = player.transform.position;
			Vector2 offset = new Vector2(
				position.x - transform.position.x,
				position.y - transform.position.y
			);
			vision.offset = offset;
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			GameObject otherObject = other.gameObject;
			if (otherObject.layer != player.layer || otherObject == player)
			{
				return;
			}
			if (visibleCells.IndexOf(otherObject) <= -1)
			{
				visibleCells.Add(otherObject);
			}
		}

		void OnTriggerExit2D(Collider2D other)
		{
			GameObject otherObject = other.gameObject;
			if (otherObject.layer != player.layer || otherObject == player)
			{
				return;
			}
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
