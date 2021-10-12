using MscModApi.Parts.ReplacementPart;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MscModApi.Parts.ReplacePart
{
	public class ReplacementPart
	{

		public List<Part> newParts = new List<Part>();
		public List<OldPart> oldParts = new List<OldPart>();

		public ReplacementPart(GameObject oldFsmGameObject, Part newPart) : this(new[] { oldFsmGameObject }, new[] { newPart })
		{
		}

		public ReplacementPart(GameObject[] oldFsmGameObjects, Part newPart) : this(oldFsmGameObjects, new[] { newPart })
		{
		}

		public ReplacementPart(GameObject oldFsmGameObject, Part[] newParts) : this(new[] { oldFsmGameObject }, newParts)
		{
		}

		public ReplacementPart(GameObject[] oldFsmGameObjects, Part[] newParts)
		{
			foreach (var newPart in newParts) {
				this.newParts.Add(newPart);
			}

			foreach (var oldFsmGameObject in oldFsmGameObjects) {
				oldParts.Add(new OldPart(oldFsmGameObject, new Action(OldPartInstalled), new Action(OldPartUninstalled)));
			}
			foreach (var newPart in newParts) {
				newPart.AddPostInstallAction(NewPartInstalled);
				newPart.AddPostUninstallAction(NewPartUninstalled);
			}
		}

		internal void NewPartInstalled()
		{
			var anyNewInstalled = newParts.Any(part => part.IsInstalled());
			foreach (var oldPart in oldParts) {
				oldPart.BlockInstall(anyNewInstalled);
			}
		}

		internal void NewPartUninstalled()
		{
			var allNewUninstalled = newParts.All(part => !part.IsInstalled());
			foreach (var oldPart in oldParts) {
				oldPart.BlockInstall(!allNewUninstalled);
			}
		}

		internal void OldPartInstalled()
		{
			var anyOldInstalled = oldParts.Any(part => part.IsInstalled());
			foreach (var oldPart in oldParts) {
				oldPart.BlockInstall(anyOldInstalled);
			}
		}

		internal void OldPartUninstalled()
		{
			var allOldUninstalled = oldParts.All(part => !part.IsInstalled());
			foreach (var newPart in newParts) {
				newPart.BlockInstall(!allOldUninstalled);
			}
		}
	}
}