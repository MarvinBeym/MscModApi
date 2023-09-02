using MSCLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MscModApi.Tools
{
	class LoggedMod
	{
		private readonly Mod mod;
		private readonly string fileName;
		private readonly string filePath;

		public LoggedMod(Mod mod, string fileName)
		{
			this.mod = mod;
			this.fileName = fileName;

			this.filePath = Helper.CombinePaths(ModLoader.GetModSettingsFolder(mod), FileName);
		}

		public Mod Mod => mod;

		public string FileName => fileName;

		public string FilePath => filePath;
	}
}