using UnityEngine;
using System.Collections.Generic;
using Finegamedesign.Utils;

namespace Finegamedesign.SmallWorld
{
	public sealed class OnJoinedSpawn : MonoBehaviour
	{
		public GameObject playerCamera;
		private Vector3 cameraLobby;
		public GameObject lobbyAnimator;
		public GameObject prefab;
		public GameObject[] spawnPoints;
		private GameObject player;
		[SerializeField]
		public SpawnController spawn = new SpawnController();
		[SerializeField]
		public SizeReferee referee = new SizeReferee();
		public int maxBots = 10;
		public float botSpawnDelay = 2.0f;
		public float botSpawnTime = 2.0f;
		public GameObject botResource;
		private List<GameObject> bots = new List<GameObject>();
		private string lobbyState = "connectBegin";

		void Start()
		{
			referee.players = PhotonBody.instances;
			spawn.spawnPoints = spawnPoints;
			spawn.Setup();
			cameraLobby = playerCamera.transform.position;
		}

		void Update()
		{
			if (PhotonNetwork.inRoom)
			{
				if (player == null || !player.activeSelf)
				{
					AudioListener.volume = 0.25f;
					if (Input.GetKeyDown(KeyCode.Return))
					{
						AudioListener.volume = 1.0f;
						lobbyState = "introEnd";
						player = spawn.SpawnWhereEmpty(prefab.name, spawnPoints, player);
						referee.StartScale(player);
					}
					else if (referee.isGameBeginOnce)
					{
						lobbyState = "gameEnd";
					}
					else
					{
						lobbyState = "introBegin";
					}
				}
				Follow(playerCamera, player, cameraLobby);
			}
			else
			{
				lobbyState = "connectBegin";
			}
			if (PhotonNetwork.isMasterClient)
			{
				spawn.Update();
				UpdateBot();
			}
			referee.player = player;
			referee.Update();
			AnimationView.SetState(lobbyAnimator, lobbyState);
		}

		private void Follow(GameObject follower, GameObject leader, Vector3 reset)
		{
			if (leader == null || !leader.activeSelf)
			{
				return;
			}
			Vector3 position = leader.transform.position;
			position.z = follower.transform.position.z;
			follower.transform.position = position;
		}

		public void UpdateBot()
		{
			if (!referee.isGameBeginOnce || referee.isGameEnd)
			{
				botSpawnTime = Time.time + botSpawnDelay;
				return;
			}
			if (Time.time < botSpawnTime)
			{
				return;
			}
			if (referee.players.Count < maxBots)
			{
				if (bots.Count < maxBots)
				{
					bots.Add(null);
				}
			}
			int botCount = 0;
			for (int index = 0; index < bots.Count; index++)
			{
				GameObject bot = bots[index];
				bool isActive = null != bot && bot.GetComponent<CellBot>().player.activeSelf;
				if (isActive)
				{
					botCount++;
					if (maxBots <= botCount)
					{
						break;
					}
				}
				else
				{
					bot = spawn.SpawnWhereEmpty(botResource.name, spawnPoints, bot);
					CellBot cellBot = bot.GetComponent<CellBot>();
					cellBot.Spawn();
					referee.StartScale(cellBot.player);
					botSpawnTime = Time.time + botSpawnDelay;
					bots[index] = bot;
					break;
				}
			}
		}
	}
}
