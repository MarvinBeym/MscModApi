using System.Data.SqlTypes;
using HutongGames.PlayMaker;

namespace MscModApi.Caching
{
	public class Game
	{
		private static FsmFloat _money;

		public static float money
		{
			get
			{
				if (_money != null) return _money.Value;
				_money = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerMoney");
				return _money.Value;
			}
			set
			{
				if (_money != null) _money.Value = value;
				_money = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerMoney");
				_money.Value = value;
			}
		}
	}
}