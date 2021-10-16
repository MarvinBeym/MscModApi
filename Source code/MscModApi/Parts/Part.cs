using MscModApi.Tools;
using MscModApi.Trigger;
using System;
using System.Collections.Generic;
using System.Linq;
using MSCLoader;
using UnityEngine;

namespace MscModApi.Parts
{
	public class Part
	{
		private int clampsAdded;
		private bool partFixed;

		internal List<Part> childParts = new List<Part>();
		public string id;
		public PartBaseInfo partBaseInfo;
		public GameObject gameObject;
		internal PartSave partSave;
		internal Vector3 installPosition;
		internal bool uninstallWhenParentUninstalls;
		internal Vector3 installRotation;
		private GameObject parentGameObject;
		private Part parentPart;
		private List<Screw> savedScrews;
		internal Collider collider;
		public TriggerWrapper trigger;

		public Transform transform => gameObject.transform;
		
		private bool usingGameObjectInstantiation;
		private GameObject gameObjectUsedForInstantiation;
		private bool usingPartParent;

		internal List<Action> preSaveActions = new List<Action>();

		internal List<Action> preInstallActions = new List<Action>();
		internal List<Action> postInstallActions = new List<Action>();

		internal List<Action> preUninstallActions = new List<Action>();
		internal List<Action> postUninstallActions = new List<Action>();

		internal List<Action> preFixedActions = new List<Action>();
		internal List<Action> postFixedActions = new List<Action>();

		internal List<Action> preUnfixedActions = new List<Action>();
		internal List<Action> postUnfixedActions = new List<Action>();

		internal bool screwPlacementMode;
		private Vector3 defaultRotation = Vector3.zero;
		private Vector3 defaultPosition = Vector3.zero;
		private bool installBlocked;
		private static GameObject clampModel;

		private void Setup(string id, string name, GameObject parentGameObject, Vector3 installPosition,
			Vector3 installRotation, PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls,
			bool disableCollisionWhenInstalled, string prefabName)
		{
			this.id = id;
			this.partBaseInfo = partBaseInfo;
			this.installPosition = installPosition;
			this.uninstallWhenParentUninstalls = uninstallWhenParentUninstalls;
			this.installRotation = installRotation;
			this.parentGameObject = parentGameObject;

			if (usingGameObjectInstantiation)
			{
				gameObject = GameObject.Instantiate(gameObjectUsedForInstantiation);
				gameObject.SetNameLayerTag(name + "(Clone)", "PART", "Parts");
			}
			else
			{
				gameObject = Helper.LoadPartAndSetName(partBaseInfo.assetBundle, prefabName ?? id, name);
			}

			if (!partBaseInfo.partsSave.TryGetValue(id, out partSave)) {
				partSave = new PartSave();
			}

			try {
				CustomSaveLoading(partBaseInfo.mod, $"{id}_saveFile.json");
			} catch {
				// ignored
			}

			savedScrews = new List<Screw>(partSave.screws);
			partSave.screws.Clear();

			collider = gameObject.GetComponent<Collider>();

			if (parentGameObject != null)
			{
				trigger = new TriggerWrapper(this, parentGameObject, disableCollisionWhenInstalled);
			}
			
			if (partSave.installed) {
				Install();
			}

			LoadPartPositionAndRotation(gameObject, partSave);

			if (!MscModApi.modSaveFileMapping.ContainsKey(partBaseInfo.mod.ID)) {
				MscModApi.modSaveFileMapping.Add(partBaseInfo.mod.ID, partBaseInfo.saveFilePath);
			}

			if (MscModApi.modsParts.ContainsKey(partBaseInfo.mod.ID)) {
				MscModApi.modsParts[partBaseInfo.mod.ID].Add(id, this);
			} else {
				MscModApi.modsParts.Add(partBaseInfo.mod.ID, new Dictionary<string, Part>
				{
					{id, this}
				});
			}

			if (MscModApi.globalScrewPlacementModeEnabled.Contains(partBaseInfo.mod))
			{
				EnableScrewPlacementMode();
			}
		}

		public Part(string id, string name, GameObject part, Part parentPart, Vector3 installPosition, Vector3 installRotation,
			PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true,
			bool disableCollisionWhenInstalled = true)
		{
			usingGameObjectInstantiation = true;
			gameObjectUsedForInstantiation = part;

			usingPartParent = true;
			this.parentPart = parentPart;

			Setup(id, name, parentPart.gameObject, installPosition, installRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, null);
			parentPart.childParts.Add(this);
		}

		public Part(string id, string name, GameObject parent, Vector3 installPosition, Vector3 installRotation,
			PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true,
			bool disableCollisionWhenInstalled = true, string prefabName = null)
		{
			Setup(id, name, parent, installPosition, installRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, prefabName);
		}

		public Part(string id, string name, Vector3 installPosition, Vector3 installRotation,
			PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true,
			bool disableCollisionWhenInstalled = true, string prefabName = null)
		{
			Setup(id, name, null, installPosition, installRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, prefabName);
		}

		public Part(string id, string name, Part parentPart, Vector3 installPosition, Vector3 installRotation,
			PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true,
			bool disableCollisionWhenInstalled = true, string prefabName = null)
		{
			usingPartParent = true;
			this.parentPart = parentPart;
			Setup(id, name, parentPart.gameObject, installPosition, installRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, prefabName);
			parentPart.childParts.Add(this);
		}


		public void EnableScrewPlacementMode() => screwPlacementMode = true;

		internal bool IsInScrewPlacementMode()
		{
			return screwPlacementMode;
		}

		public void SetPosition(Vector3 position)
		{
			if (!IsInstalled()) {
				gameObject.transform.position = position;
			}
		}

		internal void ResetScrews()
		{
			foreach (var screw in partSave.screws) {
				screw.OutBy(screw.tightness);
			}
		}

		public List<Screw> GetScrews()
		{
			return partSave.screws;
		}

		internal void SetScrewsActive(bool active)
		{
			partSave.screws.ForEach(delegate (Screw screw) { screw.gameObject.SetActive(active); });
		}

		public void SetRotation(Quaternion rotation)
		{
			if (!IsInstalled()) {
				gameObject.transform.rotation = rotation;
			}
		}

		public void Install()
		{
			trigger?.Install();
		}

		public bool IsInstalled()
		{
			return partSave.installed;
		}

		public bool IsFixed(bool ignoreUnsetScrews = true)
		{
			if (!ignoreUnsetScrews) {
				return partFixed;
			}
			return partSave.screws.Count == 0 ? IsInstalled() : partFixed;
		}

		public void SetFixed(bool partFixed) => this.partFixed = partFixed;

		public void Uninstall()
		{
			trigger?.Uninstall();
		}

		public void AddClampModel(Vector3 position, Vector3 rotation, Vector3 scale)
		{
			var clamp = GameObject.Instantiate(clampModel);
			clamp.name = $"{gameObject.name}_clamp_{clampsAdded}";
			clampsAdded++;
			clamp.transform.SetParent(gameObject.transform);
			clamp.transform.localPosition = position;
			clamp.transform.localScale = scale;
			clamp.transform.localRotation = new Quaternion { eulerAngles = rotation };
		}

		internal bool ParentInstalled()
		{
			if (usingPartParent) {
				return parentPart.IsInstalled();
			} else {
				//Todo: Implement normal msc parts installed/uninstalled
				return true;
			}
		}

		public bool ParentFixed()
		{
			if (usingPartParent)
			{
				return parentPart.IsFixed(true);
			}
			else
			{
				//Todo: Implement normal msc parts fixed
				return true;
			}
		}

		private void LoadPartPositionAndRotation(GameObject gameObject, PartSave partSave)
		{
			SetPosition(partSave.position);
			SetRotation(partSave.rotation);
		}

		public void AddScrew(Screw screw)
		{
			screw.Verify();
			screw.SetPart(this);
			screw.parentCollider = gameObject.GetComponent<Collider>();
			partSave.screws.Add(screw);

			var index = partSave.screws.IndexOf(screw);

			screw.CreateScrewModel(index);

			if (!screwPlacementMode) {
				screw.LoadTightness(savedScrews.ElementAtOrDefault(index));
				screw.InBy(screw.tightness, false, true);
			}

			screw.gameObject.SetActive(IsInstalled());

			MscModApi.screws.Add(screw.gameObject.name, screw);
		}

		internal static void LoadAssets(AssetBundle assetBundle)
		{
			clampModel = assetBundle.LoadAsset<GameObject>("clamp.prefab");
		}

		public void AddScrews(Screw[] screws, float overrideScale = 0f, float overrideSize = 0f)
		{
			foreach (var screw in screws) {
				if (overrideScale != 0f) {
					screw.scale = overrideScale;
				}
				
				if (overrideSize != 0f) {
					screw.size = overrideSize;
				}

				AddScrew(screw);
			}
		}

		public void AddPreSaveAction(Action action)
		{
			preSaveActions.Add(action);
		}
		public void AddPreInstallAction(Action action)
		{
			preInstallActions.Add(action);
		}

		public void AddPostInstallAction(Action action)
		{
			postInstallActions.Add(action);
			if (IsInstalled())
			{
				postInstallActions.InvokeAll();
			}
		}

		public void AddPreUninstallAction(Action action)
		{
			preUninstallActions.Add(action);
		}

		public void AddPostUninstallAction(Action action)
		{
			postUninstallActions.Add(action);
			if (!IsInstalled())
			{
				postUninstallActions.InvokeAll();
			}
		}

		public void AddPostFixedAction(Action action)
		{
			postFixedActions.Add(action);
			if (IsFixed())
			{
				postFixedActions.InvokeAll();
			}
		}

		public void AddPreFixedAction(Action action)
		{
			preFixedActions.Add(action);
		}

		public void AddPreUnfixedActions(Action action)
		{
			preUnfixedActions.Add(action);

		}
		public void AddPostUnfixedActions(Action action)
		{
			postUnfixedActions.Add(action);
			if (!IsFixed())
			{
				postUnfixedActions.InvokeAll();
			}

		}

		public T AddWhenInstalledMono<T>() where T : MonoBehaviour
		{
			var mono = AddComponent<T>();
			mono.enabled = IsInstalled();

			AddPostInstallAction(delegate
			{
				mono.enabled = true;
			});

			AddPostUninstallAction(delegate {
				mono.enabled = false;
			});
			return mono;
		}

		public T AddWhenUninstalledMono<T>() where T : MonoBehaviour
		{
			var mono = AddComponent<T>();
			mono.enabled = !IsInstalled();

			AddPostInstallAction(delegate {
				mono.enabled = false;
			});

			AddPostUninstallAction(delegate {
				mono.enabled = true;
			});
			return mono;
		}
		
		public T AddComponent<T>() where T : Component => gameObject.AddComponent(typeof(T)) as T;

		public T GetComponent<T>() => gameObject.GetComponent<T>();

		public void SetBought(bool bought)
		{
			partSave.bought = bought ? PartSave.BoughtState.Yes : PartSave.BoughtState.No;
		}

		public bool IsBought()
		{
			return partSave.bought == PartSave.BoughtState.Yes;
		}

		public void SetActive(bool active)
		{
			gameObject.SetActive(active);
		}

		public void SetDefaultPosition(Vector3 defaultPosition)
		{
			this.defaultPosition = defaultPosition;
		}

		public void SetDefaultRotation(Vector3 defaultRotation)
		{
			this.defaultRotation = defaultRotation;
		}

		public void ResetToDefault(bool uninstall = false)
		{
			if (uninstall && IsInstalled())
			{
				Uninstall();
			}
			SetPosition(defaultPosition);
			SetRotation(Quaternion.Euler(defaultRotation));
		}

		public void BlockInstall(bool block)
		{
			installBlocked = block;
		}

		public bool IsInstallBlocked()
		{
			return installBlocked;
		}

		public bool HasParent()
		{
			return trigger != null;
		}

		public virtual void CustomSaveLoading(Mod mod, string saveFileName)
		{
			throw new Exception("Only subclasses should not throw an error");
		}

		public virtual void CustomSaveSaving(Mod mod, string saveFileName)
		{
			throw new Exception("Only subclasses should not throw an error");
		}
	}
}