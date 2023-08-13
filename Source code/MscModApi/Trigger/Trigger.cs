using MscModApi.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using MSCLoader;
using MscModApi.Parts;
using UnityEngine;
using EventType = MscModApi.Parts.EventType;

namespace MscModApi.Trigger
{
	/// <summary>
	/// The trigger logic dealing with installing & uninstalling parts
	/// </summary>
	public class Trigger : MonoBehaviour
	{
		/// <summary>
		/// The part this trigger is for
		/// </summary>
		protected Part part;

		/// <summary>
		/// The parent gameObject of the part
		/// </summary>
		protected GameObject parentGameObject;

		/// <summary>
		/// If the collider of the part should be disabled on installation (Improved performance)
		/// </summary>
		protected bool disableCollisionWhenInstalled;

		private Rigidbody rigidBody;

		/// <summary>
		/// Flag to now when a part can be installed
		/// </summary>
		protected bool canBeInstalled;

		/// <summary>
		/// Coroutine to run to deal with uninstalled after a part has been installed
		/// </summary>
		protected Coroutine handleUninstallRoutine;

		/// <summary>
		/// Verifies the part was installed correctly (running a loop, installing the part until it's actually on the part
		/// </summary>
		protected Coroutine verifyInstalledRoutine;

		/// <summary>
		/// Verifies the part was uninstalled correctly (running a loop, uninstalling the part until it's actually removed
		/// </summary>
		protected Coroutine verifyUninstalledRoutine;

		/// <summary>
		/// Handles uninstall
		/// </summary>
		/// <returns></returns>
		protected IEnumerator HandleUninstall()
		{
			while (part.installed) {
				if (!part.bolted && part.gameObject.IsLookingAt() && UserInteraction.EmptyHand() &&
				    !Tool.HasToolInHand()) {
					if (part.screwPlacementMode) {
						ScrewPlacementAssist.HandlePartInteraction(part);
					}
					else {
						UserInteraction.GuiInteraction(UserInteraction.Type.Disassemble,
							$"Uninstall {part.gameObject.name}");

						if (UserInteraction.RightMouseDown) {
							UserInteraction.GuiInteraction(UserInteraction.Type.None);
							part.gameObject.PlayDisassemble();
							Uninstall();
						}
					}
				}

				yield return null;
			}

			handleUninstallRoutine = null;
		}

		/// <summary>
		/// Verifies the installation
		/// </summary>
		/// <returns></returns>
		protected IEnumerator VerifyInstalled()
		{
			while (part.installed && part.gameObject.transform.parent != parentGameObject.transform) {
				Destroy(rigidBody);
				part.gameObject.transform.parent = parentGameObject.transform;
				part.gameObject.transform.localPosition = part.installPosition;
				part.gameObject.transform.localRotation = Quaternion.Euler(part.installRotation);
				yield return null;
			}

			verifyInstalledRoutine = null;
		}

		/// <summary>
		/// Verifies the uninstallation
		/// </summary>
		/// <returns></returns>
		protected IEnumerator VerifyUninstalled()
		{
			while (!part.installed && part.gameObject.transform.parent == parentGameObject.transform) {
				rigidBody = part.gameObject.AddComponent<Rigidbody>();
				rigidBody.mass = 3;
				part.gameObject.transform.parent = null;
				part.gameObject.transform.Translate(Vector3.up * 0.025f);
				yield return null;
			}

			verifyUninstalledRoutine = null;
		}

		/// <summary>
		/// Executes the install logic
		/// </summary>
		public void Install()
		{
			if (!part.installPossible) {
				return;
			}

			part.GetEvents(EventTime.Pre, EventType.Install).InvokeAll();

			part.partSave.installed = true;
			part.gameObject.tag = "Untagged";

			if (handleUninstallRoutine == null) {
				handleUninstallRoutine = StartCoroutine(HandleUninstall());
			}

			if (verifyInstalledRoutine == null) {
				verifyInstalledRoutine = StartCoroutine(VerifyInstalled());
			}


			if (disableCollisionWhenInstalled) {
				part.collider.isTrigger = true;
			}

			part.SetScrewsActive(true);

			canBeInstalled = false;

			part.GetEvents(EventTime.Post, EventType.Install).InvokeAll();
		}

		/// <summary>
		/// Executes the uninstall logic
		/// </summary>
		public void Uninstall()
		{
			part.GetEvents(EventTime.Pre, EventType.Uninstall).InvokeAll();

			part.ResetScrews();

			part.childParts.ForEach(delegate(Part part)
			{
				if (part.uninstallWhenParentUninstalls) {
					part.Uninstall();
				}
			});

			part.partSave.installed = false;
			part.gameObject.tag = "PART";

			if (!part.installed && verifyUninstalledRoutine == null) {
				verifyUninstalledRoutine = StartCoroutine(VerifyUninstalled());
			}

			if (disableCollisionWhenInstalled) {
				part.collider.isTrigger = false;
			}

			part.SetScrewsActive(false);
			//part.trigger.SetActive(true);

			part.GetEvents(EventTime.Post, EventType.Uninstall).InvokeAll();
		}

		/// <summary>
		/// Triggered when the collider of the part stays in the trigger
		/// </summary>
		/// <param name="collider"></param>
		protected void OnTriggerStay(Collider collider)
		{
			if (!canBeInstalled || !UserInteraction.LeftMouseDown) return;

			UserInteraction.GuiInteraction(UserInteraction.Type.None);
			collider.gameObject.PlayAssemble();
			canBeInstalled = false;
			Install();
		}

		/// <summary>
		/// Triggered when the collider of the part enters the trigger
		/// </summary>
		/// <param name="collider"></param>
		protected void OnTriggerEnter(Collider collider)
		{
			if (
				!collider.gameObject.IsHolding()
				|| collider.gameObject != part.gameObject
				|| !part.installPossible
			) {
				return;
			}

			UserInteraction.GuiInteraction(UserInteraction.Type.Assemble, $"Install {part.gameObject.name}");
			canBeInstalled = true;
		}

		/// <summary>
		/// Triggered when the collider of the part leaves the trigger
		/// </summary>
		/// <param name="collider"></param>
		protected void OnTriggerExit(Collider collider)
		{
			if (!canBeInstalled) return;

			canBeInstalled = false;
			UserInteraction.GuiInteraction(UserInteraction.Type.None);
		}

		/// <summary>
		/// Initializes the trigger logic
		/// </summary>
		/// <param name="part">The part</param>
		/// <param name="parentGameObject">Parent GameObject</param>
		/// <param name="disableCollisionWhenInstalled">Should the collider be disabled when the part installs</param>
		public void Init(Part part, GameObject parentGameObject, bool disableCollisionWhenInstalled)
		{
			this.part = part;
			this.parentGameObject = parentGameObject;
			this.disableCollisionWhenInstalled = disableCollisionWhenInstalled;
			rigidBody = part.gameObject.GetComponent<Rigidbody>();
			if (!rigidBody) {
				rigidBody = part.gameObject.AddComponent<Rigidbody>();
			}
		}
	}
}