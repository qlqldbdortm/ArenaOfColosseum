namespace Colosseum.Unit
{
    public enum CharacterClass
    {
        // 편의상 0x100 단위로 클래스를 좀 잘라놓을 거임.
        None = 0,
        
        // 근접 딜러
        Warrior = 0x100,
        // 원거리 딜러 1
        Archer = 0x200,
        // 원거리 딜러 2 
        Ranger = 0x300,
        // 마법 딜러
        WizardIce = 0x400,
        WizardFire = 0x500,
        // 떡탱커
        Egoist = 0x600,
        // 근점 딜러 2
        Knight = 0x700,
        // 마법 탱커
        ArmoredMagician = 0x800,
        
        // 힐러
        Priest = 0x900,
        // 버퍼
        Bard = 0x1000,
    }
}