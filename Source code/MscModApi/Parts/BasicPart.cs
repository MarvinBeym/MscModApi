using System;
using System.Collections.Generic;
using UnityEngine;

namespace MscModApi.Parts
{
	/// <summary>
	/// The absolute base class for objects acting like a part (Part, Box, Kit)
	/// </summary>
	public abstract class BasicPart
	{
		protected List<BasicPart> childParts = new List<BasicPart>();

		public abstract GameObject gameObject { get; protected set; }

		/// <summary>
		/// The default position, used for example when resetting.
		/// </summary>
		public Vector3 defaultPosition { get; set; } = Vector3.zero;

		/// <summary>
		/// The default rotation, used for example when resetting.
		/// </summary>
		public Vector3 defaultRotation { get; set; } = Vector3.zero;

		public bool uninstallWhenParentUninstalls { get; protected set; }

		/// <summary>
		/// Is the part bought
		/// </summary>
		public abstract bool bought { get; set; }

		/// <summary>
		/// The position of the part
		/// </summary>
		public abstract Vector3 position { get; set; }

		/// <summary>
		/// The rotation of the part.
		/// </summary>
		public abstract Vector3 rotation { get; set; }


		/// <summary>
		/// Is the part gameObject active
		/// </summary>
		public abstract bool active { get; set; }

		/// <summary>
		/// Returns the name of the part
		/// </summary>
		public abstract string name { get; }

		/// <summary>
		/// Returns a clean name of the part (Removing '(Clone)')
		/// </summary>
		public string cleanName => name.Replace("(Clone)", "");

		/// <summary>
		/// Returns if the player is currently looking at this part
		/// </summary>
		public abstract bool isLookingAt { get; }

		/// <summary>
		/// Returns if the player is currently holding this part
		/// </summary>
		public abstract bool isHolding { get; }

		/// <summary>
		/// Returns if the part is currently installed to it's parent
		/// </summary>
		public abstract bool installed { get; }

		/// <summary>
		/// Returns if the part is currently bolted to it's parent
		/// </summary>
		public abstract bool bolted { get; }

		/// <summary>
		/// Returns if the part is currently installed on the car
		/// </summary>
		public abstract bool installedOnCar { get; }

		/// <summary>
		/// Add a part object as the child of this part
		/// Should only be used by logic itself, never by mod makers!
		/// </summary>
		/// <param name="part"></param>
		public void AddChild(BasicPart part)
		{
			childParts.Add(part);
		}

		/// <summary>
		/// Returns a list of all child parts of this part
		/// </summary>
		/// <returns></returns>
		public List<BasicPart> GetChilds()
		{
			return new List<BasicPart>(childParts);
		}

		/// <summary>
		/// Uninstalls the part
		/// </summary>
		public abstract void Uninstall();

		/// <summary>
		/// Resets the part to its defaultPosition & defaultRotation
		/// </summary>
		/// <param name="uninstall">Should an installed part be uninstalled prior to resetting</param>
		public abstract void ResetToDefault(bool uninstall = false);

		[Obsolete("Use 'active' property instead", true)]
		public void SetActive(bool active)
		{
			this.active = active;
		}

		[Obsolete("Use 'position' property instead", true)]
		public void SetPosition(Vector3 position)
		{
			this.position = position;
		}

		[Obsolete("Use 'rotation' property instead", true)]
		public void SetRotation(Quaternion rotation)
		{
			this.rotation = rotation.eulerAngles;
		}

		[Obsolete("Use 'defaultPosition' property instead", true)]
		public void SetDefaultPosition(Vector3 defaultPosition)
		{
			this.defaultPosition = defaultPosition;
		}

		[Obsolete("Use 'defaultRotation'", true)]
		public void SetDefaultRotation(Vector3 defaultRotation)
		{
			this.defaultRotation = defaultRotation;
		}

		[Obsolete("Use 'bought' property instead", true)]
		public bool IsBought()
		{
			return bought;
		}

		[Obsolete("Use 'bought' property instead", true)]
		public void SetBought(bool bought)
		{
			this.bought = bought;
		}
	}
}