public class Define
{
    public enum MoveDir
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        NONE = -1
    }

    public enum State
    {
        IDLE,
        MOVING,
        ATTACK,
        SKILL,
        DEAD,
        NONE = -1
    }

    public enum WeaponType
    {
        BAREHAND,
        BOW,
        NONE = -1
    }

    public enum MapId
    {
        TOWN = 1,
        DUNGEON = 2,
        NONE = -1,
    }
}
