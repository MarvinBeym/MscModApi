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
			if (kit.spawnedCounter < kit.parts.Length && gameObject.IsLookingAt())
			{
				UserInteraction.GuiInteraction(
					$"Press [{cInput.GetText("Use")}] to {"Unpack " + kit.parts[kit.spawnedCounter].gameObject.name.Replace("(Clone)", "")}"
				);
				if (UserInteraction.UseButtonDown)
				{
					Part part = kit.parts[kit.spawnedCounter];

					part.SetPosition(gameObject.transform.position + gameObject.transform.up * 0.3f);
					part.SetActive(true);
					kit.spawnedCounter++;
				}
			}

			if (kit.spawnedCounter >= kit.parts.Length)
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