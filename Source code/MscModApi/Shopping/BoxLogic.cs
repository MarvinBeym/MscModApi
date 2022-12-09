using MscModApi.Parts;
using MscModApi.Tools;
using UnityEngine;


namespace MscModApi.Shopping
{
	public class BoxLogic : MonoBehaviour
	{
		private Box box;
		private string actionToDisplay;

		private Part[] parts;

		void Update()
		{
			if (gameObject.IsLookingAt() && box.spawnedCounter < parts.Length)
			{
				UserInteraction.GuiInteraction($"Press [{cInput.GetText("Use")}] to {actionToDisplay}");
				if (UserInteraction.UseButtonDown)
				{
					Part part = parts[box.spawnedCounter];

					part.SetPosition(gameObject.transform.position + gameObject.transform.up * 0.3f);

					part.SetActive(true);
					box.spawnedCounter++;
				}
			}

			if (box.spawnedCounter >= parts.Length)
			{
				this.gameObject.SetActive(false);
			}
		}

		public void Init(Part[] parts, string actionToDisplay, Box box)
		{
			this.box = box;
			this.parts = parts;
			this.actionToDisplay = actionToDisplay;
		}
	}
}