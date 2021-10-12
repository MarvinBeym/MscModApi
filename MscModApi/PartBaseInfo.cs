﻿using MSCLoader;
using MscModApi.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace MscModApi
{
	public class PartBaseInfo
	{
		internal Mod mod;
		internal AssetBundle assetBundle;

		internal string saveFilePath;
		internal Dictionary<string, PartSave> partsSave;

		public PartBaseInfo(Mod mod, AssetBundle assetBundle, string saveFilePath)
		{
			this.mod = mod;
			this.assetBundle = assetBundle;
			this.saveFilePath = saveFilePath;
			partsSave = Helper.LoadSaveOrReturnNew<Dictionary<string, PartSave>>(mod, saveFilePath);
		}
	}
}