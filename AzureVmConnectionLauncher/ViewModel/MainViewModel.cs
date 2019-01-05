using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using AzureVmConnectionLauncher.Service;

namespace AzureVmConnectionLauncher.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IAzureOperationService operationService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IAzureOperationService operationService)
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            this.operationService = operationService;

            ConnectAccountCommand = new RelayCommand(ConnectAccount);
        }

        public RelayCommand ConnectAccountCommand { get; private set; }

        private void ConnectAccount()
        {
            Subscriptions = operationService.ConnectAccount();
        }

        private ObservableCollection<AzureSubscriptionViewModel> subscriptionTreeViewItems;
        public ObservableCollection<AzureSubscriptionViewModel> Subscriptions
        {
            get
            {
                return subscriptionTreeViewItems;
            }
            set
            {
                subscriptionTreeViewItems = value;
                RaisePropertyChanged(nameof(Subscriptions));
            }
        }
    }
}
