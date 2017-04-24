using UnityEngine;
using Finegamedesign.Utils;

namespace Finegamedesign.SmallWorld
{
	[System.Serializable]
	public sealed class SpawnController
	{
		public GameObject maintainedResource;
		public GameObject[] maintainedObjects;
		public GameObject[] spawnPoints;
		public float emptyTime = 10.0f;
		public float spawnTime = -1.0f;
		public float spawnDelay = 2.0f;
		private int spawnIndex = 0;

		public void Setup()
		{
			maintainedObjects = new GameObject[spawnPoints.Length];
		}

		public void Update()
		{
			UpdateMaintainedObjects();
		}

		public GameObject SpawnWhereEmpty(string resourceName, GameObject[] spawnPoints, GameObject spawnedObject = null)
		{
			GameObject spawnTarget = WhereEmpty(spawnPoints);
			if (null == spawnedObject)
			{
				spawnedObject = PhotonNetwork.Instantiate(
					resourceName, spawnTarget.transform.position,
					Quaternion.identity, 0);
			}
			else if (null != spawnTarget)
			{
				PhotonBody photonBody = spawnedObject.GetComponent<PhotonBody>();
				if (null != photonBody)
				{
					photonBody.Setup();
					PhotonBody.RPC(photonBody.photon,
						"Spawn",
						spawnTarget.transform.position);
				}
			}
			return spawnedObject;
		}

		private GameObject WhereEmpty(GameObject[] spawnPoints)
		{
			GameObject spawnTarget = null;
			float readyTime = Time.time - emptyTime;
			int spawnLength = spawnPoints.Length;
			if (null == spawnPoints)
			{
				Debug.Log("OnJoinedRoom: No prefab or no spawn points.");
				return spawnTarget;
			}
			int attemptCount = 1;
			for (int attempt = 0; attempt < attemptCount; attempt++)
			{
				float attemptTime = readyTime + emptyTime - 0.5f * attempt / attemptCount;
				for (int index = 0; index < spawnLength; index++)
				{
					int pointIndex = (spawnIndex + index) % spawnLength;
					GameObject spawn = spawnPoints[pointIndex];
					CountTrigger trigger = spawn.GetComponent<CountTrigger>();
					if (null != trigger && trigger.count <= 0 && trigger.emptyTime <= attemptTime)
					{
						spawnIndex = (pointIndex + 1) % spawnLength;
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
			if (Time.time < spawnTime)
			{
				return;
			}
			for (int index = 0; index < maintainedObjects.Length; index++)
			{
				GameObject maintainedObject = maintainedObjects[index];
				if (null != maintainedObject && maintainedObject.activeSelf)
				{
					continue;
				}
				maintainedObject = SpawnWhereEmpty(maintainedResource.name, spawnPoints, maintainedObject);
				maintainedObjects[index] = maintainedObject;
				spawnTime = Time.time + spawnDelay;
			}
		}
	}
}
