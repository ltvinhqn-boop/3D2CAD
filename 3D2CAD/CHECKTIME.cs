using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3D2CAD
{
    class CHECKTIME
    {
        public static bool CHECKLICENSE(Double time)
        {
            bool CHECK = false;
            DateTime networkTime = NtpTime.GetNetworkTime();
            string dateTimeString = networkTime.ToString("yyyy MM dd").Replace(" ", "");
            if(time-Double.Parse(dateTimeString)>0)
            {
                CHECK = true;
            }
            return CHECK;
        }
    }
}
     