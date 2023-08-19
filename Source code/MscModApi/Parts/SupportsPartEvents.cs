using System;
using System.Collections.Generic;
using static MscModApi.Parts.PartEvent;

namespace MscModApi.Parts
{
	public interface SupportsPartEvents
	{
		void AddEventListener(PartEvent.Time eventTime, PartEvent.Type Type, Action action, bool invokeActionIfConditionMet = true);

		List<Action> GetEvents(PartEvent.Time eventTime, PartEvent.Type Type);

	}
}