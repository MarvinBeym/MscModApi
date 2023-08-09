using System;
using HutongGames.PlayMaker;

namespace MscModApi.Tools
{
	public class FsmAction : FsmStateAction
	{
		public Action action { get; protected set; }

		public FsmAction(Action action)
		{
			this.action = action;
		}
		public override void OnEnter()
		{
			action?.Invoke();
			Finish();
		}
	}
}