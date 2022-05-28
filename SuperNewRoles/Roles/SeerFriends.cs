using SuperNewRoles.CustomOption;
using SuperNewRoles.CustomRPC;
using SuperNewRoles.Patch;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperNewRoles.Roles
{
    class SeerFriends
    {
        public static List<byte> CheckedJackal;
        public static bool CheckJackal(PlayerControl p)
        {
            if (!RoleClass.SeerFriends.IsJackalCheck) return false;
            if (!p.isRole(RoleId.SeerFriends)) return false;
            if (CheckedJackal.Contains(p.PlayerId)) return true;
            /*
            SuperNewRolesPlugin.Logger.LogInfo("�W���b�J���`�F�b�N�^�X�N��:"+RoleClass.SeerFriends.JackalCheckTask);
            SuperNewRolesPlugin.Logger.LogInfo("�I���^�X�N��:"+TaskCount.TaskDate(p.Data).Item1);*/
            SuperNewRolesPlugin.Logger.LogInfo("�L����:" + (RoleClass.SeerFriends.JackalCheckTask <= TaskCount.TaskDate(p.Data).Item1));
            if (RoleClass.SeerFriends.JackalCheckTask <= TaskCount.TaskDate(p.Data).Item1)
            {
                SuperNewRolesPlugin.Logger.LogInfo("�L����Ԃ��܂���");
                return true;
            }
            // SuperNewRolesPlugin.Logger.LogInfo("��ԉ��܂Œʉ�");
            return false;
        }
    }
}
