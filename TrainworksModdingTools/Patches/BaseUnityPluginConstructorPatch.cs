﻿using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using BepInEx;
using Trainworks.Managers;

namespace Trainworks.Patches
{
    /// <summary>
    /// Registers plugins with the PluginManager on startup.
    /// </summary>
    [HarmonyPatch(typeof(BaseUnityPlugin), MethodType.Constructor)]
    class BaseUnityPluginConstructorPatch
    {
        static void Postfix(ref BaseUnityPlugin __instance)
        {
            PluginManager.RegisterPlugin(__instance);
        }
    }
}
