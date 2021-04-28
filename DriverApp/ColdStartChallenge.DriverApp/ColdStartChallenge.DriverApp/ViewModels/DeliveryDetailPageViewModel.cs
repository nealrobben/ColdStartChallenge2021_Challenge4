using System;
using System.Threading.Tasks;
using ColdStartChallenge.DriverApp.Models;
using ColdStartChallenge.DriverApp.Navigation;
using ColdStartChallenge.DriverApp.Services;
using Xamarin.Forms;

namespace ColdStartChallenge.DriverApp.ViewModels
{
    public class DeliveryDetailPageViewModel : ViewModelBase
    {
        private readonly OrderService _orderService;
        private readonly DriverService _driverService;

        private Guid _orderId;
        private OrderStatus _orderStatus;
        private Order _order;
        private bool _isStatusVisible;

        public DeliveryDetailPageViewModel(INavigation navgiation, Guid orderId, OrderStatus orderStatus)
            : base(navgiation)
        {
            _orderService = new OrderService();
            _driverService = new DriverService();
            _orderId = orderId;
            _orderStatus = orderStatus;
        }

        public Order Order
        {
            get => _order;
            set
            {
                if (_order != value)
                {
                    _order = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsStatusVisible
        {
            get => _isStatusVisible;

            set
            {
                if (_isStatusVisible != value)
                {
                    _isStatusVisible = value;
                    RaisePropertyChanged();
                }
            }
        }

        public OrderStatus Status
        {
            get => _orderStatus;

            set
            {
                if (_orderStatus != value)
                {
                    _orderStatus = value;
                    RaisePropertyChanged();
                }
            }
        }

        protected override async Task OnNavigatedTo(NavigationMode mode)
        {
            if (mode == NavigationMode.New)
            {
                IsBusy = true;

                await LoadOrder(_orderId, _orderStatus);

                IsBusy = false;
            }
        }

        private async Task LoadOrder(Guid orderId, OrderStatus orderStatus)
        {
            Order = await _orderService.GetOrder(orderId, orderStatus);
            
            if (Order != null)
            {
                IsStatusVisible = (Order.OrderStatus == OrderStatus.Ready);
                Status = Order.OrderStatus;
            }
        }

        private async Task OnSave()
        {
            Order.OrderStatus = OrderStatus.Delivering;
            Order.Driver = AppData.Instance.User;

            await _orderService.UpdateOrder(Order);

            IsStatusVisible = false;
        }
    }
}