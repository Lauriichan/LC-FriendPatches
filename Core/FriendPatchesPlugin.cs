﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using FriendPatches.Callbacks;
using FriendPatches.Patches;
using HarmonyLib;

namespace FriendPatches.Core
{
    public static class FriendPatchesInfo
    {
        public const string GUID = "lauriichan.friendpatches";
        public const string PrintName = "FriendPatches";
        public const string Version = "1.0.4";
    }

    public static class FriendPatchesSettings
    {
        public static bool SetPlayedWithOnSteam
        {
            get;
            private set;
        }
        public static bool UsernameFix
        {
            get;
            private set;
        }


        internal static void SetupConfig(ConfigFile cfg)
        {
            UsernameFix = cfg.Bind<bool>("General", "Enable the username fix patches", true, "Enables patches that listen to Steam callbacks for username changes and updates the player names accordingly.").Value;
            SetPlayedWithOnSteam = cfg.Bind<bool>("Steam", "Enable PlayedWith patches", true, "Enables patches that add players that joined your lobby to the 'Played With' list on Steam.").Value;
        }

    }

    [BepInPlugin(FriendPatchesInfo.GUID, FriendPatchesInfo.PrintName, FriendPatchesInfo.Version)]
    internal class FriendPatchesPlugin : BaseUnityPlugin
    {
        public static ManualLogSource Log
        {
            get;
            private set;
        }
        private Harmony harmony;

        /*
         Startup
        */

        public void Awake()
        {
            Log = Logger;
            FriendPatchesSettings.SetupConfig(Config);
            SetupCallbacks();
            SetupPatches();
        }

        private void SetupPatches()
        {
            if (harmony != null)
            {
                return;
            }
            harmony = new Harmony(FriendPatchesInfo.GUID);
            GameNetworkManager_Patches.Apply(harmony);
        }

        private void SetupCallbacks()
        {
            SteamFriendsCallback.Apply();
        }

        /*
         Shutdown
        */

        public void OnDestroy()
        {
            if (harmony != null)
            {
                harmony.UnpatchSelf();
                harmony = null;
            }
            ShutdownCallbacks();
        }

        private void ShutdownCallbacks()
        {
            SteamFriendsCallback.Unapply();
        }

    }
}
