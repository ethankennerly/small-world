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
		public float winScale = 10.0f;
		public int playerRank;
		public GameObject player;

		public void Update()
		{
			UpdateRank();
		}

		private void UpdateRank()
		{
			ReverseByScaleX(players);
			Rank(player);
			string text;
			if (0 == playerRank)
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

		private void ReverseByScaleX(List<GameObject> scaledObjects)
		{
			scaledObjects.Sort(
				delegate(GameObject a, GameObject b)
				{
					return b.transform.localScale.x.CompareTo(
						a.transform.localScale.x);
				}
			);
		}
	}
}
