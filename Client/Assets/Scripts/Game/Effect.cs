using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Effect : BaseObject
{
    protected Animator _animator;
    public override Vector3Int CellPos { get => base.CellPos; set => base.CellPos = value; }
}

