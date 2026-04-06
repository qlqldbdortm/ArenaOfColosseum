namespace Colosseum.LifeCycle
{
    public interface IInit<T>
    {
        public void OnInit(T data);
    }
}