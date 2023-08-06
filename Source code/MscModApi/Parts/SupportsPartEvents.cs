using System;
using System.Collections.Generic;

namespace MscModApi.Parts
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

	public interface SupportsPartEvents
	{
		void AddEventListener(EventTime eventTime, EventType eventType, Action action);

		List<Action> GetEvents(EventTime eventTime, EventType eventType);
	}
}