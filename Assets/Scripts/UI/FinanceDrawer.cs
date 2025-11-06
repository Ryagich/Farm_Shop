using Inventory.Finance;
using TMPro;
using UniRx;

namespace UI
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FinanceDrawer
    {
        private readonly TMP_Text text;
        private CompositeDisposable disposables = new CompositeDisposable();
        
        public FinanceDrawer
            (
                FinanceManager financeManager,
                TMP_Text text
            )
        {
            this.text = text;
            financeManager.Value
                          .Subscribe(OnValueChanged)
                          .AddTo(disposables);
        }
        
        private void OnValueChanged(int newValue)
        {
            text.text = $"{newValue}$";
        }
    }
}