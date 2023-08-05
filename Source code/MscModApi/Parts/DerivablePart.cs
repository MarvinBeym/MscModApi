using UnityEngine;

namespace MscModApi.Parts
{
	public abstract class DerivablePart : Part
	{
		protected abstract Vector3 partInstallPosition { get; }
		protected abstract Vector3 partInstallRotation { get; }
		protected abstract string partName { get; }
		protected abstract string partId { get; }

		protected DerivablePart(GameObject part, Part parentPart, PartBaseInfo partBaseInfo,
			bool uninstallWhenParentUninstalls = true, bool disableCollisionWhenInstalled = true)
		{
			usingGameObjectInstantiation = true;
			gameObjectUsedForInstantiation = part;

			usingPartParent = true;
			this.parentPart = parentPart;

			Setup(partId, partName, parentPart.gameObject, partInstallPosition, partInstallRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, null);
			parentPart.childParts.Add(this);
		}

		protected DerivablePart(GameObject parent, PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true,
			bool disableCollisionWhenInstalled = true, string prefabName = null)
		{
			Setup(partId, partName, parent, partInstallPosition, partInstallRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, prefabName);
		}

		protected DerivablePart(PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true,
			bool disableCollisionWhenInstalled = true, string prefabName = null)
		{
			Setup(partId, partName, null, Vector3.zero, Vector3.zero, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, prefabName);
		}

		protected DerivablePart(Part parentPart, PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true,
			bool disableCollisionWhenInstalled = true, string prefabName = null)
		{
			usingPartParent = true;
			this.parentPart = parentPart;
			Setup(partId, partName, parentPart.gameObject, partInstallPosition, partInstallRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, prefabName);
			parentPart.childParts.Add(this);
		}
	}
}