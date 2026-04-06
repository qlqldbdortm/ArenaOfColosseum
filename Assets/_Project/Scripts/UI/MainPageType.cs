namespace Colosseum.UI
{
    public enum MainPageType
    {
        None = 0,
        /// <summary>
        /// 게임 Connect 페이지
        /// </summary>
        Loading,
        /// <summary>
        /// 로그인, 회원가입, 닉네임 등록이 존재하는 페이지
        /// </summary>
        Auth,
        /// <summary>
        /// 룸을 들어가기 전에 화면
        /// </summary>
        Lobby,
        /// <summary>
        /// 플레이를 기다리는 룸 페이지
        /// </summary>
        Room,
    }
}