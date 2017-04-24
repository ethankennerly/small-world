using UnityEngine;
using System.Collections.Generic;
using Finegamedesign.Utils;

namespace Finegamedesign.SmallWorld
{
	[RequireComponent(typeof(Rigidbody2D), typeof(PhotonView))]
	public sealed class PhotonBody : MonoBehaviour
	{
		public static float eatRadiusThreshold = 1.125f;
		public static int playerLayer = 8;
		public static List<GameObject> mobiles = new List<GameObject>();
		public bool isBot = false;
		public bool isStatic = false;
		public float particleResetDelay = 6.0f;
		public float startScale = 0.5f;

	 	public PhotonView photon;
	 	public Rigidbody2D body;
		public GameObject eatenParticle;
		private GameObject particle;
		private float particleResetTime = -1.0f;

		// AllBufferedViaServer
		// """ Sends the RPC to everyone (including this client) through the server and buffers it for players joining later.

		// This client executes the RPC like any other when it received it from the server. Benefit: The server's order of sending the RPCs is the same on all clients."""
		// https://doc-api.photonengine.com/en/pun/current/group__public_api.html

		public static void RPC(PhotonView owner, string methodName)
		{
			owner.RPC(methodName, PhotonTargets.All);
		}

		public static void RPC(PhotonView owner, string methodName, Vector3 position)
		{
			owner.RPC(methodName, PhotonTargets.All, position);
		}

		public void Awake()
		{
			Setup();
		}

		void OnEnable()
		{
			Setup();
		}

		public void Setup()
		{
			if (!isStatic && mobiles.IndexOf(gameObject) <= -1)
			{
				mobiles.Add(gameObject);
			}
			if (!isStatic)
			{
				StartScale();
			}
			body = GetComponent<Rigidbody2D>();
			photon = GetComponent<PhotonView>();
			if (null == particle)
			{
				particle = (GameObject) GameObject.Instantiate(eatenParticle, transform.position, Quaternion.identity);
				particle.SetActive(false);
			}
		}

		public void StartScale()
		{
			Vector3 scale = transform.localScale;
			scale.x = startScale;
			scale.y = startScale;
			scale.z = startScale;
			transform.localScale = scale;
		}

		void FixedUpdate()
		{
			if (null != gameObject && gameObject.activeSelf && particleResetTime <= Time.time)
			{
				particle.SetActive(false);
			}
			if (!photon.isMine || isBot || isStatic)
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
			if (null != gameObject && gameObject.activeSelf && null != particle)
			{
				particle.SetActive(false);
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
			if (null == otherObject)
			{
				return;
			}
			if (gameObject.layer != otherObject.layer)
			{
				return;
			}
			if (MayEatSmaller(otherObject))
			{
				otherObject.GetComponent<PhotonView>().RPC(
					"OnEaten", PhotonTargets.All);
			}
		}

		bool MayEatSmaller(GameObject otherObject)
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
				return true;
			}
			return false;
		}

		[PunRPC]
		void Spawn(Vector3 position)
		{
			Setup();
			if (null != gameObject && !gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}
			transform.position = position;
		}

		[PunRPC]
		public void OnEaten()
		{
			if (null == particle)
			{
				return;
			}
			Vector3 position = transform.position;
			position.z -= 2.0f;
			Vector3 scale = transform.localScale;
			scale.z = scale.x;
			scale.y = scale.x;
			particle.transform.position = position;
			particle.transform.localScale = scale;
			particle.SetActive(true);
			particleResetTime = Time.time + particleResetDelay;
			gameObject.SetActive(false);
		}
	}
}
