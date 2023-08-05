using MSCLoader;
using MscModApi.Tools;
using MscModApi.Trigger;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MscModApi.Parts
{
	public class Part : BasicPart
	{
		public enum EventTime
		{
			Pre,
			Post
		}

		public enum EventType
		{
			Save,
			Install,
			Uninstall,
			Fixed,
			Unfixed
		}

		protected static GameObject clampModel;

		protected int clampsAdded;

		public List<Part> childParts { get; protected set; } = new List<Part>();

		public string id { get; protected set; }

		public PartBaseInfo partBaseInfo { get; protected set; }

		public GameObject gameObject { get; protected set; }
		internal PartSave partSave;

		public Vector3 installPosition { get; protected set; }
		public Vector3 installRotation { get; protected set; }

		public bool uninstallWhenParentUninstalls { get; protected set; }
		protected GameObject parentGameObject;
		protected Part parentPart;
		protected List<Screw> savedScrews;
		internal Collider collider;

		public TriggerWrapper trigger { get; protected set; }

		public Transform transform => gameObject.transform;

		protected bool usingGameObjectInstantiation;
		protected GameObject gameObjectUsedForInstantiation;
		protected bool usingPartParent;

		/// <summary>
		/// Stores all events that a developer may have added to this part object
		/// </summary>
		protected Dictionary<EventTime, Dictionary<EventType, List<Action>>> events =
			new Dictionary<EventTime, Dictionary<EventType, List<Action>>>();

		protected Dictionary<Screw, int> preScrewPlacementModeEnableTightnessMap = new Dictionary<Screw, int>();

		private bool _screwPlacementMode;
		protected bool injectedScrewPlacementDisablePreUninstall = false;

		public bool screwPlacementMode
		{
			get => _screwPlacementMode;
			set
			{
				if (!installed)
				{
					return;
				}

				if (!injectedScrewPlacementDisablePreUninstall)
				{
					injectedScrewPlacementDisablePreUninstall = true;
					AddEventListener(EventTime.Pre, EventType.Uninstall, () =>
					{
						this.screwPlacementMode = false;
					});
				}

				foreach (Screw screw in screws)
				{
					if (!value)
					{
						if (!preScrewPlacementModeEnableTightnessMap.TryGetValue(screw, out int preEnableTightness))
						{
							continue;
						}

						screw.tightness = Screw.maxTightness;
						screw.OutBy(Screw.maxTightness);
						screw.InBy(preEnableTightness);
						preScrewPlacementModeEnableTightnessMap.Remove(screw);
						continue;
					}

					if (preScrewPlacementModeEnableTightnessMap.ContainsKey(screw))
					{
						continue;
					}
					preScrewPlacementModeEnableTightnessMap.Add(screw, screw.tightness);
					screw.InBy(Screw.maxTightness);
					screw.tightness = 0;
				}

				if (!value && ScrewPlacementAssist.selectedPart == this)
				{
					ScrewPlacementAssist.HidePartInteraction();
				}

				_screwPlacementMode = value;
			}
		}

		public bool hasParent => trigger != null;

		public bool installBlocked { get; set; }

		public List<Screw> screws => partSave.screws;

		public bool installed => partSave.installed;

		/// <inheritdoc />
		protected Part()
		{
		}

		public Part(string id, string name, GameObject part, Part parentPart, Vector3 installPosition,
			Vector3 installRotation,
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

		public Part(string id, string name, PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls = true,
			bool disableCollisionWhenInstalled = true, string prefabName = null)
		{
			Setup(id, name, null, Vector3.zero, Vector3.zero, partBaseInfo,
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

		protected void Setup(string id, string name, GameObject parentGameObject, Vector3 installPosition,
			Vector3 installRotation, PartBaseInfo partBaseInfo, bool uninstallWhenParentUninstalls,
			bool disableCollisionWhenInstalled, string prefabName)
		{
			InitEventStorage();


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

			if (!partBaseInfo.partsSave.TryGetValue(id, out partSave))
			{
				partSave = new PartSave();
			}

			try
			{
				CustomSaveLoading(partBaseInfo.mod, $"{id}_saveFile.json");
			}
			catch
			{
				// ignored
			}

			savedScrews = new List<Screw>(partSave.screws);
			partSave.screws.Clear();

			collider = gameObject.GetComponent<Collider>();

			if (parentGameObject != null)
			{
				trigger = new TriggerWrapper(this, parentGameObject, disableCollisionWhenInstalled);
			}

			if (partSave.installed)
			{
				Install();
			}

			LoadPartPositionAndRotation(gameObject, partSave);

			if (!MscModApi.modSaveFileMapping.ContainsKey(partBaseInfo.mod.ID))
			{
				MscModApi.modSaveFileMapping.Add(partBaseInfo.mod.ID, partBaseInfo.saveFilePath);
			}

			if (MscModApi.modsParts.ContainsKey(partBaseInfo.mod.ID))
			{
				MscModApi.modsParts[partBaseInfo.mod.ID].Add(id, this);
			}
			else
			{
				MscModApi.modsParts.Add(partBaseInfo.mod.ID, new Dictionary<string, Part>
				{
					{ id, this }
				});
			}

			partBaseInfo.AddToPartsList(this);
		}

		private void InitEventStorage()
		{
			foreach (EventTime eventTime in Enum.GetValues(typeof(EventTime)))
			{
				Dictionary<EventType, List<Action>> eventTypeDict = new Dictionary<EventType, List<Action>>();

				foreach (EventType eventType in Enum.GetValues(typeof(EventType)))
				{
					eventTypeDict.Add(eventType, new List<Action>());
				}

				events.Add(eventTime, eventTypeDict);
			}
		}

		/// <inheritdoc />
		public override string name => gameObject.name;

		/// <inheritdoc />
		public override bool isLookingAt => gameObject.IsLookingAt();


		/// <inheritdoc />
		public override bool isHolding => gameObject.IsHolding();



		public bool parentInstalled
		{
			get
			{
				if (usingPartParent)
				{
					return parentPart.installed;
				}
				else
				{
					//Todo: Implement normal msc parts installed/uninstalled
					return true;
				}
			}
		}

		public bool parentFixed
		{
			get
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
		}

		/// <inheritdoc />
		public override bool bought
		{
			get => partSave.bought == PartSave.BoughtState.Yes;
			set => partSave.bought = value ? PartSave.BoughtState.Yes : PartSave.BoughtState.No;
		}

		/// <inheritdoc />
		public override Vector3 position
		{
			get => gameObject.transform.position;
			set
			{
				if (!installed)
				{
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
				if (!installed)
				{
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

		internal void ResetScrews()
		{
			foreach (var screw in partSave.screws)
			{
				screw.OutBy(screw.tightness);
			}
		}

		[Obsolete("Use 'screws' property instead", true)]
		public List<Screw> GetScrews()
		{
			return screws;
		}

		internal void SetScrewsActive(bool active)
		{
			partSave.screws.ForEach(delegate(Screw screw) { screw.gameObject.SetActive(active); });
		}

		public void Install()
		{
			trigger?.Install();
		}

		[Obsolete("Use 'installed' property instead", true)]
		public bool IsInstalled()
		{
			return installed;
		}

		public bool IsFixed(bool ignoreUnsetScrews = true)
		{
			if (!ignoreUnsetScrews)
			{
				return isFixed;
			}

			return partSave.screws.Count == 0 ? installed : isFixed;
		}

		public bool isFixed
		{
			get
			{
				return screws.All(screw => screw.tightness == Screw.maxTightness);
			}
		}

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


		[Obsolete("Use 'parentInstalled' property instead", true)]
		internal bool ParentInstalled()
		{
			return parentInstalled;
		}

		[Obsolete("Use 'parentFixed' property instead", true)]
		public bool ParentFixed()
		{
			return parentFixed;
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
			foreach (var screw in screws)
			{
				if (overrideScale != 0f)
				{
					screw.scale = overrideScale;
				}

				if (overrideSize != 0f)
				{
					screw.size = overrideSize;
				}

				AddScrew(screw);
			}
		}

		public void AddEventListener(EventTime eventTime, EventType eventType, Action action)
		{
			events[eventTime][eventType].Add(action);

			if (eventTime == EventTime.Post)
			{
				switch (eventType)
				{
					//ToDo: check if invoking just the newly added action is enough of if all have to be invoked
					case EventType.Install:
						if (installed)
						{
							action.Invoke();
						}
						break;
					case EventType.Uninstall:
						if (!installed)
						{
							action.Invoke();
						}
						break;
					case EventType.Fixed:
						if (IsFixed())
						{
							action.Invoke();
						}
						break;
					case EventType.Unfixed:
						if (!IsFixed())
						{
							action.Invoke();
						}
						break;
				}
			}
		}

		public List<Action> GetEvents(EventTime eventTime, EventType eventType)
		{
			return events[eventTime][eventType];
		}


		[Obsolete("Use cleaner 'AddEventListener' method instead", true)]
		public void AddPreSaveAction(Action action)
		{
			AddEventListener(EventTime.Pre, EventType.Save, action);
		}

		[Obsolete("Use cleaner 'AddEventListener' method instead", true)]
		public void AddPreInstallAction(Action action)
		{
			AddEventListener(EventTime.Pre, EventType.Install, action);
		}

		[Obsolete("Use cleaner 'AddEventListener' method instead", true)]
		public void AddPostInstallAction(Action action)
		{
			AddEventListener(EventTime.Post, EventType.Install, action);
		}

		[Obsolete("Use cleaner 'AddEventListener' method instead", true)]
		public void AddPreUninstallAction(Action action)
		{
			AddEventListener(EventTime.Pre, EventType.Uninstall, action);
		}

		[Obsolete("Use cleaner 'AddEventListener' method instead", true)]
		public void AddPostUninstallAction(Action action)
		{
			AddEventListener(EventTime.Post, EventType.Uninstall, action);
		}

		[Obsolete("Use cleaner 'AddEventListener' method instead", true)]
		public void AddPostFixedAction(Action action)
		{
			AddEventListener(EventTime.Post, EventType.Fixed, action);
		}

		[Obsolete("Use cleaner 'AddEventListener' method instead", true)]
		public void AddPreFixedAction(Action action)
		{
			AddEventListener(EventTime.Pre, EventType.Fixed, action);
		}

		[Obsolete("Use cleaner 'AddEventListener' method instead", true)]
		public void AddPreUnfixedActions(Action action)
		{
			AddEventListener(EventTime.Pre, EventType.Unfixed, action);
		}

		[Obsolete("Use cleaner 'AddEventListener' method instead", true)]
		public void AddPostUnfixedActions(Action action)
		{
			AddEventListener(EventTime.Post, EventType.Unfixed, action);
		}

		[Obsolete("Use AddWhenInstalledBehaviour instead. Will be removed in a later version", true)]
		public T AddWhenInstalledMono<T>() where T : MonoBehaviour
		{
			return AddWhenInstalledBehaviour<T>();
		}

		[Obsolete("Use AddWhenUninstalledBehaviour instead. Will be removed in a later version", true)]
		public T AddWhenUninstalledMono<T>() where T : MonoBehaviour
		{
			return AddWhenUninstalledBehaviour<T>();
		}

		public T AddWhenInstalledBehaviour<T>() where T : Behaviour
		{
			var behaviour = AddComponent<T>();
			behaviour.enabled = installed;


			AddEventListener(EventTime.Post, EventType.Install, delegate { behaviour.enabled = true; });

			AddEventListener(EventTime.Post, EventType.Uninstall, delegate { behaviour.enabled = false; });
			return behaviour;
		}

		public T AddWhenUninstalledBehaviour<T>() where T : Behaviour
		{
			var behaviour = AddComponent<T>();
			behaviour.enabled = !installed;

			AddEventListener(EventTime.Post, EventType.Install, delegate { behaviour.enabled = false; });

			AddEventListener(EventTime.Post, EventType.Uninstall, delegate { behaviour.enabled = true; });
			return behaviour;
		}

		public T AddComponent<T>() where T : Component => gameObject.AddComponent(typeof(T)) as T;

		public T GetComponent<T>() => gameObject.GetComponent<T>();

		/// <inheritdoc />
		public override void ResetToDefault(bool uninstall = false)
		{
			if (uninstall && installed)
			{
				Uninstall();
			}

			position = defaultPosition;
			rotation = defaultRotation;
		}

		[Obsolete("Use 'installBlocked' property instead", true)]
		public void BlockInstall(bool block)
		{
			installBlocked = block;
		}

		[Obsolete("Use 'installBlocked' property instead", true)]
		public bool IsInstallBlocked()
		{
			return installBlocked;
		}

		[Obsolete("Use 'hasParent' property instead", true)]
		public bool HasParent()
		{
			return hasParent;
		}

		[Obsolete("Use 'screwPlacementMode' property instead", true)]
		public void EnableScrewPlacementMode()
		{
			screwPlacementMode = true;
		}

		[Obsolete("Use 'screwPlacementMode' property instead", true)]
		internal bool IsInScrewPlacementMode()
		{
			return screwPlacementMode;
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