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

		private void Rank(GameObject player)
		{
			playerRank = players.IndexOf(player);
			if (-1 == playerRank)
			{
				playerRank = players.Count;
			}
			else
			{
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
