using UnityEngine;
using System.Collections.Generic;
using Finegamedesign.Utils;

namespace Finegamedesign.SmallWorld
{
	[System.Serializable]
	public sealed class SizeReferee
	{
		public List<GameObject> players;
		public GameObject rankText;
		public string rankFormat = "#{0}";
		public string rankEmpty = "?";
		public float startScale = 0.5f;
		public float winScale = 5.0f;
		public int playerRank;
		public GameObject player;
		public bool isGameBeginOnce = false;
		public bool isGameEnd = false;
		public int playerActiveCount = 0;

		public void Update()
		{
			UpdateGameEnd();
			if (!isGameEnd)
			{
				UpdateRank();
			}
		}

		private void UpdateGameEnd()
		{
			int index;
			GameObject aPlayer;
			playerActiveCount = 0;
			for (index = 0; index < players.Count; index++)
			{
				aPlayer = players[index];
				if (aPlayer != null && aPlayer.activeSelf)
				{
					playerActiveCount++;
				}
			}
			if (isGameEnd && 1 <= playerActiveCount)
			{
				isGameEnd = false;
			}
			else if (playerActiveCount <= 0)
			{
				isGameEnd = true;
			}
			else
			{
				isGameEnd = IsBiggest(winScale);
			}
			if(!isGameEnd)
			{
				return;
			}
			for (index = 0; index < players.Count; index++)
			{
				aPlayer = players[index];
				aPlayer.SetActive(false);
				StartScale(aPlayer);
			}
		}

		public void StartScale(GameObject player)
		{
			Vector3 scale = player.transform.localScale;
			scale.x = startScale;
			scale.y = startScale;
			scale.z = startScale;
			player.transform.localScale = scale;
		}

		private bool IsBiggest(float scale)
		{
			if (null == players || players.Count <= 0)
			{
				return false;
			}
			if (null == players[0])
			{
				return false;
			}
			return scale <= players[0].transform.localScale.x;
		}

		private void UpdateRank()
		{
			for (int index = players.Count - 1; 0 <= index; index--)
			{
				if (null == players[index])
				{
					players.RemoveAt(index);
				}
			}
			ReverseByScaleX(players);
			Rank(player);
			if (null == player || !player.activeSelf)
			{
				return;
			}
			isGameBeginOnce = true;
			string text;
			if (0 == playerRank || !isGameBeginOnce)
			{
				text = rankEmpty;
			}
			else
			{
				text = rankFormat.Replace("{0}", playerRank.ToString());
			}
			TextView.SetText(rankText, text);
		}

		// Resolve ties to worse rank.
		private void Rank(GameObject player)
		{
			playerRank = players.IndexOf(player);
			if (-1 == playerRank)
			{
				playerRank = players.Count;
			}
			else
			{
				for ( ; playerRank < players.Count - 1; playerRank++)
				{
					if (players[playerRank].transform.localScale.x
					> players[playerRank + 1].transform.localScale.x)
					{
						break;
					}
				}
				playerRank++;
			}
		}

		public static void ReverseByScaleX(List<GameObject> scaledObjects)
		{
			scaledObjects.Sort(
				delegate(GameObject a, GameObject b)
				{
					float aScale = 0.0f;
					if (null != a)
					{
						aScale = a.transform.localScale.x;
					}
					float bScale = 0.0f;
					if (null != b)
					{
						bScale = b.transform.localScale.x;
					}
					return bScale.CompareTo(aScale);
				}
			);
		}
	}
}
