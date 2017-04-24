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
		public bool isStatic = false;
		public float particleResetDelay = 6.0f;

	 	private PhotonView photon;
	 	public Rigidbody2D body;
		public GameObject eatenParticle;
		private GameObject particle;
		private float particleResetTime = -1.0f;

		void Awake()
		{
			if (!isStatic && instances.IndexOf(gameObject) <= -1)
			{
				instances.Add(gameObject);
			}
			body = GetComponent<Rigidbody2D>();
			photon = GetComponent<PhotonView>();
			if (null == particle)
			{
				particle = (GameObject) GameObject.Instantiate(eatenParticle, transform.position, Quaternion.identity);
				particle.SetActive(false);
			}
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
				otherObject.GetComponent<PhotonView>().RPC("OnEaten", PhotonTargets.All);
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

		void OnEnable()
		{
			photon.RPC("Enable", PhotonTargets.All);
		}

		[PunRPC]
		public void Enable()
		{
			if (null != gameObject && !gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}
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
