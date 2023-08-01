using MscModApi.Parts;
using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.Shopping
{
	public class KitLogic : MonoBehaviour
	{
		private Kit kit;

		void Update()
		{
			if (kit.spawnedCounter < kit.GetParts().Count && gameObject.IsLookingAt())
			{
				UserInteraction.GuiInteraction(
					$"Press [{cInput.GetText("Use")}] to {"Unpack " + kit.GetParts()[kit.spawnedCounter].gameObject.name.Replace("(Clone)", "")}"
				);
				if (UserInteraction.UseButtonDown)
				{
					Part part = kit.GetParts()[kit.spawnedCounter];

					part.SetPosition(gameObject.transform.position + gameObject.transform.up * 0.3f);
					part.SetActive(true);
					kit.spawnedCounter++;
				}
			}

			if (kit.spawnedCounter >= kit.GetParts().Count)
			{
				gameObject.SetActive(false);
			}
		}

		public void Init(Kit kit)
		{
			this.kit = kit;
		}
	}
}