using MSCLoader;
using MscModApi.Tools;
using MscModApi.Trigger;
using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
namespace MscModApi.Parts
{
	public abstract class EventSupportingBasicPart : BasicPart
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

		/// <summary>
		/// Stores all events that a developer may have added to this part object
		/// </summary>
		protected Dictionary<EventTime, Dictionary<EventType, List<Action>>> events =
			new Dictionary<EventTime, Dictionary<EventType, List<Action>>>();

		public EventSupportingBasicPart()
		{
			InitEventStorage();
		}

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
						if (bolted)
						{
							action.Invoke();
						}
						break;
					case EventType.Unfixed:
						if (!bolted)
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
	}
}