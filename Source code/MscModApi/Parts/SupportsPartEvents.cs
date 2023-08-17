using System;
using System.Collections.Generic;
using static MscModApi.Parts.PartEvent;

namespace MscModApi.Parts
{
	public interface SupportsPartEvents
	{
		void AddEventListener(PartEvent.EventTime eventTime, PartEvent.EventType eventType, Action action);

		List<Action> GetEvents(PartEvent.EventTime eventTime, PartEvent.EventType eventType);

	}
}