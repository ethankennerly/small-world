using UnityEngine;

namespace FineGameDesign.SmallWorld
{
	public sealed class PhotonBody : MonoBehaviour
	{
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
	}
}
