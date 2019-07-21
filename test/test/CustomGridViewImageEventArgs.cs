using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    public class CustomGridViewImageEventArgs : EventArgs
    {
        public DataGridView DataGridView { get; set; }
    }
}
