using MscModApi.Parts;
using MscModApi.Tools;
using System.Linq;
using UnityEngine;


namespace MscModApi.Shopping
{
	public class Box : PartBox
	{
		public GameObject box;
		public int spawnedCounter = 0;
		public Part[] parts;
		public BoxLogic logic;
		private static GameObject boxModel;

		public Box(string partId, string partName, GameObject partGameObject, int numberOfParts,
			Part parent, Vector3[] installLocations, Vector3[] installRotations, Vector3 defaultPosition,
			bool uninstallWhenParentUninstalls = true, bool disableCollisionWhenInstalled = true)

		{
			Setup(
				partId,
				partName,
				GameObject.Instantiate(boxModel),
				partGameObject,
				numberOfParts,
				parent,
				installLocations,
				installRotations,
				defaultPosition,
				uninstallWhenParentUninstalls,
				disableCollisionWhenInstalled
			);
		}

		public Box(string partId, string partName, int numberOfParts,
			Part parent, Vector3[] installLocations, Vector3[] installRotations, Vector3 defaultPosition,
			bool uninstallWhenParentUninstalls = true, bool disableCollisionWhenInstalled = true)

		{
			Setup(
				partId,
				partName,
				GameObject.Instantiate(boxModel),
				parent.partBaseInfo.assetBundle.LoadAsset<GameObject>("fuel_injector.prefab"),
				numberOfParts,
				parent,
				installLocations,
				installRotations,
				defaultPosition,
				uninstallWhenParentUninstalls,
				disableCollisionWhenInstalled
			);
		}

		private void Setup(string partId, string partName, GameObject box, GameObject partGameObject, int numberOfParts,
			Part parent,
			Vector3[] installLocations, Vector3[] installRotations, Vector3 defaultPosition,
			bool uninstallWhenParentUninstalls, bool disableCollisionWhenInstalled)
		{
			PartBaseInfo partBaseInfo = parent.partBaseInfo;
			this.box = box;

			parts = new Part[numberOfParts];

			for (int i = 0; i < numberOfParts; i++)
			{
				int iOffset = i + 1;

				parts[i] = new Part(
					$"{partId}_{i}", partName + " " + iOffset, partGameObject,
					parent, installLocations[i], installRotations[i], partBaseInfo, uninstallWhenParentUninstalls,
					disableCollisionWhenInstalled);
				parts[i].SetDefaultPosition(defaultPosition);
				if (!parts[i].IsBought())
				{
					parts[i].Uninstall();
					parts[i].SetActive(false);
				}
			}

			logic = box.AddComponent<BoxLogic>();
			logic.Init(parts, "Unpack " + partName, this);
		}

		public Box(string partId, string partName, GameObject box, GameObject partGameObject, int numberOfParts,
			Part parent, Vector3[] installLocations, Vector3[] installRotations, Vector3 defaultPosition,
			bool uninstallWhenParentUninstalls = true, bool disableCollisionWhenInstalled = true)
		{
			Setup(
				partId,
				partName,
				box,
				partGameObject,
				numberOfParts,
				parent,
				installLocations,
				installRotations,
				defaultPosition,
				uninstallWhenParentUninstalls,
				disableCollisionWhenInstalled
			);
		}

		internal override void CheckUnpackedOnSave()
		{
			if (!AnyBought())
			{
				return;
			}

			if (spawnedCounter < parts.Length)
			{
				foreach (Part part in parts)
				{
					if (part.IsInstalled() || part.gameObject.activeSelf) continue;
					part.SetPosition(box.transform.position);
					part.SetActive(true);
				}
			}

			box.SetActive(false);
			box.transform.position = new Vector3(0, 0, 0);
			box.transform.localPosition = new Vector3(0, 0, 0);
		}

		internal void AddScrews(Screw[] screws, float overrideScale = 0f, float overrideSize = 0f)
		{
			foreach (Part part in parts)
			{
				part.AddScrews(screws.CloneToNew(), overrideScale, overrideSize);
			}
		}

		public bool IsBought()
		{
			return parts.All(part => part.IsBought());
		}

		public bool AnyBought()
		{
			return parts.Any(part => part.IsBought());
		}

		internal static void LoadAssets(AssetBundle assetBundle)
		{
			boxModel = assetBundle.LoadAsset<GameObject>("cardboard_box.prefab");
		}
	}
}