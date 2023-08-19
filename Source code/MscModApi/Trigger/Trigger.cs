using MscModApi.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MSCLoader;
using MscModApi.Caching;
using MscModApi.Parts;
using UnityEngine;


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
		/// Stores a list of childs installed on this part before this part uninstalls
		/// (otherwise if child uninstalls with parent, the childs events may not get called)
		/// </summary>
		protected List<BasicPart> childsInstalledBeforeUninstall = new List<BasicPart>();

		/// <summary>
		/// The parent gameObject of the part
		/// </summary>
		protected BasicPart parent;

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
		/// Returns the Transform object of the parent
		/// (Wrapper for if parent is GameObject)
		/// </summary>
		protected Transform parentTransform => parent.gameObject.transform;

		/// <summary>
		/// Verifies the installation
		/// </summary>
		/// <returns></returns>
		protected IEnumerator VerifyInstalled()
		{
			while (part.installed && part.gameObject.transform.parent != parentTransform) {
				Destroy(rigidBody);
				part.gameObject.transform.parent = parentTransform;
				part.gameObject.transform.localPosition = part.installPosition;
				part.gameObject.transform.localRotation = Quaternion.Euler(part.installRotation);
				yield return null;
			}

			verifyInstalledRoutine = null;

			part.GetEvents(PartEvent.Time.Post, PartEvent.Type.Install).InvokeAll();


			if (part.installedOnCar)
			{
				part.GetEvents(PartEvent.Time.Post, PartEvent.Type.InstallOnCar).InvokeAll();

				foreach (BasicPart child in part.childs)
				{
					//Part was installed on car so installed childs will as well.
					if (child.installed && child.GetType().GetInterfaces().Contains(typeof(SupportsPartEvents)))
					{
						SupportsPartEvents partEventSupportingPart = (SupportsPartEvents)child;
						partEventSupportingPart.GetEvents(PartEvent.Time.Post, PartEvent.Type.InstallOnCar).InvokeAll();
					}
				}
			}
		}

		/// <summary>
		/// Verifies the uninstallation
		/// </summary>
		/// <returns></returns>
		protected IEnumerator VerifyUninstalled()
		{
			while (!part.installed && part.gameObject.transform.parent == parentTransform) {
				Rigidbody existingRigidBody = part.gameObject.GetComponent<Rigidbody>();
				if (existingRigidBody != null)
				{
					//May happen if a different component is adding a RigidBody itself (Like a HingeJoint)
					rigidBody = existingRigidBody;
				}
				else
				{
					rigidBody = part.gameObject.AddComponent<Rigidbody>();
				}

				rigidBody.mass = 3;
				part.gameObject.transform.parent = null;
				part.gameObject.transform.Translate(Vector3.up * 0.025f);
				yield return null;
			}

			verifyUninstalledRoutine = null;

			part.GetEvents(PartEvent.Time.Post, PartEvent.Type.Uninstall).InvokeAll();
			if (!part.installedOnCar)
			{
				//Probably called always because installedOnCar is likely already false at this point (can't be still installed on car)
				part.GetEvents(PartEvent.Time.Post, PartEvent.Type.UninstallFromCar).InvokeAll();

				foreach (BasicPart child in part.childs)
				{
					//Part was uninstalled from car so installed childs are as well.
					if (childsInstalledBeforeUninstall.Contains(child) || child.installed)
					{
						if (child.GetType().GetInterfaces().Contains(typeof(SupportsPartEvents)))
						{
							SupportsPartEvents partEventSupportingPart = (SupportsPartEvents)child;
							partEventSupportingPart.GetEvents(PartEvent.Time.Post, PartEvent.Type.UninstallFromCar).InvokeAll();
						}

						childsInstalledBeforeUninstall.Remove(child);
					}
				}
			}
		}

		/// <summary>
		/// Executes the install logic
		/// </summary>
		public void Install()
		{
			if (!part.installPossible) {
				return;
			}

			part.GetEvents(PartEvent.Time.Pre, PartEvent.Type.Install).InvokeAll();

			if (parent != null && parent.installedOnCar)
			{
				//Parent is installed on car so part is also gonna be installed on the car soon
				part.GetEvents(PartEvent.Time.Pre, PartEvent.Type.InstallOnCar).InvokeAll();

				foreach (BasicPart child in part.childs)
				{
					//Part will soon be installed on car so installed childs will as well.
					if (child.installed && child.GetType().GetInterfaces().Contains(typeof(SupportsPartEvents)))
					{
						SupportsPartEvents partEventSupportingPart = (SupportsPartEvents)child;
						partEventSupportingPart.GetEvents(PartEvent.Time.Pre, PartEvent.Type.InstallOnCar).InvokeAll();
					}
				}
			}

			part.partSave.installed = true;
			part.gameObject.tag = "Untagged";

			if (handleUninstallRoutine == null) {
				handleUninstallRoutine = StartCoroutine(HandleUninstall());
			}

			if (verifyInstalledRoutine == null) {
				verifyInstalledRoutine = StartCoroutine(VerifyInstalled());
			}

			part.SetScrewsActive(true);

			canBeInstalled = false;
		}

		/// <summary>
		/// Executes the uninstall logic
		/// </summary>
		public void Uninstall()
		{
			part.GetEvents(PartEvent.Time.Pre, PartEvent.Type.Uninstall).InvokeAll();

			if (parent != null && parent.installedOnCar)
			{
				part.GetEvents(PartEvent.Time.Pre, PartEvent.Type.UninstallFromCar).InvokeAll();
			}

			if (parent != null && parent.installedOnCar)
			{
				//Parent is installed on car so part is also gonna be uninstalled from the car soon
				part.GetEvents(PartEvent.Time.Pre, PartEvent.Type.UninstallFromCar).InvokeAll();

				foreach (BasicPart child in part.childs)
				{
					//Part will soon be uninstalled from car so installed childs will as well.
					if (child.installed && child.GetType().GetInterfaces().Contains(typeof(SupportsPartEvents)))
					{
						SupportsPartEvents partEventSupportingPart = (SupportsPartEvents)child;
						partEventSupportingPart.GetEvents(PartEvent.Time.Pre, PartEvent.Type.UninstallFromCar).InvokeAll();
					}
				}
			}

			part.ResetScrews();

			foreach (BasicPart child in part.childs)
			{
				if (child.installed)
				{
					childsInstalledBeforeUninstall.Add(child);
				}
				if (child.uninstallWhenParentUninstalls)
				{
					child.Uninstall();
				}
			}

			part.partSave.installed = false;
			part.gameObject.tag = "PART";

			if (!part.installed && verifyUninstalledRoutine == null) {
				verifyUninstalledRoutine = StartCoroutine(VerifyUninstalled());
			}

			part.SetScrewsActive(false);
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
		/// <param name="parent">Parent object</param>
		public void Init(Part part, BasicPart parent)
		{
			this.part = part;
			this.parent = parent;
			rigidBody = part.gameObject.GetComponent<Rigidbody>();
			if (!rigidBody) {
				rigidBody = part.gameObject.AddComponent<Rigidbody>();
			}
		}
	}
}