using UnityEngine;

namespace FineGameDesign.SmallWorld
{
	public sealed class PhotonBody : MonoBehaviour
	{
	 	PhotonView photon;
	 	RigidBody2D body;

		void Awake()
		{
			body = GetComponent<RigidBody2D>();
			photon = GetComponent<PhotonView>();
		}

		void FixedUpdate()
		{
			if (!photon.isMine)
			{
				return;
			}
			UpdateMovement();
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
			body.velocity = velocity;
		}
	}
}
