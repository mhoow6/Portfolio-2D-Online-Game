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

    public enum ObjectType
    {
        PLAYER,
        MONSTER,
        DEATH_EFFECT,
        NONE = -1
    }

    public enum MapId
    {
        TOWN = 1,
        NONE = -1,
    }
}
