using UnityEngine;
using System.Collections.Generic;

public class ItemAppearanceController : MonoBehaviour
{
    [Header("วันที่ไอเทมชิ้นนี้จะโผล่มาให้เก็บ (เช่น วันที่ 1, 3, 8, 10)")]
    public List<int> spawnDays = new List<int>();

    void Start()
    {
        UpdateItemAppearance();
    }

    public void UpdateItemAppearance()
    {
        int today = 1;
        if (GameManagerSetup.Instance != null)
        {
            today = GameManagerSetup.Instance.currentDay;
        }
        else
        {
            DayManager dayManager = Object.FindAnyObjectByType<DayManager>();
            if (dayManager != null)
            {
                today = dayManager.currentDay;
            }
        }
        
        if (spawnDays != null)
        {
            gameObject.SetActive(spawnDays.Contains(today));
        }
    }
}