
using System.Collections.Generic;
using MSCLoader;
using MscPartApi.Tools;
using UnityEngine;

namespace MscPartApi
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