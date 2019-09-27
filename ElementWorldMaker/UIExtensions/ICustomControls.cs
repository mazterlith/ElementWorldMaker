using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ElementWorldMaker.UIExtensions
{
    public interface ICustomControls
    {
        Control[] ExtraControls { get; }
    }
}
