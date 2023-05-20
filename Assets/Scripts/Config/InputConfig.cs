
using System;
using System.Collections.Generic;
using UnityEngine;

namespace XiaoCao
{
    public static class InputConfig
    {



    }
    public static class PlayEventMsg
    {
        public static string SetCanMove = "SetCanMove"; //�ƶ�����
        public static string SetCanRotate = "SetCanRotate"; //��ת����
        public static string SetUnMoveTime = "SetUnMoveTime"; //���ò��ܶ���ʱ��
        public static string ActivePlayerRender = "ActivePlayerRender"; //�������Mesh
        public static string TimeStop = "TimeStop"; //��֡
        public static string SetNoGravityT = "SetNoGravityT"; //��������
        public static string SetNoBreakTime = "SetNoBreakTime"; //���忪��
        public static string PlayAudio = "PlayAudio"; //���忪��

    }



    public enum ClientEventType
    {
        Start,
        Stop,
        Change,
        ValueChange,
    }


    public enum PlayerStateEnum
    {
        Idle,
        NorAck,
        PlayerSkill,
        //Jump,
        //Roll,
        Dead
    }


    public class PlayerInputSetting
    {
        public Dictionary<KeyCode, string> SkillKeyMsg = new Dictionary<KeyCode, string>();

        public KeyCode NorAck = KeyCode.Mouse0;

        public KeyCode Roll = KeyCode.LeftShift;

        public KeyCode Jump = KeyCode.Space;

        public float NorAckTime = 1.6f;
    }
    //ͨ��������Ϣ
    public struct PlayerMessge
    {
        public DataType dataType;
        public int intMes;
        public float floatMes;
        public string strMes;
        public object objMes;

        public int IntMes { get => intMes; set { intMes = value; dataType = DataType.Int; } }
        public float FloatMes { get => floatMes; set { floatMes = value; dataType = DataType.Float; } }
        public string StrMes { get => strMes; set { strMes = value; dataType = DataType.String; } }
        public object ObjMes { get => objMes; set { objMes = value; dataType = DataType.Object; } }

        public override string ToString()
        {
            switch (dataType)
            {
                case DataType.Int:
                    return intMes.ToString();
                case DataType.Float:
                    return floatMes.ToString();
                case DataType.String:
                    return strMes;
                default:
                    break;
            }
            return base.ToString();
        }

    }

    public enum DataType : int
    {
        Null,
        Int,
        Float,
        String,
        Object
    }

    //�������� ������ܹ���һ��
    //Owner�Ǽ��ܵ�����/���ڵ� ������player,��������Ч 
    public class SkillOwner
    {
        public uint netId;
        //trigger
        public int triggerLayer;

        public bool enableAck; // ��ʵ����Ȩ�� �൱��local

        public bool isCustomObject; //����������

        public MonoAttacker attacker;

        public Transform AckerTF => attacker.transform;

        //event��ʵ�����, ������Ч
        public Transform eventOwnerTF;

        public SkillOwner() { }

        public SkillOwner(MonoAttacker monoAttacker)
        {
            this.attacker = monoAttacker;
            this.netId = monoAttacker.netId;
            this.enableAck = monoAttacker.isEnableAck;
            this.eventOwnerTF = monoAttacker.transform;
            this.triggerLayer = GameSetting.GetAckLayer(monoAttacker.AgentTag);
        }

        public static SkillOwner CopyNew(SkillOwner owner)
        {
            SkillOwner newOwner = new SkillOwner()
            {
                enableAck = owner.enableAck,
                netId = owner.netId,
                triggerLayer = owner.triggerLayer,
                attacker = owner.attacker,
            };
            return newOwner;
        }
    }

    public enum PlayerNetEventName : int
    {
        Null,
        Invoke,
        String,
        UpDateHp,

    }
}
