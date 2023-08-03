using MscModApi.Parts;
using MscModApi.Tools;
using System.Linq;
using UnityEngine;


namespace MscModApi.Shopping
{
	public class Box : PartBox
	{
		public int spawnedCounter = 0;
		public BoxLogic logic;
		private static GameObject boxModel;

		public Box(string boxName, string partId, string partName, GameObject partGameObject, int numberOfParts,
			Part parent, Vector3[] installLocations, Vector3[] installRotations, Vector3 defaultPosition,
			bool uninstallWhenParentUninstalls = true, bool disableCollisionWhenInstalled = true)

		{
			Setup(
				boxName,
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

		public Box(string boxName, string partId, string partName, GameObject box, GameObject partGameObject, int numberOfParts,
			Part parent, Vector3[] installLocations, Vector3[] installRotations, Vector3 defaultPosition,
			bool uninstallWhenParentUninstalls = true, bool disableCollisionWhenInstalled = true)
		{
			Setup(
				boxName,
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

		private void Setup(string boxName, string partId, string partName, GameObject box, GameObject partGameObject, int numberOfParts,
			Part parent,
			Vector3[] installLocations, Vector3[] installRotations, Vector3 defaultPosition,
			bool uninstallWhenParentUninstalls, bool disableCollisionWhenInstalled)
		{
			PartBaseInfo partBaseInfo = parent.partBaseInfo;
			box.SetNameLayerTag(boxName + "(Clone)");

			for (int i = 0; i < numberOfParts; i++)
			{
				int iOffset = i + 1;

				Part part = new Part(
					$"{partId}_{i}", partName + " " + iOffset, partGameObject,
					parent, installLocations[i], installRotations[i], partBaseInfo, uninstallWhenParentUninstalls,
					disableCollisionWhenInstalled);
				part.SetDefaultPosition(defaultPosition);
				if (!part.IsBought())
				{
					part.Uninstall();
					part.SetActive(false);
				}
				AddPart(part);
			}

			logic = box.AddComponent<BoxLogic>();
			logic.Init("Unpack " + partName, this);

			SetBoxGameObject(box);
		}

		internal override void CheckUnpackedOnSave()
		{
			if (!AnyBought())
			{
				return;
			}

			GameObject box = GetBoxGameObject();

			if (spawnedCounter < GetParts().Count)
			{
				foreach (Part part in GetParts())
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
			foreach (Part part in GetParts())
			{
				part.AddScrews(screws.CloneToNew(), overrideScale, overrideSize);
			}
		}

		public bool IsBought()
		{
			return GetParts().All(part => part.IsBought());
		}

		public bool AnyBought()
		{
			return GetParts().Any(part => part.IsBought());
		}

		internal static void LoadAssets(AssetBundle assetBundle)
		{
			boxModel = assetBundle.LoadAsset<GameObject>("cardboard_box.prefab");
		}
	}
}