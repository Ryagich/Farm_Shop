using Inventory.Finance;
using TMPro;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace UI
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FinanceDrawer : IStartable
    {
        private readonly TMP_Text finance;
        
        private CompositeDisposable disposables = new CompositeDisposable();
        
        public FinanceDrawer
            (
                FinanceManager financeManager,
                [Key("Finance")] TMP_Text finance
            )
        {
            this.finance = finance;
            financeManager.Value
                          .Subscribe(OnValueChanged)
                          .AddTo(disposables);
        }
        
        private void OnValueChanged(int newValue)
        {
            finance.text = $"{newValue}$";
        }

        public void Start() { }
    }
}