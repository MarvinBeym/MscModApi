using System;

namespace MscModApi.Parts
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
		}
	}
}