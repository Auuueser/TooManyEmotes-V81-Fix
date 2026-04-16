using BepInEx.Bootstrap;
using GameNetcodeStuff;
using HarmonyLib;
using MoreCompany;
using MoreCompany.Cosmetics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TooManyEmotes.Audio;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using static TooManyEmotes.CustomLogging;
using static TooManyEmotes.HelperTools;

namespace TooManyEmotes.Compatibility
{
    [HarmonyPatch]
    internal static class MoreCompany_Compat
    {
        internal static bool Enabled { get { return Chainloader.PluginInfos.ContainsKey("me.swipez.melonloader.morecompany"); } }
        private static MethodInfo applyCosmeticMethod;
        private static bool searchedForApplyCosmeticMethod;
        private static int compatibilityWarningCount;
        private static int applyCosmeticArgumentCount;

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ShowLocalCosmetics(Transform playerRoot = null)
        {
            try
            {
                // If cosmetics not enabled in MoreCompany
                if (/*!MainClass.cosmeticsSyncOther.Value || */CosmeticRegistry.locallySelectedCosmetics.Count <= 0)
                    return;

                Transform cosmeticRoot = playerRoot != null ? playerRoot : StartOfRound.Instance?.localPlayerController?.transform;
                if (cosmeticRoot == null)
                    return;

                var cosmeticApplication = cosmeticRoot?.GetComponentInChildren<CosmeticApplication>();

                if (cosmeticApplication && cosmeticApplication.spawnedCosmetics.Count != 0)
                {
                    foreach (var item in cosmeticApplication.spawnedCosmetics)
                    {
                        SetAllChildrenLayer(item.transform, 0);
                        item.gameObject.SetActive(true);
                    }
                    return;
                }

                if (!cosmeticApplication)
                    cosmeticApplication = cosmeticRoot.gameObject.AddComponent<CosmeticApplication>();
                foreach (var cosmetic in CosmeticRegistry.locallySelectedCosmetics)
                    TryApplyCosmetic(cosmeticApplication, cosmetic);
                foreach (var cosmetic in cosmeticApplication.spawnedCosmetics)
                    cosmetic.transform.localScale *= CosmeticRegistry.COSMETIC_PLAYER_SCALE_MULT;
            }
            catch (Exception e) { LogCompatibilityWarning("Failed to show MoreCompany cosmetics. Continuing without cosmetic sync.", e); }
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void HideLocalCosmetics()
        {
            try
            {
                Transform cosmeticRoot = StartOfRound.Instance?.localPlayerController?.transform;
                if (cosmeticRoot == null)
                    return;

                var cosmeticApplication = cosmeticRoot?.GetComponentInChildren<CosmeticApplication>();

                if (cosmeticApplication && cosmeticApplication.spawnedCosmetics.Count != 0)
                {
                    foreach (var item in cosmeticApplication.spawnedCosmetics)
                        SetAllChildrenLayer(item.transform, 23);
                }
            }
            catch (Exception e) { LogCompatibilityWarning("Failed to hide MoreCompany cosmetics. Continuing without cosmetic sync.", e); }
        }

        private static void TryApplyCosmetic(CosmeticApplication cosmeticApplication, string cosmetic)
        {
            if (!cosmeticApplication || string.IsNullOrEmpty(cosmetic))
                return;

            var method = GetApplyCosmeticMethod();
            if (method == null)
            {
                LogCompatibilityWarning("MoreCompany ApplyCosmetic(string, bool) was not found. Skipping cosmetic sync.");
                return;
            }

            try
            {
                object[] args = applyCosmeticArgumentCount == 1 ? new object[] { cosmetic } : new object[] { cosmetic, true };
                method.Invoke(cosmeticApplication, args);
            }
            catch (Exception e)
            {
                LogCompatibilityWarning("MoreCompany ApplyCosmetic call failed. Skipping cosmetic sync.", UnwrapReflectionException(e));
            }
        }

        private static MethodInfo GetApplyCosmeticMethod()
        {
            if (searchedForApplyCosmeticMethod)
                return applyCosmeticMethod;

            searchedForApplyCosmeticMethod = true;
            applyCosmeticMethod = typeof(CosmeticApplication).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(m =>
                {
                    if (m.Name != "ApplyCosmetic")
                        return false;

                    var parameters = m.GetParameters();
                    return parameters.Length == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool);
                });

            if (applyCosmeticMethod == null)
            {
                applyCosmeticMethod = typeof(CosmeticApplication).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .FirstOrDefault(m =>
                    {
                        if (m.Name != "ApplyCosmetic")
                            return false;

                        var parameters = m.GetParameters();
                        return parameters.Length == 1 && parameters[0].ParameterType == typeof(string);
                    });
            }

            applyCosmeticArgumentCount = applyCosmeticMethod?.GetParameters().Length ?? 0;

            return applyCosmeticMethod;
        }

        private static Exception UnwrapReflectionException(Exception exception)
        {
            return exception is TargetInvocationException targetInvocationException && targetInvocationException.InnerException != null ? targetInvocationException.InnerException : exception;
        }

        private static void LogCompatibilityWarning(string message, Exception exception = null)
        {
            if (compatibilityWarningCount++ >= 3)
                return;

            if (exception != null)
                LogWarning(message + " Error: " + exception.GetType().Name + ": " + exception.Message);
            else
                LogWarning(message);
        }


        private static void SetAllChildrenLayer(Transform transform, int layer)
        {
            try
            {
                transform.gameObject.layer = layer;
                foreach (var light in transform.gameObject.GetComponents<Light>())
                    light.cullingMask = 1 << layer;

                foreach (Transform item in transform)
                    SetAllChildrenLayer(item, layer);
            }
            catch { } // Probably fine
        }
    }
}
