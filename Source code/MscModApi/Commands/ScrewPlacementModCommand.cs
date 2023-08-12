using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSCLoader;
using MscModApi.Parts;
using MscModApi.Tools;

namespace MscModApi.Commands
{
	class ScrewPlacementModCommand : ConsoleCommand
	{
		private readonly MscModApi mod;
		protected List<Command> availableCommands = new List<Command>();
		private readonly Dictionary<string, Dictionary<string, Part>> modsParts;

		private readonly Command helpCommand;
		private readonly Command enableCommand;
		private readonly Command disableCommand;
		private readonly Command listCommand;
		protected class Command
		{
			public string name
			{
				get;
			}

			public string help
			{
				get;
			}


			public string parameterCallExample
			{
				get;
			}

			public Command(string name, string help, string parameterCallExample = "")
			{
				this.name = name;
				this.help = help;
				this.parameterCallExample = parameterCallExample;
			}
		}

		public ScrewPlacementModCommand(MscModApi mod, Dictionary<string, Dictionary<string, Part>> modsParts)
		{
			this.mod = mod;
			this.modsParts = modsParts;

			helpCommand = new Command("help", "This help", "Escape further command arguments with spaces, with two <color=blue>'</color>");
			
			enableCommand = new Command(
				"enable",
				"Enable screw placement mode for a part",
				"mod-api-screw enable '<your-mod-id>' '<part-id>'"
			);

			disableCommand = new Command(
				"disable",
				"Disable screw placement mode for a part",
				"mod-api-screw disable '<your-mod-id>' '<part-id>'"
			);

			listCommand = new Command(
				"list",
				"Lists all available parts with their screw placement mode status",
				"mod-api-screw list '<your-mod-id>'"
			);

			availableCommands.AddRange(new []
			{
				helpCommand,
				listCommand,
				enableCommand,
				disableCommand,
			});
		}
		public override void Run(string[] args)
		{
			Command mainCommand = null;
			foreach (Command command in availableCommands)
			{
				if (command.name == args.ElementAtOrDefault(0))
				{
					mainCommand = command;
				}
			}

			string partId;
			string modId;
			Part part;
			switch (mainCommand)
			{
				case Command cmd when cmd == helpCommand:
					foreach (Command command in availableCommands)
					{
						ModConsole.Print($"<color=orange>{command.name}</color>: {command.help}");
						if (command.parameterCallExample != "")
						{
							ModConsole.Print($"<color=orange>=></color> {command.parameterCallExample}");
						}
					}
					break;
				case Command cmd1 when cmd1 == enableCommand:
				case Command cmd2 when cmd2 == disableCommand:
					modId = args.ElementAtOrDefault(1);
					partId = args.ElementAtOrDefault(2);

					if (string.IsNullOrEmpty(modId) || string.IsNullOrEmpty(partId))
					{
						goto default;
					}

					//Removing potential ''
					modId = modId.Replace("'", "");
					partId = partId.Replace("'", "");

					if (!ScrewPlacementAssist.IsScrewPlacementModeEnabled(modId))
					{
						ModConsole.Print($"ScrewPlacementMode not enabled for mod with id '{modId}'");
						break;
					}


					if (!modsParts.ContainsKey(modId))
					{
						ModConsole.Error($"No mod with id <color=blue>{modId}</color> found that has added parts");
						break;
					}

					if (!modsParts[modId].ContainsKey(partId))
					{
						ModConsole.Error($"No part with id <color=blue>{partId}</color> found for mod with id <color=blue>{modId}</color>");
						break;
					}

					part = modsParts[modId][partId];
					part.screwPlacementMode = mainCommand == enableCommand;
					break;
				case Command cmd2 when cmd2 == listCommand:
					modId = args.ElementAtOrDefault(1);

					if (string.IsNullOrEmpty(modId))
					{
						goto default;
					}

					//Removing potential ''
					modId = modId.Replace("'", "");

					if (!ScrewPlacementAssist.IsScrewPlacementModeEnabled(modId))
					{
						ModConsole.Print($"ScrewPlacementMode not enabled for mod with id '{modId}'");
						break;
					}

					if (!modsParts.ContainsKey(modId))
					{
						ModConsole.Error($"No mod with id <color=blue>{modId}</color> found that has added parts");
						break;
					}

					foreach (var partDict in modsParts[modId])
					{
						part = partDict.Value;
						ModConsole.Print($"<color=orange>{part.id}</color> => {(part.screwPlacementMode ? "Enabled" : "Disabled")}");
					}

					break;
				default:
					ModConsole.Error($"Invalid command <color=blue>mod-api-screw {string.Join(" ", args)}</color>");
					break;
			}
		}

		public override string Name => "mod-api-screw";

		public override string Help =>
			"Run <color=blue>mod-api-screw help</color> for a list of commands and arguments";
	}
}
