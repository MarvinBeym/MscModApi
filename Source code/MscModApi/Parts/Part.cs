using System;
using System.Collections.Generic;
using System.Linq;
using MSCLoader;
using MscModApi.Caching;
using MscModApi.Parts.ReplacePart;
using MscModApi.Tools;
using MscModApi.Trigger;
using UnityEngine;
using static MscModApi.Parts.PartEvent;

namespace MscModApi.Parts
{
	/// <summary>
	/// When should the collider of a part be disabled
	/// </summary>
	public enum DisableCollision
	{
		/// <summary>
		/// Disables the part collider when the part is installed on the parent (installed = true)
		/// </summary>
		InstalledOnParent,

		/// <summary>
		/// Disables the part collider when the part is installed to the car (installedOnCar = true)
		/// </summary>
		InstalledOnCar,

		/// <summary>
		/// Never disables the part collider, part can collide with everything.
		/// CAUTION!! potential conflicts with other GameObjects as well as extreme performance impact are possible
		/// </summary>
		Never
	}

	public class Part : BasicPart, SupportsPartEvents, SupportsPartBehaviourEvents
	{
		protected static GameObject clampModel;
		protected int clampsAdded;
		internal PartSave partSave;
		protected Dictionary<Screw, int> preScrewPlacementModeEnableTightnessMap = new Dictionary<Screw, int>();
		private bool _screwPlacementMode;
		protected bool injectedScrewPlacementDisablePreUninstall;

		/// <summary>
		/// Stores all events that a developer may have added to this part object
		/// </summary>
		protected Dictionary<PartEvent.Time, Dictionary<PartEvent.Type, List<Action>>> events =
			new Dictionary<PartEvent.Time, Dictionary<PartEvent.Type, List<Action>>>();

		/// <inheritdoc />
		protected Part()
		{
		}

		public Part(string id, string name, GameObject part, Part parent, Vector3 installPosition,
			Vector3 installRotation,
			PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = false,
			DisableCollision disableCollisionWhenInstalled = DisableCollision.InstalledOnCar) : this(id, name, part,
			(BasicPart)parent, installPosition, installRotation, partBaseInfo, uninstallWhenParentUninstalls,
			disableCollisionWhenInstalled)
		{
		}

		public Part(string id, string name, GameObject part, GamePart parent, Vector3 installPosition,
			Vector3 installRotation,
			PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = false,
			DisableCollision disableCollisionWhenInstalled = DisableCollision.InstalledOnCar) : this(id, name, part,
			(BasicPart)parent, installPosition, installRotation, partBaseInfo, uninstallWhenParentUninstalls,
			disableCollisionWhenInstalled)
		{
		}

		protected Part(string id, string name, GameObject part, BasicPart parent, Vector3 installPosition,
			Vector3 installRotation,
			PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = false,
			DisableCollision disableCollisionWhenInstalled = DisableCollision.InstalledOnCar)
		{
			gameObjectUsedForInstantiation = part;

			Setup(id, name, parent, installPosition, installRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, null);
		}

		public Part(string id, string name, PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = false,
			DisableCollision disableCollisionWhenInstalled = DisableCollision.InstalledOnCar, string prefabName = null)
		{
			Setup(id, name, null, Vector3.zero, Vector3.zero, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, prefabName);
		}

		public Part(string id, string name, Part parent, Vector3 installPosition, Vector3 installRotation,
			PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = false,
			DisableCollision disableCollisionWhenInstalled = DisableCollision.InstalledOnCar,
			string prefabName = null) : this(id, name, (BasicPart)parent, installPosition, installRotation,
			partBaseInfo, uninstallWhenParentUninstalls, disableCollisionWhenInstalled, prefabName)
		{
		}

		public Part(string id, string name, GamePart parent, Vector3 installPosition, Vector3 installRotation,
			PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = false,
			DisableCollision disableCollisionWhenInstalled = DisableCollision.InstalledOnCar,
			string prefabName = null) : this(id, name, (BasicPart)parent, installPosition, installRotation,
			partBaseInfo, uninstallWhenParentUninstalls, disableCollisionWhenInstalled, prefabName)
		{
		}

		protected Part(string id, string name, BasicPart parent, Vector3 installPosition, Vector3 installRotation,
			PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = false,
			DisableCollision disableCollisionWhenInstalled = DisableCollision.InstalledOnCar, string prefabName = null)
		{
			Setup(id, name, parent, installPosition, installRotation, partBaseInfo,
				uninstallWhenParentUninstalls, disableCollisionWhenInstalled, prefabName);
		}

		public string id { get; protected set; }

		public PartBaseInfo partBaseInfo { get; protected set; }

		public override GameObject gameObject { get; protected set; }

		public Vector3 installPosition { get; protected set; }

		public Vector3 installRotation { get; protected set; }

		public BasicPart parent { get; protected set; }

		public bool uninstallWhenParentUninstalls { get; protected set; }

		protected List<Screw> savedScrews;

		public Collider collider { get; protected set; }

		public TriggerWrapper trigger { get; protected set; }

		public Transform transform => gameObject.transform;

		public GameObject gameObjectUsedForInstantiation { get; protected set; }

		public bool hasParent => parent != null;

		public override bool installBlocked { get; set; }

		public List<Screw> screws => partSave.screws;

		public override bool installed => partSave.installed;

		public bool screwPlacementMode
		{
			get => _screwPlacementMode;
			set
			{
				if (!installed) {
					return;
				}

				if (!injectedScrewPlacementDisablePreUninstall) {
					injectedScrewPlacementDisablePreUninstall = true;
					AddEventListener(PartEvent.Time.Pre, PartEvent.Type.Uninstall,
						() => { screwPlacementMode = false; });
				}

				foreach (Screw screw in screws) {
					if (!value) {
						if (!preScrewPlacementModeEnableTightnessMap.TryGetValue(screw, out int preEnableTightness)) {
							continue;
						}

						screw.tightness = Screw.maxTightness;
						screw.OutBy(Screw.maxTightness);
						screw.InBy(preEnableTightness);
						preScrewPlacementModeEnableTightnessMap.Remove(screw);
						continue;
					}

					if (preScrewPlacementModeEnableTightnessMap.ContainsKey(screw)) {
						continue;
					}

					preScrewPlacementModeEnableTightnessMap.Add(screw, screw.tightness);
					screw.InBy(Screw.maxTightness);
					screw.tightness = 0;
				}

				if (!value && ScrewPlacementAssist.selectedPart == this) {
					ScrewPlacementAssist.HidePartInteraction();
				}

				_screwPlacementMode = value;
			}
		}

		/// <inheritdoc />
		public override string name => gameObject.name;

		/// <inheritdoc />
		public override bool isLookingAt => gameObject.IsLookingAt();

		/// <inheritdoc />
		public override bool isHolding => gameObject.IsHolding();

		public bool installPossible => !installBlocked && bought && trigger != null;

		/// <inheritdoc />
		public override bool bought
		{
			get => partSave.bought == PartSave.BoughtState.Yes || partSave.bought == PartSave.BoughtState.NotConfigured;
			set => partSave.bought = value ? PartSave.BoughtState.Yes : PartSave.BoughtState.No;
		}

		/// <inheritdoc />
		public override Vector3 position
		{
			get => gameObject.transform.position;
			set
			{
				if (!installed) {
					gameObject.transform.position = value;
				}
			}
		}

		/// <inheritdoc />
		public override Vector3 rotation
		{
			get => gameObject.transform.rotation.eulerAngles;
			set
			{
				if (!installed) {
					gameObject.transform.rotation = Quaternion.Euler(value);
				}
			}
		}

		/// <inheritdoc />
		public override bool active
		{
			get => gameObject.activeSelf;
			set => gameObject.SetActive(value);
		}

		public override bool bolted
		{
			get { return screws.Count > 0 && screws.All(screw => screw.tightness == Screw.maxTightness) && installed; }
		}

		public override bool installedOnCar => installed && gameObject.transform.root == CarH.satsuma.transform;

		protected void Setup(string id, string name, BasicPart parent, Vector3 installPosition,
			Vector3 installRotation, PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls,
			DisableCollision disableCollisionWhenInstalled, string prefabName)
		{
			InitEventStorage();
			this.id = id;
			this.partBaseInfo = partBaseInfo;
			this.installPosition = installPosition;
			this.uninstallWhenParentUninstalls = uninstallWhenParentUninstalls;
			this.installRotation = installRotation;
			this.parent = parent;

			if (gameObjectUsedForInstantiation != null) {
				gameObject = GameObject.Instantiate(gameObjectUsedForInstantiation);
				gameObject.SetNameLayerTag(name + "(Clone)");
			}
			else {
				gameObject = Helper.LoadPartAndSetName(partBaseInfo.assetBundle, prefabName ?? id, name);
			}

			if (!partBaseInfo.partsSave.TryGetValue(id, out partSave)) {
				partSave = new PartSave();
			}

			try {
				CustomSaveLoading(partBaseInfo.mod, $"{id}_saveFile.json");
			}
			catch (Exception) {
				// ignored
			}

			savedScrews = new List<Screw>(partSave.screws);
			partSave.screws.Clear();

			collider = gameObject.GetComponent<Collider>();

			if (parent != null) {
				trigger = new TriggerWrapper(this, parent, disableCollisionWhenInstalled);
				parent.AddChild(this);
			}

			if (partSave.installed) {
				Install();
			}

			LoadPartPositionAndRotation(gameObject, partSave);

			if (!MscModApi.modSaveFileMapping.ContainsKey(partBaseInfo.mod.ID)) {
				MscModApi.modSaveFileMapping.Add(partBaseInfo.mod.ID, partBaseInfo.saveFilePath);
			}

			if (MscModApi.modsParts.TryGetValue(partBaseInfo.mod.ID, out var modParts)) {
				modParts.Add(id, this);
			}
			else {
				MscModApi.modsParts.Add(partBaseInfo.mod.ID, new Dictionary<string, Part>
				{
					{ id, this }
				});
			}

			partBaseInfo.AddToPartsList(this);
		}

		protected void InitEventStorage()
		{
			foreach (PartEvent.Time eventTime in Enum.GetValues(typeof(PartEvent.Time))) {
				Dictionary<PartEvent.Type, List<Action>> TypeDict = new Dictionary<PartEvent.Type, List<Action>>();

				foreach (PartEvent.Type Type in Enum.GetValues(typeof(PartEvent.Type))) {
					TypeDict.Add(Type, new List<Action>());
				}

				events.Add(eventTime, TypeDict);
			}
		}

		internal void ResetScrews()
		{
			foreach (var screw in partSave.screws) {
				screw.OutBy(screw.tightness);
			}
		}

		internal void SetScrewsActive(bool active)
		{
			partSave.screws.ForEach(delegate(Screw screw) { screw.gameObject.SetActive(active); });
		}

		public void Install()
		{
			trigger?.Install();
		}

		public override void Uninstall()
		{
			trigger?.Uninstall();
		}

		private void LoadPartPositionAndRotation(GameObject gameObject, PartSave partSave)
		{
			position = partSave.position;
			Quaternion tmpRotation = (partSave.rotation);
			rotation = tmpRotation.eulerAngles;
		}

		public void AddScrew(Screw screw)
		{
			screw.Verify();
			screw.SetPart(this);
			screw.parentCollider = gameObject.GetComponent<Collider>();
			partSave.screws.Add(screw);

			var index = partSave.screws.IndexOf(screw);

			screw.CreateScrewModel(index);

			screw.LoadTightness(savedScrews.ElementAtOrDefault(index));
			screw.InBy(screw.tightness, false, true);

			screw.gameObject.SetActive(installed);

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

		/// <inheritdoc />
		public T AddEventBehaviour<T>(PartEvent.Type Type) where T : Behaviour
		{
			var behaviour = AddComponent<T>();
			switch (Type) {
				case PartEvent.Type.Install:
					behaviour.enabled = installed;
					AddEventListener(PartEvent.Time.Post, Type, () => behaviour.enabled = true);
					AddEventListener(PartEvent.Time.Post, PartEvent.Type.Uninstall, () => behaviour.enabled = false);
					break;
				case PartEvent.Type.Uninstall:
					behaviour.enabled = !installed;
					AddEventListener(PartEvent.Time.Post, Type, () => behaviour.enabled = true);
					AddEventListener(PartEvent.Time.Post, PartEvent.Type.Install, () => behaviour.enabled = false);
					break;
				case PartEvent.Type.InstallOnCar:
					behaviour.enabled = installedOnCar;
					AddEventListener(PartEvent.Time.Post, Type, () => behaviour.enabled = true);
					AddEventListener(PartEvent.Time.Post, PartEvent.Type.UninstallFromCar,
						() => behaviour.enabled = false);
					break;
				case PartEvent.Type.UninstallFromCar:
					behaviour.enabled = !installedOnCar;
					AddEventListener(PartEvent.Time.Post, Type, () => behaviour.enabled = true);
					AddEventListener(PartEvent.Time.Post, PartEvent.Type.InstallOnCar, () => behaviour.enabled = false);
					break;
				case PartEvent.Type.Bolted:
					behaviour.enabled = bolted;
					AddEventListener(PartEvent.Time.Post, Type, () => behaviour.enabled = true);
					AddEventListener(PartEvent.Time.Post, PartEvent.Type.Unbolted, () => behaviour.enabled = false);
					break;
				case PartEvent.Type.Unbolted:
					behaviour.enabled = !bolted;
					AddEventListener(PartEvent.Time.Post, Type, () => behaviour.enabled = true);
					AddEventListener(PartEvent.Time.Post, PartEvent.Type.Bolted, () => behaviour.enabled = false);
					break;
				case PartEvent.Type.BoltedOnCar:
					behaviour.enabled = bolted && installedOnCar;
					AddEventListener(PartEvent.Time.Post, Type, () => behaviour.enabled = true);
					AddEventListener(PartEvent.Time.Post, PartEvent.Type.UnboltedOnCar,
						() => behaviour.enabled = false);
					break;
				case PartEvent.Type.UnboltedOnCar:
					behaviour.enabled = !bolted && installedOnCar;
					AddEventListener(PartEvent.Time.Post, Type, () => behaviour.enabled = true);
					AddEventListener(PartEvent.Time.Post, PartEvent.Type.BoltedOnCar, () => behaviour.enabled = false);
					break;
			}

			return behaviour;
		}

		/// <summary>
		/// When this part installs, the "partsToBlock" parts will be blocked from being installed (installBlocked = true)
		/// When this part uninstalls (opposite of "Type") the "partsToBlock" parts will be unblocked from being installed (installBlocked = false)
		/// </summary>
		/// <param name="Type">The event of this part after which installation of the "partsToBlock" will be blocked/unblocked</param>
		/// <param name="partsToBlock">The parts to block when the "Type" is called on this part</param>
		public void BlockOtherPartInstallOnEvent(PartEvent.Type Type, IEnumerable<BasicPart> partsToBlock)
		{
			AddEventListener(PartEvent.Time.Post, Type, () =>
			{
				foreach (BasicPart partToBlock in partsToBlock) {
					partToBlock.installBlocked = true;
				}
			});
			AddEventListener(PartEvent.Time.Post, GetOppositeEvent(Type), () =>
			{
				foreach (BasicPart partToBlock in partsToBlock) {
					partToBlock.installBlocked = false;
				}
			});
		}

		/// <summary>
		/// When this part installs, the "partToBlock" part will be blocked from being installed (installBlocked = true)
		/// When this part uninstalls (opposite of "Type") the "partToBlock" part will be unblocked from being installed (installBlocked = false)
		/// </summary>
		/// <param name="Type">The event of this part after which installation of the "partToBlock" will be blocked/unblocked</param>
		/// <param name="partToBlock">The part to block when the "Type" is called on this part</param>
		public void BlockOtherPartInstallOnEvent(PartEvent.Type Type, BasicPart partToBlock)
		{
			AddEventListener(PartEvent.Time.Post, Type, () => { partToBlock.installBlocked = true; });
			AddEventListener(PartEvent.Time.Post, GetOppositeEvent(Type),
				() => { partToBlock.installBlocked = false; });
		}

		public void AddEventListener(PartEvent.Time eventTime, PartEvent.Type Type, Action action,
			bool invokeActionIfConditionMet = true)
		{
			events[eventTime][Type].Add(action);

			if (invokeActionIfConditionMet && eventTime == PartEvent.Time.Post) {
				switch (Type) {
					//ToDo: check if invoking just the newly added action is enough of if all have to be invoked
					case PartEvent.Type.Install:
						if (installed) {
							action.Invoke();
						}

						break;
					case PartEvent.Type.Uninstall:
						if (!installed) {
							action.Invoke();
						}

						break;
					case PartEvent.Type.Bolted:
						if (bolted) {
							action.Invoke();
						}

						break;
					case PartEvent.Type.Unbolted:
						if (!bolted) {
							action.Invoke();
						}

						break;
					case PartEvent.Type.InstallOnCar:
						if (installedOnCar) {
							action.Invoke();
						}

						break;
					case PartEvent.Type.UninstallFromCar:
						if (!installedOnCar) {
							action.Invoke();
						}

						break;
					case PartEvent.Type.BoltedOnCar:
						if (bolted && installedOnCar) {
							action.Invoke();
						}

						break;
					case PartEvent.Type.UnboltedOnCar:
						if (!bolted && installedOnCar) {
							action.Invoke();
						}

						break;
				}
			}
		}

		public List<Action> GetEvents(PartEvent.Time eventTime, PartEvent.Type Type)
		{
			return events[eventTime][Type];
		}

		public T AddComponent<T>() where T : Component => gameObject.AddComponent(typeof(T)) as T;

		public T GetComponent<T>() => gameObject.GetComponent<T>();

		/// <inheritdoc />
		public override void ResetToDefault(bool uninstall = false)
		{
			if (uninstall && installed) {
				Uninstall();
			}

			position = defaultPosition;
			rotation = defaultRotation;
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