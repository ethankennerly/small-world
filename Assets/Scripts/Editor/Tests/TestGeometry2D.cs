using NUnit.Framework;

namespace FineGameDesign.Utils
{
	public sealed class TestGeometry2D
	{
		[Test]
		public void RadiusDifference()
		{
			Assert.AreEqual(1.0f, Geometry2D.RadiusDifference(4.0f, 3.0f));
		}
	}
}
