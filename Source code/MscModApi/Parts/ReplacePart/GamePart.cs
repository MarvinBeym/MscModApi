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
		protected Dictionary<PartEvent.Time, Dictionary<PartEvent.Type, List<Action>>> events =
			new Dictionary<PartEvent.Time, Dictionary<PartEvent.Type, List<Action>>>();

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
			if (!mainFsmGameObject) {
				throw new Exception($"Unable to find main fsm part GameObject using '{mainFsmPartName}'");
			}

			dataFsm = mainFsmGameObject.FindFsm("Data");
			if (!dataFsm) {
				throw new Exception($"Unable to find data fsm on GameObject with name '{mainFsmGameObject.name}'");
			}

			triggerFsmGameObject = dataFsm.FsmVariables.FindFsmGameObject("Trigger").Value;
			if (!triggerFsmGameObject) {
				throw new Exception(
					$"Unable to find trigger GameObject on GameObject with name '{mainFsmGameObject.name}'");
			}

			gameObject = dataFsm.FsmVariables.FindFsmGameObject("ThisPart").Value;
			if (!gameObject) {
				throw new Exception(
					$"Unable to find part GameObject on GameObject with name '{mainFsmGameObject.name}'");
			}

			boltedState = dataFsm.FsmVariables.FindFsmBool("Bolted");
			damagedState = dataFsm.FsmVariables.FindFsmBool("Damaged") ?? new FsmBool("Damaged");
			installedState = dataFsm.FsmVariables.FindFsmBool("Installed") ?? new FsmBool("Installed");
			purchasedState = dataFsm.FsmVariables.FindFsmBool("Purchased") ?? new FsmBool("Purchased");

			assemblyFsm = triggerFsmGameObject.FindFsm("Assembly");
			removalFsm = gameObject.FindFsm("Removal");
			boltCheckFsm = gameObject.FindFsm("BoltCheck");

			if (!assemblyFsm.Fsm.Initialized) {
				assemblyFsm.InitializeFSM();
			}

			if (!removalFsm.Fsm.Initialized) {
				removalFsm.InitializeFSM();
			}

			if (!boltCheckFsm.Fsm.Initialized) {
				boltCheckFsm.InitializeFSM();
			}

			tightness = boltCheckFsm.FsmVariables.FindFsmFloat("Tightness");

			AddActionAsFirst(assemblyFsm.FindState("Assemble"),
				() => { GetEvents(PartEvent.Time.Pre, PartEvent.Type.Install).InvokeAll(); });

			AddActionAsLast(assemblyFsm.FindState("End"), () =>
				{
					GetEvents(PartEvent.Time.Post, PartEvent.Type.Install).InvokeAll();
					if (installedOnCar)
					{
						GetEvents(PartEvent.Time.Post, PartEvent.Type.InstallOnCar).InvokeAll();
					}
				});

			AddActionAsFirst(removalFsm.FindState("Remove part"),
				() => { GetEvents(PartEvent.Time.Pre, PartEvent.Type.Uninstall).InvokeAll(); });
			AddActionAsLast(removalFsm.FindState("Remove part"), () =>
				{
					GetEvents(PartEvent.Time.Post, PartEvent.Type.Uninstall).InvokeAll();
					if (!installedOnCar)
					{
						//Check probably not needed, likely already not on car because part can't be connected to something else after being uninstalled
						GetEvents(PartEvent.Time.Post, PartEvent.Type.UninstallFromCar).InvokeAll();
					}
				});

			if (tightness == null) {
				throw new Exception($"Unable to find tightness on bolt check fsm of part '{gameObject.name}'");
			}

			if (boltedState != null) {
				if (simpleBoltedStateDetection) {
					SetupSimpleBoltedStateDetection();
				}
				else {
					SetupAdvancedBoltedStateDetection();
				}
			}
			else {
				boltedState = new FsmBool(); //Avoiding null
			}
		}

		/// <summary>
		/// Usable when wanting to extend from GamePart and implement everything yourself.
		/// An Example for the usability of this is the Class "SatsumaGamePart" which is a wrapper to make the Satsuma
		/// (which has no part logic from the game) to allow using as a parent for "Part" objects
		/// </summary>
		protected GamePart()
		{

		}

		/// <summary>
		/// Setups the advanced bolted state detection requiring all bolts of the part to be tight before bolted events get called
		/// </summary>
		protected void SetupAdvancedBoltedStateDetection()
		{
			GameObject boltsGameObject = gameObject.FindChild("Bolts");
			if (!boltsGameObject) {
				ModConsole.Print(
					$"GamePart: Unable to find 'Bolts' child of '{gameObject.name}'. Bolted event listening not possible");
			}

			for (int i = 0; i < boltsGameObject.transform.childCount; i++) {
				GameObject boltGameObject;
				try {
					boltGameObject = boltsGameObject.transform.GetChild(i).gameObject;
					if (!boltGameObject) {
						throw new Exception("Null GameObject");
					}
				}
				catch (Exception) {
					continue;
				}

				PlayMakerFSM boltFsm = boltGameObject.FindFsm("Screw");
				if (!boltFsm) {
					continue;
				}


				FsmState tightState = boltFsm.FindState("8 2");
				FsmState unscrewPreState = boltFsm.FindState("Unscrew 2");
				FsmState unscrewPostState = boltFsm.FindState("Wait 4");
				if (tightState == null || unscrewPreState == null || unscrewPostState == null) {
					return;
				}

				if (!boltFsm.Fsm.Initialized) {
					boltFsm.InitializeFSM();
				}

				AddActionAsFirst(tightState, () => OnTight(PartEvent.Time.Pre));
				AddActionAsLast(tightState, () => OnTight(PartEvent.Time.Post));

				AddActionAsFirst(unscrewPreState, () => OnUnscrew(PartEvent.Time.Pre));
				AddActionAsLast(unscrewPostState, () => OnUnscrew(PartEvent.Time.Post));
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

				if (alreadyCalledPreUnbolted) {
					return;
				}

				GetEvents(PartEvent.Time.Pre, PartEvent.Type.Unbolted).InvokeAll();
				if (installedOnCar)
				{
					GetEvents(PartEvent.Time.Pre, PartEvent.Type.UnboltedOnCar).InvokeAll();
				}
			});
			AddActionAsLast(boltCheckFsm.FindState("Bolts OFF"), () =>
			{
				alreadyCalledPostBolted = false;


				if (alreadyCalledPostUnbolted) {
					return;
				}

				GetEvents(PartEvent.Time.Post, PartEvent.Type.Unbolted).InvokeAll();
				if (installedOnCar)
				{
					GetEvents(PartEvent.Time.Post, PartEvent.Type.UnboltedOnCar).InvokeAll();
				}
			});


			AddActionAsFirst(boltCheckFsm.FindState("Bolts ON"), () =>
			{
				alreadyCalledPreUnbolted = false;

				if (alreadyCalledPreBolted) {
					return;
				}

				GetEvents(PartEvent.Time.Pre, PartEvent.Type.Bolted).InvokeAll();
				if (installedOnCar)
				{
					GetEvents(PartEvent.Time.Pre, PartEvent.Type.BoltedOnCar).InvokeAll();
				}
			});
			AddActionAsLast(boltCheckFsm.FindState("Bolts ON"), () =>
			{
				alreadyCalledPostUnbolted = false;


				if (alreadyCalledPostBolted) {
					return;
				}

				GetEvents(PartEvent.Time.Post, PartEvent.Type.Bolted).InvokeAll();
				if (installedOnCar)
				{
					GetEvents(PartEvent.Time.Post, PartEvent.Type.BoltedOnCar).InvokeAll();
				}
			});
		}

		/// <summary>
		/// Initializes the event dictionary
		/// </summary>
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

		/// <summary>
		/// Block installation of the part by disabling the trigger object
		/// </summary>
		public override bool installBlocked
		{
			get => triggerFsmGameObject.activeSelf;
			set => triggerFsmGameObject.SetActive(!value);
		}

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
		public override GameObject gameObject { get; protected set; }

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
				if (simpleBoltedStateDetection) {
					return boltedState.Value;
				}

				return boltedState.Value && tightness.Value >= maxTightness;
			}
		}

		/// <inheritdoc />
		public override bool installedOnCar => gameObject.transform.root == CarH.satsuma.transform;

		/// <inheritdoc />
		public override bool active
		{
			get => gameObject.activeSelf;
			set => gameObject.SetActive(value);
		}

		/// <inheritdoc />
		public override string name => gameObject.name;

		/// <inheritdoc />
		public override bool isLookingAt => gameObject.IsLookingAt();

		/// <inheritdoc />
		public override bool isHolding => gameObject.IsHolding();

		/// <summary>
		/// Sends the REMOVE event to the Part
		/// </summary>
		public override void Uninstall()
		{
			removalFsm.SendEvent("REMOVE");
		}

		public override void ResetToDefault(bool uninstall = false)
		{
			if (uninstall && installed) {
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
			if (fsmState == null) {
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
			if (fsmState == null) {
				return;
			}

			var actions = new List<FsmStateAction>(fsmState.Actions) { new FsmAction(action) };
			fsmState.Actions = actions.ToArray();
		}

		/// <summary>
		/// Gets called by the advanced bolted state detection when Unscrewing a bolt
		/// </summary>
		/// <param name="eventTime"></param>
		protected void OnUnscrew(PartEvent.Time eventTime)
		{
			alreadyCalledPreBolted = false;
			alreadyCalledPostBolted = false;

			switch (eventTime) {
				case PartEvent.Time.Pre:
					if (alreadyCalledPreUnbolted) {
						return;
					}

					alreadyCalledPreUnbolted = true;
					GetEvents(PartEvent.Time.Pre, PartEvent.Type.Unbolted).InvokeAll();
					if (installedOnCar)
					{
						GetEvents(PartEvent.Time.Pre, PartEvent.Type.UnboltedOnCar).InvokeAll();
					}
					break;
				case PartEvent.Time.Post:
					if (alreadyCalledPostUnbolted) {
						return;
					}

					alreadyCalledPostUnbolted = true;
					GetEvents(PartEvent.Time.Post, PartEvent.Type.Unbolted).InvokeAll();
					if (installedOnCar)
					{
						GetEvents(PartEvent.Time.Post, PartEvent.Type.UnboltedOnCar).InvokeAll();
					}
					break;
			}
		}

		/// <summary>
		/// Gets called by the advanced bolted state detection when a bolt reaches the state "8" (tight)
		/// </summary>
		/// <param name="eventTime"></param>
		protected void OnTight(PartEvent.Time eventTime)
		{
			if (tightness.Value < maxTightness) {
				return; //Wait for all screws to be tight
			}

			alreadyCalledPreUnbolted = false;
			alreadyCalledPostUnbolted = false;


			switch (eventTime) {
				case PartEvent.Time.Pre:
					if (alreadyCalledPreBolted) {
						return;
					}

					alreadyCalledPreBolted = true;
					GetEvents(PartEvent.Time.Pre, PartEvent.Type.Bolted).InvokeAll();
					if (installedOnCar)
					{
						GetEvents(PartEvent.Time.Pre, PartEvent.Type.BoltedOnCar).InvokeAll();
					}
					break;
				case PartEvent.Time.Post:
					if (alreadyCalledPostBolted) {
						return;
					}

					alreadyCalledPostBolted = true;
					GetEvents(PartEvent.Time.Post, PartEvent.Type.Bolted).InvokeAll();
					if (installedOnCar)
					{
						GetEvents(PartEvent.Time.Post, PartEvent.Type.BoltedOnCar).InvokeAll();
					}
					break;
			}
		}

		/// <summary>
		/// Not implemented for the 
		/// </summary>
		/// <param name="eventTime"></param>
		/// <param name="Type"></param>
		/// <returns></returns>
		public List<Action> GetEvents(PartEvent.Time eventTime, PartEvent.Type Type)
		{
			return events[eventTime][Type];
		}

		/// <inheritdoc />
		public void AddEventListener(PartEvent.Time eventTime, PartEvent.Type Type, Action action, bool invokeActionIfConditionMet = true)
		{
			if (
				eventTime == PartEvent.Time.Pre 
				&& (Type == PartEvent.Type.InstallOnCar || Type == PartEvent.Type.UninstallFromCar)
			) {
				throw new Exception($"Event {Type} can't be detected at '{eventTime}'. Unsupported!");
			}

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
							//ToDo: bolted state should only be true if maxTightness is also reached
							action.Invoke();
						}

						break;
					case PartEvent.Type.Unbolted:
						if (!bolted) {
							//ToDo: bolted state should only be true if maxTightness is also reached
							action.Invoke();
						}
						break;
					case PartEvent.Type.InstallOnCar:
						if (installedOnCar)
						{
							action.Invoke();
						}
						break;
					case PartEvent.Type.UninstallFromCar:
						if (!installedOnCar)
						{
							action.Invoke();
						}
						break;
					case PartEvent.Type.BoltedOnCar:
						if (bolted && installedOnCar)
						{
							action.Invoke();
						}
						break;
					case PartEvent.Type.UnboltedOnCar:
						if (!bolted && installedOnCar)
						{
							action.Invoke();
						}
						break;
				}
			}
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
				foreach (BasicPart partToBlock in partsToBlock)
				{
					partToBlock.installBlocked = true;
				}
			});
			AddEventListener(PartEvent.Time.Post, PartEvent.GetOppositeEvent(Type), () =>
			{
				foreach (BasicPart partToBlock in partsToBlock)
				{
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
			AddEventListener(PartEvent.Time.Post, PartEvent.GetOppositeEvent(Type), () => { partToBlock.installBlocked = false; });
		}
	}
}