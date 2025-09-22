using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class LocationManager : MonoBehaviour
{
    [SerializeField] TMP_Text locationText;
    [SerializeField] List<LocationInSpace> locations = new();
    public List<LocationInSpace> Locations => locations;

    //Not Optimized but ok for this small game
    private void Update()
    {
        string newList = "Locations:\n";

        foreach (var loc in locations)
        {
            newList += $"- {loc.LocationName}\n";
        }

        locationText.text = newList;
    }
}

[Serializable]
public class LocationInSpace
{
    [SerializeField] string locationName;
    [SerializeField] float xDay;
    [SerializeField] string dayString => GetDayString(xDay);
    [SerializeField] float yMonth;
    [SerializeField] string monthString => GetMonthString(yMonth);
    [SerializeField] float zYear;
    public string LocationName {
        get
        {
            string strikeOne = visitedLocation ? "<s>" : "";
            string strikeTwo = visitedLocation ? "</s>" : "";
            return $"{locationName}:\n   {strikeOne}{xDay} / {yMonth} / {zYear}{strikeTwo}";
        }
    }
    [SerializeField] VideoClip videoClip;
    bool visitedLocation = false;
    public bool VisitedLocation => visitedLocation;

    public bool InRange(Vector3 center, float dx, float dy, float dz)
    {
        Debug.Log(center);
        if(visitedLocation) return false;
        return Mathf.Abs(xDay - center.x) <= dx &&
               Mathf.Abs(yMonth - center.y) <= dy &&
               Mathf.Abs(zYear - center.z) <= dz;
    }

    public VideoClip visitLocation()
    {
        Debug.Log($"Visiting location {locationName}");
        visitedLocation = true;
        return videoClip;
    }

    static readonly string[] MonthNames = {
        "Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"
    };
    public static string GetDayString(float xDay)
    {
        float minX = -UniverseController.MaxX;
        float maxX = UniverseController.MaxX;
        float t = Mathf.InverseLerp(minX, maxX, Mathf.Clamp(xDay, minX, maxX));
        int day = Mathf.RoundToInt(t * 30f); // change to +1 if you want 1..31
        return day.ToString();
    }

    public static string GetMonthString(float yMonth)
    {
        float minY = -UniverseController.MaxY;
        float maxY = UniverseController.MaxY;
        float t = Mathf.InverseLerp(minY, maxY, Mathf.Clamp(yMonth, minY, maxY));
        int idx = Mathf.Clamp(Mathf.FloorToInt(t * 12f), 0, 11);
        return MonthNames[idx];
    }

    public string GetYearStringZoom(
        float currentZoom, float minZoom, float maxZoom,
        int baseYear = -9000,
        float minYearsPerUnit = 2f,
        float maxYearsPerUnit = 50f,
        float zoomExponent = 1.5f)
    {
        // 0 when zoomed out, 1 when zoomed in
        float zoom01Out = Mathf.InverseLerp(minZoom, maxZoom, currentZoom);
        float zoom01In = 1f - zoom01Out;
        float yearsPerUnit = Mathf.Lerp(minYearsPerUnit, maxYearsPerUnit, Mathf.Pow(zoom01In, zoomExponent));
        int year = baseYear + Mathf.RoundToInt(zYear * yearsPerUnit);
        return year.ToString();
    }
}
