#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine.Events;

public class FullBedroomFurnisherTool : MonoBehaviour
{
    [MenuItem("Tools/ปรับความสูงตัวชนเตียงกันปีน (Fix Bed Collider Height)")]
    public static void FixBedColliderOnly()
    {
        GameObject bed = GameObject.Find("Modern_Cozy_Bed");
        if (bed == null) bed = GameObject.Find("Bed");

        if (bed != null)
        {
            BoxCollider bedCol = bed.GetComponent<BoxCollider>();
            if (bedCol == null) bedCol = bed.AddComponent<BoxCollider>();
            bedCol.center = new Vector3(0f, 0.9f, 0f);
            bedCol.size = new Vector3(2.0f, 1.8f, 2.4f);
            
            Undo.RecordObject(bedCol, "Increase Bed Collider Height");
            Debug.Log("🛡️ [BedCollider] ปรับความสูงตัวชนเตียงให้สูงขึ้นเรียบร้อยแล้วครับ!");
        }
    }

    [MenuItem("Tools/ตกแต่งห้องนอนเต็มรูปแบบ (Furnish Full Cozy Bedroom)")]
    public static void FurnishFullBedroom()
    {
        // 1. ซ่อน/ลบ เตียงเก่า และ กระจกเก่า
        GameObject oldBed = GameObject.Find("Interior_Furniture_Bed_Single_01");
        if (oldBed != null) oldBed.SetActive(false);

        GameObject oldMirror = GameObject.Find("Mirror_Group");
        if (oldMirror != null) oldMirror.SetActive(false);

        // ลบเฟอร์นิเจอร์ใหม่ชุดเดิมก่อนหน้า (ถ้าเคยสร้างไว้) เพื่ออัปเดตใหม่
        GameObject existingGroup = GameObject.Find("Furnished_Bedroom_Set");
        if (existingGroup != null) Undo.DestroyObjectImmediate(existingGroup);

        // 2. สร้าง Root Group
        GameObject furnGroup = new GameObject("Furnished_Bedroom_Set");
        Undo.RegisterCreatedObjectUndo(furnGroup, "Furnish Full Bedroom");

        string urpPath = "Assets/OuterFile/Object/Bedroom/Prefabs/";

        // 3. วางเตียงใหม่ (Bed) พร้อมตัวชนสูง
        GameObject bedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(urpPath + "Bed.prefab");
        if (bedPrefab != null)
        {
            GameObject bedInstance = (GameObject)PrefabUtility.InstantiatePrefab(bedPrefab, furnGroup.transform);
            bedInstance.name = "Modern_Cozy_Bed";
            bedInstance.transform.position = new Vector3(4.5f, 0f, -8.5f);
            bedInstance.transform.rotation = Quaternion.Euler(0f, -90f, 0f);

            BoxCollider bedCol = bedInstance.GetComponent<BoxCollider>();
            if (bedCol == null) bedCol = bedInstance.AddComponent<BoxCollider>();
            bedCol.center = new Vector3(0f, 0.9f, 0f);
            bedCol.size = new Vector3(2.0f, 1.8f, 2.4f);

            BedInteraction bedInter = bedInstance.GetComponent<BedInteraction>();
            if (bedInter == null) bedInter = bedInstance.AddComponent<BedInteraction>();

            BedroomInteractable bedInteractable = bedInstance.GetComponent<BedroomInteractable>();
            if (bedInteractable == null) bedInteractable = bedInstance.AddComponent<BedroomInteractable>();
            bedInteractable.promptMessage = "นอนพักผ่อน";
            if (bedInteractable.onInteract == null) bedInteractable.onInteract = new UnityEvent();

            UnityEventTools.AddPersistentListener(bedInteractable.onInteract, new UnityAction(bedInter.Sleep));
        }

        // 4. วางโต๊ะข้างเตียง (Table)
        GameObject tablePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(urpPath + "Table.prefab");
        if (tablePrefab != null)
        {
            GameObject tableInstance = (GameObject)PrefabUtility.InstantiatePrefab(tablePrefab, furnGroup.transform);
            tableInstance.name = "Bedside_Table";
            tableInstance.transform.position = new Vector3(4.65f, 0f, -11.0f);
            tableInstance.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }

        // 5. 💡 วางโคมไฟหัวเตียง (Lamp) พร้อมระบบเปิด-ปิดไฟ [กด F เพื่อ เปิด/ปิด]
        GameObject lampPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(urpPath + "Lamp.prefab");
        if (lampPrefab != null)
        {
            GameObject lampInstance = (GameObject)PrefabUtility.InstantiatePrefab(lampPrefab, furnGroup.transform);
            lampInstance.name = "Bedside_Lamp";
            lampInstance.transform.position = new Vector3(4.65f, 0.76f, -11.0f);
            lampInstance.transform.rotation = Quaternion.identity;

            // เพิ่ม BoxCollider เพื่อให้เล็งกด F ได้
            BoxCollider lampCol = lampInstance.GetComponent<BoxCollider>();
            if (lampCol == null) lampCol = lampInstance.AddComponent<BoxCollider>();
            lampCol.center = new Vector3(0f, 0.3f, 0f);
            lampCol.size = new Vector3(0.5f, 0.65f, 0.5f);

            // เพิ่มไฟหัวเตียง (Point Light)
            GameObject lampLightObj = new GameObject("Lamp_Light");
            lampLightObj.transform.SetParent(lampInstance.transform, false);
            lampLightObj.transform.localPosition = new Vector3(0f, 0.35f, 0f);

            Light lampLight = lampLightObj.AddComponent<Light>();
            lampLight.type = LightType.Point;
            lampLight.color = new Color(1f, 0.78f, 0.5f, 1f); // Warm Amber
            lampLight.intensity = 2.0f;
            lampLight.range = 5.0f;
            lampLight.shadows = LightShadows.Soft;

            // ใส่ LampInteraction
            LampInteraction lampInter = lampInstance.AddComponent<LampInteraction>();
            SerializedObject lampSO = new SerializedObject(lampInter);
            
            SerializedProperty lightProp = lampSO.FindProperty("lampLight");
            if (lightProp != null) lightProp.objectReferenceValue = lampLight;

            AudioClip clickSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/select_1.wav");
            if (clickSound == null) clickSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Use SFX/select_1.wav");
            SerializedProperty clickProp = lampSO.FindProperty("switchSound");
            if (clickProp != null && clickSound != null) clickProp.objectReferenceValue = clickSound;
            
            lampSO.ApplyModifiedProperties();

            // ใส่ BedroomInteractable
            BedroomInteractable lampInteractable = lampInstance.AddComponent<BedroomInteractable>();
            lampInteractable.promptMessage = "ปิดโคมไฟ";
            if (lampInteractable.onInteract == null) lampInteractable.onInteract = new UnityEvent();

            // ผูก Event OnInteract -> LampInteraction.ToggleLamp()
            UnityEventTools.AddPersistentListener(lampInteractable.onInteract, new UnityAction(lampInter.ToggleLamp));
        }

        // 6. วางตู้ลิ้นชัก (Closet / Drawers)
        GameObject closetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(urpPath + "Closet.prefab");
        if (closetPrefab != null)
        {
            GameObject closetInstance = (GameObject)PrefabUtility.InstantiatePrefab(closetPrefab, furnGroup.transform);
            closetInstance.name = "Bedroom_Closet";
            closetInstance.transform.position = new Vector3(-1.75f, 0f, -9.8f);
            closetInstance.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }

        // 7. วางกระจกใหม่ (Mirror)
        GameObject mirrorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(urpPath + "Mirror.prefab");
        if (mirrorPrefab != null)
        {
            GameObject mirrorInstance = (GameObject)PrefabUtility.InstantiatePrefab(mirrorPrefab, furnGroup.transform);
            mirrorInstance.name = "Modern_Mirror";
            mirrorInstance.transform.position = new Vector3(4.65f, 1.1f, -12.85f);
            mirrorInstance.transform.rotation = Quaternion.Euler(0f, -90f, 0f);

            MirrorInteraction mirrorInter = mirrorInstance.GetComponent<MirrorInteraction>();
            if (mirrorInter == null) mirrorInter = mirrorInstance.AddComponent<MirrorInteraction>();

            BedroomInteractable mirrorInteractable = mirrorInstance.GetComponent<BedroomInteractable>();
            if (mirrorInteractable == null) mirrorInteractable = mirrorInstance.AddComponent<BedroomInteractable>();
            mirrorInteractable.promptMessage = "ส่องกระจก";
            if (mirrorInteractable.onInteract == null) mirrorInteractable.onInteract = new UnityEvent();

            UnityEventTools.AddPersistentListener(mirrorInteractable.onInteract, new UnityAction(mirrorInter.LookInMirror));
        }

        // 8. วางแอร์ติดผนัง (AC)
        GameObject acPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(urpPath + "AC.prefab");
        if (acPrefab != null)
        {
            GameObject acInstance = (GameObject)PrefabUtility.InstantiatePrefab(acPrefab, furnGroup.transform);
            acInstance.name = "Bedroom_AC";
            acInstance.transform.position = new Vector3(-1.75f, 2.3f, -7.8f);
            acInstance.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }

        // 9. อัปเดตแสงแดดที่หน้าต่างให้สว่างสวยงามยิ่งขึ้น (Sunlight Boost)
        GameObject windowSet = GameObject.Find("Window_Curtain_Set");
        if (windowSet != null)
        {
            Transform sunT = windowSet.transform.Find("Sunlight");
            if (sunT != null)
            {
                Light sunLight = sunT.GetComponent<Light>();
                if (sunLight != null)
                {
                    sunLight.intensity = 6.5f;
                    sunLight.color = new Color(1f, 0.88f, 0.65f, 1f);
                    sunLight.range = 20f;
                }
            }
        }

        Selection.activeGameObject = furnGroup;
        Debug.Log("🏠✨ [BedroomFurnisher] ตกแต่งห้องนอนและติดตั้งระบบเปิด-ปิดโคมไฟหัวเตียงเรียบร้อยแล้วครับ!");
    }
}
#endif
