using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TenisMatch.Class
{
    public static class ExtensionMethods

    {
        /// <summary>
        /// Metodo de extension para obtener el atributo Description de las enumeraciones
        /// Es utilizado cuando se sobreescribe el ToString() de la clase Punto para mostrar el valor de la enum.
        /// </summary>
        /// <param name="en"></param>
        /// <returns>El valor de description attribute</returns>
        public static string ToDescription(this Enum en) //ext method
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {

                object[] attrs = memInfo[0].GetCustomAttributes(  typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;

            }

            return en.ToString();
        }

    

        
    }
}
