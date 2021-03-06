using UnityEngine;
using System.Collections.Generic;

namespace Finegamedesign.SmallWorld
{
	[RequireComponent(typeof(BoxCollider2D))]
	public sealed class CellBot : MonoBehaviour
	{
		public float chaseRelativeScale = 0.875f;
		public float fleeRelativeScale = 1.25f;
		public float commitDuration = 1.0f;
		private Vector2 visionSize;

		public GameObject player;
		private PhotonBody playerBehaviour;
		private GameObject prey;
		private GameObject predator;
		private BoxCollider2D vision;
		private float commitTime = -1.0f;

		private List<GameObject> visibleCells = new List<GameObject>();

		void Awake()
		{
			vision = GetComponent<BoxCollider2D>();
			visionSize = new Vector2(vision.size.x, vision.size.y);
			playerBehaviour = player.GetComponent<PhotonBody>();
		}

		void Update()
		{
			if (null == player)
			{
				return;
			}
			VisionInverseScale();
			UpdateDirection();
		}

		private void VisionInverseScale()
		{
			Vector3 scale = player.transform.localScale;
			Vector2 size = new Vector2(
				visionSize.x / scale.x,
				visionSize.y / scale.y);
			vision.size = size;
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			GameObject otherObject = other.gameObject;
			if (null == otherObject || otherObject.layer != player.layer || otherObject == player)
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
			if (null == otherObject || otherObject.layer != player.layer || otherObject == player)
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
			if (null == player || !player.activeSelf)
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
				if (null == interest || !interest.activeSelf)
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
