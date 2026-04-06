using System.Collections;
using System.Collections.Generic;
using Colosseum.UI.Audio;
using Photon.Pun;
using UnityEngine;

namespace Colosseum.Unit
{
    public partial class PlayerHandler
    {
        private void OnEmote(int emoteId)
        {
            photonView.RPC("RpcShowEmote", RpcTarget.All, emoteId);
        }
        [PunRPC]
        private void RpcShowEmote(int emoteId)
        {
            InGameAudioManager.PlaySfx(SfxType.Emote);
            unit?.OnEmotion(emoteId);
        }
    }
}
