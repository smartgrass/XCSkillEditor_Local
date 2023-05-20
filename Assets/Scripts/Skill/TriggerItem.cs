using System;
using System.Collections.Generic;
using UnityEngine;

namespace XiaoCao
{
    public class TriggerItem : MonoBehaviour
    {
        public AckInfoObject ackInfoObject = new AckInfoObject();

        public MonoAttacker attacker;

        private void OnTriggerEnter(Collider other)
        {
            if (attacker.isEnableAck)
            {
                ackInfoObject.skillPos = other.ClosestPointOnBounds(transform.position);
                ackInfoObject.skillDir = transform.forward;
                attacker.OnAckTrigger(other, ackInfoObject.ToAckInfo());
            }
            else
            {
                Debug.LogError("yns ?? has Trigger?");
            }
        }


    }

    //������ �����ڴ����
    public class AckInfoObject
    {
        public uint netId;
        public float ackValue; //�˺���ֵ
        public string baseSkillId; //������Id
        public string ackId; //�ӹ�����id �����˺���ֵ

        public Vector3 skillPos;  //��������
        public Vector3 skillDir;  //���ܳ���
        public float angleY; //����������  0��ʾ��,180�൱������

        public AckInfo ToAckInfo() 
        { 
            return new AckInfo()
            {
                netId = netId,
                ackId = ackId,
                baseSkillId = baseSkillId,
                angleY = angleY,
                skillPos= skillPos,
                skillDir= skillDir
            }; 
        }
    }

}
