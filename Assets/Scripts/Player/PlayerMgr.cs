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

        #region =====�¼�=======
        public delegate void PlayerEvent(uint netId);

        private Action truePlayerStartAct;

        public Action<uint> disableAckerAct;

        private PlayerEvent OnValueChangeEvent; //��ֵ�ı�ʱ ���¼���ߵǳ�

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
                //����Ѿ���ʼ��ֱ�ӵ���
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

        #endregion =====�¼�=======


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
        /// <param name="DamagerNetId">�ܻ�����</param>
        /// <param name="ackInfo"></param>

        public void CmdOnDam(uint DamagerNetId, AckInfo ackInfo)
        {
            Debug.Log($"yns CmdOnDam {DamagerNetId} {ackInfo}");
            SkillSetting setting = SkillSettingMgr.Instance.GetSkillSetting(ackInfo.ackId);

            //��ȡ�ܻ���
            var OnDamager = GetAcker(DamagerNetId);

            //�˺���ֵ
            float DamageValue = OnDamager.Ack * setting.AckRate * Random.Range(0.95f, 1.05f); // * random;


            //��������Ѫ��
            int targetHp = Math.Max(Mathf.RoundToInt(OnDamager.playerData.hp - DamageValue), 0);
            //�����˺�������ʾ
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


            //�޸�playerData
            OnDamager.playerData.hp = targetHp;

            //ִ�б��ֲ�
            IAttacker onDamgerer = GetAcker(DamagerNetId);
            onDamgerer.OnDam(ackInfo);
        }



    }


}
