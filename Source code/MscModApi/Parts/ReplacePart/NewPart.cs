using System;
using HutongGames.PlayMaker;
using MSCLoader;
using System.Linq;
using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.Parts.ReplacePart
{
	public class NewPart
	{
		public Part part;
		private readonly bool canBeInstalledWithoutReplacing;

		public NewPart(Part part, bool canBeInstalledWithoutReplacing = false)
		{
			this.part = part;
			this.canBeInstalledWithoutReplacing = canBeInstalledWithoutReplacing;
		}

		public bool IsInstalled()
		{
			return part.IsInstalled();
		}

		public bool IsFixed(bool ignoreUnsetScrews = true)
		{
			return part.IsFixed(ignoreUnsetScrews);
		}

		public void BlockInstall(bool block)
		{
			if (!canBeInstalledWithoutReplacing)
			{
				part.BlockInstall(block);
			}
		}

		public bool CanBeInstalledWithoutReplacing()
		{
			return canBeInstalledWithoutReplacing;
		}
	}
}