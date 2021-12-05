using MNBXml.Entities;
using MNBXml.MnbServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

namespace MNBXml
{
    public partial class Form1 : Form
    {
        BindingList<RateData> rates = new BindingList<RateData>();
        BindingList<string> currencies = new BindingList<string>();

        public Form1()
        {
            InitializeComponent();

            /*egybe kell szervezni az alábbi függvényeket:
             * oadXml(getRates());
            dataGridView1.DataSource = rates;

            makeChart();

            Ehhez kijelölöm a 3 függvényt, jobb klikk, Qick actions and refactorings, extract method,
            elnevezem az új függvényt, 
            apply*/

            loadCurrencyXml(getCurrencies());

            RefreshData();
        }

        private void RefreshData()
        {            
            rates.Clear();

            loadXml(getRates());
            dataGridView1.DataSource = rates;
            valuta.DataSource = currencies;

            makeChart();
        }

        private void makeChart()
        {
            chartRateData.DataSource = rates;
            Series sorozatok = chartRateData.Series[0];
            sorozatok.ChartType = SeriesChartType.Line;
            sorozatok.XValueMember = "Date";
            sorozatok.YValueMembers = "Value";

            var jelmagyarazat = chartRateData.Legends[0];
            jelmagyarazat.Enabled = false;

            var diagramterulet = chartRateData.ChartAreas[0];
            diagramterulet.AxisX.MajorGrid.Enabled = false;
            diagramterulet.AxisY.IsStartedFromZero = false;
            diagramterulet.AxisY.MajorGrid.Enabled = false;

        }

        private void loadXml(string xmlstring)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlstring);

            foreach (XmlElement item in xml.DocumentElement)
            {
                RateData rateData = new RateData();
                rateData.Date = DateTime.Parse(item.GetAttribute("date"));
                XmlElement childElement = (XmlElement)item.ChildNodes[0];
                if (childElement!= null)
                {
                    rateData.Currency = childElement.GetAttribute("curr");
                    decimal unit = decimal.Parse(childElement.GetAttribute("unit"));
                    rateData.Value = decimal.Parse(childElement.InnerText);

                    if (unit != 0)
                    {
                        rateData.Value = rateData.Value / unit;
                    }

                    rates.Add(rateData);
                }

            }
        }

        private void loadCurrencyXml(string xmlstring)
        {
            currencies.Clear();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlstring);

            foreach (XmlElement item in xml.DocumentElement.ChildNodes[0])
            {
                string s = item.InnerText;
                currencies.Add(s);
            }
        }


        private string getRates()
        {
            var mnbService = new MNBArfolyamServiceSoapClient();

            GetExchangeRatesRequestBody req = new GetExchangeRatesRequestBody();

            /*Át kell alakítani az alábbi 3 sort, mivel az adatokat a form1-re rakott felületről kell vennie
             * req.currencyNames = "EUR";
            req.startDate = "2020-01-01";
            req.endDate = "2020-6-30";*/

            req.currencyNames = (string)valuta.SelectedItem;
            req.startDate = tolPicker.Value.ToString("yyyy-MM-dd");
            req.endDate = igPicker.Value.ToString("yyyy-MM-dd");

            var response = mnbService.GetExchangeRates(req);
            //var result = response.GetExchangeRatesResult;
            return response.GetExchangeRatesResult;

            //File.WriteAllText("text.xml", result);

        }

        private string getCurrencies()
        {
            var mnbService = new MNBArfolyamServiceSoapClient();
            GetCurrenciesRequestBody req = new GetCurrenciesRequestBody();
            var response1 = mnbService.GetCurrencies(req);

            string result = response1.GetCurrenciesResult;

            File.WriteAllText("currency.xml", result);
            
            return response1.GetCurrenciesResult;
        }
        

        private void paraChanged(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}
