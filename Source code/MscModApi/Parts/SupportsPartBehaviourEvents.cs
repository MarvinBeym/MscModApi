using System;
using System.Collections.Generic;
using UnityEngine;

namespace MscModApi.Parts
{
	/// <summary>
	/// Classes implementing this interface can add Unity Behaviors that will be enabled/disabled when the used PartEvent.EventType gets triggered
	/// </summary>
	public interface SupportsPartBehaviourEvents
	{
		/// <summary>
		/// Add a behaviour to a GameObject, enabled state controlled by event type triggering
		/// </summary>
		/// <typeparam name="T">The Class of the behaviour. Ex.: MonoBehaviour</typeparam>
		/// <param name="eventType">The 'main' event type. The 'main' type will enable the behaviour when triggered, the counterpart will disable it.</param>
		/// <returns></returns>
		T AddEventBehaviour<T>(PartEvent.EventType eventType) where T : Behaviour;
	}
}