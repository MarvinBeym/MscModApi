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
			if (gameObject.IsLookingAt() && box.hasPartsToUnpack)
			{
				UserInteraction.GuiInteraction($"Press [{cInput.GetText("Use")}] to {actionToDisplay}");
				if (UserInteraction.UseButtonDown)
				{
					Part part = box.parts[box.partsUnpackedCount];

					part.position = gameObject.transform.position + gameObject.transform.up * 0.3f;
					part.active = true;
					box.IncrementUnpackedCount();
				}
			}

			if (!box.hasPartsToUnpack)
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