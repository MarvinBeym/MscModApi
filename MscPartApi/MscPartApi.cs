using System.Collections.Generic;
using MSCLoader;
using MscPartApi.Tools;
using UnityEngine;

namespace MscPartApi
{
    internal class MscPartApi : Mod
    {
        public override string ID => "MscPartApi";
        public override string Name => "MscPartApi";
        public override string Author => "DonnerPlays";
        public override string Version => "1.0";

        public override string Description =>
            "This allows developers to make their parts installable on the car. Also adds screws";

        public override bool UseAssetsFolder => true;
        private static Settings showBoltSizeSetting = new Settings("showBoltSizeSetting", "Show screw size", false);
        public static bool ShowScrewSize => (bool) showBoltSizeSetting.Value;
        private const string AssetsFile = "mscPartApi.unity3d";
        internal static AudioClip soundClip;
        internal static GameObject clampModel;
        private Tool tool;

        internal static Dictionary<string, Screw> screws = new Dictionary<string, Screw>();
        private Screw previousScrew;

        public override void ModSetup()
        {
            SetupFunction(Setup.OnNewGame, Mod_OnNewGame);
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
            SetupFunction(Setup.OnSave, Mod_OnSave);
            SetupFunction(Setup.Update, Mod_Update);

            LoadAssets();
        }

        public override void ModSettings()
        {
            Settings.AddCheckBox(this, showBoltSizeSetting);
        }

        private void Mod_OnNewGame()
        {
        }

        private void Mod_OnLoad()
        {
            tool = new Tool();
        }

        private void Mod_OnSave()
        {
        }

        private void Mod_Update()
        {
            var toolInHand = tool.GetToolInHand();
            if (toolInHand == Tool.ToolType.None)
            {
                if (previousScrew != null)
                {
                    previousScrew.Highlight(false);
                    previousScrew = null;
                }

                return;
            }

            Screw screw = DetectScrew();

            if (screw == null) return;

            if (ShowScrewSize && screw.showSize)
            {
                UserInteraction.ShowGuiInteraction(UserInteraction.Type.None,
                    $"Screw size: {screw.size.ToString("#.#").Replace(".00", "")}mm");
            }

            if (!tool.CheckScrewFits(screw)) return;

            screw.Highlight(true);

            if (!tool.CheckBoltingSpeed()) return;

            if (UserInteraction.MouseScrollWheel.Up)
            {
                switch (toolInHand)
                {
                    case Tool.ToolType.RatchetTighten:
                        screw.In();
                        break;
                    case Tool.ToolType.RatchetLoosen:
                        screw.Out();
                        break;
                    default:
                        screw.In();
                        break;
                }
            }
            else if (UserInteraction.MouseScrollWheel.Down)
            {
                switch (toolInHand)
                {
                    case Tool.ToolType.RatchetTighten:
                        screw.In();
                        break;
                    case Tool.ToolType.RatchetLoosen:
                        screw.Out();
                        break;
                    default:
                        screw.Out();
                        break;
                }
            }


            //ModConsole.Print();
        }

        private Screw DetectScrew()
        {
            if (previousScrew != null)
            {
                previousScrew.Highlight(false);
                previousScrew = null;
            }

            if (Camera.main == null)
            {
                return null;
            }

            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 1f,
                1 << LayerMask.NameToLayer("DontCollide")))
            {
                return null;
            }

            if (!hit.collider)
            {
                return null;
            }

            var hitObject = hit.collider.gameObject;
            if (hitObject == null || !hitObject.name.Contains("_screw_")) return null;

            if (!screws.TryGetValue(hitObject.name, out var screw)) return null;
            if (!screw.part.IsInstalled()) return null;

            previousScrew = screw;

            return screw;
        }

        private void LoadAssets()
        {
            var assetBundle = Helper.LoadAssetBundle(this, AssetsFile);
            Screw.material = assetBundle.LoadAsset<Material>("Screw-Material.mat");
            soundClip = assetBundle.LoadAsset<AudioClip>("screwable_sound.wav");
            clampModel = assetBundle.LoadAsset<GameObject>("Tube_Clamp.prefab");

            Screw.nutModel = assetBundle.LoadAsset<GameObject>("screwable_nut.prefab");
            Screw.screwModel = assetBundle.LoadAsset<GameObject>("screwable_screw1.prefab");
            Screw.normalModel = assetBundle.LoadAsset<GameObject>("screwable_screw2.prefab");
            Screw.longModel = assetBundle.LoadAsset<GameObject>("screwable_screw3.prefab");
            assetBundle.Unload(false);
        }
    }
}