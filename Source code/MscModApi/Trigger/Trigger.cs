using MscModApi.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using MscModApi.Parts;
using UnityEngine;

namespace MscModApi.Trigger
{
	internal class Trigger : MonoBehaviour
	{
		private Part part;
		private GameObject parentGameObject;
		private bool disableCollisionWhenInstalled;
		private Rigidbody rigidBody;
		private bool canBeInstalled;
		private Coroutine handleUninstallRoutine;
		private Coroutine verifyInstalledRoutine;
		private Coroutine verifyUninstalledRoutine;

		private IEnumerator HandleUninstall()
		{
			while (part.installed) {
				
				if (!part.IsFixed(false) && part.gameObject.IsLookingAt() && UserInteraction.EmptyHand() &&
					!Tool.HasToolInHand()) {
					if (part.screwPlacementMode) {
						ScrewPlacementAssist.HandlePartInteraction(part);
					} else {
						UserInteraction.GuiInteraction(UserInteraction.Type.Disassemble, $"Uninstall {part.gameObject.name}");

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

		private IEnumerator VerifyInstalled()
		{
			while (part.installed && part.gameObject.transform.parent != parentGameObject.transform) {
				rigidBody.isKinematic = true;
				part.gameObject.transform.parent = parentGameObject.transform;
				part.gameObject.transform.localPosition = part.installPosition;
				part.gameObject.transform.localRotation = Quaternion.Euler(part.installRotation);
				yield return null;
			}

			verifyInstalledRoutine = null;
		}

		private IEnumerator VerifyUninstalled()
		{
			while (!part.installed && part.gameObject.transform.parent == parentGameObject.transform) {
				rigidBody.isKinematic = false;
				part.gameObject.transform.parent = null;
				part.gameObject.transform.Translate(Vector3.up * 0.025f);
				yield return null;
			}

			verifyUninstalledRoutine = null;
		}

		internal void Install()
		{
			part.preInstallActions.InvokeAll();

			if (part.installBlocked)
			{
				return;
			}

			if (part.partSave.bought == PartSave.BoughtState.No)
			{
				return;
			}

			if (part.uninstallWhenParentUninstalls && !part.parentInstalled) {
				return;
			}

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
			//part.trigger.SetActive(false);

			canBeInstalled = false;

			part.postInstallActions.InvokeAll();
		}

		internal void Uninstall()
		{
			part.preUninstallActions.InvokeAll();

			part.ResetScrews();

			part.childParts.ForEach(delegate (Part part) {
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

			part.postUninstallActions.InvokeAll();
		}

		private void OnTriggerStay(Collider collider)
		{
			if (!canBeInstalled || !UserInteraction.LeftMouseDown) return;

			UserInteraction.GuiInteraction(UserInteraction.Type.None);
			collider.gameObject.PlayAssemble();
			canBeInstalled = false;
			Install();
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (
				(part.uninstallWhenParentUninstalls && !part.parentInstalled) 
			    || !collider.gameObject.IsHolding()
			    || collider.gameObject != part.gameObject
			    || part.installBlocked
			){
				return;
			}

			UserInteraction.GuiInteraction(UserInteraction.Type.Assemble, $"Install {part.gameObject.name}");
			canBeInstalled = true;
		}

		private void OnTriggerExit(Collider collider)
		{
			if (!canBeInstalled) return;

			canBeInstalled = false;
			UserInteraction.GuiInteraction(UserInteraction.Type.None);
		}

		internal void Init(Part part, GameObject parentGameObject, bool disableCollisionWhenInstalled)
		{
			this.part = part;
			this.parentGameObject = parentGameObject;
			this.disableCollisionWhenInstalled = disableCollisionWhenInstalled;
			rigidBody = part.gameObject.GetComponent<Rigidbody>();
		}
	}
}