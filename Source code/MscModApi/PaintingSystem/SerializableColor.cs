using System;
using UnityEngine;

namespace MscModApi.PaintingSystem
{
	/// <summary>
	/// Class wrapping the Color class to make it serializable
	/// </summary>
	[Serializable]
	public class SerializableColor
	{
		public float r;
		public float g;
		public float b;
		public float a;

		/// <summary>
		/// Creates a serializable color object
		/// </summary>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		/// <param name="a"></param>
		public SerializableColor(float r, float g, float b, float a)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		/// <summary>
		/// converts a SerializableColor object into a Color object.
		/// </summary>
		/// <param name="color"></param>
		public static implicit operator Color(SerializableColor color)
		{
			return new Color(color.r, color.g, color.b, color.a);
		}

		/// <summary>
		/// Converts a Color object into a SerializableColor object.
		/// </summary>
		/// <param name="color"></param>
		public static implicit operator SerializableColor(Color color)
		{
			return new SerializableColor(color.r, color.g, color.b, color.a);
		}
	}
}