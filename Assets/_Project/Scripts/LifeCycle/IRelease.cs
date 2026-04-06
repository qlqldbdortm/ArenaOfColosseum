namespace Colosseum.LifeCycle
{
    public interface IRelease<T>
    {
        public void OnRelease(T data);
    }
}