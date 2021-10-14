using System;
using System.Collections.Generic;
using UnityEngine;

namespace MscModApi.Parts
{
	public class PartSave
	{
		public enum BoughtState
		{
			No,
			Yes,
			NotConfigured
		}

		public bool installed = false;
		public BoughtState bought = BoughtState.NotConfigured;
		public List<Screw> screws = new List<Screw>();
		public SerializableVector3 position = new SerializableVector3();
		public SerializableQuaternion rotation = new SerializableQuaternion();
	}

	[Serializable]
	public struct SerializableVector3
	{
		public float x;
		public float y;
		public float z;

		public SerializableVector3(float rX, float rY, float rZ)
		{
			x = rX;
			y = rY;
			z = rZ;
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

	[Serializable]
	public struct SerializableQuaternion
	{
		public float x;
		public float y;
		public float z;
		public float w;

		public SerializableQuaternion(float rX, float rY, float rZ, float rW)
		{
			x = rX;
			y = rY;
			z = rZ;
			w = rW;
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