using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class Projectile : BaseObject
    {
        protected Creature _owner;
        public virtual void V_SetOwner(Creature owner) { _owner = owner; }
    }
}
