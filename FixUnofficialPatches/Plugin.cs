﻿using BepInEx;
using SpaceCraft;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using BepInEx.Logging;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Reflection;

namespace FixUnofficialPatches
{
    [BepInPlugin("akarnokd.theplanetcraftermods.fixunofficialpatches", "(Fix) Unofficial Patches", "1.0.0.2")]
    public class Plugin : BaseUnityPlugin
    {

        static ManualLogSource logger;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin is loaded!");

            logger = Logger;

            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        void Update()
        {
            SaveFilesSelectorScrollFix();
        }

        static SaveFilesSelector saveFilesSelectorInstance;
        static FieldInfo saveFilesSelectorObjectsInList;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SaveFilesSelector), nameof(SaveFilesSelector.Start))]
        static void SaveFilesSelector_Start(SaveFilesSelector __instance)
        {
            saveFilesSelectorInstance = __instance;
            saveFilesSelectorObjectsInList = AccessTools.Field(typeof(SaveFilesSelector), "objectsInList");
        }

        static void SaveFilesSelectorScrollFix()
        {
            if (saveFilesSelectorInstance != null && saveFilesSelectorInstance.filesListContainer.transform.parent.gameObject.activeSelf)
            {
                var scrollBox = saveFilesSelectorInstance.GetComponentInChildren<ScrollRect>();
                if (scrollBox != null)
                {
                    var scroll = Mouse.current.scroll.ReadValue();
                    if (scroll.y != 0)
                    {
                        var counts = (List<GameObject>)saveFilesSelectorObjectsInList.GetValue(saveFilesSelectorInstance);
                        if (counts != null && counts.Count != 0)
                        {
                            if (scroll.y < 0)
                            {
                                scrollBox.verticalNormalizedPosition -= 1f / counts.Count;
                            }
                            else if (scroll.y > 0)
                            {
                                scrollBox.verticalNormalizedPosition += 1f / counts.Count;
                            }
                        }
                    }
                }
            }
        }

        /*
         * Fixed in 0.5.005
        /// <summary>
        /// Fixes the lack of localization Id when viewing a Craft Station T2, so the window title is not properly updated.
        /// </summary>
        /// <param name="__instance">The ActionCrafter instance of the station object</param>
        /// <param name="___titleLocalizationId">The field hosting the localization id</param>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActionCrafter), nameof(ActionCrafter.OnAction))]
        static void ActionCrafter_OnAction(ActionCrafter __instance, ref string ___titleLocalizationId)
        {
            if (__instance.GetCrafterIdentifier() == DataConfig.CraftableIn.CraftStationT2)
            {
                ___titleLocalizationId = "GROUP_NAME_CraftStation1";
            }
        }
        */

        /*
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerAimController), "HandleAiming")]
        static void PlayerAimController_HandleAiming(Ray ___aimingRay, float ___distanceHitLimit, int ___layerMask)
        {
            if (Physics.Raycast(___aimingRay, out var raycastHit, ___distanceHitLimit, ___layerMask))
            {
                logger.LogInfo("Looking at " + raycastHit.transform.gameObject.name + " (" + ___layerMask + ")");
            }
            else
            {
                logger.LogInfo("No hits");
            }
        }
        */
    }
}
