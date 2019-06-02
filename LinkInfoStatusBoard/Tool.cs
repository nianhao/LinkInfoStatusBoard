using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkInfoStatusBoard
{
    class Tool
    {
        internal static Hashtable randSelectIndex(int all,int selectNum)
        {
            Hashtable hashtablSelected = new Hashtable();
            Random rm = new Random();
            for (int i = 0; hashtablSelected.Count < selectNum; i++)
            {
                int nValue = rm.Next(all);
                if (!hashtablSelected.ContainsValue(nValue))
                {
                    hashtablSelected.Add(nValue, nValue);
                    Trace.WriteLine("选择了 " + nValue.ToString() + "位置显示");
                }
            }
            return hashtablSelected;
        }

      
    }
}
