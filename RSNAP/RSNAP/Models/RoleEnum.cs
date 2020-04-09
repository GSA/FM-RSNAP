using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RSNAP.Models
{
    public enum RoleEnum
    {
        [Description("RSNAP FUNDING OFFICER")]
        FO = 1,
        [Description("RSNAP CONTRACTING OFFICER")]
        CO = 2,
        [Description("RSNAP READ ONLY")]
        RO = 3

    }

    public static class EnumExtension
    {

        public static string GetDescription(this Enum em)
        {
            Type type = em.GetType();
            FieldInfo fd = type.GetField(em.ToString());
            if (fd == null)
                return string.Empty;
            object[] attrs = fd.GetCustomAttributes(typeof(DescriptionAttribute), false);
            string name = string.Empty;
            foreach (DescriptionAttribute attr in attrs)
            {
                name = attr.Description;
            }
            return name;
        }

    }
}
