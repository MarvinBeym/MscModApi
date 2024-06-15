﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MscModApi.Tools.Extensions;

namespace MscModApi.Parts.ReplacePart
{
	/// <summary>
	/// Allows replacing a number of original parts with own custom "Mod" parts.
	/// WARNING: Highly experimental implementation, crashes, loading/saving errors possible. Use at your own risk!!
	///
	/// Replacement for the previous (also Highly experimental) "ReplacementPart" class
	/// </summary>
	public class ReplacedGameParts: SupportsReplacedGamePartsEvent
	{
		/// <summary>
		/// Storage for the event actions
		/// </summary>
		protected Dictionary<ReplacedGamePartsEvent.Type, List<Action>> events = new Dictionary<ReplacedGamePartsEvent.Type, List<Action>>();

		/// <summary>
		/// List of all the original parts
		/// </summary>
		protected List<GamePart> originalParts { get; }

		/// <summary>
		/// List of all the new parts, all required to be installed to replace all the originalParts
		/// </summary>
		protected List<Part> newParts { get; }

		/// <summary>
		/// List of all the new parts, that are required to be installed for originals to be replaced
		/// but don't block original parts from being on the car
		/// </summary>
		protected List<Part> requiredNonReplacingParts { get; }

		/// <summary>
		/// Stores a reference to the PartEventListeners added to the individual parts,
		/// used for being able to remove a part again and remove any event added to it for this functionality.
		/// </summary>
		protected Dictionary<SupportsPartEvents, List<PartEventListener>> partEventListenerReferences = new Dictionary<SupportsPartEvents, List<PartEventListener>>(); 

		/// <summary>
		/// Replaces a number of original game parts with new custom parts
		/// All *newParts* have to be installed in order to replace the originalParts and make them "fake" installed, even though the physical object is not on the car
		/// </summary>
		/// <param name="originalParts">A list of original parts that get replaced</param>
		/// <param name="newParts">A list of new parts that replace the original parts</param>
		/// <param name="requiredNonReplacingParts">A list of new parts that are required for the replacement functionality but don't block original parts from being installed</param>
		public ReplacedGameParts(IEnumerable<GamePart> originalParts, IEnumerable<Part> newParts, IEnumerable<Part> requiredNonReplacingParts)
		{
			InitEventStorage();
			this.originalParts = originalParts.ToList();
			this.newParts = newParts.ToList();
			this.requiredNonReplacingParts = requiredNonReplacingParts.ToList();


			foreach (var originalPart in this.originalParts)
			{
				SetupOriginalPart(originalPart);
			}

			foreach (var newPart in this.newParts)
			{
				SetupNewPart(newPart);
			}

			foreach (var requiredNonReplacingPart in this.requiredNonReplacingParts)
			{
				SetupRequiredNonReplacingPart(requiredNonReplacingPart);
			}

			SetReplacedState(replaced);
		}

		/// <summary>
		/// Add a new part after creation of the ReplacedGamePart
		/// Note that it is undetermined how this affects the current "state" of the replacement
		/// </summary>
		/// <param name="newPart">The part to add</param>
		/// <param name="requiredNonReplacing">Is the part required but not blocking original parts</param>
		/// <returns></returns>
		public bool AddNewPart(Part newPart, bool requiredNonReplacing)
		{
			if (requiredNonReplacing)
			{
				if (requiredNonReplacingParts.Contains(newPart))
				{
					return false;
				}

				SetupRequiredNonReplacingPart(newPart);
				requiredNonReplacingParts.Add(newPart);
			}
			else
			{
				if (newParts.Contains(newPart))
				{
					return false;
				}

				SetupNewPart(newPart);
				newParts.Add(newPart);
			}

			SetReplacedState(replaced);

			return true;
		}


		/// <summary>
		/// Remove a new part after creation of the ReplacedGamePart
		/// Note that it is undetermined how this affects the current "state" of the replacement
		/// </summary>
		/// <param name="newPart">The part to remove</param>
		/// <param name="requiredNonReplacing">Is the part required but not blocking original parts</param>
		/// <returns></returns>
		public bool RemoveNewPart(Part newPart, bool requiredNonReplacing = false)
		{
			if (requiredNonReplacing)
			{
				if (!requiredNonReplacingParts.Contains(newPart))
				{
					return false;
				}

				
				requiredNonReplacingParts.Remove(newPart);
			}
			else
			{
				if (!newParts.Contains(newPart))
				{
					return false;
				}

				newParts.Remove(newPart);
			}

			ClearEventListenersFromPart(newPart);

			return true;
		}

		/// <summary>
		/// Setups an original part
		/// (Adding listeners and such)
		/// </summary>
		/// <param name="originalPart">The part to setup</param>
		protected void SetupOriginalPart(GamePart originalPart)
		{
			var partEventListener = originalPart.AddEventListener(PartEvent.Time.Post, PartEvent.Type.Install, delegate
			{
				foreach (var newPart in newParts)
				{
					newPart.installBlocked = true;
				}
			});
			StoreEventListenerReference(originalPart, partEventListener);


			partEventListener = originalPart.AddEventListener(PartEvent.Time.Post, PartEvent.Type.Uninstall, delegate
			{
				if (originalParts.AllHaveState(PartEvent.Type.Install))
				{
					return;
				}
				foreach (var newPart in newParts)
				{
					newPart.installBlocked = false;
				}
			});
			StoreEventListenerReference(originalPart, partEventListener);
		}

		/// <summary>
		/// Setup a new part
		/// (Adding listeners and such)
		/// </summary>
		/// <param name="newPart">The part to setup</param>
		protected void SetupNewPart(Part newPart)
		{
			var partEventListener = newPart.AddEventListener(PartEvent.Time.Post, PartEvent.Type.Install, delegate
				{
					if (newParts.AllHaveState(PartEvent.Type.Install))
					{
						GetEvents(ReplacedGamePartsEvent.Type.AllNewInstalled).InvokeAll();
					}
					foreach (var originalPart in originalParts)
					{
						originalPart.installBlocked = true;
					}
				});
			StoreEventListenerReference(newPart, partEventListener);

			partEventListener = newPart.AddEventListener(PartEvent.Time.Post, PartEvent.Type.Uninstall, delegate
				{
					GetEvents(ReplacedGamePartsEvent.Type.AnyNewUninstalled).InvokeAll();
					if (newParts.AllHaveState(PartEvent.Type.Install))
					{
						return;
					}
					foreach (var originalPart in originalParts)
					{
						originalPart.installBlocked = false;
					}
				});
			StoreEventListenerReference(newPart, partEventListener);


			partEventListener = newPart.AddEventListener(PartEvent.Time.Post, PartEvent.Type.Bolted, delegate
			{
				if (!replaced)
				{
					return;
				}
				GetEvents(ReplacedGamePartsEvent.Type.AllNewBolted).InvokeAll();
				SetReplacedState(true);
			});
			StoreEventListenerReference(newPart, partEventListener);


			partEventListener = newPart.AddEventListener(PartEvent.Time.Post, PartEvent.Type.Unbolted, delegate
				{
					GetEvents(ReplacedGamePartsEvent.Type.AnyNewUnbolted).InvokeAll();
					if (!originalParts.AllHaveState(PartEvent.Type.InstallOnCar))
					{
						//Forcing because already checked
						SetReplacedState(false, true);
					}
					else
					{
						foreach (var part in newParts)
						{
							part.installBlocked = true;
						}
					}
				});
			StoreEventListenerReference(newPart, partEventListener);
		}


		/// <summary>
		/// Setups a new part that is required for the replacement but does not block original parts from being installed
		/// (Adding listeners and such)
		/// </summary>
		/// <param name="requiredNonReplacingPart">The part to setup</param>
		protected void SetupRequiredNonReplacingPart(Part requiredNonReplacingPart)
		{
			var partEventListener = requiredNonReplacingPart.AddEventListener(PartEvent.Time.Post, PartEvent.Type.Unbolted, delegate
			{
				GetEvents(ReplacedGamePartsEvent.Type.AnyNewUnbolted).InvokeAll();
				if (!originalParts.AllHaveState(PartEvent.Type.InstallOnCar))
				{
					//Forcing because already checked
					SetReplacedState(false, true);
				}
				else
				{
					foreach (var part in newParts)
					{
						part.installBlocked = true;
					}
				}
			});
			StoreEventListenerReference(requiredNonReplacingPart, partEventListener);

			partEventListener = requiredNonReplacingPart.AddEventListener(PartEvent.Time.Post, PartEvent.Type.Bolted, delegate
			{
				if (!replaced) {return;}
				GetEvents(ReplacedGamePartsEvent.Type.AllNewBolted).InvokeAll();
				SetReplacedState(true);
			});
			StoreEventListenerReference(requiredNonReplacingPart, partEventListener);
		}

		/// <summary>
		/// Is the replacedGamePart considered "replaced"
		/// (Are no originalParts installed, are all newParts & requiredNonReplacingParts installed)
		/// </summary>
		public bool replaced
		{
			get
			{
				return originalParts.All(part => !part.installedOnCar) 
				       && newParts.All(part => part.bolted && part.installedOnCar) 
				       && requiredNonReplacingParts.All(part => part.bolted && part.installedOnCar);
			}
		}

		/// <summary>
		/// Overwrite the automatically determined replaced state
		/// Useful if other parts or settings should be considered too but can't be added as a part
		/// </summary>
		/// <param name="state">true = fake install original parts to make game think they are installed properly</param>
		/// <param name="force">If set to true, will even set the replaced state if the originalPart is installed on car (root parent is satsuma) (DANGER)</param>
		public void SetReplacedState(bool state, bool force = false)
		{
			if (force || !originalParts.AllHaveState(PartEvent.Type.InstallOnCar))
			{
				foreach (var originalPart in originalParts)
				{
					originalPart.installedState.Value = state;
					originalPart.boltedState.Value = state;
					originalPart.tightness.Value = state ? originalPart.maxTightness : 0;
				}
			}
		}

		/// <inheritdoc />
		public ReplacedPartEventListener AddEventListener(ReplacedGamePartsEvent.Type type, Action action, bool invokeActionIfConditionMet = true)
		{
			events[type].Add(action);

			if (invokeActionIfConditionMet)
			{
				switch (type)
				{
					//ToDo: check if invoking just the newly added action is enough of if all have to be invoked
					case ReplacedGamePartsEvent.Type.AllNewInstalled:
						if (newParts.AllHaveState(PartEvent.Type.Install) && requiredNonReplacingParts.AllHaveState(PartEvent.Type.Install))
						{
							action.Invoke();
						}
						break;
					case ReplacedGamePartsEvent.Type.AnyNewUninstalled:
						if (newParts.AnyHaveState(PartEvent.Type.Uninstall) || requiredNonReplacingParts.AnyHaveState(PartEvent.Type.Uninstall))
						{
							action.Invoke();
						}

						break;
					case ReplacedGamePartsEvent.Type.AllNewBolted:
						if (newParts.AllHaveState(PartEvent.Type.Bolted) && requiredNonReplacingParts.AllHaveState(PartEvent.Type.Bolted))
						{
							action.Invoke();
						}

						break;
					case ReplacedGamePartsEvent.Type.AnyNewUnbolted:
						if (newParts.AnyHaveState(PartEvent.Type.Unbolted) || requiredNonReplacingParts.AnyHaveState(PartEvent.Type.Unbolted))
						{
							action.Invoke();
						}

						break;
				}
			}

			return new ReplacedPartEventListener(type, action);
		}

		/// <inheritdoc />
		public bool RemoveEventListener(ReplacedPartEventListener partEventListener)
		{
			var actions = GetEvents(partEventListener.type);
			return actions.Contains(partEventListener.action) && actions.Remove(partEventListener.action);
		}

		/// <inheritdoc />
		public List<Action> GetEvents(ReplacedGamePartsEvent.Type type)
		{
			return events[type];
		}

		/// <summary>
		/// Initializes the event storage dictionary
		/// </summary>
		protected void InitEventStorage()
		{
			foreach (ReplacedGamePartsEvent.Type type in Enum.GetValues(typeof(ReplacedGamePartsEvent.Type)))
			{
				events.Add(type, new List<Action>());
			}
		}

		/// <summary>
		/// Stores the event listener reference for later access
		/// </summary>
		/// <param name="part">The part for which the reference should be stored</param>
		/// <param name="partEventListener">The event listener to store for the part</param>
		protected void StoreEventListenerReference(SupportsPartEvents part, PartEventListener partEventListener)
		{
			List<PartEventListener> partEventListeners;
			if (!partEventListenerReferences.ContainsKey(part))
			{
				partEventListeners = new List<PartEventListener>();

				partEventListenerReferences.Add(part, partEventListeners);
			}
			else
			{
				partEventListeners = partEventListenerReferences[part];
			}

			partEventListeners.Add(partEventListener);
		}

		/// <summary>
		/// Clears all event listeners added by the ReplaceGamePart class from the part
		/// </summary>
		/// <param name="part">The part to remove the event listeners from</param>
		protected void ClearEventListenersFromPart(SupportsPartEvents part)
		{
			var partEventListeners = partEventListenerReferences.TryGetValue(part, out var references) ? references : new List<PartEventListener>();
			foreach (var partEventListener in partEventListeners)
			{
				part.RemoveEventListener(partEventListener);
			}
			partEventListeners.Clear();
		}
	}
}