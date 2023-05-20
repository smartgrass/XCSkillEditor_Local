using UnityEngine;

namespace XiaoCao
{

    public class SceneScript : MonoBehaviour
    {
        public static SceneScript _instance = null;
        public static SceneScript Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<SceneScript>();
                }
                return _instance;
            }
        }

        private PlayerMgr playerMgr => PlayerMgr.Instance;

        public GameMode gameMode;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            Debug.Log($"yns GameStart!");
            playerMgr.AddTruePlayer();
        }

    }
}