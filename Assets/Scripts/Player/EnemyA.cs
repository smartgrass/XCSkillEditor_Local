using cfg;
using UnityEngine;

namespace XiaoCao
{
    //懒得再写一遍了...
    public class EnemyA: PlayerState
    {
        public PlayerData setData;

        //breakPoint > 1
        public int breakPoint = 20;

        private int curBreakPoint = 1;

        public override void InitPlayerData()
        {
            //设置数值
            PlayerData newData = setData;
            playerData = newData;
            Debug.Log($"yns InitPlayerData {setData} {playerData} {newData}");
            playerData.SetFull();
            breakPower.maxBreakPower = isTruePlayer ? DebugSo.maxBreakPower : DebugSo.maxBreakPowerNPC[0];
            breakPower.SetFull();
            damageState = DamageState.NoBreak;
            Debug.Log($"yns hp {playerData.hp}");
            dieTimer.Init("dieTimer", 2, OnDieEnd);
            breakTimer.Init("damageTimer", 0.3f, null);
            curBreakPoint = 1;
            breakPoint = Mathf.Max(1, breakPoint);
        }

        public override void OnHit(AckInfo ackInfo)
        {
            SkillSetting setting = SkillSettingMgr.Instance.GetSkillSetting(ackInfo.ackId);

            OnDamAciton?.Invoke(ackInfo);

            PlayHitEffect(ackInfo, setting.HitEffect);

            if (isTruePlayer && IsEnableBreakPoint())
            {
                if ((MaxHp - Hp) > (float)MaxHp / breakPoint * curBreakPoint)
                {
                    curBreakPoint++;
                    CmdOnDamAnim();
                }
            }
        }

        bool IsEnableBreakPoint()
        {
            if (curSkillRuner!=null && curSkillRuner.IsRuning)
            {
                //boss施法时 无法打断
                // && curSkillRuner.BaseSkillId == "Dragon_FlyForward"
                return false;
            }
            return true;
        }

        public override void ExitBreak()
        {
            damageState = DamageState.NoBreak;
            Debug.Log($"yns NoBreak ");
        }

       
        public void CmdOnDamAnim()
        {
            RcpOnDamAnim();
        }        
       
        public void RcpOnDamAnim()
        {
            damageState = DamageState.OnBreak;
            breakTimer.ResetTimer();
            if (curSkillRuner)
            {
                Debug.Log($"yns Break CurSkill {curSkillRuner.BaseSkillId}");
                //打断技能
                curSkillRuner.BreakSkill();
            }
            HitStop.Instance.Shake();
            playerMover.PlayAnim(AnimHash.Break);
        }

    }
}