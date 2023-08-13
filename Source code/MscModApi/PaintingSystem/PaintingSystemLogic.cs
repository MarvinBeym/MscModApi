using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MscModApi.Parts;
using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.PaintingSystem
{
	public class PaintingSystemLogic : MonoBehaviour
	{
		private PaintingStorage paintingStorage;

		void Update()
		{
			if (!PaintingSystem.IsPainting() || !gameObject.IsLookingAt()) {
				return;
			}

			Color color = PaintingSystem.GetCurrentColor();

			foreach (var pair in paintingStorage.GetGameObjectMaterialConfig()) {
				foreach (Material material in pair.Value) {
					paintingStorage.SetColorOfMaterial(material, color);
				}
			}
		}

		internal void Init(PaintingStorage paintingStorage)
		{
			this.paintingStorage = paintingStorage;
		}
	}
}