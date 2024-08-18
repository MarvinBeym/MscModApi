using System;
using System.Collections.Generic;
using MscModApi.Saving;
using UnityEngine;

namespace MscModApi.Parts
{
	internal class PartSave
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

}