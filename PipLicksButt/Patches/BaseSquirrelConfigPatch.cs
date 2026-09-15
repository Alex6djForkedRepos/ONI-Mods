using HarmonyLib;
using PipLicksButt.Content.Scripts;
using System;
using UnityEngine;

namespace PipLicksButt.Patches
{
    public class BaseSquirrelConfigPatch
    {
        [HarmonyPatch(typeof(BaseSquirrelConfig), "BaseSquirrel")]
        public class BaseSquirrelConfig_BaseSquirrel_Patch
        {
            public static void Postfix(GameObject __result)
            {
                __result.AddOrGetDef<PipKicksButtMonitor.Def>().Initialize(3, RaptorTuning.ROAR_COOLDOWN);
            }
        }
    }
}