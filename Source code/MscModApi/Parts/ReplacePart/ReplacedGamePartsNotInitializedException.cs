using System;

namespace MscModApi.Parts.ReplacePart
{
	/// <summary>
	/// Thrown by methods if the ReplacedGameParts class hasn't been initialized yet
	/// </summary>
	public class ReplacedGamePartsNotInitializedException : Exception
	{
		public ReplacedGamePartsNotInitializedException() : base("The ReplacedGameParts class hasn't been initialized yet. Please wait for it to be initialized before interacting with it")
		{
			
		}
	}
}