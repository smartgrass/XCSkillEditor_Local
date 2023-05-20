using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using XiaoCao;

public class SkillEditorScene : MonoBehaviour
{
    [Header("Ìí¼ÓÒ»¸öNpc")]
    public bool AddOnStart = true;

    public AgentModelType agentName;

    public Vector3 startPos = Vector3.zero;

    private void Start()
    {
        if (AddOnStart)
        {
        }
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void AddNpc()
    {
        var setting = ResFinder.SoUsingFinder.DebugSo;
        PlayerMgr.Instance.AddFakePlayer(startPos, setting.AI,setting.addPlayerTag,agentName);
    }


}
