using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.Parts
{
	public class KitLogic : MonoBehaviour
	{
		private Kit kit;

		void Update()
		{
			if (kit.hasPartsToUnpack && gameObject.IsLookingAt())
			{
				UserInteraction.GuiInteraction(
					$"Press [{cInput.GetText("Use")}] to {"Unpack " + kit.parts[kit.partsUnpackedCount].gameObject.name.Replace("(Clone)", "")}"
				);
				if (UserInteraction.UseButtonDown)
				{
					Part part = kit.parts[kit.partsUnpackedCount];
					part.position = gameObject.transform.position + gameObject.transform.up * 0.3f;
					part.active = true;
					kit.IncrementUnpackedCount();
				}
			}

			if (!kit.hasPartsToUnpack)
			{
				kit.active = false;
			}
		}

		public void Init(Kit kit)
		{
			this.kit = kit;
		}
	}
}