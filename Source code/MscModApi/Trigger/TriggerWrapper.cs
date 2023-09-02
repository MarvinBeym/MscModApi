using MscModApi.Parts;
using System;
using UnityEngine;

namespace MscModApi.Trigger
{
	/// <summary>
	/// Wrapper around trigger logic of parts
	/// </summary>
	public class TriggerWrapper
	{
		private Trigger logic;
		private GameObject triggerGameObject;
		private Renderer renderer;
		private static readonly Vector3 defaultScale = new Vector3(0.05f, 0.05f, 0.05f);

		/// <summary>
		/// A wrapper class around the trigger logic of a Part
		/// </summary>
		/// <param name="part">The part object the trigger will be added to</param>
		/// <param name="parentGameObject">The gameObject the trigger will be added to as a child</param>
		/// <param name="disableCollisionWhenInstalled">Disable the collision of the part when the part gets installed</param>
		public TriggerWrapper(Part part, BasicPart parent, DisableCollision disableCollisionWhenInstalled = DisableCollision.InstalledOnCar)
		{
			Setup(part, parent.gameObject, disableCollisionWhenInstalled);
			logic = triggerGameObject.AddComponent<Trigger>();
			logic.Init(part, parent);
		}

		/// <summary>
		/// Render of trigger gameObject visible
		/// </summary>
		public bool visible
		{
			get => renderer.enabled;
			set => renderer.enabled = value;
		}

		/// <summary>
		/// Scale of the trigger gameObject
		/// </summary>
		public Vector3 scale
		{
			get => triggerGameObject.transform.localScale;
			set => triggerGameObject.transform.localScale = value;
		}

		/// <summary>
		/// Position of the trigger gameObject
		/// </summary>
		public Vector3 position
		{
			get => triggerGameObject.transform.localPosition;
			set => triggerGameObject.transform.localPosition = value;
		}

		/// <summary>
		/// Rotation of the trigger gameObject
		/// </summary>
		public Vector3 rotation
		{
			get => triggerGameObject.transform.localRotation.eulerAngles;
			set => triggerGameObject.transform.localRotation = Quaternion.Euler(value);
		}

		/// <summary>
		/// Executes the install logic
		/// </summary>
		public void Install() => logic.Install();

		/// <summary>
		/// Executed the uninstall logic
		/// </summary>
		public void Uninstall() => logic.Uninstall();

		protected void Setup(Part part, GameObject parent, DisableCollision disableCollisionWhenInstalled = DisableCollision.InstalledOnCar)
		{
			triggerGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			triggerGameObject.transform.SetParent(parent.transform, false);
			triggerGameObject.name = part.gameObject.name + "_trigger";
			position = part.installPosition;
			rotation = part.installRotation;
			scale = defaultScale;

			var collider = triggerGameObject.GetComponent<Collider>();
			collider.isTrigger = true;

			renderer = triggerGameObject.GetComponent<Renderer>();
			visible = false;

			switch (disableCollisionWhenInstalled)
			{
				case DisableCollision.InstalledOnCar:
					part.AddEventListener(PartEvent.Time.Post, PartEvent.Type.InstallOnCar, () => part.collider.isTrigger = true);
					part.AddEventListener(PartEvent.Time.Post, PartEvent.Type.UninstallFromCar, () => part.collider.isTrigger = false);
					break;
				case DisableCollision.InstalledOnParent:
					part.AddEventListener(PartEvent.Time.Post, PartEvent.Type.Install, () => part.collider.isTrigger = true);
					part.AddEventListener(PartEvent.Time.Post, PartEvent.Type.Uninstall, () => part.collider.isTrigger = false);
					break;
			}
		}
	}
}