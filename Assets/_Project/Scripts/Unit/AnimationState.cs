namespace Colosseum.Unit
{
    public enum AnimationState
    {
        None = 0,

        /// <summary>
        /// 10 ~ 15번까지 전사 공격 애니메이션
        /// </summary>
        WarriorAttack1 = 10,
        WarriorAttack2,
        WarriorAttack3,
        WarriorAttack4,
        WarriorShield = 19,


        /// <summary>
        /// 20 ~ 25번까지 궁수 공격 애니메이션
        /// </summary>
        ArcherAttack1 = 20,
        ArcherAttack2,
        ArcherAttack3,
        ArcherAttack4,
        ArcherShield = 29,

        ///<summary>
        /// 30 ~ 35번까지 마법사 공격 애니메이션
        /// </summary>
        WizardAttack1 = 30,
        WizardAttack2,
        WizardAttack3,
        WizardAttack4,
        WizardSpecialAttack,
        WizardShield = 39,

        Dead = 90,
    }
}