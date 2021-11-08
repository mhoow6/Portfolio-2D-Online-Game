using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ObjectManager
{
    public Player Me { get; private set; }
    Dictionary<int, BaseObject> objects = new Dictionary<int, BaseObject>();

    public void AddMe(BaseObject player)
    {
        Me = player as Player;
        Add(Me);
    }

    public bool Add(BaseObject player)
    {
        BaseObject p = null;
        if (objects.TryGetValue(player.id, out p) == false)
        {
            p = player;

            objects.Add(player.id, p);
            return true;
        }

        return false;
    }

    public BaseObject Find(int objectId)
    {
        BaseObject obj = null;
        if (objects.TryGetValue(objectId, out obj) == true)
        {
            return obj;
        }
        return null;
    }
}
