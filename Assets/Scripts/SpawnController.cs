using UnityEngine;

namespace Finegamedesign.Utils
{
	[System.Serializable]
	public sealed class SpawnController
	{
		public GameObject maintainedResource;
		public GameObject[] maintainedObjects;
		public GameObject[] spawnPoints;
		private int spawnIndex = 0;

		public void Setup()
		{
			maintainedObjects = new GameObject[spawnPoints.Length];
		}

		public void Update()
		{
			UpdateMaintainedObjects();
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
					int pointIndex = (spawnIndex + index) % spawnPoints.Length;
					GameObject spawn = spawnPoints[pointIndex];
					CountTrigger trigger = spawn.GetComponent<CountTrigger>();
					if (null != trigger && trigger.count <= attempt)
					{
						spawnIndex = (pointIndex + 1) % spawnPoints.Length;
						spawnTarget = spawn;
						return spawnTarget;
					}
				}
			}
			if (null == spawnTarget)
			{
				spawnTarget = spawnPoints[spawnIndex];
				spawnIndex = (spawnIndex + 1) % spawnPoints.Length;
			}
			return spawnTarget;
		}

		private void UpdateMaintainedObjects()
		{
			for (int index = 0; index < maintainedObjects.Length; index++)
			{
				GameObject maintainedObject = maintainedObjects[index];
				if (null != maintainedObject)
				{
					continue;
				}
				maintainedObject = SpawnWhereEmpty(maintainedResource.name, spawnPoints);
				maintainedObjects[index] = maintainedObject;
			}
		}
	}
}
