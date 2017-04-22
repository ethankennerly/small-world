using UnityEngine;

namespace Finegamedesign.Utils
{
	public sealed class OnJoinedSpawn : MonoBehaviour
	{
		public GameObject lobbyAnimator;
		public GameObject prefab;
		public GameObject[] spawnPoints;
		private GameObject player;
		[SerializeField]
		public SpawnController spawn = new SpawnController();

		void Start()
		{
			spawn.spawnPoints = spawnPoints;
			spawn.Setup();
		}

		void Update()
		{
			if (PhotonNetwork.inRoom)
			{
				if (player == null || !player.activeSelf)
				{
					AnimationView.SetState(lobbyAnimator, "introBegin");
					if (Input.GetKeyDown(KeyCode.Return))
					{
						AnimationView.SetState(lobbyAnimator, "introEnd");
						player = spawn.SpawnWhereEmpty(prefab.name, spawnPoints, player);
					}
				}
			}
			else
			{
				AnimationView.SetState(lobbyAnimator, "connectBegin");
			}
			if (PhotonNetwork.isMasterClient)
			{
				spawn.Update();
			}
		}
	}
}
