
using Assets.Scripts.Enemy;
using UnityEngine;

namespace XiaoCao
{
    public struct AckInfo
    {
        public uint netId;
        public bool isBreak;

        public float ackValue; //�˺���ֵ
        public string baseSkillId; //������Id
        public string ackId; //�ӹ�����id �����˺���ֵ

        public Vector3 skillPos;  //��������
        public Vector3 skillDir;  //���ܳ���
        public float angleY; //����������  0��ʾ��,180�൱������
        internal DamageState lastState;
    }


    public interface DamageInfo
    {


    }

    public interface IAttacker : IDamage
    {
        public float Ack { get; }
    }

    public interface IMessager
    {
        public bool SetAIEvent(ActMsgType name, string msg,object other = null);
    }

    public interface IDamage
    {
        public GameObject SelfObject { get; set; }

        public int Hp { get; set; }
        public int MaxHp { get; set; }
        public bool IsDie { get; }
        //public AgentTag AgentTag { get; set; }
        public void OnDam(AckInfo ackInfo = default);

    }

}
