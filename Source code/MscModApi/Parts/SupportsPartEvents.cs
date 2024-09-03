using System;
using System.Collections.Generic;
using static MscModApi.Parts.PartEvent;

namespace MscModApi.Parts
{
	/// <summary>
	/// Classes implementing this interface support handling events on a part.
	/// </summary>
	public interface SupportsPartEvents
	{
		/// <summary>
		/// Adds an action to the part that get's triggered on different events (eg when part is installed or bolted).
		/// </summary>
		/// <param name="eventTime">When the event occurs</param>
		/// <param name="Type">The type of event to listen to</param>
		/// <param name="action">The action to execute when the event occurs</param>
		/// <param name="invokeActionIfConditionMet">When the condition for the Event type is already met when the event is added. The newly added event is immediately triggered</param>
		/// <returns>Returns the action added (eg for later removal)</returns>
		PartEventListener AddEventListener(PartEvent.Time eventTime, PartEvent.Type Type, Action action,
			bool invokeActionIfConditionMet = true);

		/// <summary>
		/// Remove an action from the event system
		/// </summary>
		/// <param name="partEventListener">The reference of the part event listener</param>
		/// <returns>Returns true if event was found and removed. Otherwise false</returns>
		bool RemoveEventListener(PartEventListener partEventListener);

		/// <summary>
		/// Returns a collection containing all the actions
		/// </summary>
		/// <param name="eventTime">When the event occurs</param>
		/// <param name="Type">The type of event</param>
		/// <returns>A PartEventListener collection (in order of added)</returns>
		PartEventListenerCollection GetEventListeners(PartEvent.Time eventTime, PartEvent.Type Type);
	}
}