using UnityEngine;
using FineGameDesign.Utils;

namespace FineGameDesign.SmallWorld
{
	public sealed class PhotonBody : MonoBehaviour
	{
		public static float eatRadiusThreshold = 1.125f;
		public static int playerLayer = 8;

	 	PhotonView photon;
	 	Rigidbody2D body;

		void Awake()
		{
			body = GetComponent<Rigidbody2D>();
			photon = GetComponent<PhotonView>();
		}

		void FixedUpdate()
		{
			if (!photon.isMine)
			{
				return;
			}
			UpdateMovement();
			UpdateQuit();
		}

		void UpdateMovement()
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

		void UpdateQuit()
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
				Object.Destroy(otherObject);
			}
		}
	}
}
