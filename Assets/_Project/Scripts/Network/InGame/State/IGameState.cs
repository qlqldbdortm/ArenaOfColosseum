namespace Colosseum.Network.InGame.State
{
    public interface IGameState
    {
        public void OnInit(InGameManager manager);
        
        public void OnEnter();
        public void OnExit();
    }
}