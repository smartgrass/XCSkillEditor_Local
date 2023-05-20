
using UnityEngine;

namespace XiaoCao
{
    public static class GameSetting
    {
        public static GameMode gameMode;

        public static CameraMode cameraType;

        public static bool isResetCamara = true;

        public static bool HasAIEnable =true;

        public static int Port = 1234;


        public static AgentTag GetEnamyTag(AgentTag agentTag)
        {
            if (agentTag == AgentTag.PlayerB || agentTag == AgentTag.PlayerA)
            {
                return AgentTag.enemy;
            }
            else
            {
                return AgentTag.PlayerA;
            }
        }


        //自身碰撞体layer
        public static int GetColiorLayer(AgentTag agentTag)
        {
            if (agentTag == AgentTag.enemy)
            {
                return LayerMask.NameToLayer("Enemy");
            }
            else
            {
                return LayerMask.NameToLayer("Friend");
            }
        }


        public static int GetAckLayer(AgentTag agentTag)
        {
            if (agentTag == AgentTag.enemy)
            {
                return LayerMask.NameToLayer("EnemyAck");
            }
            else
            {
                //有攻击权限的玩家 一律当友军->本地玩家
                return LayerMask.NameToLayer("FriendAck");
            }
        }
    }

   
    [EnumLabel("CameraMode")]
    public enum CameraMode
    {
        [EnumLabel("跟随")]
        Follow,
        //[EnumLabel("固定")]
        Fix
    }

    public enum GameMode
    {
        PVP,
        PVE
    }

    public enum DamageState
    {
        Nor,
        OnBreak,
        NoBreak
    }

    public enum PlayerSkin
    {
        Nor,
        Elf,
        Kagura,
    }

    //判断阵营
    public enum AgentTag
    {
        PlayerA,
        PlayerB,
        enemy,
        other
    }

    //模型
    public enum AgentModelType
    {
        Player = 0,
        EnemyA = 1,
        EnemyB = 2,
    }

    public static class AgentNameExtend 
    { 
        public static bool IsEnemy(this AgentModelType name)
        {
            return name != AgentModelType.Player;
        }
        public static string GetSkillPath(this AgentModelType name)
        {
            return IsEnemy(name)? PrefabPath.SkillDataEnemyPath : PrefabPath.SkillDataPath;
        }
    }


    public static class XCDebuger
    {
        public static bool IsLogNor = true;
        public static bool IsLogNet = false;
        public static bool IsLogSkillEvent = true;


        public static void Log(object message, LogTag tag = LogTag.Nor)
        {
            if(IsLogTag(tag))
                Debug.Log(message);
        }


        private static bool IsLogTag(LogTag tag)
        {
            switch (tag)
            {
                case LogTag.Nor:
                    return IsLogNor;
                case LogTag.Net:
                    return IsLogNet;
                case LogTag.SkillEvent:
                    return IsLogSkillEvent;
                default:
                    break;
            }
            return true;
        }
    }

    public enum LogTag
    {
        Nor,
        SkillEvent,
        Net
    }
}
