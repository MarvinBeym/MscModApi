using UnityEngine;

namespace MscModApi.Saving
{
	/// <summary>
	/// A wrapper for Unity Vector3 to make them serializable
	/// </summary>
	public class SerializableVector3
	{
		public float x;
		public float y;
		public float z;

		public SerializableVector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public SerializableVector3()
		{
			x = 0;
			y = 0;
			z = 0;
		}

		public override string ToString()
		{
			return $"[{x}, {y}, {z}]";
		}

		public static implicit operator Vector3(SerializableVector3 rValue)
		{
			return new Vector3(rValue.x, rValue.y, rValue.z);
		}

		public static implicit operator SerializableVector3(Vector3 rValue)
		{
			return new SerializableVector3(rValue.x, rValue.y, rValue.z);
		}
	}
}