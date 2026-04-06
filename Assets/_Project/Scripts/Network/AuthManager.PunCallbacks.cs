using Colosseum.UI;
using Photon.Realtime;

namespace Colosseum.Network
{
    public partial class AuthManager
    {
        public override void OnEnable()
        {
            base.OnEnable();
        }
        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void OnConnectedToMaster()
        {
            // Photon 접속 성공
            connectedPhoton = true;
            OnConnectMessage?.Invoke($"서버 연결 성공");
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            connectedPhoton = false;
            OnConnectMessage?.Invoke($"서버 연결 실패");
            DialogMessage.ShowMessage("서버 연결 실패", cause.ToString());
        }
    }
}