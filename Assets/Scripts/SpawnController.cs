using UnityEngine;

namespace Finegamedesign.Utils
{
	public sealed class SpawnController
	{
		public void Update()
		{
		}

		public GameObject SpawnWhereEmpty(string resourceName, GameObject[] spawnPoints)
		{
			GameObject spawnTarget = WhereEmpty(spawnPoints);
			GameObject spawnedObject = PhotonNetwork.Instantiate(
				resourceName, spawnTarget.transform.position,
				Quaternion.identity, 0);
			return spawnedObject;
		}

		private GameObject WhereEmpty(GameObject[] spawnPoints)
		{
			GameObject spawnTarget = null;
			if (null == spawnPoints)
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
