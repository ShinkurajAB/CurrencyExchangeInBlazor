using CurrencyExchange.Model;
using CurrencyExchange.Service.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Reflection;
using System.Text;

namespace CurrencyExchange.UI.Pages
{
    public partial class ErrorTracing
    {
        [Inject]
        ICurrencyService CurrencyService { get; set; }
        
        LogDetailsModel logDetails=new LogDetailsModel();
        ErrorMessageModel ErrorMessage=new ErrorMessageModel();


        string FileName = string.Empty;
        protected async override Task OnInitializedAsync()
        {
        
            logDetails=await CurrencyService.GetlLogDetails();

            this.StateHasChanged();
        }

        private async void SelectedFile(string file)
        {
            FileName = file;
            ErrorMessage=await CurrencyService.GetErrorMessages(file);
            this.StateHasChanged();
        }

        private async void DownloadErrorDetails()
        {
            byte[] bytes = Encoding.ASCII.GetBytes(ErrorMessage.Message);
            string serializedString = Convert.ToBase64String(bytes);
            await js.InvokeVoidAsync("DownLoadFile", "text/plain",serializedString , FileName);
        }

    }
}
