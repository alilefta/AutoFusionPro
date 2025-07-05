using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Core.SharedDTOs.ComboboxFillDto
{
    /// <summary>
    /// Used to convert nullable boolean property into 3 states combobox items
    /// </summary>
    /// <param name="title"></param>
    /// <param name="value"></param>
    public record ComboboxSelectableItemDto(string title, bool? value);

}
