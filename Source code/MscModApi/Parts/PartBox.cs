using System;
using System.Collections.Generic;
using System.Linq;
using MscModApi.Tools;
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

		public override GameObject gameObject { get; protected set; }

		/// <summary>
		/// Returns the name of the box model
		/// </summary>
		public override string name => gameObject.name;

		/// <summary>
		/// Returns if the player is currently looking at the box
		/// </summary>
		public override bool isLookingAt => gameObject.IsLookingAt();

		/// <summary>
		/// Returns if the player is currently holding the box
		/// </summary>
		public override bool isHolding => gameObject.IsHolding();

		internal void IncrementUnpackedCount()
		{
			if (!hasPartsToUnpack) {
				return;
			}

			partsUnpackedCount++;
		}

		public override bool installBlocked
		{
			get => childs.All(childPart => childPart.installBlocked);
			set
			{
				foreach (Part childPart in childs)
				{
					childPart.installBlocked = value;
				}
			}
		}

		/// <summary>
		/// Returns if there are still parts that can be unpacked
		/// </summary>
		public bool hasPartsToUnpack => partsUnpackedCount < partsCount;

		/// <summary>
		/// Returns the number of parts contained in this box
		/// </summary>
		public int partsCount => childs.Count;

		/// <summary>
		/// The position of the box model
		/// </summary>
		public override Vector3 position
		{
			get => gameObject.transform.position;
			set => gameObject.transform.position = value;
		}

		/// <summary>
		/// The rotation of the box model.
		/// </summary>
		public override Vector3 rotation
		{
			get => gameObject.transform.rotation.eulerAngles;
			set => gameObject.transform.rotation = Quaternion.Euler(value);
		}

		/// <summary>
		/// Is the box (and all parts contained in the box) bought
		/// </summary>
		public override bool bought
		{
			get { return childs.Any(part => part.bought); }
			set
			{
				foreach (Part part in childs) {
					part.bought = value;
				}
			}
		}

		/// <summary>
		/// Returns if all parts contained in this box are installed
		/// (Only made available through inheritance, rare use cases)
		/// </summary>
		public override bool installed => childs.All(part => part.installed);

		public new List<Part> childs = new List<Part>();


		/// <summary>
		/// Returns if all parts contained in this box are bolted
		/// (Only made available through inheritance, rare use cases)
		/// </summary>
		public override bool bolted => childs.All(part => part.bolted);


		/// <summary>
		/// Returns if all parts contained in this box are installed on the car
		/// </summary>
		public override bool installedOnCar => childs.All(part => part.installedOnCar);

		/// <summary>
		/// Is the box model gameObject active
		/// </summary>
		public override bool active
		{
			get => gameObject.activeSelf;
			set => gameObject.SetActive(value);
		}

		/// <summary>
		/// Executed when the game saves to make sure any parts not yet "manually" unpacked are unpacked and have a proper position on load.
		/// </summary>
		public void CheckUnpackedOnSave()
		{
			if (!bought) {
				return;
			}

			if (hasPartsToUnpack) {
				foreach (var part in childs.Where(part => !part.installed && !part.gameObject.activeSelf)) {
					part.position = gameObject.transform.position;
					part.active = active;
				}
			}

			gameObject.SetActive(false);
			gameObject.transform.position = new Vector3(0, 0, 0);
			gameObject.transform.localPosition = new Vector3(0, 0, 0);
		}

		/// <summary>
		/// Resets both the box model gameObject as well as the parts to their defaultPosition & defaultRotation
		/// </summary>
		/// <param name="uninstall">Should an installed part be uninstalled prior to resetting</param>
		public override void ResetToDefault(bool uninstall = false)
		{
			if (active) {
				position = defaultPosition;
				rotation = defaultRotation;
			}

			foreach (Part part in childs) {
				if (uninstall && part.installed) {
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
			AddChild(part);
		}
        

		/// <summary>
		/// Adds multiple parts to the box
		/// </summary>
		/// <param name="parts"></param>
		protected void AddParts(IEnumerable<Part> parts)
		{
			foreach (Part part in parts) {
				AddPart(part);
			}
		}

		/// <inheritdoc />
		public override void Uninstall()
		{
			//Not Implemented for PartBox
		}

		[Obsolete("Use 'AddParts' method instead, this method actually doesn't set but Add parts", true)]
		protected void SetParts(IEnumerable<Part> parts)
		{
			AddParts(parts);
		}

		[Obsolete("Use 'parts' property instead", true)]
		public List<Part> GetParts()
		{
			return childs;
		}

		[Obsolete("Use 'partsCount' property instead", true)]
		public int GetPartCount()
		{
			return childs.Count;
		}

		[Obsolete("Use 'gameObject' property instead", true)]
		public GameObject GetBoxGameObject()
		{
			return gameObject;
		}

		[Obsolete("Use 'gameObject' property instead", true)]
		internal void SetBoxGameObject(GameObject box)
		{
			gameObject = box;
		}

		[Obsolete("Use 'bought' property instead.", true)]
		public bool AnyBought()
		{
			return bought;
		}

		[Obsolete("Use 'bought' property instead.", true)]
		public bool IsBought()
		{
			return bought;
		}
	}
}