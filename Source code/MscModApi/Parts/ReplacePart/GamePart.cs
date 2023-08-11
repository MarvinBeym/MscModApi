using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using MSCLoader;
using MscModApi.Caching;
using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.Parts.ReplacePart
{
	/// <summary>
	/// (NOTE this class is considered "BETA" with not a lot of tests for different parts)
	/// A wrapper for parts made/added by the game itself
	/// In the future, this may become a replacement for the 'OldPart' class used for the 'ReplacementPart' as a more generic Wrapper
	/// </summary>
	public class GamePart : BasicPart, SupportsPartEvents
	{
		/// <summary>
		/// Stores all events that a developer may have added to this GamePart object
		/// </summary>
		protected Dictionary<EventTime, Dictionary<EventType, List<Action>>> events =
			new Dictionary<EventTime, Dictionary<EventType, List<Action>>>();

		/// <summary>
		/// Flag used to avoid calling the pre bolted event multiple times
		/// </summary>
		protected bool alreadyCalledPreBolted;
		
		/// <summary>
		/// Flag used to avoid calling the post bolted event multiple times
		/// </summary>
		protected bool alreadyCalledPostBolted;

		/// <summary>
		/// Flag used to avoid calling the pre unbolted event multiple times
		/// </summary>
		protected bool alreadyCalledPreUnbolted = true;

		/// <summary>
		/// Flag used to avoid calling the post unbolted event multiple times
		/// </summary>
		protected bool alreadyCalledPostUnbolted = true;

		/// <summary>
		/// Flag that defines if the part should be setup with simple or advanced bolted state detection
		/// </summary>
		protected readonly bool simpleBoltedStateDetection;


		/// <summary>
		/// Creates a new GamePart wrapper object
		/// </summary>
		/// <param name="mainFsmPartName">The main GameObject name (capital letter name) Ex.: "Steel Headers"</param>
		/// <param name="simpleBoltedStateDetection">Should the simple bolt detection be used (one bolt tightened a bit = whole part tightened)</param>
		public GamePart(string mainFsmPartName, bool simpleBoltedStateDetection = true)
		{
			InitEventStorage();
			this.simpleBoltedStateDetection = simpleBoltedStateDetection;
			mainFsmGameObject = Cache.Find(mainFsmPartName);
			if (!mainFsmGameObject)
			{
				throw new Exception($"Unable to find main fsm part GameObject using '{mainFsmPartName}'");
			}

			dataFsm = mainFsmGameObject.FindFsm("Data");
			if (!dataFsm)
			{
				throw new Exception($"Unable to find data fsm on GameObject with name '{mainFsmGameObject.name}'");
			}

			triggerFsmGameObject = dataFsm.FsmVariables.FindFsmGameObject("Trigger").Value;
			if (!triggerFsmGameObject)
			{
				throw new Exception($"Unable to find trigger GameObject on GameObject with name '{mainFsmGameObject.name}'");
			}

			partFsmGameObject = dataFsm.FsmVariables.FindFsmGameObject("ThisPart").Value;
			if (!partFsmGameObject)
			{
				throw new Exception($"Unable to find part GameObject on GameObject with name '{mainFsmGameObject.name}'");
			}

			boltedState = dataFsm.FsmVariables.FindFsmBool("Bolted");
			damagedState = dataFsm.FsmVariables.FindFsmBool("Damaged") ?? new FsmBool("Damaged");
			installedState = dataFsm.FsmVariables.FindFsmBool("Installed") ?? new FsmBool("Installed");
			purchasedState = dataFsm.FsmVariables.FindFsmBool("Purchased") ?? new FsmBool("Purchased");

			assemblyFsm = triggerFsmGameObject.FindFsm("Assembly");
			removalFsm = partFsmGameObject.FindFsm("Removal");
			boltCheckFsm = partFsmGameObject.FindFsm("BoltCheck");

			if (!assemblyFsm.Fsm.Initialized)
			{
				assemblyFsm.InitializeFSM();
			}

			if (!removalFsm.Fsm.Initialized)
			{
				removalFsm.InitializeFSM();
			}

			if (!boltCheckFsm.Fsm.Initialized)
			{
				boltCheckFsm.InitializeFSM();
			}

			tightness = boltCheckFsm.FsmVariables.FindFsmFloat("Tightness");

			AddActionAsFirst(assemblyFsm.FindState("Assemble"), () =>
			{
				GetEvents(EventTime.Pre, EventType.Install).InvokeAll();
			});

			AddActionAsLast(assemblyFsm.FindState("End"), () =>
			{
				GetEvents(EventTime.Post, EventType.Install).InvokeAll();
			});

			AddActionAsFirst(removalFsm.FindState("Remove part"), () =>
			{
				GetEvents(EventTime.Pre, EventType.Uninstall).InvokeAll();
			});
			AddActionAsLast(removalFsm.FindState("Remove part"), () =>
			{
				GetEvents(EventTime.Post, EventType.Uninstall).InvokeAll();
			});

			if (tightness == null)
			{
				throw new Exception($"Unable to find tightness on bolt check fsm of part '{partFsmGameObject.name}'");
			}

			if (boltedState != null)
			{
				if (simpleBoltedStateDetection)
				{
					SetupSimpleBoltedStateDetection();
				}
				else
				{
					SetupAdvancedBoltedStateDetection();
				}
			}
			else
			{
				boltedState = new FsmBool(); //Avoiding null
			}
		}

		/// <summary>
		/// Setups the advanced bolted state detection requiring all bolts of the part to be tight before bolted events get called
		/// </summary>
		protected void SetupAdvancedBoltedStateDetection()
		{
			GameObject boltsGameObject = partFsmGameObject.FindChild("Bolts");
			if (!boltsGameObject)
			{
				ModConsole.Print($"GamePart: Unable to find 'Bolts' child of '{partFsmGameObject.name}'. Bolted event listening not possible");
			}

			for (int i = 0; i < boltsGameObject.transform.childCount; i++)
			{
				GameObject boltGameObject;
				try
				{
					boltGameObject = boltsGameObject.transform.GetChild(i).gameObject;
					if (!boltGameObject)
					{
						throw new Exception("Null GameObject");
					}
				}
				catch (Exception)
				{
					continue;
				}

				PlayMakerFSM boltFsm = boltGameObject.FindFsm("Screw");
				if (!boltFsm)
				{
					continue;
				}


				FsmState tightState = boltFsm.FindState("8 2");
				FsmState unscrewPreState = boltFsm.FindState("Unscrew 2");
				FsmState unscrewPostState = boltFsm.FindState("Wait 4");
				if (tightState == null || unscrewPreState == null || unscrewPostState == null)
				{
					return;
				}

				if (!boltFsm.Fsm.Initialized)
				{
					boltFsm.InitializeFSM();
				}

				AddActionAsFirst(tightState, () => OnTight(EventTime.Pre));
				AddActionAsLast(tightState, () => OnTight(EventTime.Post));

				AddActionAsFirst(unscrewPreState, () => OnUnscrew(EventTime.Pre));
				AddActionAsLast(unscrewPostState, () => OnUnscrew(EventTime.Post));
				maxTightness += 8;
			}
		}

		/// <summary>
		/// Setups the simple bolted state detection requiring just the "Bolted" state
		/// of the part to change state to true/false for events to trigger
		/// </summary>
		protected void SetupSimpleBoltedStateDetection()
		{
			AddActionAsFirst(boltCheckFsm.FindState("Bolts OFF"), () =>
			{
				alreadyCalledPreBolted = false;

				if (alreadyCalledPreUnbolted)
				{
					return;
				}
				GetEvents(EventTime.Pre, EventType.Uninstall).InvokeAll();
			});
			AddActionAsLast(boltCheckFsm.FindState("Bolts OFF"), () =>
			{
				alreadyCalledPostBolted = false;


				if (alreadyCalledPostUnbolted)
				{
					return;
				}
				GetEvents(EventTime.Post, EventType.Uninstall).InvokeAll();
			});


			AddActionAsFirst(boltCheckFsm.FindState("Bolts ON"), () =>
			{
				alreadyCalledPreUnbolted = false;

				if (alreadyCalledPreBolted)
				{
					return;
				}
				GetEvents(EventTime.Pre, EventType.Install).InvokeAll();
			});
			AddActionAsLast(boltCheckFsm.FindState("Bolts ON"), () =>
			{
				alreadyCalledPostUnbolted = false;


				if (alreadyCalledPostBolted)
				{
					return;
				}
				GetEvents(EventTime.Post, EventType.Install).InvokeAll();
			});
		}

		/// <summary>
		/// Initializes the event dictionary
		/// </summary>
		protected void InitEventStorage()
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

		/// <summary>
		/// Block installation of the part by disabling the trigger object
		/// </summary>
		public bool installBlocked { get => triggerFsmGameObject.activeSelf; set => triggerFsmGameObject.SetActive(!value); }

		/// <summary>
		/// The parts tightness (sum of all screw tightness (8 x screw count = all bolted))
		/// </summary>
		public FsmFloat tightness { get; protected set; }

		/// <summary>
		/// Returns the max tightness of the part
		/// (Sum of all screws found x 8)
		/// (only set when using advanced bolted state detection)
		/// 
		/// </summary>
		protected float maxTightness { get; set; }

		/// <summary>
		/// The main Fsm GameObject
		/// </summary>
		public GameObject mainFsmGameObject { get; protected set; }

		/// <summary>
		/// The data FSM object of the part
		/// </summary>
		public PlayMakerFSM dataFsm { get; protected set; }

		/// <summary>
		/// Returns if the part is bought
		/// (defaults to false)
		/// </summary>
		public FsmBool purchasedState { get; protected set; }

		/// <summary>
		/// Returns if the part is installed
		/// (defaults to false)
		/// </summary>
		public FsmBool installedState { get; protected set; }

		/// <summary>
		/// Returns if the part is damaged
		/// (defaults to false)
		/// </summary>
		public FsmBool damagedState { get; protected set; }

		/// <summary>
		/// Returns if the part is bought
		/// (defaults to false)
		/// </summary>
		public FsmBool boltedState { get; protected set; }

		/// <summary>
		/// The removal FSM dealing with removing an installed part
		/// </summary>
		public PlayMakerFSM removalFsm { get; protected set; }

		/// <summary>
		/// The assembly FSM dealing with installing a part
		/// </summary>
		public PlayMakerFSM assemblyFsm { get; protected set; }

		/// <summary>
		/// The bolt check FSM dealing with the bolted state of the part
		/// </summary>
		public PlayMakerFSM boltCheckFsm { get; protected set; }

		/// <summary>
		/// The part fsm gameObject (The one you can pickup)
		/// </summary>
		public GameObject partFsmGameObject { get; protected set; }

		/// <summary>
		/// The trigger fsm gameObject (deals with installing the part onto the car)
		/// </summary>
		public GameObject triggerFsmGameObject { get; protected set; }

		/// <inheritdoc />
		public override bool bought
		{
			get => purchasedState.Value;
			set => purchasedState.Value = value;
		}

		/// <inheritdoc />
		public override Vector3 position
		{
			get => partFsmGameObject.transform.position;
			set
			{
				if (!installed)
				{
					partFsmGameObject.transform.position = value;
				}
			}
		}

		/// <inheritdoc />
		public override Vector3 rotation
		{
			get => partFsmGameObject.transform.rotation.eulerAngles;
			set
			{
				if (!installed)
				{
					partFsmGameObject.transform.rotation = Quaternion.Euler(value);
				}
			}
		}

		/// <summary>
		/// Returns if the game part is installed
		/// </summary>
		public override bool installed => installedState.Value;

		/// <summary>
		/// Returns if the game part is bolted
		/// </summary>
		public override bool bolted
		{
			get
			{
				if (simpleBoltedStateDetection)
				{
					return boltedState.Value;
				}

				return boltedState.Value && tightness.Value >= maxTightness;
			}
		}

		/// <inheritdoc />
		public override bool active
		{
			get => partFsmGameObject.activeSelf;
			set => partFsmGameObject.SetActive(value);
		}

		/// <inheritdoc />
		public override string name => partFsmGameObject.name;

		/// <inheritdoc />
		public override bool isLookingAt => partFsmGameObject.IsLookingAt();

		/// <inheritdoc />
		public override bool isHolding => partFsmGameObject.IsHolding();

		/// <summary>
		/// Sends the REMOVE event to the Part
		/// </summary>
		public void Uninstall()
		{
			removalFsm.SendEvent("REMOVE");
		}

		public override void ResetToDefault(bool uninstall = false)
		{
			if (uninstall && installed)
			{
				Uninstall();
			}

			position = defaultPosition;
			rotation = defaultRotation;
		}

		/// <summary>
		/// Helper method for adding an Action to an fsmState as the first item in the actions list
		/// </summary>
		protected void AddActionAsFirst(FsmState fsmState, Action action)
		{
			if (fsmState == null)
			{
				return;
			}

			var actions = new List<FsmStateAction>(fsmState.Actions);
			actions.Insert(0, new FsmAction(action));
			fsmState.Actions = actions.ToArray();
		}

		/// <summary>
		/// Helper method for adding an Action to an fsmState as the last item in the actions list
		/// </summary>
		protected void AddActionAsLast(FsmState fsmState, Action action)
		{
			if (fsmState == null)
			{
				return;
			}

			var actions = new List<FsmStateAction>(fsmState.Actions) { new FsmAction(action) };
			fsmState.Actions = actions.ToArray();
		}

		/// <summary>
		/// Gets called by the advanced bolted state detection when Unscrewing a bolt
		/// </summary>
		/// <param name="eventTime"></param>
		protected void OnUnscrew(EventTime eventTime)
		{
			alreadyCalledPreBolted = false;
			alreadyCalledPostBolted = false;

			switch (eventTime)
			{
				case EventTime.Pre:
					if (alreadyCalledPreUnbolted)
					{
						return;
					}

					alreadyCalledPreUnbolted = true;
					GetEvents(EventTime.Pre, EventType.Unbolted).InvokeAll();
					break;
				case EventTime.Post:
					if (alreadyCalledPostUnbolted)
					{
						return;
					}

					alreadyCalledPostUnbolted = true;
					GetEvents(EventTime.Post, EventType.Unbolted).InvokeAll();
					break;
			}
		}

		/// <summary>
		/// Gets called by the advanced bolted state detection when a bolt reaches the state "8" (tight)
		/// </summary>
		/// <param name="eventTime"></param>
		protected void OnTight(EventTime eventTime)
		{
			if (tightness.Value < maxTightness)
			{
				return; //Wait for all screws to be tight
			}
			
			alreadyCalledPreUnbolted = false;
			alreadyCalledPostUnbolted = false;


			switch (eventTime)
			{
				case EventTime.Pre:
					if (alreadyCalledPreBolted)
					{
						return;
					}

					alreadyCalledPreBolted = true;
					GetEvents(EventTime.Pre, EventType.Bolted).InvokeAll();
					break;
				case EventTime.Post:
					if (alreadyCalledPostBolted)
					{
						return;
					}

					alreadyCalledPostBolted = true;
					GetEvents(EventTime.Post, EventType.Bolted).InvokeAll();
					break;
			}
		}

		/// <summary>
		/// Not implemented for the 
		/// </summary>
		/// <param name="eventTime"></param>
		/// <param name="eventType"></param>
		/// <returns></returns>
		public List<Action> GetEvents(EventTime eventTime, EventType eventType)
		{
			return events[eventTime][eventType];
		}

		/// <inheritdoc />
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
					case EventType.Bolted:
						if (bolted)
						{
							//ToDo: bolted state should only be true if maxTightness is also reached
							action.Invoke();
						}
						break;
					case EventType.Unbolted:
						if (!bolted)
						{
							//ToDo: bolted state should only be true if maxTightness is also reached
							action.Invoke();
						}
						break;
				}
			}
		}
	}
}