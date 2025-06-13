using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Core.Enums.DTOEnums.Inventory
{
    public enum StockReturnType // Place in Core.Enums or similar
    {
        CustomerReturn,     // Increases stock
        ReturnToSupplier    // Decreases stock
    }
}
