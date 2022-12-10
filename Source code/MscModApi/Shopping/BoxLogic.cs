using System.Collections.Generic;
using MscModApi.Parts;
using MscModApi.Tools;
using UnityEngine;


namespace MscModApi.Shopping
{
	public class BoxLogic : MonoBehaviour
	{
		private Box box;
		private string actionToDisplay;

		void Update()
		{
			if (gameObject.IsLookingAt() && box.spawnedCounter < box.GetParts().Count)
			{
				UserInteraction.GuiInteraction($"Press [{cInput.GetText("Use")}] to {actionToDisplay}");
				if (UserInteraction.UseButtonDown)
				{
					Part part = box.GetParts()[box.spawnedCounter];

					part.SetPosition(gameObject.transform.position + gameObject.transform.up * 0.3f);

					part.SetActive(true);
					box.spawnedCounter++;
				}
			}

			if (box.spawnedCounter >= box.GetParts().Count)
			{
				this.gameObject.SetActive(false);
			}
		}

		public void Init(string actionToDisplay, Box box)
		{
			this.box = box;
			this.actionToDisplay = actionToDisplay;
		}
	}
}