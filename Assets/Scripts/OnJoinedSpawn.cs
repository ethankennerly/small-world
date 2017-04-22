using UnityEngine;

namespace Finegamedesign.Utils
{
	public sealed class OnJoinedSpawn : MonoBehaviour
	{
		public GameObject lobbyAnimator;
		public GameObject prefab;
		public GameObject[] spawnPoints;
		private GameObject player;
		private SpawnController spawn = new SpawnController();

		void Update()
		{
			if (PhotonNetwork.isMasterClient)
			{
				spawn.Update();
			}
			if (PhotonNetwork.inRoom)
			{
				if (player == null)
				{
					AnimationView.SetState(lobbyAnimator, "introBegin");
					if (Input.GetKeyDown(KeyCode.Return))
					{
						AnimationView.SetState(lobbyAnimator, "introEnd");
						player = spawn.SpawnWhereEmpty(prefab.name, spawnPoints);
					}
				}
			}
			else
			{
				AnimationView.SetState(lobbyAnimator, "connectBegin");
			}
		}
	}
}
