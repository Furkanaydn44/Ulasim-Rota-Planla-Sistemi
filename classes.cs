using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinLogHelper;
using WinFormsApp1;

namespace WinFormsClass
{
    class TransportData
    {
        public string City { get; set; }
        public Taxi Taxi { get; set; }
        public List<Durak> Duraklar { get; set; }
    }

    abstract class Yolcu
    {
        public string YolcuId { get; set; }

        public string YolcuTipi { get; set; }
        public double BaslangıcLat { get; set; }
        public double BaslangıcLon { get; set; }

        public double BitisLat { get; set; }
        public double BitisLon { get; set; }

        public abstract double indirim(double ödenecekMiktar);


    }
    class Genel : Yolcu
    {
        public override double indirim(double ödenecekMiktar)
        {
            return ödenecekMiktar;
        }


    }
    class Ogrenci : Yolcu
    {
        public override double indirim(double ödenecekMiktar)
        {
            ödenecekMiktar = ödenecekMiktar * 0.6;
            return ödenecekMiktar;
        }


    }
    class Yasli : Yolcu
    {
        public override double indirim(double ödenecekMiktar)
        {
            ödenecekMiktar = 0;
            return ödenecekMiktar;
        }

    }
    abstract class Arac
    {
        public abstract List<string> CalculateRoute(Durak startStop, Durak endStop, List<Durak> allStops, TransportData data);
        public abstract void DisplayRoute(List<string> route, Dictionary<string, Durak> duraklar, TransportData data);
        public abstract void DisplayFinalInfo(List<string> route, Dictionary<string, Durak> duraklar, Yolcu yolcu, Odeme odeme, TransportData data);

        public double toplamMesafe = 0;
        public int toplamSure = 0;
        public double toplamUcret = 0;

    }

    class Otobus : Arac
    {
        public override List<string> CalculateRoute(Durak startStop, Durak endStop, List<Durak> allStops, TransportData data)
        {
            Graph graph = new Graph();
            foreach (var durak in allStops)
            {
                foreach (var next in durak.NextStops)
                {
                    graph.AddEdge(durak.Id, next.StopId, next.Mesafe);
                }
            }
            return graph.Dijkstra(startStop.Id, endStop.Id);
        }

        public override void DisplayRoute(List<string> route, Dictionary<string, Durak> duraklar, TransportData data)
        {
            LogHelper.Ekle("Otobüs Rotası:");
            foreach (string step in route)
            {
                LogHelper.Ekle($"-> {step}");
            }
        }

        public override void DisplayFinalInfo(List<string> route, Dictionary<string, Durak> duraklar, Yolcu yolcu, Odeme odeme, TransportData data)
        {
              toplamMesafe = 0;
              toplamSure = 0;
              toplamUcret = 0;

            for (int i = 0; i < route.Count - 1; i++)
            {
                string mevcutDurakId = route[i];
                string sonrakiDurakId = route[i + 1];

                Durak mevcutDurak = duraklar[mevcutDurakId];
                NextStop nextStop = mevcutDurak.NextStops.FirstOrDefault(n => n.StopId == sonrakiDurakId);
                if (nextStop != null)
                {
                    toplamMesafe += nextStop.Mesafe;
                    toplamSure += nextStop.Sure;
                    toplamUcret += nextStop.Ucret;
                }

                if (mevcutDurak.Transfer != null && mevcutDurak.Transfer.TransferStopId == sonrakiDurakId)
                {
                    toplamSure += mevcutDurak.Transfer.TransferSure;
                    toplamUcret += mevcutDurak.Transfer.TransferUcret;
                }

            }
            double indirimliUcret = yolcu.indirim(toplamUcret);

            double kalanBakiye = odeme.yontem(odeme.bakiye, indirimliUcret);

            indirimliUcret = odeme.bakiye - kalanBakiye;
            
            LogHelper.Ekle($"Otobüs ile gidilen Mesafe: {toplamMesafe:F2} km");
            LogHelper.Ekle($"Otobüs ile gidilen Süre: {toplamSure} dk");
            LogHelper.Ekle($"Otobüs Ücreti: {toplamUcret:F2} TL");
            LogHelper.Ekle($"İndirimli Ücret: {indirimliUcret:F2} TL");

        }



    }

    class Tramvay : Arac
    {
        public override List<string> CalculateRoute(Durak startStop, Durak endStop, List<Durak> allStops, TransportData data)
        {
            Graph graph = new Graph();
            foreach (var durak in allStops)
            {
                foreach (var next in durak.NextStops)
                {
                    graph.AddEdge(durak.Id, next.StopId, next.Mesafe);
                }
                if (durak.Transfer != null)
                {
                    graph.AddEdge(durak.Id, durak.Transfer.TransferStopId, durak.Transfer.TransferSure);
                }
            }
            return graph.Dijkstra(startStop.Id, endStop.Id);
        }

        public override void DisplayRoute(List<string> route, Dictionary<string, Durak> duraklar, TransportData data)
        {
            LogHelper.Ekle("Tramvay Öncelikli Rota:");


            string lastStopId = route.Last();
            Durak lastStop = duraklar[lastStopId];

            if (lastStop.Type != "tram")
            {

                int transferIndex = route.FindIndex(id => duraklar[id].Type != "tram");
                for (int i = 0; i < transferIndex; i++)
                {
                    LogHelper.Ekle($"-> {route[i]}");
                }

                LogHelper.Ekle("-> Otobüse transfer");


                for (int i = transferIndex; i < route.Count; i++)
                {
                    LogHelper.Ekle($"-> {route[i]}");
                }
            }
            else
            {
                foreach (string step in route)
                {
                    LogHelper.Ekle($"-> {step}");
                }
            }
        }
        public override void DisplayFinalInfo(List<string> route, Dictionary<string, Durak> duraklar, Yolcu yolcu, Odeme odeme, TransportData data)
        {
             toplamMesafe = 0;
             toplamSure = 0;
             toplamUcret = 0;

            for (int i = 0; i < route.Count - 1; i++)
            {
                string mevcutDurakId = route[i];
                string sonrakiDurakId = route[i + 1];

                Durak mevcutDurak = duraklar[mevcutDurakId];
                NextStop nextStop = mevcutDurak.NextStops.FirstOrDefault(n => n.StopId == sonrakiDurakId);
                if (nextStop != null)
                {
                    toplamMesafe += nextStop.Mesafe;
                    toplamSure += nextStop.Sure;
                    toplamUcret += nextStop.Ucret;
                }

                if (mevcutDurak.Transfer != null && mevcutDurak.Transfer.TransferStopId == sonrakiDurakId)
                {
                    toplamSure += mevcutDurak.Transfer.TransferSure;
                    toplamUcret += mevcutDurak.Transfer.TransferUcret;
                }

            }

            double indirimliUcret = yolcu.indirim(toplamUcret);

            double kalanBakiye = odeme.yontem(odeme.bakiye, indirimliUcret);

            indirimliUcret = odeme.bakiye - kalanBakiye;

            LogHelper.Ekle($"Tramvay ile gidilen Mesafe: {toplamMesafe:F2} km");
            LogHelper.Ekle($"Tramvay ile gidilen Süre: {toplamSure} dk");
            LogHelper.Ekle($"Tramvay Ücreti: {toplamUcret:F2} TL");
            LogHelper.Ekle($"İndirimli Ücret: {indirimliUcret:F2} TL");

        }

    }

    class Taxi : Arac
    {
        public double OpeningFee;
        public double CostPerKm;
        

        public override List<string> CalculateRoute(Durak startStop, Durak endStop, List<Durak> allStops, TransportData data)
        {

            return new List<string> { "Direct taxi route" };
        }

        public override void DisplayRoute(List<string> route, Dictionary<string, Durak> duraklar, TransportData data)
        {
            LogHelper.Ekle("-> Taksi ile doğrudan gidildi");
        }

        public override void DisplayFinalInfo(List<string> route, Dictionary<string, Durak> duraklar, Yolcu yolcu, Odeme odeme, TransportData data)
        {
            toplamSure = 0;

            toplamMesafe = DistanceCalculator.Haversine(yolcu.BaslangıcLat, yolcu.BaslangıcLon,
                                                  yolcu.BitisLat, yolcu.BitisLon);
             toplamUcret = data.Taxi.OpeningFee + (toplamMesafe * data.Taxi.CostPerKm);

            double kalanBakiye = odeme.yontem(odeme.bakiye, toplamUcret);

             toplamSure = (int)((toplamMesafe /30)* 60);

            LogHelper.Ekle($"Taksi ile gidilen Mesafe: {toplamMesafe:F2} km");
            LogHelper.Ekle($"Taksi Ücreti: {toplamUcret:F2} TL");
            LogHelper.Ekle($"Taksi ile gidilen Süre: {toplamSure} dk");
            
        }

    }
    abstract class Odeme
    {
        public double bakiye { get; set; }

        public abstract double yontem(double bakiye, double ödenecekTutar);


    }

    class Nakit : Odeme
    {
        public override double yontem(double bakiye, double ödenecekTutar)
        {
            double kalanPara = bakiye - ödenecekTutar;
            return kalanPara;
        }


    }

    class KentKart : Odeme
    {
        public override double yontem(double bakiye, double ödenecekTutar)
        {
            ödenecekTutar = ödenecekTutar * 0.8;
            double kalanPara = bakiye - ödenecekTutar;
            return kalanPara;
        }


    }

    class KrediKartı : Odeme
    {
        public override double yontem(double bakiye, double ödenecekTutar)
        {
            ödenecekTutar = ödenecekTutar * 1.2;
            double kalanPara = bakiye - ödenecekTutar;
            return kalanPara;
        }

    }


    class Durak
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public bool SonDurak { get; set; }
        public List<NextStop> NextStops { get; set; }
        public Transfer Transfer { get; set; }
    }
    class NextStop
    {
        public string StopId { get; set; }
        public double Mesafe { get; set; }
        public int Sure { get; set; }
        public double Ucret { get; set; }
    }
    class Transfer
    {
        public string TransferStopId { get; set; }
        public int TransferSure { get; set; }
        public double TransferUcret { get; set; }
    }

    class Graph
    {
        Dictionary<string, List<(string, double)>> adjacencyList = new Dictionary<string, List<(string, double)>>();

        public void AddEdge(string from, string to, double weight)
        {
            if (!adjacencyList.ContainsKey(from))
                adjacencyList[from] = new List<(string, double)>();

            adjacencyList[from].Add((to, weight));

            if (!adjacencyList.ContainsKey(to))
                adjacencyList[to] = new List<(string, double)>();

            adjacencyList[to].Add((from, weight));
        }

        public List<string> Dijkstra(string start, string target)
        {
            var queue = new SortedSet<(double, string)>();
            var distances = new Dictionary<string, double>();
            var previousNodes = new Dictionary<string, string>();

            foreach (var node in adjacencyList.Keys)
            {
                distances[node] = double.MaxValue;
                previousNodes[node] = null;
            }

            distances[start] = 0;
            queue.Add((0, start));

            while (queue.Count > 0)
            {
                var (currentDistance, currentNode) = queue.Min;
                queue.Remove(queue.Min);

                if (currentNode == target)
                    break;

                foreach (var (neighbor, weight) in adjacencyList[currentNode])
                {
                    double distance = currentDistance + weight;
                    if (distance < distances[neighbor])
                    {
                        queue.Remove((distances[neighbor], neighbor));
                        distances[neighbor] = distance;
                        previousNodes[neighbor] = currentNode;
                        queue.Add((distance, neighbor));
                    }
                }
            }

            var path = new List<string>();
            string step = target;
            while (step != null)
            {
                path.Insert(0, step);
                step = previousNodes[step];
            }

            return path;
        }
    }
    class GraphWithTransfers
    {
        Dictionary<string, List<(string, string)>> adjacencyList = new Dictionary<string, List<(string, string)>>();

        public void AddEdge(string from, string to, string type)
        {
            if (!adjacencyList.ContainsKey(from))
                adjacencyList[from] = new List<(string, string)>();

            adjacencyList[from].Add((to, type));

            if (!adjacencyList.ContainsKey(to))
                adjacencyList[to] = new List<(string, string)>();

            adjacencyList[to].Add((from, type));
        }

        public List<string> BFSWithTransfers(string start, string target, Dictionary<string, string> stopTypes)
        {
            var queue = new Queue<(string, List<string>, int)>();
            var visited = new HashSet<string>();

            queue.Enqueue((start, new List<string> { start }, 0));
            visited.Add(start);

            while (queue.Count > 0)
            {
                var (current, path, transferCount) = queue.Dequeue();

                if (current == target)
                    return path;

                foreach (var (neighbor, type) in adjacencyList[current])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        var newPath = new List<string>(path) { neighbor };
                        int newTransferCount = transferCount;

                        if (stopTypes[current] != stopTypes[neighbor]) 
                        {
                            newTransferCount++;
                        }

                        queue.Enqueue((neighbor, newPath, newTransferCount));
                    }
                }
            }

            return new List<string>(); 
        }
    }

    public static class DistanceCalculator
    {
        private const double EarthRadiusKm = 6371;
        public static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return EarthRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }
        private static double ToRadians(double degrees) => degrees * (Math.PI / 180);
    }
    
}