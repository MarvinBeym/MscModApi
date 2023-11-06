using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace MscModApi.Parts
{
	/// <summary>
	/// The absolute base class for objects acting like a part (Part, Box, Kit)
	/// </summary>
	public abstract class BasicPart
	{
		/// <summary>
		/// Internal list of part objects that are a child of this part
		/// </summary>
		protected readonly List<BasicPart> _childs = new List<BasicPart>();

		/// <summary>
		/// A readonly list of BasicPart childs of this part
		/// </summary>
		public readonly ReadOnlyCollection<BasicPart> childs;

		protected BasicPart()
		{
			this.childs = _childs.AsReadOnly();
		}

		public abstract GameObject gameObject { get; protected set; }

		/// <summary>
		/// The default position, used for example when resetting.
		/// </summary>
		public Vector3 defaultPosition { get; set; } = Vector3.zero;
		
		/// <summary>
		/// The default rotation, used for example when resetting.
		/// </summary>
		public Vector3 defaultRotation { get; set; } = Vector3.zero;

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
		/// Blocks the part from being installed on the parent
		/// </summary>
		public abstract bool installBlocked { get; set; }

		/// <summary>
		/// Add a part object as the child of this part
		/// </summary>
		/// <param name="part"></param>
		public virtual void AddChild(BasicPart part)
		{
			_childs.Add(part);
		}


		/// <summary>
		/// Adds multiple parts as a child
		/// </summary>
		/// <param name="parts"></param>
		protected virtual void AddChilds(IEnumerable<BasicPart> parts)
		{
			foreach (BasicPart part in parts) {
				AddChild(part);
			}
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
	}
}