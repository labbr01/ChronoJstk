using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace WebAppSignalR2.SignalR.Sample
{
    public class TempsPatineurTicker // est à stockTicker
    {
        // Singleton instance
        private readonly static Lazy<TempsPatineurTicker> _instance = new Lazy<TempsPatineurTicker>(
            () => new TempsPatineurTicker(GlobalHost.ConnectionManager.GetHubContext<TempsPatineurTickerHub>().Patineurs));

        private readonly object _marketStateLock = new object();
        private readonly object _updateStockPricesLock = new object();

        private readonly ConcurrentDictionary<string, TempsPatineur> _tempsPatineur = new ConcurrentDictionary<string, TempsPatineur>();

        // Stock can go up or down by a percentage of this factor on each change
        private readonly double _rangePercent = 0.002;

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
        private readonly Random _updateOrNotRandom = new Random();

        private Timer _timer;
        private volatile bool _updatingStockPrices;
        //private volatile MarketState _marketState;

        private TempsPatineurTicker(IHubConnectionContext<dynamic> patineurs)
        {
            PatineursVague = patineurs;
            //LoadDefaultStocks();
        }

        public static TempsPatineurTicker Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext<dynamic> PatineursVague
        {
            get;
            set;
        }

        //public MarketState MarketState
        //{
        //    get { return _marketState; }
        //    private set { _marketState = value; }
        //}

        public IEnumerable<TempsPatineur> GetObtenirToutPatineur()
        {
            return _tempsPatineur.Values;
        }

        //public void OpenMarket()
        //{
        //    lock (_marketStateLock)
        //    {
        //        if (MarketState != MarketState.Open)
        //        {
        //            _timer = new Timer(UpdateStockPrices, null, _updateInterval, _updateInterval);

        //            MarketState = MarketState.Open;

        //            BroadcastMarketStateChange(MarketState.Open);
        //        }
        //    }
        //}

        //public void CloseMarket()
        //{
        //    lock (_marketStateLock)
        //    {
        //        if (MarketState == MarketState.Open)
        //        {
        //            if (_timer != null)
        //            {
        //                _timer.Dispose();
        //            }

        //            MarketState = MarketState.Closed;

        //            BroadcastMarketStateChange(MarketState.Closed);
        //        }
        //    }
        //}

        //public void Reset()
        //{
        //    lock (_marketStateLock)
        //    {
        //        if (MarketState != MarketState.Closed)
        //        {
        //            throw new InvalidOperationException("Market must be closed before it can be reset.");
        //        }

        //        LoadDefaultStocks();
        //        BroadcastMarketReset();
        //    }
        //}

        //private void LoadDefaultStocks()
        //{
        //    _stocks.Clear();

        //    var stocks = new List<Stock>
        //    {
        //        new Stock { Symbol = "MSFT", Price = 41.68m },
        //        new Stock { Symbol = "AAPL", Price = 92.08m },
        //        new Stock { Symbol = "GOOG", Price = 543.01m }
        //    };

        //    stocks.ForEach(stock => _stocks.TryAdd(stock.Symbol, stock));
        //}

        //private void UpdateStockPrices(object state)
        //{
        //    // This function must be re-entrant as it's running as a timer interval handler
        //    lock (_updateStockPricesLock)
        //    {
        //        if (!_updatingStockPrices)
        //        {
        //            _updatingStockPrices = true;

        //            foreach (var stock in _stocks.Values)
        //            {
        //                if (TryUpdateStockPrice(stock))
        //                {
        //                    BroadcastStockPrice(stock);
        //                }
        //            }

        //            _updatingStockPrices = false;
        //        }
        //    }
        //}

        //private bool TryUpdateStockPrice(Stock stock)
        //{
        //    // Randomly choose whether to udpate this stock or not
        //    var r = _updateOrNotRandom.NextDouble();
        //    if (r > 0.1)
        //    {
        //        return false;
        //    }

        //    // Update the stock price by a random factor of the range percent
        //    var random = new Random((int)Math.Floor(stock.Price));
        //    var percentChange = random.NextDouble() * _rangePercent;
        //    var pos = random.NextDouble() > 0.51;
        //    var change = Math.Round(stock.Price * (decimal)percentChange, 2);
        //    change = pos ? change : -change;

        //    stock.Price += change;
        //    return true;
        //}

        //private void BroadcastMarketStateChange(MarketState marketState)
        //{
        //    switch (marketState)
        //    {
        //        case MarketState.Open:
        //            Clients.All.marketOpened();
        //            break;
        //        case MarketState.Closed:
        //            Clients.All.marketClosed();
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //private void BroadcastMarketReset()
        //{
        //    Clients.All.marketReset();
        //}

        //private void BroadcastStockPrice(Stock stock)
        //{
        //    Clients.All.updateStockPrice(stock);
        //}
    }
}