using AutoFusionPro.Core.Enums.RepoEnums;

namespace AutoFusionPro.Domain.Models.Base
{
    public class DataChangedEventArgs<T> : EventArgs
    {
        public T Entity { get; set; }
        public DataChangeType ChangeType { get; set; }
    }
}
