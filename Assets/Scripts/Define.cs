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
        NONE = -1
    }

    public enum MapId
    {
        TOWN = 1,
        NONE = -1,
    }
}
