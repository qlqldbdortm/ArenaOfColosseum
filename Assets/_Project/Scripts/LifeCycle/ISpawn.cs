namespace Colosseum.LifeCycle
{
    public interface ISpawn<T>
    {
        public void OnSpawn(T data);
    }
}