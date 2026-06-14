using System.Collections.Generic;
using UnityEngine;

public class ACOManager : MonoBehaviour {

    public static ACOManager Instance { get; private set; }

    [Header("Alpha: Feromonun önemi - Beta: Mesafenin önemi")]
    [SerializeField] private float alpha = 1.0f; //Feromona verilen önem
    [SerializeField] private float beta = 1.0f;  //Mesafeye verilen önem

    [Header("Yollar için feromon özellikleri")]
    [SerializeField] private float startPheremoneLevel = 1f; //Yollardaki baþlagýç feromon miktarý
    [SerializeField] private float evaporationRate = 0.05f; //Buharlaþma hýzý (0 ile 1 arasý)
    [SerializeField] private float minPheromoneLevel = 0.1f; //Feromonun yok olmamasý için minimum sýnýr

    [Header("Q sabit deðeri (Býrakýlan feromon: Q / L^2)")]
    [SerializeField] private int Q = 10000; //Q sabiti

    private List<Road> allRoads; //Sahne üzerindeki tüm yollar, bilgileri graphmanager'dan alacak

    //Awake metodu start'dan önce çalýþýr, ACOManager nesnesi önceden oluþturulduðu için sahne baþlar baþlamaz çaðrýlýr
    private void Awake() {
        if(Instance != null) {
            Debug.LogError("Sahnede birden fazla ACOManager var!");
        }
        Instance = this;

        allRoads = GraphManager.Instance.GetAllRoads();
    }

    private void Start() {
        Debug.Log("ACO feromonun önemi: " + alpha + " - Yolun önemi: " + beta);
    }

    private void Update() {
        if (!LevelManager.Instance.IsSimulationInitiated)
            return;

        //Her karede tüm yollardaki feromonu buharlaþtýrýr
        foreach (Road road in allRoads) {
            road.pheromoneLevel *= (1f - evaporationRate * Time.deltaTime * LevelManager.TimeScale);

            if (road.pheromoneLevel < minPheromoneLevel) {
                road.pheromoneLevel = minPheromoneLevel;
            }
        }
    }

    public void AddStartPheromone() {
        foreach (Road road in allRoads) {
            road.pheromoneLevel = startPheremoneLevel;
        }
    }

    public void ResetStartPheromone() {
        foreach (Road road in allRoads) {
            road.pheromoneLevel = 0;
        }
    }

    public CitySO ChooseNextCity(CitySO currentCity, List<CitySO> visitedCities) {

        List<CitySO> availableNeighbors = new List<CitySO>(); //Daha önce gidilmemiþ komþularý tanýmlar

        foreach (CitySO neighbor in currentCity.neighbors) {
            if (!visitedCities.Contains(neighbor)) {
                availableNeighbors.Add(neighbor);
            }
        }

        //Eðer ziyaret edilmemiþ þehir kalmadýysa rastgele seçer
        if (availableNeighbors.Count == 0) {
            return currentCity.neighbors[Random.Range(0, currentCity.neighbors.Count)];
        }

        //Mevcut komþular için olasýlýk hesaplar
        List<float> scores = new List<float>();
        float totalScore = 0f;

        foreach (CitySO neighbor in availableNeighbors) {
            Road road = GraphManager.Instance.GetRoadBetween(currentCity, neighbor);
            if (road != null) {
                //Formül: P = (Feromon^alpha) * ((1/Mesafe)^beta)
                float tau = Mathf.Pow(road.pheromoneLevel, alpha);
                float eta = Mathf.Pow(1f / road.distance, beta);
                float score = tau * eta;

                scores.Add(score);
                totalScore += score;
                //Olasýlýk bulma hesabýný standart ACO formülündeki toplam skora bölme iþlemiyle
                //yapmadýk, çünkü halihazýrda rulet tekerleði ile gidilecek yolu hesapladýðýmýz için
                //bu bölme iþlemi yapýlsa da rulet dilimlerinin büyüklüðü, yüzdelikleri ayný olacak.
                //Ayrýca sahnedeki yüzlerce araç için bölme iþlemini gerçekleþtirmeyerek performanstan da
                //kazanç saðlamýþ oluyoruz
            }
        }

        //Rulet tekerleði seçimi ile gidilecek þehri seçer
        float randomValue = Random.Range(0f, totalScore);
        float cumulativeScore = 0f;

        for (int i = 0; i < availableNeighbors.Count; i++) {
            cumulativeScore += scores[i];
            if (randomValue <= cumulativeScore) {
                return availableNeighbors[i];
            }
        }

        return availableNeighbors[0]; //Güvenlik amaçlý, þehir seçilemediyse ilk mevcut þehre dön
    }

    public void AddPheromones(List<Road> traveledRoads) {
        float totalDistance = 0;
        foreach (Road r in traveledRoads) { //Toplam mesafeyi hesaplar
            totalDistance += r.distance;
        }

        //(Q / L^2 formülü) þehirler arasýndaki yollarýn uzunluðu birbirine oldukça yakýn olduðu için
        //standart formülde araçlar birkaç kmlik kýsa olan yolu seçtiklerinde formül yeterince cezalandýrýcý
        //ve ödüllendirici olamayabiliyor, o yüzden förmülü kýyasla uzun yollar seçildiðinde
        //daha cezelandýrýcý bir þekilde az feromon býrakmasý þeklinde güncelledik
        float pheromoneToAdd = Q / (totalDistance * totalDistance); 

        foreach (Road r in traveledRoads) { //tüm yollara feromon ekler
            r.pheromoneLevel += pheromoneToAdd;
        }

    }


    //getter ve setterlar
    public float GetAlpha() {
        return alpha;
    }
    public void SetAlpha(float alpha) {
        this.alpha = alpha;
    }

    public float GetBeta() {
        return beta;
    }
    public void SetBeta(float beta) {
        this.beta = beta;
    }

    public float GetStartPheromoneLevel() {
        return startPheremoneLevel;
    }
    public void SetStartPheromoneLevel(float startPheremoneLevel) {
        this.startPheremoneLevel = startPheremoneLevel;
    }

    public float GetEvaporationRate() {
        return evaporationRate;
    }
    public void SetEvaporationRate(float evaporationRate) {
        this.evaporationRate = Mathf.Clamp01(evaporationRate);
    }

    public float GetMinPheromoneLevel() {
        return minPheromoneLevel;
    }
    public void SetMinPheromoneLevel(float minPheromone) {
        this.minPheromoneLevel = Mathf.Max(0.0001f, minPheromone);
    }

    public int GetQ() {
        return Q;
    }
    public void SetQ(int Q) {
        this.Q = Q;
    }
}