using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AramisIDE.Utils;

namespace AramisIDE.Actions
    {
    class CurrentDayPeriod : BaseAction
        {
        public CurrentDayPeriod()
            {
            var str = string.Format(@"declare @StartDate datetime2 = '{0}';
declare @FinishDate datetime2 = '{0} 23:59:59.999';
--	.[Date] between @StartDate and @FinishDate
", DateTime.Now.ToString("yyyy-MM-dd"));
            ToClipboard(str);
            }
        }
    }
