using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MscModApi.Parts
{
	/// <summary>
	/// The base class for every object acting as a "box" (objects that contain Part objects)
	/// </summary>
	public abstract class PartBox : BasicPart
	{
		/// <summary>
		/// Returns internal unpacked counter (starting at 0)
		/// </summary>
		public int partsUnpackedCount { get; protected set; } = 0;

		internal void IncrementUnpackedCount()
		{
			if (!hasPartsToUnpack)
			{
				return;
			}

			partsUnpackedCount++;
		}

		/// <summary>
		/// Returns if there are still parts that can be unpacked
		/// </summary>
		public bool hasPartsToUnpack => partsUnpackedCount < partsCount;

		/// <summary>
		/// Returns the box GameObject model
		/// </summary>
		public GameObject boxModel { get; protected set; }

		/// <summary>
		/// Returns the list of parts contained in this box
		/// </summary>
		public List<Part> parts { get; protected set; } = new List<Part>();

		/// <summary>
		/// Returns the number of parts contained in this box
		/// </summary>
		public int partsCount => parts.Count;

		/// <summary>
		/// The position of the box model
		/// </summary>
		public override Vector3 position
		{
			get => boxModel.transform.position;
			set => boxModel.transform.position = value;
		}

		/// <summary>
		/// The rotation of the box model.
		/// </summary>
		public override Vector3 rotation
		{
			get => boxModel.transform.rotation.eulerAngles;
			set => boxModel.transform.rotation = Quaternion.Euler(value);
		}

		/// <summary>
		/// Is the box (and all parts contained in the box) bought
		/// </summary>
		public override bool bought
		{
			get { return parts.Any(part => part.bought); }
			set
			{
				foreach (Part part in parts)
				{
					part.bought = value;
				}
			}
		}

		/// <summary>
		/// Is the box model gameObject active
		/// </summary>
		public override bool active
		{
			get => boxModel.activeSelf;
			set => boxModel.SetActive(value);
		}

		/// <summary>
		/// Executed when the game saves to make sure any parts not yet "manually" unpacked are unpacked and have a proper position on load.
		/// </summary>
		public void CheckUnpackedOnSave()
		{
			if (!bought)
			{
				return;
			}

			if (hasPartsToUnpack)
			{
				foreach (var part in parts.Where(part => !part.installed && !part.gameObject.activeSelf))
				{
					part.position = boxModel.transform.position;
					part.active = active;
				}
			}

			boxModel.SetActive(false);
			boxModel.transform.position = new Vector3(0, 0, 0);
			boxModel.transform.localPosition = new Vector3(0, 0, 0);
		}

		/// <summary>
		/// Resets both the box model gameObject as well as the parts to their defaultPosition & defaultRotation
		/// </summary>
		/// <param name="uninstall">Should an installed part be uninstalled prior to resetting</param>
		public override void ResetToDefault(bool uninstall = false)
		{
			if (active)
			{
				position = defaultPosition;
				rotation = defaultRotation;
			}

			foreach (Part part in parts)
			{
				if (uninstall && part.installed)
				{
					part.Uninstall();
				}

				part.position = defaultPosition;
				part.rotation = defaultRotation;
			}
		}

		/// <summary>
		/// Adds a part to the box
		/// </summary>
		/// <param name="part"></param>
		protected void AddPart(Part part)
		{
			parts.Add(part);
		}

		/// <summary>
		/// Adds multiple parts to the box
		/// </summary>
		/// <param name="parts"></param>
		protected void AddParts(IEnumerable<Part> parts)
		{
			foreach (Part part in parts)
			{
				AddPart(part);
			}
		}

		[Obsolete("Use 'AddParts' method instead, this method actually doesn't set but Add parts")]
		protected void SetParts(IEnumerable<Part> parts)
		{
			AddParts(parts);
		}

		[Obsolete("Use 'parts' property instead")]
		public List<Part> GetParts()
		{
			return parts;
		}

		[Obsolete("Use 'partsCount' property instead")]
		public int GetPartCount()
		{
			return parts.Count;
		}

		[Obsolete("Use 'boxModel' property instead")]
		public GameObject GetBoxGameObject()
		{
			return boxModel;
		}

		[Obsolete("Use 'boxModel' property instead")]
		internal void SetBoxGameObject(GameObject box)
		{
			boxModel = box;
		}

		[Obsolete("Use 'bought' property instead.")]
		public bool AnyBought()
		{
			return bought;
		}

		[Obsolete("Use 'bought' property instead.")]
		public bool IsBought()
		{
			return bought;
		}
	}
}