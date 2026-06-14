using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    private State state;

    private static int MoveSpeed;

    private float rotateSpeed = 100f;
    private Vector3 targetRotation; //Aracýn bakacaðý yönü ifade eder

    private List<Transform> waypoints = new List<Transform>(); //Aracýn hareketini saðlayan, seyahat edeceði yoldan elde edilen takip nesneleri
    private int currentWaypointIndex = -1; //Araç konumunu liste içindeki waypoint'lere eþitleyerek hareket eder, her yeni yola baþladýðýnda sýfýrlanýr

    private List<Road> traveledRoads = new List<Road>(); //Ziyaret edilen yollar, feromon eklenmesi için
    private List<CitySO> visitedCities = new List<CitySO>(); //Önceden ziyaret edilen þehirlere gitme durumunu kontrol etmek için

    private CitySO homeCity, currentCity, nextCity; //Baþlangýç þehri - þuan bulunduðu þehir - gideceði þehir

    [SerializeField] private GameObject cargoPackageVisual; //Kargo aracýnýn arkasýndaki sarý kutu modeli, seyahat tamamlandýðýnda görünmez hale getirilir

    public enum State {
        Idle,
        Traveling,
        Returning
    }

    //Start metodu ilgili nesne oluþturulduðunda(instantiate) çaðrýlýr, kargo aracý oluþurken çaðrýlan fonksiyonlar
    private void Start() {  
        MoveSpeed = VehicleManager.Instance.GetVehicleSpeed();
        homeCity = GraphManager.Instance.StartCitySO;
        currentCity = homeCity;

        state = State.Traveling;

        if (!visitedCities.Contains(currentCity)) visitedCities.Add(currentCity);
        TravelNextCity();
    }

    private void Update() {
        switch (state) {
            case State.Idle:
                //Idle state'i var fakat simülasyonda araçlarýn idle'da olduðu bir durum yok, ileride eklenebilir
                //Debug.Log("Araç boþta...");
                break;

            case State.Traveling:
                HandleTraveling();
                break;

            case State.Returning:
                HandleReturning();
                break;
        }
    }

    //Liste içindeki waypointleri her karede kendi konumuna eþitleyerek hareketi saðlar
    private void MoveToNextWaypoint() {
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex + 1].position,
            Time.deltaTime * MoveSpeed * LevelManager.TimeScale);

        targetRotation = waypoints[currentWaypointIndex + 1].position - transform.position;
        if (targetRotation != Vector3.zero) {
            transform.forward = Vector3.Slerp(transform.forward, targetRotation, Time.deltaTime * rotateSpeed * LevelManager.TimeScale);
        }
    }

    //Hedef þehre giderken çaðrýlan fonksiyonlar, waypointIndex'i günceller 
    private void HandleTraveling() {
        if (currentWaypointIndex < waypoints.Count - 1) {
            MoveToNextWaypoint();

            if (transform.position == waypoints[currentWaypointIndex + 1].position) {
                currentWaypointIndex++;

                if (currentWaypointIndex == waypoints.Count - 1) {
                    //mevcut waypointparentin tüm waypointleri ziyaret edildi
                    currentCity = nextCity;
                    if (currentCity == GraphManager.Instance.TargetCitySO) {
                        //hedef þehre varýldý
                        DepositPheromones();

                        TravelHome(isForceReturn: false);
                        cargoPackageVisual.SetActive(false);
                    }
                    else {
                        //hedef þehre gelinmedi, seyahate devam
                        TravelNextCity();
                    }
                }
            }
        }
    }
    
    //Dikstra'nýn hesapladýðý rotadaki waypointleri takip eder
    private void HandleReturning() {
        if (currentWaypointIndex < waypoints.Count - 1) {
            MoveToNextWaypoint();

            if (transform.position == waypoints[currentWaypointIndex + 1].position) {
                currentWaypointIndex++;

                if (currentWaypointIndex == waypoints.Count - 1) {
                    //araç eve vardý
                    Destroy(gameObject);
                }
            }
        }
    }

    //ACO ile sonraki seyahat edilecek þehri belirler
    private void TravelNextCity() {
        nextCity = ACOManager.Instance.ChooseNextCity(currentCity, visitedCities);
        //Debug.Log(nextCity);

        if (nextCity != null) {
            Road road = GraphManager.Instance.GetRoadBetween(currentCity, nextCity);

            traveledRoads.Add(road);

            if (!visitedCities.Contains(nextCity)) visitedCities.Add(nextCity);

            road.useCount++;
            UpdateWaypoints(road.waypointParent);
            currentWaypointIndex = -1;
        }
    }

    //Baþlangýç þehrine dönmek üzere Dijkstra'nýn hesapladýðý rotayý alýr
    public void TravelHome(bool isForceReturn = true) {
        if(state != State.Returning) { //Dönen araçlarda çaðrýldýðýnda uçarak targetcity'e dönmemesine karþýn
            waypoints.Clear();

            if (currentCity == homeCity) { //Baþlangýçtan sonra bir þehri ziyaret edememiþse
                //Araçlar baþlangýçtan sonra henüz bir þehre varamadýysa kitleniyorlar, bunu önlemek için
                //mevcut bulunduðu yolun waypointleri alýnarak doðrudan baþlangýca döner
                Road road = GraphManager.Instance.GetRoadBetween(nextCity, homeCity);
                foreach (Transform child in road.waypointParent)
                    waypoints.Add(child);
            }
            else { //Baþlangýçtan sonra bir þehri ziyaret ettiyse, Dijkstra hesaplanýr
                List<CitySO> pathCitiesSO = null;
                if (!isForceReturn && Dijkstra.CachedStartToTargetPath != null) //Araçlar hedefe varýldýðýnda çaðrýlýr
                    pathCitiesSO = Dijkstra.GetCachedReturnPath(); //path deðiþkeni Dijkstra'nýn hesapladýðý rotanýn þehirlerini içerir
                else //Simülasyon durdurulduðunda çaðrýlýr
                    pathCitiesSO = Dijkstra.FindShortestPath(currentCity, homeCity); 
                

                CitySO stepStart = currentCity; //Dönüþü baþlatacaðý þehir üzerinden baþlangýca doðru olan
                foreach (CitySO stepEnd in pathCitiesSO) {//tüm yollarýn waypointini alýr
                    Road road = GraphManager.Instance.GetRoadBetween(stepStart, stepEnd);
                    if (road != null) {
                        foreach (Transform child in road.waypointParent) {
                            waypoints.Add(child);
                        }
                    }
                    stepStart = stepEnd;
                }
            }
            currentWaypointIndex = -1;
            state = State.Returning; 
        }
    }

    //Teslimat sonunda ziyaret ettiði yollara feromon býrakýr ve teslimat bilgilerini kaydeder
    private void DepositPheromones() {
        RegisterDelivery();

        ACOManager.Instance.AddPheromones(traveledRoads);

        traveledRoads.Clear();
        visitedCities.Clear();
    }

    //StatsManager'a Teslimat bilgilerini iletir
    private void RegisterDelivery() {
        //Ýstatistik paneli için ziyaret edilen yollardan þehir bilgilerini alýr
        List<CitySO> acoPathCitiesSO = new List<CitySO> { homeCity };
        float acoLength = 0f;
        foreach (Road r in traveledRoads) {
            acoPathCitiesSO.Add(r.endCitySO);
            acoLength += r.distance;
        }

        StatsManager.Instance.RegisterDelivery(acoPathCitiesSO, acoLength);  
    }

    //Aracýn takip ettiði waypoint listesini günceller
    private void UpdateWaypoints(Transform waypointParent) {
        waypoints.Clear();
        foreach (Transform child in waypointParent) {
            waypoints.Add(child);
        }
    }

    public static void SetSpeed(int moveSpeed) {
        MoveSpeed = moveSpeed;
    }

    public void SetState(State state) {
        this.state = state;
    }

}
