using CurrencyExchange.Model;
using CurrencyExchange.Service;
using CurrencyExchange.Service.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CurrencyExchange.UI.Pages
{
    public partial class Index
    {
        [Inject]
        ICurrencyService currencyservice { get; set; }
         


        List<CurrencyModel> currencylist = new List<CurrencyModel>();

        List<ExchangeRateModel> ExchangeRate = new List<ExchangeRateModel>();

        decimal? LastSevenDaysGrowth;

        decimal? TodayCurrencyRate;

        protected async override Task OnInitializedAsync()
        {

            currencylist = await currencyservice.GetAllCurrency();
            this.StateHasChanged();
        }

        #region DropDown Currency Changing Event
        private async void CurrencySelectionChange(ChangeEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Value.ToString()))
            {
                // Get Currency Exchange Rate
                string Currency = args.Value.ToString();

                // Call API and Get Last 7 days Exchange  Rate by Currency
                ExchangeRate = await currencyservice.GetExchangeRateByCurrencyName(Currency);
                ExchangeRate = ExchangeRate.OrderBy(x => x.CurrencyDate).ToList();


                // find out the last 7 Days Growth 
                var minimumDate = ExchangeRate.Min(x => x.CurrencyDate);
                var MaximumDate = ExchangeRate.Max(x => x.CurrencyDate);

                decimal? todayRate=ExchangeRate.First(x=> x.CurrencyDate==MaximumDate).CurrencyRate;
                decimal? LastseventhDayRate=ExchangeRate.First(x=> x.CurrencyDate== minimumDate).CurrencyRate;
                decimal? diffrence = todayRate - LastseventhDayRate;
                if (diffrence.HasValue)
                {
                    LastSevenDaysGrowth = (diffrence / todayRate) * 100;
                }

                // Today Currency rate
                TodayCurrencyRate = ExchangeRate.FirstOrDefault(x => x.CurrencyDate == MaximumDate).CurrencyRate;

                // Create the Chart
                List<string?> labels = ExchangeRate.Select(x => x.CurrencyDate.ToString("yyyy-MM-dd")).ToList();
                List<decimal?> rates = ExchangeRate.Select(x => x.CurrencyRate).ToList();
                await js.InvokeVoidAsync("CreateChart", labels, rates, Currency);
                this.StateHasChanged();
            }
        }
        #endregion
    }

}

