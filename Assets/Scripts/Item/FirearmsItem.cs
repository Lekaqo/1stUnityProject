using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Item
{
    public class FirearmsItem : BaseItem
    {
        public enum FirearmsType
        {
            AssultRefile,
            HandGun,
        }

        public FirearmsType CurrentFirearmsType;
        public String ArmsName;
    }
}
