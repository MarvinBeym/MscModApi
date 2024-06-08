using System;

namespace MscModApi.Parts
{
	public class ReplacedPartEventListener
	{
		/// <summary>
		/// The type of event
		/// </summary>
		public ReplacedGamePartsEvent.Type type { get; protected set; }


		/// <summary>
		/// The action executed when the event triggers
		/// </summary>
		public Action action { get; protected set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type">The type of event</param>
		/// <param name="action">The action executed when the event triggers</param>
		public ReplacedPartEventListener(ReplacedGamePartsEvent.Type type, Action action)
		{
			this.type = type;
			this.action = action;
		}
	}
}