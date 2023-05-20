using Assets.Scripts.Enemy;
using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XiaoCao;
using Random = UnityEngine.Random;

namespace XiaoCao
{
    public class PlayerMgr : Singleton<PlayerMgr>
    {
        #region NetWorkManager

        public uint LocalNetId = 0;

        public PlayerState LocalPlayer;
        public bool IsLocalPlayerReady => LocalPlayer != null;

        public SkillCDTimer localSkillCD = new SkillCDTimer();


        public Dictionary<uint, MonoAttacker> MonoAttackerDic = new Dictionary<uint, MonoAttacker>();

        public List<MonoAttacker> AgentList = new List<MonoAttacker>(); //All PlayerState

        #endregion

        #region =====事件=======
        public delegate void PlayerEvent(uint netId);

        private Action truePlayerStartAct;

        public Action<uint> disableAckerAct;

        private PlayerEvent OnValueChangeEvent; //数值改变时 或登录或者登出

        public void AddListener(ClientEventType eventType, PlayerEvent player)
        {
            if ((eventType == ClientEventType.ValueChange))
                OnValueChangeEvent += player;
        }
        public void RemoveListener(ClientEventType eventType, PlayerEvent player)
        {
            if ((eventType == ClientEventType.ValueChange))
                OnValueChangeEvent -= player;
        }

        public void AddPlayerStartedAction(Action addAciton)
        {
            if (IsLocalPlayerReady)
            {
                //如果已经开始就直接调用
                addAciton.Invoke();
            }
            else
            {
                truePlayerStartAct += addAciton;
            }
        }
        public void DoPlayerStart()
        {
            truePlayerStartAct?.Invoke();
        }

        #endregion =====事件=======


        public MonoAttacker GetAcker(uint netID)
        {
            if (!MonoAttackerDic.ContainsKey(netID)) return null;
            return MonoAttackerDic[netID];
        }

        private void RegisterAttacker(MonoAttacker attacker)
        {
            uint netID = attacker.netId;
            if (!MonoAttackerDic.ContainsKey(netID))
            {
                MonoAttackerDic.Add(netID, attacker);
            }
        }
        private void DisRegisterAttacker(uint netID)
        {
            if (MonoAttackerDic.ContainsKey(netID))
            {
                MonoAttackerDic.Remove(netID);
            }
            disableAckerAct?.Invoke(netID);
        }

        public void Register(MonoAttacker player)
        {
            uint netID = player.netId;
            Debug.Log("yns PlayerManager Register " + netID);

            AgentList.Add(player);
            RegisterAttacker(player);
            OnValueChangeEvent?.Invoke(netID);

        }
        public void DisRegister(uint netID)
        {
            OnValueChangeEvent?.Invoke(netID);
            DisRegisterAttacker(netID);
        }

        public void UpdatePlayerValue(uint netID = 0)
        {
            OnValueChangeEvent?.Invoke(netID);
        }

        public void SendBool(uint netId, bool isLocalOnly,string name, bool msg)
        {
            var acker = GetAcker(netId);
            if (acker == null)
                return;

            if (!isLocalOnly || (isLocalOnly && acker.isTruePlayer))
            {
                acker.SetBool(name, msg);
                Debug.Log($"yns Msg {name} {msg}");
            }
        }

        public void SendAll(uint netId, bool isLocalOnly, string name, float num, bool isOn =false, string str="")
        {
            var acker = GetAcker(netId);
            if (acker == null)
                return;

            if (!isLocalOnly || (isLocalOnly && acker.isTruePlayer))
            {
                acker.SendAll(name, num, isOn, str);
            }
        }


        public void AddTruePlayer()
        {
            var Player =  ResFinder.GetResObject<PlayerState>(PrefabPath.Player,true);
            Player.isTruePlayer = true;
            Player.Init();
        }

        public void AddFakePlayer(Vector3 startPos, bool isAi,
            AgentTag agentTag, AgentModelType agentName = AgentModelType.Player)
        {
            string prefabPath = agentName == AgentModelType.Player ? PrefabPath.Player : PrefabPath.EnemyB;

            PlayerState Player = ResFinder.GetResObject<PlayerState>(prefabPath, true);

            Player.isTruePlayer = false;
            Player.Init();
            Player.transform.position = startPos;
            Player.agentType = agentName;
            Player.AgentTag = agentTag;
            if (Player.AI != null)
            {
                Player.AI.enabled = isAi;
            }
        }







        /// <summary>
        /// 
        /// </summary>
        /// <param name="DamagerNetId">受击对象</param>
        /// <param name="ackInfo"></param>

        public void CmdOnDam(uint DamagerNetId, AckInfo ackInfo)
        {
            Debug.Log($"yns CmdOnDam {DamagerNetId} {ackInfo}");
            SkillSetting setting = SkillSettingMgr.Instance.GetSkillSetting(ackInfo.ackId);

            //获取受击者
            var OnDamager = GetAcker(DamagerNetId);

            //伤害数值
            float DamageValue = OnDamager.Ack * setting.AckRate * Random.Range(0.95f, 1.05f); // * random;


            //结算最终血量
            int targetHp = Math.Max(Mathf.RoundToInt(OnDamager.playerData.hp - DamageValue), 0);
            //用于伤害数字显示
            ackInfo.ackValue = DamageValue;

            ackInfo.lastState = OnDamager.damageState;

            if (OnDamager.damageState == DamageState.NoBreak)
            {
                ackInfo.isBreak = false;
            }
            else if (OnDamager.damageState == DamageState.OnBreak)
            {
                ackInfo.isBreak = true;

            }
            else
            {
                OnDamager.breakPower.noBreakPower -= (int)setting.BreakPower;
                if (OnDamager.breakPower.noBreakPower <= 0)
                {
                    //OnDamager.playerData.noBreakPower = OnDamager.playerData.maxBreakPower / 2;
                    OnDamager.damageState = DamageState.OnBreak;
                    ackInfo.isBreak = true;
                }
                else
                {
                    ackInfo.isBreak = false;
                }
            }


            //修改playerData
            OnDamager.playerData.hp = targetHp;

            //执行表现层
            IAttacker onDamgerer = GetAcker(DamagerNetId);
            onDamgerer.OnDam(ackInfo);
        }



    }


}
