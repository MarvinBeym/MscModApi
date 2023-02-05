using MSCLoader;
using MscModApi.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace MscModApi.Parts
{
	public class PartBaseInfo
	{
		public Mod mod;
		public AssetBundle assetBundle;

		internal string saveFilePath;
		private List<Part> partsList;
		internal Dictionary<string, PartSave> partsSave;

		public PartBaseInfo(Mod mod, AssetBundle assetBundle, List<Part> partsList = null, string saveFilePath = "parts_saveFile.json")
		{
			this.mod = mod;
			this.assetBundle = assetBundle;
			this.saveFilePath = saveFilePath;
			this.partsList = partsList;
			partsSave = Helper.LoadSaveOrReturnNew<Dictionary<string, PartSave>>(mod, saveFilePath);
		}

		internal void AddToPartsList(Part part)
		{
			partsList?.Add(part);
		}
	}
}