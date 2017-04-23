using UnityEngine;
using System.Collections.Generic;

namespace Finegamedesign.SmallWorld
{
	[RequireComponent(typeof(Collider2D))]
	public sealed class CellBot : MonoBehaviour
	{
		public float chaseRelativeScale = 0.875f;
		public float fleeRelativeScale = 1.25f;
		public float commitDuration = 1.0f;

		public GameObject player;
		public PhotonBody playerBehaviour;
		private GameObject prey;
		private GameObject predator;
		private Collider2D vision;
		private float commitTime = -1.0f;

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
			if (Time.time < commitTime)
			{
				return;
			}
			SizeReferee.ReverseByScaleX(visibleCells);
			UpdateFleeOrChase();
			UpdateMove();
		}

		private void UpdateFleeOrChase()
		{
			predator = null;
			if (null != prey)
			{
				if (!prey.activeSelf)
				{
					prey = null;
				}
			}
			float scale = player.transform.localScale.x;
			for (int index = 0; index < visibleCells.Count; index++)
			{
				GameObject interest = visibleCells[index];
				if (!interest.activeSelf)
				{
					continue;
				}
				float interestScale = interest.transform.localScale.x;
				float interestRelativeScale = interestScale / scale;
				if (fleeRelativeScale <= interestRelativeScale)
				{
					predator = interest;
					break;
				}
				else if (null == prey && interestRelativeScale <= chaseRelativeScale)
				{
					prey = interest;
					break;
				}
			}
		}

		private void UpdateMove()
		{
			if (null != predator)
			{
				playerBehaviour.MoveAwayFrom(predator.transform.position);
			}
			else if (null != prey)
			{
				playerBehaviour.MoveToward(prey.transform.position);
			}
			else
			{
				playerBehaviour.MoveToward(Vector3.zero);
			}
			commitTime = commitDuration + Time.time;
		}
	}
}
