using UnityEngine;
using System.Collections.Generic;

public class ItemAppearanceController : MonoBehaviour
{
    [Header("วันที่ไอเทมชิ้นนี้จะโผล่มาให้เก็บ (เช่น วันที่ 1, 3, 8, 10)")]
    public List<int> spawnDays = new List<int>();

    [Header("ให้ปรากฏตั้งแต่วันเริ่มต้นเป็นต้นไป (คงอยู่จนกว่าจะเก็บ)")]
    public bool spawnFromDayOnward = false;
    public int minDay = 1;

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

        // ตรวจสอบว่าไอเทมชิ้นนี้ถูกเก็บไปแล้วหรือยัง
        PickupItem pickup = GetComponent<PickupItem>();
        if (pickup != null && GameManagerSetup.Instance != null)
        {
            string id = pickup.GetUniqueID();
            if (GameManagerSetup.Instance.IsItemPickedUp(id))
            {
                gameObject.SetActive(false);
                return;
            }
        }

        if (spawnFromDayOnward)
        {
            gameObject.SetActive(today >= minDay);
            return;
        }

        if (spawnDays != null && spawnDays.Count > 0)
        {
            gameObject.SetActive(spawnDays.Contains(today));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}