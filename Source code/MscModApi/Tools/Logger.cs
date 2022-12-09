using MSCLoader;
using System;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace MscModApi.Tools
{
	public static class Logger
	{
		private static Mod mod;
		private const string fileName = "logs.log";
		private static string filePath = "";
		private static int maxLineLength = 62;
		internal static void InitLogger(Mod mod)
		{
			Logger.mod = mod;
			filePath = Helper.CombinePaths(ModLoader.GetModSettingsFolder(mod), fileName);
			InitFile();
		}

		public static string GetLogFilePath()
		{
			return filePath;
		}

		public static void New(string message) => New(message, "", null);
		public static void New(string message, string additionalInfo) => New(message, additionalInfo, null);
		public static void New(string message, Exception ex) => New(message, "", ex);
		public static void New(string message, string additionalInfo, Exception ex)
		{
			using (var sw = new StreamWriter(filePath, true)) {
				var errorLogLine = AddBaseLogLine(message);
				if (additionalInfo != "")
				{
					errorLogLine = AddAdditionalInfoLine(errorLogLine, additionalInfo);
				}

				if (ex != null && ex.Message != "")
				{
					errorLogLine = AddExceptionLine(errorLogLine, ex);
				}
				errorLogLine += Environment.NewLine;
				sw.Write(errorLogLine);
			}
		}

		private static void InitFile()
		{
			var modsInstalled = GetModsInstalled();
			var baseInformation =
				$@"╔{GenerateHeader(" Environment ")}
║ Steam:        {ModLoader.CheckSteam().ToXY("Yes", "No")}
║ OS:           {GetOperatingSystem()}
╠{GenerateHeader(" Mod ")}
║ Name:         {mod.Name}
║ Version:      {mod.Version}
║ Author:       {mod.Author}
╠{GenerateHeader(" ModLoader ")}
║ Version:      {ModLoader.MSCLoader_Ver}
║ Experimental: {ModLoader.experimental}
╠{GenerateHeader(" Mods ")}
║
{modsInstalled}
╚{GenerateHeader("")}
";
			using (var streamWriter = new StreamWriter(filePath, false)) {
				streamWriter.Write(baseInformation);
			}
		}
		private static string GenerateHeader(string description, char headerLine = '═')
		{
			var header = "════";
			header += description;
			header += new string(headerLine, (maxLineLength - header.Length));
			return header;
		}

		private static string GetOperatingSystem()
		{
			var operatingSystem = SystemInfo.operatingSystem;
			var build = int.Parse(operatingSystem.Split('(')[1].Split(')')[0].Split('.')[2]);
			if (build <= 9600) return operatingSystem;
			operatingSystem = $"Windows 10 (10.0.{build})";

			if (SystemInfo.operatingSystem.Contains("64bit")) {
				operatingSystem += " 64bit";
			}

			return operatingSystem;
		}

		private static string GetModsInstalled()
		{
			var modsInstalled = "";
			foreach (var mod in ModLoader.LoadedMods) {
				// Ignore MSCLoader.
				if (mod.ID == "MSCLoader_Console" || mod.ID == "MSCLoader_Settings")
					continue;
				var modLine = string.Format(
					"║ [{0}] ID: {1} Name: {2} Version: {3}" + Environment.NewLine,
					mod.isDisabled.ToXY("DISABLED", " ACTIVE "),
					mod.ID,
					mod.Name,
					mod.Version
					);
				modsInstalled += modLine;
				if (maxLineLength < modLine.Length)
				{
					maxLineLength = modLine.Length;
				}
			}

			modsInstalled += "║";
			return modsInstalled;
		}
		private static string AddBaseLogLine(string message)
		{
			DateTime dateTime = DateTime.Now;
			string formattedDateTime = dateTime.ToString("G", CultureInfo.CreateSpecificCulture("de-DE"));
			return $"[{formattedDateTime}] {message}";
		}

		private static string AddAdditionalInfoLine(string errorLogLine, string info)
		{
			errorLogLine += $"{Environment.NewLine}=> Additional infos: {info}";

			return errorLogLine;
		}

		private static string AddExceptionLine(string errorLogLine, Exception ex)
		{
			if (ex != null) {
				errorLogLine += $"{Environment.NewLine}=> Exception message: {ex.Message}";
			}

			return errorLogLine;
		}
	}
}