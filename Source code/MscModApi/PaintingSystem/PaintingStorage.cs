using MSCLoader;
using System.Collections.Generic;
using UnityEngine;

namespace MscModApi.PaintingSystem
{
	public class PaintingStorage
	{
		private Mod mod;
		private string id;
		private Dictionary<GameObject, List<Material>> paintMaterialConfig;
		private Color currentColor = new Color(0.75f, 0.75f, 0.75f);

		public PaintingStorage(Mod mod, string id, Dictionary<GameObject, List<Material>> paintMaterialConfig)
		{
			this.mod = mod;
			this.id = id;
			this.paintMaterialConfig = paintMaterialConfig;

			SetColor(currentColor);
		}

		internal Mod GetMod()
		{
			return mod;
		}

		internal string GetPaintingId()
		{
			return id;
		}

		internal Dictionary<GameObject, List<Material>> GetGameObjectMaterialConfig()
		{
			return paintMaterialConfig;
		}

		internal void SetColorOfMaterial(Material material, Color color)
		{
			material.SetColor("_Color", color);
			currentColor = color;
		}

		internal Color GetCurrentColor()
		{
			return currentColor;
		}

		internal Color GetColorOfMaterial(Material material)
		{
			return material.GetColor("_Color");
		}

		public PaintingStorage SetMetallic(float metallic, float glossiness)
		{
			foreach (var pair in paintMaterialConfig) {
				foreach (Material material in pair.Value) {
					material.SetFloat("_Metallic", metallic);
					material.SetFloat("_Glossiness", glossiness);
				}
			}

			return this;
		}

		public PaintingStorage SetColor(Color color)
		{
			foreach (var pair in paintMaterialConfig) {
				foreach (Material material in pair.Value) {
					SetColorOfMaterial(material, color);
				}
			}

			return this;
		}

		public PaintingStorage SetColor(Color color, Color emisionColor, Color specColor)
		{
			foreach (var pair in paintMaterialConfig) {
				foreach (Material material in pair.Value) {
					SetColorOfMaterial(material, color);
					material.SetColor("_EmissionColor", emisionColor);
					material.SetColor("_SpecColor", specColor);
				}
			}

			return this;
		}

		public PaintingStorage ApplyMaterial(string materialName, bool withColor = false)
		{
			return ApplyMaterial(PaintingSystem.FindMaterial(materialName), withColor);
		}

		public PaintingStorage ApplyMaterial(Material materialToApply, bool withColor = false,
			bool withMainTexture = false)
		{
			if (materialToApply == null) {
				return this;
			}

			foreach (var pair in paintMaterialConfig) {
				pair.Value.Clear();
				foreach (Renderer renderer in pair.Key.GetComponentsInChildren<Renderer>(true)) {
					Texture oldMainTexture = renderer.material.GetTexture("_MainTex");
					Color oldColor = GetColorOfMaterial(renderer.material);

					renderer.material = materialToApply;
					if (!withColor) {
						SetColorOfMaterial(renderer.material, oldColor);
					}

					if (!withMainTexture) {
						renderer.material.SetTexture("_MainTex", oldMainTexture);
					}


					pair.Value.Add(renderer.material);
				}

				continue;
			}

			return this;
		}
	}
}