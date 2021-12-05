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

        public Form1()
        {
            InitializeComponent();
            
            loadXml(getRates());
            dataGridView1.DataSource = rates;

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
                var childElement = (XmlElement)item.ChildNodes[0];
                rateData.Currency = childElement.GetAttribute("curr");
                decimal unit = decimal.Parse(childElement.GetAttribute("unit"));
                rateData.Value = decimal.Parse(childElement.InnerText);

                if (unit !=0)
                {
                    rateData.Value = rateData.Value / unit;
                }

                rates.Add(rateData);

            }
        }




        private string getRates()
        {
            var mnbService = new MNBArfolyamServiceSoapClient();

            GetExchangeRatesRequestBody req = new GetExchangeRatesRequestBody();

            req.currencyNames = "EUR";
            req.startDate = "2020-01-01";
            req.endDate = "2020-6-30";
            var response = mnbService.GetExchangeRates(req);
            //var result = response.GetExchangeRatesResult;
            return response.GetExchangeRatesResult;

            //File.WriteAllText("text.xml", result);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
