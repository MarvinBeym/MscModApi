using System;
using UnityEngine;

namespace MscModApi.Saving
{
	/// <summary>
	/// A wrapper for Unity Quaternion to make them serializable
	/// </summary>
	[Serializable]
	public class SerializableQuaternion
	{
		public float x;
		public float y;
		public float z;
		public float w;

		public SerializableQuaternion(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public SerializableQuaternion()
		{
			x = 0;
			y = 0;
			z = 0;
			w = 0;
		}

		public override string ToString()
		{
			return $"[{x}, {y}, {z},{w}]";
		}

		public static implicit operator Quaternion(SerializableQuaternion rValue)
		{
			return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
		}

		public static implicit operator SerializableQuaternion(Quaternion rValue)
		{
			return new SerializableQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
		}
	}
}