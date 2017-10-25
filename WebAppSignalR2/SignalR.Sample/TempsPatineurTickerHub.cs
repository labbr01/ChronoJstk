using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;

namespace WebAppSignalR2.SignalR.Sample
{
    [HubName("tempsPatineurTicker")]
    public class TempsPatineurTickerHub : Hub // est à StockTickerHub         
    {
        private readonly TempsPatineurTicker _tempsPatineurTicker;

        public TempsPatineurTickerHub() :
                this(TempsPatineurTicker.Instance)
            {

        }

        public TempsPatineurTickerHub(TempsPatineurTicker stockTicker)
        {
            _tempsPatineurTicker = stockTicker;
        }

        public IEnumerable<TempsPatineur> GetObtenirToutPatineur()
        {
            return _tempsPatineurTicker.GetObtenirToutPatineur();
        }

        public string GetMarketState()
        {
            return _stockTicker.MarketState.ToString();
        }

        public void OpenMarket()
        {
            _stockTicker.OpenMarket();
        }

        public void CloseMarket()
        {
            _stockTicker.CloseMarket();
        }

        public void Reset()
        {
            _stockTicker.Reset();
        }
    }
}
