using System;
using System.Collections.Generic;

namespace MscModApi.Parts.ReplacePart.EventSystem
{
	/// <summary>
	/// Classes implementing this interface support handling events on a part.
	/// </summary>
	public interface SupportsReplacedGamePartsEvent
	{
		/// <summary>
		/// Adds an action to the part that get's triggered on different events (eg when part is installed or bolted).
		/// If the ReplacedGameParts hasn't been initialized yet, events are wrapped into an ReplacedGamePartsEvent.Type.Initialized and added after the object was initialized.
		/// ReplacedGamePartsEvent.Type.Initialized events are always added even when not yet initialized (obviously)
		/// </summary>
		/// <param name="Type">The type of event to listen to</param>
		/// <param name="action">The action to execute when the event occurs</param>
		/// <param name="invokeActionIfConditionMet">When the condition for the Event type is already met when the event is added. The newly added event is immediately triggered</param>
		/// <returns>Returns a reference to the listener added</returns>
		ReplacedPartEventListener AddEventListener(ReplacedGamePartsEvent.Type Type, Action action,
			bool invokeActionIfConditionMet = true);

		/// <summary>
		/// Remove an event listener from the part (only when the ReplacedGameParts was initialized, before that no events are stored)
		/// (Excluding ReplacedGamePartsEvent.Type.Initialized which is available and can be removed)
		/// </summary>
		/// <param name="Type">The type of event the action was added to</param>
		/// <param name="action">The action to remove</param>
		/// <returns>Returns true if event was found and removed. Otherwise false</returns>
		bool RemoveEventListener(ReplacedPartEventListener partEventListener);

		/// <summary>
		/// Returns all Events added (May differ depending on if the ReplacedGameParts has been initialized or not)
		/// </summary>
		/// <param name="Type">The type of event</param>
		/// <returns>A list object of all actions (in order of added)</returns>
		List<Action> GetEvents(ReplacedGamePartsEvent.Type Type);
	}
}