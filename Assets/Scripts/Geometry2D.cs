using UnityEngine;

namespace FineGameDesign.Utils
{
	public sealed class Geometry2D
	{
		public static float RadiusDifference(float radius, float otherRadius)
		{
			float area = Mathf.PI * Mathf.Pow(radius, 2);
			float otherArea = Mathf.PI * Mathf.Pow(otherRadius, 2);
			float combinedArea = area + otherArea;
			float combinedRadius = Mathf.Sqrt(combinedArea / Mathf.PI);
			return combinedRadius - radius;
		}
	}
}
