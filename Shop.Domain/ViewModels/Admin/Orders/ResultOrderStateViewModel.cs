using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Admin.Orders
{
    public class ResultOrderStateViewModel
    {
        public int RequestCount { get; set; }
        public int ProcessingCount { get; set; }
        public int SentCount { get; set; }
        public int CancelCount { get; set; }
    }

}
