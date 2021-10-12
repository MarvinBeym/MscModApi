using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using MSCLoader;
using MscPartApi.Parts.ReplacementPart;
using UnityEngine;
using UnityEngine.Events;

namespace MscPartApi.Parts.ReplacePart
{
	public class ReplacementPart
	{

		public List<Part> newParts;
		public List<OldPart> oldParts = new List<OldPart>();
		private List<GameObject> oldGameObjects;

		public ReplacementPart(GameObject[] oldFsmGameObjects, List<Part> newParts)
		{
			this.newParts = newParts;

			foreach (var oldFsmGameObject in oldFsmGameObjects)
			{
				oldParts.Add(new OldPart(oldFsmGameObject, new Action(OldPartInstalled), new Action(OldPartUninstalled)));
			}
			foreach (var newPart in newParts)
			{
				newPart.AddPostInstallAction(NewPartInstalled);
				newPart.AddPostUninstallAction(NewPartUninstalled);
			}
		}

		internal void NewPartInstalled()
		{
			var anyNewInstalled = newParts.Any(part => part.IsInstalled());
			foreach (var oldPart in oldParts)
			{
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