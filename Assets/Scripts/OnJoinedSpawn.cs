using UnityEngine;

namespace Finegamedesign.Utils
{
	public sealed class OnJoinedSpawn : MonoBehaviour
	{
		public GameObject lobbyAnimator;
		public GameObject prefab;
		public GameObject[] spawnPoints;
		private GameObject player;

		void Update()
		{
			if (PhotonNetwork.inRoom)
			{
				if (player == null)
				{
					AnimationView.SetState(lobbyAnimator, "introBegin");
					if (Input.GetKeyDown(KeyCode.Return))
					{
						AnimationView.SetState(lobbyAnimator, "introEnd");
						SpawnWhereEmpty();
					}
				}
			}
			else
			{
				AnimationView.SetState(lobbyAnimator, "connectBegin");
			}
		}

		private void SpawnWhereEmpty()
		{
			GameObject spawnTarget = WhereEmpty();
			player = PhotonNetwork.Instantiate(prefab.name, spawnTarget.transform.position, Quaternion.identity, 0);
		}

		private GameObject WhereEmpty()
		{
			GameObject spawnTarget = null;
			if (null == prefab || null == spawnPoints)
			{
				Debug.Log("OnJoinedRoom: No prefab or no spawn points.");
				return spawnTarget;
			}
			for (int attempt = 0; attempt < spawnPoints.Length; attempt++)
			{
				for (int index = 0; index < spawnPoints.Length; index++)
				{
					GameObject spawn = spawnPoints[index];
					CountTrigger trigger = spawn.GetComponent<CountTrigger>();
					if (null != trigger && trigger.count <= attempt)
					{
						spawnTarget = spawn;
						return spawnTarget;
					}
				}
			}
			if (null == spawnTarget)
			{
				spawnTarget = spawnPoints[0];
			}
			return spawnTarget;
		}
	}
}
