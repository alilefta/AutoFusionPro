namespace AutoFusionPro.Application.Interfaces.Dialogs
{
    public interface IDialogViewModelWithResult<TResult> : IDialogAware // Assuming IDialogAware is still needed
    {
        TResult? GetResult();
    }
}
