using System;
using MscModApi.Parts;
using MscModApi.Tools;
using System.Linq;
using UnityEngine;


namespace MscModApi.Shopping
{
	/// <summary>
	/// A box that contains multiple parts that are the same. Adds a numeric index to the part name and automatically creates the parts
	/// </summary>
	public class Box : PartBox
	{
		public BoxLogic logic;
		private static GameObject boxTemplateModel;

		/// <summary>
		/// Constructor for a new Box
		/// </summary>
		/// <param name="boxName">What should be shown when the player looks at the box</param>
		/// <param name="partId">The partId used for every part created (with added index starting from 0). Has to be unique in your mod</param>
		/// <param name="partName">The name the created part(s) should have (with added index starting from 1)</param>
		/// <param name="partGameObject">The GameObject instantiated when the Part object is created</param>
		/// <param name="numberOfParts">the amount of parts that should be created</param>
		/// <param name="parent">The parent (Part) for each created Part (All install to the same parent)</param>
		/// <param name="installLocations">The individual install positions on the parent for each created part</param>
		/// <param name="installRotations">The individual install rotations on the parent for each created part</param>
		/// <param name="defaultPosition">The default position for both the box and all parts (where to place after purchase or when reset)</param>
		/// <param name="uninstallWhenParentUninstalls">Should the part uninstall from the parent if the parent is uninstalled</param>
		/// <param name="disableCollisionWhenInstalled">Disables the collider of the part when installed to the parent (reduces lag, avoids problematic collisions)</param>
		public Box(string boxName, string partId, string partName, GameObject partGameObject, int numberOfParts,
			Part parent, Vector3[] installLocations, Vector3[] installRotations, Vector3 defaultPosition,
			bool uninstallWhenParentUninstalls = true, bool disableCollisionWhenInstalled = true)

		{
			Setup(
				boxName,
				partId,
				partName,
				GameObject.Instantiate(boxTemplateModel),
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



		/// <summary>
		/// Constructor for a new Box
		/// </summary>
		/// <param name="boxName">What should be shown when the player looks at the box</param>
		/// <param name="partId">The partId used for every part created (with added index starting from 0). Has to be unique in your mod</param>
		/// <param name="partName">The name the created part(s) should have (with added index starting from 1)</param>
		/// <param name="customBoxModel">A custom (instantiated, unique) gameObject used as the "box" model</param>
		/// <param name="partGameObject">The GameObject instantiated when the Part object is created</param>
		/// <param name="numberOfParts">the amount of parts that should be created</param>
		/// <param name="parent">The parent (Part) for each created Part (All install to the same parent)</param>
		/// <param name="installLocations">The individual install positions on the parent for each created part</param>
		/// <param name="installRotations">The individual install rotations on the parent for each created part</param>
		/// <param name="defaultPosition">The default position for both the box and all parts (where to place after purchase or when reset)</param>
		/// <param name="uninstallWhenParentUninstalls">Should the part uninstall from the parent if the parent is uninstalled</param>
		/// <param name="disableCollisionWhenInstalled">Disables the collider of the part when installed to the parent (reduces lag, avoids problematic collisions)</param>
		public Box(string boxName, string partId, string partName, GameObject customBoxModel, GameObject partGameObject, int numberOfParts,
			Part parent, Vector3[] installLocations, Vector3[] installRotations, Vector3 defaultPosition,
			bool uninstallWhenParentUninstalls = true, bool disableCollisionWhenInstalled = true)
		{
			Setup(
				boxName,
				partId,
				partName,
				customBoxModel,
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

		/// <summary>
		/// The Setup method called by all constructors to allow a clean calling structure
		/// </summary>
		/// <param name="boxName">What should be shown when the player looks at the box</param>
		/// <param name="partId">The partId used for every part created (with added index starting from 0). Has to be unique in your mod</param>
		/// <param name="partName">The name the created part(s) should have (with added index starting from 1)</param>
		/// <param name="boxModel">The gameObject used as the "box" model</param>
		/// <param name="partGameObject">The GameObject instantiated when the Part object is created</param>
		/// <param name="numberOfParts">the amount of parts that should be created</param>
		/// <param name="parent">The parent (Part) for each created Part (All install to the same parent)</param>
		/// <param name="installLocations">The individual install positions on the parent for each created part</param>
		/// <param name="installRotations">The individual install rotations on the parent for each created part</param>
		/// <param name="defaultPosition">The default position for both the box and all parts (where to place after purchase or when reset)</param>
		/// <param name="uninstallWhenParentUninstalls">Should the part uninstall from the parent if the parent is uninstalled</param>
		/// <param name="disableCollisionWhenInstalled">Disables the collider of the part when installed to the parent (reduces lag, avoids problematic collisions)</param>
		protected void Setup(string boxName, string partId, string partName, GameObject boxModel, GameObject partGameObject, int numberOfParts,
			Part parent,
			Vector3[] installLocations, Vector3[] installRotations, Vector3 defaultPosition,
			bool uninstallWhenParentUninstalls, bool disableCollisionWhenInstalled)
		{
			PartBaseInfo partBaseInfo = parent.partBaseInfo;
			boxModel.SetNameLayerTag(boxName + "(Clone)");

			for (int i = 0; i < numberOfParts; i++)
			{
				int iOffset = i + 1;

				Part part = new Part(
					$"{partId}_{i}", partName + " " + iOffset, partGameObject,
					parent, installLocations[i], installRotations[i], partBaseInfo, uninstallWhenParentUninstalls,
					disableCollisionWhenInstalled);
				part.defaultPosition = defaultPosition;
				if (!part.bought)
				{
					part.Uninstall();
					part.active = false;
				}
				AddPart(part);
			}

			logic = boxModel.AddComponent<BoxLogic>();
			logic.Init("Unpack " + partName, this);

			this.boxModel = boxModel;
		}

		/// <summary>
		/// Method for adding screws to each part inside the box, screws get cloned to a new screw for each part.
		/// </summary>
		/// <param name="screws"></param>
		/// <param name="overrideScale"></param>
		/// <param name="overrideSize"></param>
		protected void AddScrews(Screw[] screws, float overrideScale = 0f, float overrideSize = 0f)
		{
			foreach (Part part in parts)
			{
				part.AddScrews(screws.CloneToNew(), overrideScale, overrideSize);
			}
		}

		internal static void LoadAssets(AssetBundle assetBundle)
		{
			boxTemplateModel = assetBundle.LoadAsset<GameObject>("cardboard_box.prefab");
		}
	}
}