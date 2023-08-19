using MscModApi.Parts.ReplacePart;
using UnityEngine;

namespace MscModApi.Parts
{
	public abstract class DerivablePart : Part
	{
		protected abstract Vector3 partInstallPosition { get; }
		protected abstract Vector3 partInstallRotation { get; }
		protected abstract string partName { get; }
		protected abstract string partId { get; }

		protected DerivablePart(GameObject part, Part parent, PartBaseInfo partBaseInfo,
			bool uninstallWhenParentUninstalls = true, DisableCollision disableCollisionWhenInstalled = DisableCollision.InstalledOnCar)
		{
			gameObjectUsedForInstantiation = part;

			Setup(partId, partName, parent, partInstallPosition, partInstallRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, null);
		}

		protected DerivablePart(GameObject part, GamePart parent, PartBaseInfo partBaseInfo,
			bool uninstallWhenParentUninstalls = true, DisableCollision disableCollisionWhenInstalled = DisableCollision.InstalledOnCar)
		{
			gameObjectUsedForInstantiation = part;

			Setup(partId, partName, parent, partInstallPosition, partInstallRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, null);
		}

		protected DerivablePart(Part parent, PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true,
			DisableCollision disableCollisionWhenInstalled = DisableCollision.InstalledOnCar, string prefabName = null)
		{
			Setup(partId, partName, parent, partInstallPosition, partInstallRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, prefabName);
		}

		protected DerivablePart(GamePart parent, PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true,
			DisableCollision disableCollisionWhenInstalled = DisableCollision.InstalledOnCar, string prefabName = null)
		{
			Setup(partId, partName, parent, partInstallPosition, partInstallRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, prefabName);
		}

		protected DerivablePart(PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true,
			DisableCollision disableCollisionWhenInstalled = DisableCollision.InstalledOnCar, string prefabName = null)
		{
			Setup(partId, partName, null, Vector3.zero, Vector3.zero, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, prefabName);
		}
	}
}