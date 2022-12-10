using System.Collections.Generic;
using MscModApi.Parts;
using UnityEngine;

namespace MscModApi.Shopping
{
	public abstract class PartBox
	{
		private GameObject box;
		private List<Part> parts = new List<Part>();

		internal abstract void CheckUnpackedOnSave();

		protected void SetParts(IEnumerable<Part> parts)
		{
			foreach (Part part in parts)
			{
				AddPart(part);
			}
		}

		protected void AddPart(Part part)
		{
			parts.Add(part);
		}

		public List<Part> GetParts()
		{
			return parts;
		}

		public int GetPartCount()
		{
			return parts.Count;
		}

		public GameObject GetBoxGameObject()
		{
			return box;
		}

		internal void SetBoxGameObject(GameObject box)
		{
			this.box = box;
		}
	}
}