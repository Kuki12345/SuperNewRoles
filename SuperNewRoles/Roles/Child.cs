using Hazel;
using SuperNewRoles.CustomRPC;
using SuperNewRoles.Mode;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SuperNewRoles.Roles
{
    public static class Child
    {
        public static void FixedUpdate()
        {
            foreach (PlayerControl IsChildAlive in RoleClass.Child.ChildPlayer)
            {
                if (!IsChildAlive.isAlive())
                {
                    if (ModeHandler.isMode(ModeId.Default))
                    {
                        foreach (PlayerControl p in RoleClass.God.GodPlayer)
                        {
                            if (p.isAlive())
                            {
                                SuperNewRolesPlugin.Logger.LogInfo("�_����");
                                RPCProcedure.RPCMurderPlayer(p.PlayerId, p.PlayerId, byte.MaxValue);
                            }
                        }
                        foreach (PlayerControl p in RoleClass.Opportunist.OpportunistPlayer)
                        {
                            if (p.isAlive())
                            {
                                SuperNewRolesPlugin.Logger.LogInfo("�I�|�`������");
                                RPCProcedure.RPCMurderPlayer(p.PlayerId, p.PlayerId, byte.MaxValue);
                            }
                        }
                    }
                    else if (ModeHandler.isMode(ModeId.SuperHostRoles))
                    {
                        if (AmongUsClient.Instance.AmHost)
                        {
                            foreach (PlayerControl p in RoleClass.God.GodPlayer)
                            {
                                if (p.isAlive())
                                {
                                    SuperNewRolesPlugin.Logger.LogInfo("�_����");
                                    p.RpcMurderPlayer(p);
                                }
                            }
                            foreach (PlayerControl p in RoleClass.Opportunist.OpportunistPlayer)
                            {
                                if (p.isAlive())
                                {
                                    SuperNewRolesPlugin.Logger.LogInfo("�I�|�`������");
                                    p.RpcMurderPlayer(p);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}