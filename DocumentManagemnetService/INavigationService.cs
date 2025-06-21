using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagementService
{
    public interface INavigationService
    {
        void Navigate(string pageKey);
    }
}
