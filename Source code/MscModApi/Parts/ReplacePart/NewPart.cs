using System;
using HutongGames.PlayMaker;
using MSCLoader;
using System.Linq;
using MscModApi.Tools;
using UnityEngine;

namespace MscModApi.Parts.ReplacePart
{
	public class NewPart : BasicPart
	{
		public Part part;

		public NewPart(Part part, bool canBeInstalledWithoutReplacing = false)
		{
			this.part = part;
			this.canBeInstalledWithoutReplacing = canBeInstalledWithoutReplacing;
		}

		/// <inheritdoc />
		public override bool isLookingAt => part.isLookingAt;

		/// <inheritdoc />
		public override bool isHolding => part.isHolding;

		/// <inheritdoc />
		public override string name => part.name;

		public override bool installed => part.installed;

		[Obsolete("Use 'bolted' property instead")]
		public bool IsFixed(bool ignoreUnsetScrews = true)
		{
			return part.IsFixed(ignoreUnsetScrews);
		}

		public bool canBeInstalledWithoutReplacing { get; protected set; }

		public override bool bought
		{
			get => part.bought;
			set => part.bought = value;
		}

		public override Vector3 position
		{
			get => part.position;
			set => part.position = value;
		}

		public override Vector3 rotation
		{
			get => part.rotation;
			set => part.rotation = value;
		}

		public override bool active
		{
			get => part.active;
			set => part.active = value;
		}

		public override void ResetToDefault(bool uninstall = false)
		{
			part.ResetToDefault(uninstall);
		}

		public bool installBlocked
		{
			get => part.installBlocked;
			set
			{
				if (!canBeInstalledWithoutReplacing)
				{
					part.installBlocked = value;
				}
			}
		}

		public override bool bolted => part.bolted;

		[Obsolete("Use 'installBlocked' property instead", true)]
		public void BlockInstall(bool block)
		{
			installBlocked = block;
		}

		[Obsolete("Use 'canBeInstalledWithoutReplacing' property instead", true)]
		public bool CanBeInstalledWithoutReplacing()
		{
			return canBeInstalledWithoutReplacing;
		}
	}
}