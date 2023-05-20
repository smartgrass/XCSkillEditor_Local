using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoCao
{
    public class DataMgr
    {
        #region get
        static DebugSo DebugSo => ResFinder.SoUsingFinder.DebugSo;
        public struct MaxData
        {
            public int MaxHp;
            public int MaxBraekPower;
        }

        #endregion
        public static MaxData GetPlayData(bool isTruePlayer, int level = 0)
        {
            MaxData maxData = new MaxData
            {
                MaxHp = isTruePlayer ? DebugSo.MaxHp : (int)(DebugSo.MaxHp * 1.5f),
                MaxBraekPower = isTruePlayer ? DebugSo.maxBreakPower : DebugSo.maxBreakPowerNPC[0]
            };
            return maxData;
        }

        public static PlayerSkin GetSkin(bool isTruePlayer)
        {
            return isTruePlayer ? DebugSo.playerSkin : DebugSo.npcSkin;
        }

        static uint idCounter = 1;
        public static uint GetID(bool isTruePlayer)
        {
            if (isTruePlayer)
            {
                return 0;
            }
            else
            {
                return idCounter++;
            }
        }







    }


    //public class Data
    //{
    //    public PlayerData data;
    //}

}
