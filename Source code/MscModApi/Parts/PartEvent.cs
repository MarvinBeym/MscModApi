using System;

namespace MscModApi.Parts
{
	public static class PartEvent
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

		/// <summary>
		/// Returns the opposite event to the one passed
		/// </summary>
		/// <param name="eventType">The event you want to get the opposite of</param>
		/// <returns>The opposite event</returns>
		/// <exception cref="Exception">When passed event is not supported by this method</exception>
		public static PartEvent.EventType GetOppositeEvent(PartEvent.EventType eventType)
		{
			switch (eventType)
			{
				case PartEvent.EventType.Install:
					return PartEvent.EventType.Uninstall;
				case PartEvent.EventType.Uninstall:
					return PartEvent.EventType.Install;
				case PartEvent.EventType.Bolted:
					return PartEvent.EventType.Unbolted;
				case PartEvent.EventType.Unbolted:
					return PartEvent.EventType.Bolted;
				case PartEvent.EventType.InstallOnCar:
					return PartEvent.EventType.UninstallFromCar;
				case PartEvent.EventType.UninstallFromCar:
					return PartEvent.EventType.InstallOnCar;
				case PartEvent.EventType.BoltedOnCar:
					return PartEvent.EventType.UnboltedOnCar;
				case PartEvent.EventType.UnboltedOnCar:
					return PartEvent.EventType.BoltedOnCar;
			}

			throw new Exception($"Unsupported PartEvent.EventType '{eventType}' used");
		}
	}
}