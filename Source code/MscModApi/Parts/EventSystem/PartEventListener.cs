using System;

namespace MscModApi.Parts.EventSystem
{
	public class PartEventListener
	{
		/// <summary>
		/// When this event triggers
		/// </summary>
		public PartEvent.Time eventTime { get; protected set; }

		/// <summary>
		/// The type of event
		/// </summary>
		public PartEvent.Type type { get; protected set; }


		/// <summary>
		/// The action executed when the event triggers
		/// </summary>
		public Action action { get; protected set; }

		/// <summary>
		/// Marks the PartEventListener to be deleted from the EventListener list
		/// (When being deleted/removed while in an Event)
		/// </summary>
		public bool delete { internal set; get; } = false;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventTime">When this event triggers</param>
		/// <param name="type">The type of event</param>
		/// <param name="action">The action executed when the event triggers</param>
		public PartEventListener(PartEvent.Time eventTime, PartEvent.Type type, Action action)
		{
			this.eventTime = eventTime;
			this.type = type;
			this.action = action;
		}
	}
}