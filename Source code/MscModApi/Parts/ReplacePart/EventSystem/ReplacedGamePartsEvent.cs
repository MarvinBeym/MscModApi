namespace MscModApi.Parts.ReplacePart.EventSystem
{
	public static class ReplacedGamePartsEvent
	{
		/// <summary>
		/// All supported events on a ReplacedGamePart object
		/// </summary>
		public enum Type
		{
			/// <summary>
			/// All new parts installed
			/// </summary>
			AllNewInstalled,

			/// <summary>
			/// All new parts bolted
			/// </summary>
			AllNewBolted,

			/// <summary>
			/// Any new part uninstalled
			/// </summary>
			AnyNewUninstalled,

			/// <summary>
			/// Any new part unbolted
			/// </summary>
			AnyNewUnbolted,

			/// <summary>
			/// Triggered when the ReplaceGameParts is initialized after the game & satsuma are fully loaded
			/// </summary>
			Initialized,
		}
	}
}