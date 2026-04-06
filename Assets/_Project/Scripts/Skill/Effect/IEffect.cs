namespace Colosseum.Skill.Effect
{
    public interface IEffect
    {
        public void OnEffect(Unit.Unit unit);
        public void SetInfluence(float influence);
    }
}