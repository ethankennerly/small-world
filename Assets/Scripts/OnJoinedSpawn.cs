using UnityEngine;

namespace FineGameDesign.Utils
{
	public sealed class OnJoinedSpawn : MonoBehaviour
	{
		public GameObject prefab;
		public GameObject[] spawnPoints;

		public void OnJoinedRoom()
		{
			GameObject spawnTarget = SpawnWhereEmpty();
			PhotonNetwork.Instantiate(prefab.name, spawnTarget.transform.position, Quaternion.identity, 0);
		}

		private GameObject SpawnWhereEmpty()
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
