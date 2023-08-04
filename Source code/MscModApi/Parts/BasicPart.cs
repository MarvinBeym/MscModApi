using System;
using UnityEngine;

namespace MscModApi.Parts
{
	/// <summary>
	/// The absolute base class for objects acting like a part (Part, Box, Kit)
	/// </summary>
	public abstract class BasicPart
	{
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
		public abstract string name
		{
			get;
		}
		
		/// <summary>
		/// Returns a clean name of the part (Removing '(Clone)')
		/// </summary>
		public string cleanName => name.Replace("(Clone)", "");

		/// <summary>
		/// Resets the part to its defaultPosition & defaultRotation
		/// </summary>
		/// <param name="uninstall">Should an installed part be uninstalled prior to resetting</param>
		public abstract void ResetToDefault(bool uninstall = false);

		[Obsolete("Use 'active' property instead")]
		public void SetActive(bool active)
		{
			this.active = active;
		}

		[Obsolete("Use 'position' property instead")]
		public void SetPosition(Vector3 position)
		{
			this.position = position;
		}

		[Obsolete("Use 'rotation' property instead")]
		public void SetRotation(Quaternion rotation)
		{
			this.rotation = rotation.eulerAngles;
		}

		[Obsolete("Use 'defaultPosition' property instead")]
		public void SetDefaultPosition(Vector3 defaultPosition)
		{
			this.defaultPosition = defaultPosition;
		}

		[Obsolete("Use 'defaultRotation'")]
		public void SetDefaultRotation(Vector3 defaultRotation)
		{
			this.defaultRotation = defaultRotation;
		}

		[Obsolete("Use 'bought' property instead")]
		public bool IsBought()
		{
			return bought;
		}

		[Obsolete("Use 'bought' property instead")]
		public void SetBought(bool bought)
		{
			this.bought = bought;
		}
	}
}