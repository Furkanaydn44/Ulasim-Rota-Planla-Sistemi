using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms;
using GMap.NET.MapProviders;
using GMap.NET;
using WinFormsClass;
using Newtonsoft.Json;
using WinLogHelper;

namespace WinFormsApp1
{
    public partial class Form2 : Form
    {
        Genel genel = new Genel();
        Arac otobus = new Otobus();
        Arac tramvay = new Tramvay();
        Arac taksi = new Taxi();

        private GMapOverlay routeOverlay = null;

        List<string> tramRoute;
        List<string> busRoute;
        List<string> taxiRoute;

        TransportData data;

        double smap_lat; double rmap_lat;
        double smap_lon; double rmap_lon;

        public Form2(double start_lat, double start_lon, double route_lat, double route_lon, double bakiye
            , string yolcu, string odeme)
        {
            InitializeComponent();
            string jsonFilePath = Path.Combine(Application.StartupPath, "veriler", "locations.json");

            if (!File.Exists(jsonFilePath))
            {

                MessageBox.Show("Hata: JSON dosyası bulunamadı!");
            }

            string jsonContent = File.ReadAllText(jsonFilePath);


            data = JsonConvert.DeserializeObject<TransportData>(jsonContent);

            smap_lat = start_lat; smap_lon = start_lon; rmap_lon = route_lon; rmap_lat = route_lat;


            label1.Text = $"Başlangıç meridyeni ve paraleli:\n {start_lat}, {start_lon}";
            label2.Text = $"Hedef meridyeni ve paraleli:\n {route_lat}, {route_lon}";
            label15.Text = $"Bakiye :{bakiye}";
            label16.Text = $"Yolcu :{yolcu}";
            label17.Text = $"Odeme :{odeme}";

            HaritayiBaslat();
            MarkerEkle(data);

            TekilMarkerEkle(true, start_lat, start_lon);
            TekilMarkerEkle(false, route_lat, route_lon);

            Yolcu yolcu1;

            switch (yolcu)
            {
                case "Genel":
                    yolcu1 = new Genel();
                    break;
                case "Yaşlı":
                    yolcu1 = new Yasli();
                    break;
                case "Öğrenci":
                    yolcu1 = new Ogrenci();
                    break;
                default:
                    yolcu1 = null;
                    break;
            }

            Odeme odeme1;
            switch (odeme)
            {
                case "Nakit":
                    odeme1 = new Nakit();
                    odeme1.bakiye = bakiye;
                    break;
                case "Kredi Kartı":
                    odeme1 = new KrediKartı();
                    odeme1.bakiye = bakiye;
                    break;
                case "Kentkart":
                    odeme1 = new KentKart();
                    odeme1.bakiye = bakiye;
                    break;
                default:
                    odeme1 = null;
                    odeme1.bakiye = bakiye;
                    break;
            }
            genel.BaslangıcLat = start_lat;
            genel.BaslangıcLon = start_lon;
            genel.BitisLat = route_lat;
            genel.BitisLon = route_lon;

            List<Durak> busStops = data.Duraklar.Where(d => d.Type == "bus").ToList();
            Durak baslangicDuragiBus = EnYakinDurak(busStops, genel.BaslangıcLat, genel.BaslangıcLon);
            Durak hedefDuragiBus = EnYakinDurak(busStops, genel.BitisLat, genel.BitisLon);

            Durak baslangicDuragiTram = EnYakinDurak(data.Duraklar.Where(d => d.Type == "tram").ToList(),
                                                  genel.BaslangıcLat, genel.BaslangıcLon);
            Durak hedefDuragiAny = EnYakinDurak(data.Duraklar, genel.BitisLat, genel.BitisLon);

            double mesafeIlkDurak = DistanceCalculator.Haversine(genel.BaslangıcLat, genel.BaslangıcLon,
                                                              baslangicDuragiBus.Lat, baslangicDuragiBus.Lon);
            double mesafeSonDurak = DistanceCalculator.Haversine(genel.BitisLat, genel.BitisLon,
                                                              hedefDuragiBus.Lat, hedefDuragiBus.Lon);

            double yurunenSure = (int)(mesafeIlkDurak * 15);
            double yurunenSonSure = (int)(mesafeSonDurak * 15);

            double taksiSure = (int)(mesafeIlkDurak * 1.5);
            double taksiSonSure = (int)(mesafeSonDurak * 1.5);

            double taksiFB = 0;
            double taksiLB = 0;
            double taksisüreB = 0;

            busRoute = otobus.CalculateRoute(baslangicDuragiBus, hedefDuragiBus, busStops, data);

            if (mesafeIlkDurak > 0)
            {
                if (mesafeIlkDurak < 2)
                {
                    LogHelper.Ekle("-> Yürüme" + yurunenSure + "dk");
                    taksisüreB += yurunenSure;
                }

                else
                {
                    LogHelper.Ekle("-> Taksi" + taksiSure + "dk");
                    taksiFB = taksiSure > 0 ? data.Taxi.OpeningFee + data.Taxi.CostPerKm * taksiSure : 0;
                    taksisüreB += taksiSure;
                }
            }

            otobus.DisplayRoute(busRoute, data.Duraklar.ToDictionary(d => d.Id), data);

            if (mesafeSonDurak > 0)
            {
                if (mesafeSonDurak < 2)
                { 
                    LogHelper.Ekle("-> Yürüme " + yurunenSonSure + "dk");
                    taksisüreB += yurunenSonSure;
                }
            else
            {
                LogHelper.Ekle("-> Taksi" + taksiSonSure + "dk");
                taksiLB = taksiSonSure > 0 ? data.Taxi.OpeningFee + data.Taxi.CostPerKm * taksiSonSure : 0;
                    taksisüreB += taksiSonSure;
                }
            }

            otobus.DisplayFinalInfo(busRoute, data.Duraklar.ToDictionary(d => d.Id), yolcu1, odeme1, data);


            double kalanBakiyeB = bakiye - (otobus.toplamUcret + taksiFB + taksiLB);
            LogHelper.Ekle($"Kalan Bakiye: {kalanBakiyeB:F2} TL");

            label12.Text = LogHelper.LogBuilder.ToString();

            label3.Text = $"{otobus.toplamSure + taksisüreB}dk";
            label4.Text = $"{otobus.toplamUcret + taksiFB + taksiLB} TL";
            label5.Text = $"{(otobus.toplamMesafe + mesafeIlkDurak + mesafeSonDurak):0.0} km";

            LogHelper.LogBuilder.Clear();


            tramRoute = tramvay.CalculateRoute(baslangicDuragiTram, hedefDuragiAny, data.Duraklar, data);

            double mesafeIlkDurakT = DistanceCalculator.Haversine(genel.BaslangıcLat, genel.BaslangıcLon,
                                                               baslangicDuragiTram.Lat, baslangicDuragiTram.Lon);

            double mesafeSonDurakT = DistanceCalculator.Haversine(genel.BitisLat, genel.BitisLon,
                                                               hedefDuragiAny.Lat, hedefDuragiAny.Lon);

            double yurunenSureT = (int)(mesafeIlkDurakT * 15);
            double yurunenSonSureT = (int)(mesafeSonDurakT * 15);

            double taksiSureT = (int)(mesafeIlkDurakT * 1.5);
            double taksiSonSureT = (int)(mesafeSonDurakT * 1.5);

            double taksiIlkUcret = 0;
            double taksiSonUcret = 0;
            double taksisüreT = 0;

            if (mesafeIlkDurakT > 2) {
                LogHelper.Ekle("-> Taksi " + taksiSureT + "dk");
                taksiIlkUcret = taksiSureT > 0 ? data.Taxi.OpeningFee + data.Taxi.CostPerKm * taksiSureT : 0;
                taksisüreT += taksiSonSureT;
            }
            else if (mesafeIlkDurakT > 0) {
                taksisüreT += yurunenSureT;
                LogHelper.Ekle("-> Yürüme " + yurunenSureT + "dk");
            }

            tramvay.DisplayRoute(tramRoute, data.Duraklar.ToDictionary(d => d.Id), data);


            if (mesafeSonDurakT > 2) {
                LogHelper.Ekle("-> Taksi" + taksiSonSureT + "dk");
                taksiSonUcret = taksiSonSureT > 0 ? data.Taxi.OpeningFee + data.Taxi.CostPerKm * taksiSonSureT : 0;
                taksisüreT += taksiSonSureT;
            }

            else if (mesafeSonDurakT > 0)
            { 
                LogHelper.Ekle("-> Yürüme " + yurunenSonSureT + "dk");
                taksisüreT += yurunenSonSureT;
            }       


            tramvay.DisplayFinalInfo(tramRoute, data.Duraklar.ToDictionary(d => d.Id), yolcu1, odeme1, data);                    
        
            double kalanBakiyeT = bakiye - (tramvay.toplamUcret + taksiIlkUcret + taksiSonUcret);

            LogHelper.Ekle($"Kalan Bakiye: {kalanBakiyeT:F2} TL");

            label13.Text = LogHelper.LogBuilder.ToString();

            label6.Text = $"{tramvay.toplamSure + taksisüreT}dk";
            label7.Text = $"{tramvay.toplamUcret + taksiIlkUcret + taksiSonUcret} TL";
            label8.Text = $"{(tramvay.toplamMesafe + mesafeIlkDurakT + mesafeSonDurakT):0.0} km";
            LogHelper.LogBuilder.Clear();


            taxiRoute = taksi.CalculateRoute(null, null, null, data);
            taksi.DisplayRoute(taxiRoute, null, data);
            taksi.DisplayFinalInfo(taxiRoute, data.Duraklar.ToDictionary(d => d.Id), genel, odeme1, data);

            LogHelper.Ekle($"Kalan Bakiye: {bakiye - taksi.toplamUcret:F2} TL");
            label14.Text = LogHelper.LogBuilder.ToString();

            label9.Text = $"{taksi.toplamSure} dk";
            label10.Text = $"{taksi.toplamUcret:F2} TL";
            label11.Text = $"{taksi.toplamMesafe:F2} km";
            LogHelper.LogBuilder.Clear();

        }
        private void HaritayiBaslat()
        {
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CanDragMap = true;
            gMapControl1.MapProvider = GMapProviders.OpenStreetMap;
            gMapControl1.Position = new PointLatLng(40.76524, 29.92461);
            gMapControl1.MinZoom = 1;
            gMapControl1.MaxZoom = 20;
            gMapControl1.Zoom = 13;
            gMapControl1.AutoScroll = true;
        }
        private void MarkerEkle(TransportData data)
        {
            GMapOverlay markers = new GMapOverlay("markers");

            foreach (var durak in data.Duraklar)
            {
                GMarkerGoogleType markerRenk = GMarkerGoogleType.red_dot;

                if (durak.Type == "bus")
                    markerRenk = GMarkerGoogleType.red_dot;
                else
                    markerRenk = GMarkerGoogleType.blue_dot;

                GMarkerGoogle marker = new GMarkerGoogle(
                    new PointLatLng(durak.Lat, durak.Lon),
                    markerRenk
                );

                marker.ToolTipText = durak.Name;
                markers.Markers.Add(marker);
            }

            gMapControl1.Overlays.Add(markers);
        }
        private void TekilMarkerEkle(bool start, double lat, double lon)
        {
            GMapOverlay markers = new GMapOverlay("route");



            GMarkerGoogle marker;

            if (start)
            {
                marker = new GMarkerGoogle(

             new PointLatLng(lat, lon), GMarkerGoogleType.purple_dot);
                marker.ToolTipText = "Başlangıç Konumu";

            }
            else
            {
                marker = new GMarkerGoogle(

             new PointLatLng(lat, lon), GMarkerGoogleType.yellow_dot);
                marker.ToolTipText = "Bitiş Konumu";
            }
            markers.Markers.Add(marker);

            gMapControl1.Overlays.Add(markers);
            gMapControl1.Refresh();
            gMapControl1.Invalidate();
        }
        static Durak EnYakinDurak(List<Durak> duraklar, double userLat, double userLon)
        {
            Durak enYakin = null;
            double minMesafe = double.MaxValue;

            foreach (var durak in duraklar)
            {

                double mesafe = Math.Sqrt(Math.Pow(durak.Lat - userLat, 2) + Math.Pow(durak.Lon - userLon, 2));
                if (mesafe < minMesafe)
                {
                    minMesafe = mesafe;
                    enYakin = durak;
                }
            }

            return enYakin;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label12.Visible = true;
            label13.Visible = false;
            label14.Visible = false;

            if (routeOverlay != null)
            {
                routeOverlay.Routes.Clear();

            }

            List<PointLatLng> routePoints = new List<PointLatLng>();


            routePoints.Add(new PointLatLng(smap_lat, smap_lon));

            foreach (string durakId in busRoute)
            {
                Durak durak = data.Duraklar.FirstOrDefault(d => d.Id == durakId);
                if (durak != null)
                {
                    routePoints.Add(new PointLatLng(durak.Lat, durak.Lon));
                }
            }
            routePoints.Add(new PointLatLng(rmap_lat, rmap_lon));
            GMapRoute routeLine = new GMapRoute(routePoints, "otobüsRota");
            routeLine.Stroke = new Pen(Color.Blue, 2);

            routeOverlay = new GMapOverlay("rotaKatmani");
            routeOverlay.Routes.Add(routeLine);
            gMapControl1.Overlays.Add(routeOverlay);
            gMapControl1.Refresh();
            gMapControl1.Invalidate();

        }


        private void button3_Click(object sender, EventArgs e)
        {
            label12.Visible = false;
            label13.Visible = false;
            label14.Visible = true;

            if (routeOverlay != null)
            {
                routeOverlay.Routes.Clear();

            }

            List<PointLatLng> routePoints = new List<PointLatLng>();


            routePoints.Add(new PointLatLng(smap_lat, smap_lon));

            foreach (string durakId in taxiRoute)
            {
                Durak durak = data.Duraklar.FirstOrDefault(d => d.Id == durakId);
                if (durak != null)
                {
                    routePoints.Add(new PointLatLng(durak.Lat, durak.Lon));
                }
            }
            routePoints.Add(new PointLatLng(rmap_lat, rmap_lon));
            GMapRoute routeLine = new GMapRoute(routePoints, "TaksiRota");
            routeLine.Stroke = new Pen(Color.Blue, 2);

            routeOverlay = new GMapOverlay("rotaKatmani");
            routeOverlay.Routes.Add(routeLine);
            gMapControl1.Overlays.Add(routeOverlay);
            gMapControl1.Refresh();
            gMapControl1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label12.Visible = false;
            label13.Visible = true;
            label14.Visible = false;

            if (routeOverlay != null)
            {
                routeOverlay.Routes.Clear();

            }

            List<PointLatLng> routePoints = new List<PointLatLng>();


            routePoints.Add(new PointLatLng(smap_lat, smap_lon));

            foreach (string durakId in tramRoute)
            {
                Durak durak = data.Duraklar.FirstOrDefault(d => d.Id == durakId);
                if (durak != null)
                {
                    routePoints.Add(new PointLatLng(durak.Lat, durak.Lon));
                }
            }
            routePoints.Add(new PointLatLng(rmap_lat, rmap_lon));
            GMapRoute routeLine = new GMapRoute(routePoints, "tramvayRota");
            routeLine.Stroke = new Pen(Color.Blue, 2);

            routeOverlay = new GMapOverlay("rotaKatmani");
            routeOverlay.Routes.Add(routeLine);
            gMapControl1.Overlays.Add(routeOverlay);
            gMapControl1.Refresh();         

        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void gMapControl1_Click(object sender, EventArgs e)
        {
            gMapControl1.Refresh();
        }
    }


}

