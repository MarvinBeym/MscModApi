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
		/// <summary>
		/// Called when part saves
		/// </summary>
		Save,
		/// <summary>
		/// Called when part is installed on parent
		/// </summary>
		Install,
		/// <summary>
		/// Called when part is uninstalled from parent
		/// </summary>
		Uninstall,
		/// <summary>
		/// Called when part is bolted while on parent 
		/// </summary>
		Bolted,
		/// <summary>
		/// Called when part is unbolted while on parent
		/// </summary>
		Unbolted,
		/// <summary>
		/// Called when part is installed on car
		/// (even if part is installed on on parent and parent get's installed on car)
		/// </summary>
		InstallOnCar,
		/// <summary>
		/// Called when part is uninstalled from car
		/// (even if part is installed on on parent and parent get's uninstalled from car)
		/// </summary>
		UninstallFromCar,
		/// <summary>
		/// Called when part is bolted while on car
		/// </summary>
		BoltedOnCar,
		/// <summary>
		/// Called when part is unbolted while on car
		/// </summary>
		UnboltedOnCar
	}

	public interface SupportsPartEvents
	{
		void AddEventListener(EventTime eventTime, EventType eventType, Action action);

		List<Action> GetEvents(EventTime eventTime, EventType eventType);
	}
}