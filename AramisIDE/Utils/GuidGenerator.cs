using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AramisIDE.Utils
    {
    class GuidGenerator
        {
        public GuidGenerator(char firstCharOfGuid)
            {
            NewGuid = createGuid(firstCharOfGuid);
            }

        public string NewGuid { get; private set; }

        private string createGuid(char firstCharOfGuid)
            {
            while (true)
                {
                var guid = Guid.NewGuid().ToString().ToUpper();
                if (guid[0].Equals(firstCharOfGuid))
                    {
                    return guid;
                    }
                }
            }

        public override string ToString()
            {
            return NewGuid;
            }
        }
    }
