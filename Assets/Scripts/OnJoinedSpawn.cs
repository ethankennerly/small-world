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
		public GameObject botResource;
		private List<GameObject> bots = new List<GameObject>();

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
					AnimationView.SetState(lobbyAnimator, "introBegin");
					if (Input.GetKeyDown(KeyCode.Return))
					{
						AnimationView.SetState(lobbyAnimator, "introEnd");
						player = spawn.SpawnWhereEmpty(prefab.name, spawnPoints, player);
						referee.StartScale(player);
					}
				}
				Follow(playerCamera, player, cameraLobby);
			}
			else
			{
				AnimationView.SetState(lobbyAnimator, "connectBegin");
			}
			if (PhotonNetwork.isMasterClient)
			{
				spawn.Update();
				UpdateBot();
			}
			referee.player = player;
			referee.Update();
			if (referee.isGameEnd && null != player)
			{
				player.SetActive(false);
			}
		}

		private void Follow(GameObject follower, GameObject leader, Vector3 reset)
		{
			if (leader == null || !leader.activeSelf)
			{
				follower.transform.position = reset;
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
					cellBot.playerBehaviour.isBot = true;
					cellBot.player.SetActive(true);
					bots[index] = bot;
					break;
				}
			}
		}
	}
}
