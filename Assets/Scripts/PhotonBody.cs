using UnityEngine;
using System.Collections.Generic;
using Finegamedesign.Utils;

namespace Finegamedesign.SmallWorld
{
	public sealed class PhotonBody : MonoBehaviour
	{
		public static float eatRadiusThreshold = 1.125f;
		public static int playerLayer = 8;
		public static List<GameObject> instances = new List<GameObject>();
		public bool isBot = false;

	 	private PhotonView photon;
	 	public Rigidbody2D body;

		void Awake()
		{
			if (instances.IndexOf(gameObject) <= -1)
			{
				instances.Add(gameObject);
			}
			body = GetComponent<Rigidbody2D>();
			photon = GetComponent<PhotonView>();
		}

		void FixedUpdate()
		{
			if (!photon.isMine || isBot)
			{
				return;
			}
			UpdateMovement();
			UpdateQuit();
		}

		private void UpdateMovement()
		{
			Vector2 velocity = body.velocity;
			if (Input.GetAxisRaw("Horizontal") < -0.5f)
			{
				velocity.x = -1.0f;
			}
			else if (0.5f < Input.GetAxisRaw("Horizontal"))
			{
				velocity.x = 1.0f;
			}
			if (Input.GetAxisRaw("Vertical") < -0.5f)
			{
				velocity.y = -1.0f;
			}
			else if (0.5f < Input.GetAxisRaw("Vertical"))
			{
				velocity.y = 1.0f;
			}
			body.velocity = velocity.normalized;
		}

		public void MoveToward(Vector3 target)
		{
			Move(target - transform.position);
		}

		public void MoveAwayFrom(Vector3 away)
		{
			Move(transform.position - away);
		}

		public void Move(Vector3 direction)
		{
			Vector2 velocity = body.velocity;
			velocity.x = direction.x;
			velocity.y = direction.y;
			if (0.00001f < velocity.magnitude)
			{
				body.velocity = velocity.normalized;
			}
			else
			{
				body.velocity = Vector2.zero;
			}
		}

		public void ResetPosition()
		{
			transform.localPosition = Vector3.zero;
			body.velocity = Vector2.zero;
		}

		private void UpdateQuit()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}
		}

		void OnCollisionEnter2D(Collision2D other)
		{
			GameObject otherObject = other.gameObject;
			Debug.Log("OnCollisionEnter2D: other layer " + otherObject.layer
				+ " my layer " + gameObject.layer);
			if (gameObject.layer != otherObject.layer)
			{
				return;
			}
			MayEatSmaller(otherObject);
		}

		void MayEatSmaller(GameObject otherObject)
		{
			Vector3 otherScale = otherObject.transform.localScale;
			float otherRadius = Mathf.Abs(otherScale.x);
			Vector3 scale = transform.localScale;
			float radius = Mathf.Abs(scale.x);
			if (otherRadius * eatRadiusThreshold <= radius)
			{
				float gain = Geometry2D.RadiusDifference(radius, otherRadius);
				scale.x += gain;
				scale.y += gain;
				transform.localScale = scale;
				otherObject.SetActive(false);
			}
		}
	}
}
