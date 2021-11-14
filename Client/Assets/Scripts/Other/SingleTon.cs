public class SingleTon<T> where T : class, new()
{
    private static T inst = null;

    public SingleTon() { }

    public static T Instance
    {
        get
        {
            if (inst == null)
                inst = new T();
            return inst;
        }
    }

}
