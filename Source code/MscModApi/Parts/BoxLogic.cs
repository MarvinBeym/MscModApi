using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.Parts
{
	/// <summary>
	/// Logic handling unpacking a Part from a PartBox object
	/// </summary>
	public class BoxLogic : MonoBehaviour
	{
		private PartBox box;

		void Update()
		{
			if (box.hasPartsToUnpack && box.isLookingAt) {
				Part nextPart = (Part)box.childs[box.partsUnpackedCount];
				UserInteraction.GuiInteraction($"Press [{cInput.GetText("Use")}] to Unpack {nextPart.cleanName}");
				if (UserInteraction.UseButtonDown) {
					nextPart.position = gameObject.transform.position + gameObject.transform.up * 0.3f;
					nextPart.active = true;
					box.IncrementUnpackedCount();
				}
			}

			if (!box.hasPartsToUnpack) {
				box.active = false;
			}
		}

		/// <summary>
		/// Initializes the Box logic by passing the required PartBox object
		/// </summary>
		/// <param name="box">The 'PartBox' object instance</param>
		public void Init(PartBox box)
		{
			this.box = box;
		}
	}
}