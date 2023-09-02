using System;
using System.Data.SqlTypes;
using HutongGames.PlayMaker;

namespace MscModApi.Caching
{
	/// <summary>
	/// Utility class for everything related to the game.
	/// </summary>
	public class Game
	{
		private static FsmFloat _money;

		/// <summary>
		/// Returns the current amount of money the player has.
		/// </summary>
		public static float money
		{
			get
			{
				if (_money != null) return _money.Value;
				_money = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerMoney");
				return (float)Math.Round(_money.Value, 1);
			}
			set
			{
				if (_money != null) _money.Value = value;
				_money = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerMoney");
				_money.Value = value;
			}
		}

		/// <summary>
		/// Called when the MscModApi mod loads to cleanup static data
		/// </summary>
		public static void LoadCleanup()
		{
			_money = null;
		}
	}
}