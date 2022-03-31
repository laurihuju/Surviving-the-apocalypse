using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeItemType : UsableItem
{
    public override bool CanUse()
    {
        return true;
    }

    public override void OnCollect()
    {
        throw new System.NotImplementedException();
    }

    public override void OnDrop()
    {
        throw new System.NotImplementedException();
    }

    public override void Use()
    {
        
    }
}
