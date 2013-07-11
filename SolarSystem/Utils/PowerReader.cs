using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SolarSystem.Log;

namespace SolarSystem.Utils
{
    class PowerReader
    {
        public delegate void PowerInputEvent(PowerChangedEventArgs args);
        public event PowerInputEvent PowerChanged;
        public event EventHandler ReadingCancelled;

        private string _currentIP;
        private readonly IPFinder _ipFinder;

        private readonly Logger _logger;

        public PowerReader()
        {
            _logger = Logger.Instance.Value;

            _ipFinder = new IPFinder();
            _ipFinder.IPFound += finder_IPFound;
            _ipFinder.FindIP();
        }

        private void finder_IPFound(IPFoundEventArgs args)
        {
            _currentIP = "http://" + args.IP + "/home.htm";
        }

        public void ReadPower()
        {
            try {
                var doc = new HtmlWeb().Load(_currentIP);
                if (doc == null) {
                    _logger.Log("The page was not found!", LogType.ERROR);
                    _ipFinder.FindIP();
                    ReadingCancelled(this, new EventArgs());
                    return;
                }

                var element = doc.GetElementbyId("Power");
                if (element == null) {
                    _logger.Log("The \"Power\" element was not found!", LogType.ERROR);
                    ReadingCancelled(this, new EventArgs());
                    return;
                }

                var power = element.InnerHtml;
                if (String.IsNullOrEmpty(power)) {
                    _logger.Log("Power value is empty!", LogType.ERROR);
                    ReadingCancelled(this, new EventArgs());
                    return;
                }

                power = power.Trim();
                if (power.ToLowerInvariant().Contains("kw")) {
                    power = Regex.Replace(power, @" .*", "");
                    power = Regex.Replace(power, @"\.", ",");

                    float kwPower;
                    if (float.TryParse(power, out kwPower)) {
                        kwPower *= 1000;
                        PowerChanged(new PowerChangedEventArgs((int) kwPower));
                    } else {
                        Logger.Instance.Value.Log("Cannot convert the power to float - " + power, LogType.ERROR);
                    }
                } else {
                    power = Regex.Replace(power, @"[^\d]", "");

                    int intPower;
                    if (int.TryParse(power, out intPower)) {
                        PowerChanged(new PowerChangedEventArgs(intPower));
                    } else {
                        Logger.Instance.Value.Log("Cannot convert the power to int - " + power, LogType.ERROR);
                    }
                }   
            } catch (Exception e) {
                _logger.Log("Exception occured: " + e.Message + Environment.NewLine + e.StackTrace, LogType.ERROR);
                _ipFinder.FindIP();
                ReadingCancelled(this, new EventArgs());
            }
        }
    }
}
