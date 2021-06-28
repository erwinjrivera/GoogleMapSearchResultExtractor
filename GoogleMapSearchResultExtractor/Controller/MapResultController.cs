using GoogleMapSearchResultExtractor.Model;
using GoogleMapSearchResultExtractor.Service;
using GoogleMapSearchResultExtractor.Utils.Logger;
using GoogleMapSearchResultExtractor.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleMapSearchResultExtractor.Controller
{
    public class MapResultController
    {
        private IMainFormView _view;
        private MapResult _model;
        private IGoogleService _service;
        private BackgroundWorker _bgWorker;
        private BackgroundWorker _bgWorkerExtract;
        private CardController _controller;
        private System.Windows.Forms.Timer _timer1;
        private Stopwatch _stopwatch;
        private int hr;
        private int min;
        private int sec;

        public MapResultController(IMainFormView view, MapResult model, IGoogleService service)
        {
            _view = view;
            _model = model;
            _service = service;
        }

        public void Initialize()
        {
            _bgWorker = new System.ComponentModel.BackgroundWorker();
            _bgWorker.WorkerReportsProgress = true;
            _bgWorker.WorkerSupportsCancellation = true;

            _bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker_DoWork);
            _bgWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorker_ProgressChanged);
            _bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker_RunWorkerCompleted);

            _bgWorkerExtract = new System.ComponentModel.BackgroundWorker();
            _bgWorkerExtract.WorkerReportsProgress = true;
            _bgWorkerExtract.WorkerSupportsCancellation = true;

            _bgWorkerExtract.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerExtract_DoWork);
            _bgWorkerExtract.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorkerExtract_ProgressChanged);
            _bgWorkerExtract.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorkerExtract_RunWorkerCompleted);

            _view.StatusText = string.Empty;

            _timer1 = new System.Windows.Forms.Timer(new System.ComponentModel.Container());
            _timer1.Interval = 1000;
            _timer1.Tick += new System.EventHandler(this.timer1_Tick);

            UpdateResultCount(0);
        }

        private List<MapResult> _cards = new List<MapResult>();
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _cards.Clear();

            int limit = 20;

            Logger.Info("Results:");
            Logger.Info("");
            
            for (int offset = 0; offset != limit; offset += 20)
            {
                Debug.WriteLine($"offset: {offset}");
                try
                {
                    var searchTerm = (string)e.Argument;
                    var result = _service.GetListOfMapResult(searchTerm, offset);

                    if (result != null && result.Count > 0)
                    {

                        foreach (var model in result)
                        {
                            if (!_bgWorker.CancellationPending)
                            {
                                Logger.Info($"<a href='{model.URL}'>{model.Title}</a>");

                                ICardView view = new CardItemUserControl();

                                _controller = new CardController(view, model);
                                _controller.Initialize();

                                Logger.Info("Extracting additional information...");

                                //Thread.Sleep(new Random().Next(5, 10) * 1000);
                                var card = _service.GetCardDetails(model.URL, model.Title);
                                card.SearchTerm = searchTerm;

                                if (card != null)
                                {
                                    _controller.UpdateModel(card);
                                }

                                _cards.Add(card);

                                Logger.Info("");

                                _bgWorker.ReportProgress(100, view);
                            }
                            else
                            {
                                Logger.Info("Cancelling search...");
                                e.Cancel = true;
                            }
                        } //for
                    }
                    else
                    {
                        Logger.Warning("No result found.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error encountered in the background worker. " + ex.Message);
                    Logger.Error($"{ex.Message} {ex.StackTrace}");
                }

                Logger.Info($"Offset: {offset}");

                //Thread.Sleep(new Random().Next(5, 10) * 1000);
            } //for
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                try
                {
                    _view.AddItemResult((ICardView)e.UserState);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Logger.Error($"{ex.Message} {ex.StackTrace}");
                }
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                
            }
            else if (e.Cancelled)
            {
                Logger.Info("Search has been cancelled.");
            }
            else
            {
                if (e.Result != null)
                {
                    List<MapResult> list = null;

                    try
                    {
                        list = (List<MapResult>)e.Result;

                        AddResultItems(list);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        Logger.Error($"{ex.Message} {ex.StackTrace}");
                    }
                    finally
                    {
                        if (list != null)
                        {
                            UpdateResultCount(list.Count);
                        }
                    }
                }
            }

            _view.Searching = false;
            _view.SearchBoxEnabled = true;
            _view.ExtractButtonEnabled = true;
            _view.ChangeButtonBackground("inactive");

            if (_stopwatch != null)
            {
                _timer1.Enabled = false;
                _timer1.Stop();

                _stopwatch.Stop();
                TimeSpan ts = _stopwatch.Elapsed;

                string result = String.Format("{0:N0} search {1} {2}",
                    _view.GetResultCount(),
                    (_view.GetResultCount() > 1 ? "results" : "result"),
                    string.Format("(elapsed time {0:00}:{1:00}:{2:00}.{2})", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds)
                    );

                _view.ResultCount = result;

                _view.StatusText = string.Empty;

                Logger.Info("Search completed.");
                Logger.Info(result);
                Logger.Info("End of search.");
            }
        }

        public void UpdateResultCount(int count)
        {
            _view.ResultCount = String.Format("{0:N0} search {1}", count, (count > 1 ? "results" : "result"));
        }

        private void AddResultItems(List<MapResult> items)
        {
            foreach (var item in items)
            {
                //_view.AddItemResult(item);
            }
        }

        public void Search(string keyword)
        {
            if (!_bgWorker.IsBusy)
            {
                if (!String.IsNullOrEmpty(keyword))
                {
                    hr = 0;
                    min = 0;
                    sec = 0;

                    _timer1.Enabled = true;
                    _timer1.Start();

                    _stopwatch = new Stopwatch();
                    _stopwatch.Start();

                    _view.Searching = true;
                    _view.SearchBoxEnabled = false;
                    _view.ExtractButtonEnabled = false;
                    _view.ChangeButtonBackground("searching");
                    _view.ClearItems();

                    _timer1.Enabled = true;

                    UpdateResultCount(0);

                    Logger.Info("Search initiated.");
                    Logger.Info("Keyphrase: " + keyword);

                    _bgWorker.RunWorkerAsync(keyword);
                }
            }
            else
            {
                _view.ChangeButtonBackground("stopping");
                _view.SearchButtonEnabled = false;

                Logger.Warning("Initiated search cancellation.");

                _bgWorker.CancelAsync();
            }
        }

        
        private void timer1_Tick(object sender, EventArgs e)
        {
            sec++;

            _view.StatusText = string.Format("{0:00}:{1:00}:{2:00}", hr, min, sec);

            if (sec == 59)
            {
                sec = 0;
                min++;
            }
            if (min == 59)
            {
                min = 0;
                hr++;
            }
        }

        public void ExtractResultToTextFile(string filename)
        {
            if (!_bgWorkerExtract.IsBusy)
            {
                _view.StatusText = "Exporting results to " + filename;

                _view.Searching = true;
                _view.SearchButtonEnabled = false;
                _view.SearchBoxEnabled = false;

                _bgWorkerExtract.RunWorkerAsync(filename);
            }
            else
            {
                //_bgWorkerExtract.CancelAsync();
            }
        }

        private void BackgroundWorkerExtract_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                string header = String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\n",
                    "Search Term",
                    "Title",
                    "Description",
                    "Rating",
                    "Reviews",
                    "Address",
                    "Hours",
                    "Phone",
                    "Website",
                    "Email"
                    );

                sb.Append(header);

                if (_cards != null && _cards.Count > 0)
                {
                    foreach (var card in _cards)
                    {
                        if (!_bgWorkerExtract.CancellationPending)
                        {
                            string row = String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\n",
                                card.SearchTerm,
                                card.Title,
                                card.Description,
                                card.Rating,
                                card.Reviews,
                                card.Address,
                                card.Hours,
                                card.Phone,
                                card.Website,
                                card.Email
                                );

                            sb.Append(row);
                        }
                        else
                        {
                            e.Cancel = true;
                        }

                        using (System.IO.StreamWriter file = new System.IO.StreamWriter((string)e.Argument))
                        {
                            file.WriteLine(sb.ToString()); 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while saving the extract file. " + ex.Message);
                Logger.Error($"{ex.Message} {ex.StackTrace}");
            }

            /*
            int itemCount = (int)e.Argument;

            for (int x = 0; x != itemCount; x++)
            {
                _bgWorkerExtract.ReportProgress(100);
                Thread.Sleep(150);
            }
            */
        }

        private void BackgroundWorkerExtract_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                
            }

            _view.AddItemResult(new CardItemUserControl());
        }

        private void BackgroundWorkerExtract_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else if (e.Cancelled)
            {

            }
            else
            {
                _view.StatusText = string.Empty;
            }

            _view.Searching = false;
            _view.SearchBoxEnabled = true;
            _view.SearchButtonEnabled = true;
        }

        public void TestFlowLayoutPanel(int itemCount)
        {
            _bgWorkerExtract.RunWorkerAsync(itemCount);
        }
    }
}
