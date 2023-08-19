using MscModApi.Parts.ReplacePart;
using UnityEngine;

namespace MscModApi.Parts
{
	public abstract class DerivablePart : Part
	{
		/// <summary>
		/// The position of the part on it's parent when installed
		/// </summary>
		protected abstract Vector3 partInstallPosition { get; }
		/// <summary>
		/// The rotation of the part on it's parent when installed
		/// </summary>
		protected abstract Vector3 partInstallRotation { get; }
		/// <summary>
		/// The name of the part (clean name without any (Clone) and such)
		/// </summary>
		protected abstract string partName { get; }
		/// <summary>
		/// The id of the part (for example used when loading data from save)
		/// </summary>
		protected abstract string partId { get; }
		/// <summary>
		/// disabled collision when part is installed
		/// </summary>
		protected virtual DisableCollision disableCollisionWhenInstalled => DisableCollision.InstalledOnCar;

		protected DerivablePart(GameObject part, Part parent, PartBaseInfo partBaseInfo,
			bool uninstallWhenParentUninstalls = true)
		{
			gameObjectUsedForInstantiation = part;

			Setup(partId, partName, parent, partInstallPosition, partInstallRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, null);
		}

		protected DerivablePart(GameObject part, GamePart parent, PartBaseInfo partBaseInfo,
			bool uninstallWhenParentUninstalls = true)
		{
			gameObjectUsedForInstantiation = part;

			Setup(partId, partName, parent, partInstallPosition, partInstallRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, null);
		}

		protected DerivablePart(Part parent, PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true, string prefabName = null)
		{
			Setup(partId, partName, parent, partInstallPosition, partInstallRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, prefabName);
		}

		protected DerivablePart(GamePart parent, PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true, string prefabName = null)
		{
			Setup(partId, partName, parent, partInstallPosition, partInstallRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, prefabName);
		}

		protected DerivablePart(PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true, string prefabName = null)
		{
			Setup(partId, partName, null, Vector3.zero, Vector3.zero, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, prefabName);
		}
	}
}