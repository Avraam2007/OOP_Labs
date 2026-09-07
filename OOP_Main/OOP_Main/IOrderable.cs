using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main {
    public interface IOrderable {
        int GetDeliveryDays();
        int GetSupplierId();
    }
}
