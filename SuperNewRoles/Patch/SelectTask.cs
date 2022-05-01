﻿using BepInEx.IL2CPP.Utils;
using HarmonyLib;
using SuperNewRoles.CustomOption;
using SuperNewRoles.CustomRPC;
using SuperNewRoles.Mode;
using SuperNewRoles.Mode.SuperHostRoles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static GameData;

namespace SuperNewRoles.Patch
{
    public static class SelectTask
    {

        public static bool IsAwakeEnd = false;
        [HarmonyPatch(typeof(GameData), nameof(GameData.SetTasks))]
        class SetTasksPatch
        {
            public static void Prefix(GameData __instance,
            [HarmonyArgument(0)] byte playerId,
            [HarmonyArgument(1)] ref UnhollowerBaseLib.Il2CppStructArray<byte> taskTypeIds)
            {
                AmongUsClient.Instance.StartCoroutine(SetTaskCoro(__instance, playerId, taskTypeIds));
            }
            static void SetTaskPatch(GameData __instance, byte playerId, UnhollowerBaseLib.Il2CppStructArray<byte> ids)
            {
                SuperNewRolesPlugin.Logger.LogInfo("長さ:" + ids.Length);
                GameData.PlayerInfo playerById = __instance.GetPlayerById(playerId);
                if (playerById == null)
                {
                    SuperNewRolesPlugin.Logger.LogInfo((object)("Could not set tasks for player id: " + playerId));
                }
                else
                {
                    if (playerById.Disconnected)
                    {
                        return;
                    }
                    if (!playerById.Object)
                    {
                        SuperNewRolesPlugin.Logger.LogInfo((object)("Could not set tasks for player (" + playerById.PlayerName + "): " + playerId));
                        return;
                    }
                    playerById.Tasks = new Il2CppSystem.Collections.Generic.List<TaskInfo>(ids.Length);
                    for (int i = 0; i < ids.Length; i++)
                    {
                        playerById.Tasks.Add(new TaskInfo(ids[i], (uint)i));
                        playerById.Tasks[i].Id = (uint)i;
                    }
                    playerById.Object.SetTasks(playerById.Tasks);
                    __instance.SetDirtyBit((uint)(1 << (int)playerById.PlayerId));
                }
            }
            static IEnumerator SetTaskCoro(GameData __instance, byte playerid, UnhollowerBaseLib.Il2CppStructArray<byte> ids)
            {
                while (true)
                {
                    if (!IsAwakeEnd)
                    {
                        yield return null;
                    }
                    else
                    {
                        break;
                    }
                }
                SetTaskPatch(__instance,playerid,ids);
            }
        }
        [HarmonyPatch(typeof(GameData), nameof(GameData.RpcSetTasks))]
        class RpcSetTasksPatch
        {
            public static void Prefix(GameData __instance,
            [HarmonyArgument(0)] byte playerId,
            [HarmonyArgument(1)] ref UnhollowerBaseLib.Il2CppStructArray<byte> taskTypeIds)
            {
                if (ModeHandler.isMode(ModeId.SuperHostRoles) || ModeHandler.isMode(ModeId.Default) && AmongUsClient.Instance.GameMode != GameModes.FreePlay)
                {
                    if (GetTaskCount(GameData.Instance.GetPlayerById(playerId).Object) == (PlayerControl.GameOptions.NumCommonTasks, PlayerControl.GameOptions.NumShortTasks, PlayerControl.GameOptions.NumLongTasks))
                    {
                        SuperNewRolesPlugin.Logger.LogInfo("ﾘﾀｰﾝ");
                        return;
                    }
                    PlayerControl.GameOptions.NumCommonTasks = 100;
                    PlayerControl.GameOptions.NumShortTasks = 100;
                    PlayerControl.GameOptions.NumLongTasks = 100;
                    var (commont, shortt, longt) = GameData.Instance.GetPlayerById(playerId).Object.GetTaskCount();
                    var TasksList = ModHelpers.generateTasks(commont, shortt, longt);
                    taskTypeIds = new UnhollowerBaseLib.Il2CppStructArray<byte>(TasksList.Count);
                    for (int i = 0; i < TasksList.Count; i++)
                    {
                        taskTypeIds[i] = TasksList[i];
                    }
                    PlayerControl.GameOptions.NumCommonTasks = SyncSetting.OptionData.NumCommonTasks;
                    PlayerControl.GameOptions.NumShortTasks = SyncSetting.OptionData.NumShortTasks;
                    PlayerControl.GameOptions.NumLongTasks = SyncSetting.OptionData.NumLongTasks;
                }
            }
        }
        public static (int,int,int) GetTaskCount(this PlayerControl p)
        {
            if (p.isRole(RoleId.MadMate))
            {
                if (CustomOptions.MadMateIsCheckImpostor.getBool())
                {
                    int commont = (int)CustomOptions.MadMateCommonTask.getFloat();
                    int shortt = (int)CustomOptions.MadMateShortTask.getFloat();
                    int longt = (int)CustomOptions.MadMateLongTask.getFloat();
                    if (!(commont == 0 && shortt == 0 && longt == 0))
                    {
                        return (commont, shortt, longt);
                    }
                }
            } else if (p.isRole(RoleId.Jester))
            {
                if (CustomOptions.JesterIsWinCleartask.getBool())
                {
                    int commont = (int)CustomOptions.JesterCommonTask.getFloat();
                    int shortt = (int)CustomOptions.JesterShortTask.getFloat();
                    int longt = (int)CustomOptions.JesterLongTask.getFloat();
                    if (!(commont == 0 && shortt == 0 && longt == 0))
                    {
                        return (commont, shortt, longt);
                    }
                }
            } else if (p.isRole(RoleId.God))
            {
                if (CustomOptions.GodIsEndTaskWin.getBool())
                {
                    int commont = (int)CustomOptions.GodCommonTask.getFloat();
                    int shortt = (int)CustomOptions.GodShortTask.getFloat();
                    int longt = (int)CustomOptions.GodLongTask.getFloat();
                    if (!(commont == 0 && shortt == 0 && longt == 0))
                    {
                        return (commont, shortt, longt);
                    }
                }
            } else if (p.isRole(RoleId.Workperson))
            {
                int commont = (int)CustomOptions.WorkpersonCommonTask.getFloat();
                int shortt = (int)CustomOptions.WorkpersonShortTask.getFloat();
                int longt = (int)CustomOptions.WorkpersonLongTask.getFloat();
                if (!(commont == 0 && shortt == 0 && longt == 0))
                {
                    return (commont, shortt, longt);
                }
            }

            else if (p.IsLovers() && !p.isImpostor())
            {
                int commont = (int)CustomOptions.LoversCommonTask.getFloat();
                int shortt = (int)CustomOptions.LoversShortTask.getFloat();
                int longt = (int)CustomOptions.LoversLongTask.getFloat();
                if (!(commont == 0 && shortt == 0 && longt == 0))
                {
                    return (commont, shortt, longt);
                }
            }
            return (SyncSetting.OptionData.NumCommonTasks, SyncSetting.OptionData.NumShortTasks, SyncSetting.OptionData.NumLongTasks);
        }
        public static (CustomOption.CustomOption, CustomOption.CustomOption, CustomOption.CustomOption) TaskSetting(int commonid,int shortid,int longid,CustomOption.CustomOption Child = null, CustomOptionType type = CustomOptionType.Generic,bool IsSHROn = false)
        {
            CustomOption.CustomOption CommonOption = CustomOption.CustomOption.Create(commonid, IsSHROn, type, "GameCommonTasks", 1, 0, 12, 1, Child);
            CustomOption.CustomOption ShortOption = CustomOption.CustomOption.Create(shortid, IsSHROn, type,"GameShortTasks", 1, 0, 69, 1, Child); ;
            CustomOption.CustomOption LongOption = CustomOption.CustomOption.Create(longid, IsSHROn, type, "GameLongTasks", 1, 0, 45, 1, Child);
            return (CommonOption, ShortOption, LongOption);
        }
    }
}
