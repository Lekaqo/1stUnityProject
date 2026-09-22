using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Item;
using UnityEngine;

public class AttachmentItem : BaseItem
{
    public enum AttachmentType
    {
        Scope,
        Other
    }

    public AttachmentType CurrentAttachmentType;
}
