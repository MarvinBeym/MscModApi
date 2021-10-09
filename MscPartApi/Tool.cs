
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using MSCLoader;
using MscPartApi.Tools;
using UnityEngine;

namespace MscPartApi
{
    internal class Tool
    {
        public enum ToolType
        {
            None,
            Spanner,
            Ratchet,
            RatchetTighten,
            RatchetLoosen,
            Screwdriver
        }

        private static bool hasToolInHand;
        private FsmFloat boltingSpeed;
        private FsmFloat wrenchSize;

        private Dictionary<ToolType, GameObject> toolGameObjects = new Dictionary<ToolType, GameObject>();

        private float timer;
        private FsmBool ratchetSwitch;
        private static GameObject selectItem;

        internal Tool()
        {
            var spannerPickFsm = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/2Spanner/Pick")
                .FindFsm("PickUp");

            GameObject ratchet = spannerPickFsm.FsmVariables.FindFsmGameObject("Ratchet").Value;

            ratchetSwitch = ratchet.FindFsm("Switch").FsmVariables.FindFsmBool("Switch");

            toolGameObjects.Add(ToolType.Spanner, spannerPickFsm.FsmVariables.FindFsmGameObject("HandSpanner").Value);
            toolGameObjects.Add(ToolType.Screwdriver,
                spannerPickFsm.FsmVariables.FindFsmGameObject("HandScrewdriver").Value);
            toolGameObjects.Add(ToolType.Ratchet, ratchet);

            boltingSpeed = PlayMakerGlobals.Instance.Variables.GetFsmFloat("BoltingSpeed");
            wrenchSize = PlayMakerGlobals.Instance.Variables.GetFsmFloat("ToolWrenchSize");
        }

        public ToolType GetToolInHand()
        {
            if (!HasToolInHand()) return ToolType.None;

            var toolGameObject = toolGameObjects.FirstOrDefault(keyValue => keyValue.Value.activeSelf);

            if (toolGameObject.Key == ToolType.Ratchet)
            {
                return ratchetSwitch.Value ? ToolType.RatchetTighten : ToolType.RatchetLoosen;
            }

            return toolGameObject.Value == null ? ToolType.None : toolGameObject.Key;
        }

        internal static bool HasToolInHand()
        {
            if (selectItem != null) return hasToolInHand;

            selectItem = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/SelectItem").gameObject;
            if (selectItem == null) return hasToolInHand;

            FsmHook.FsmInject(selectItem, "Hand", delegate { hasToolInHand = false; });
            FsmHook.FsmInject(selectItem, "Tools", delegate { hasToolInHand = true; });

            return hasToolInHand;
        }

        public bool CheckScrewFits(Screw screw)
        {
            float toolSize = this.wrenchSize.Value * 10f;

            return screw.size >= toolSize - 0.25f && screw.size <= toolSize + 0.25f;
        }

        internal bool CheckBoltingSpeed()
        {
            timer += Time.deltaTime;
            if (timer < boltingSpeed.Value) return false;

            timer = 0;
            return true;
        }
    }
}