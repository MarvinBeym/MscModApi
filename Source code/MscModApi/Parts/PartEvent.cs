using System;

namespace MscModApi.Parts
{
	public static class PartEvent
	{
		public enum Time
		{
			Pre,
			Post
		}

		public enum Type
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
		/// <param name="Type">The event you want to get the opposite of</param>
		/// <returns>The opposite event</returns>
		/// <exception cref="Exception">When passed event is not supported by this method</exception>
		public static PartEvent.Type GetOppositeEvent(PartEvent.Type Type)
		{
			switch (Type)
			{
				case PartEvent.Type.Install:
					return PartEvent.Type.Uninstall;
				case PartEvent.Type.Uninstall:
					return PartEvent.Type.Install;
				case PartEvent.Type.Bolted:
					return PartEvent.Type.Unbolted;
				case PartEvent.Type.Unbolted:
					return PartEvent.Type.Bolted;
				case PartEvent.Type.InstallOnCar:
					return PartEvent.Type.UninstallFromCar;
				case PartEvent.Type.UninstallFromCar:
					return PartEvent.Type.InstallOnCar;
				case PartEvent.Type.BoltedOnCar:
					return PartEvent.Type.UnboltedOnCar;
				case PartEvent.Type.UnboltedOnCar:
					return PartEvent.Type.BoltedOnCar;
			}

			throw new Exception($"Unsupported PartEvent.Type '{Type}' used");
		}
	}
}