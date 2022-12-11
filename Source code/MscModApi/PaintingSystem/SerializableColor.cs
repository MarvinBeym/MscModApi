using System;
using UnityEngine;

namespace MscModApi.PaintingSystem
{
	[Serializable]
	public class SerializableColor
	{
		public float r;
		public float g;
		public float b;
		public float a;

		public SerializableColor(float r, float g, float b, float a)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public static implicit operator Color(SerializableColor color)
		{
			return new Color(color.r, color.g, color.b, color.a);
		}

		public static implicit operator SerializableColor(Color color)
		{
			return new SerializableColor(color.r, color.g, color.b, color.a);
		}
	}
}