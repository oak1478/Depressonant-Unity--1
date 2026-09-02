#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine.Events;

public class CozyWindowSetupTool : MonoBehaviour
{
    [MenuItem("Tools/ปรับแสงแดดหน้าต่างให้นุ่มนวลเป็นธรรมชาติ (Set Natural Window Sunlight)")]
    public static void SetNaturalSunlight()
    {
        GameObject windowSet = GameObject.Find("Window_Curtain_Set");
        if (windowSet != null)
        {
            Transform sunT = windowSet.transform.Find("Sunlight");
            if (sunT != null)
            {
                // 1. ลบดวงไฟกลม (Sun_Ambient_Glow) ออก ไม่ให้มีแสงเป็นก้อนดวงไฟ
                Transform glowT = sunT.Find("Sun_Ambient_Glow");
                if (glowT != null)
                {
                    Undo.DestroyObjectImmediate(glowT.gameObject);
                }

                // 2. ปรับแสงแดดให้เป็นแสงธรรมชาติ นุ่มนวล ไม่จ้าแสบตา
                Light mainSpot = sunT.GetComponent<Light>();
                if (mainSpot != null)
                {
                    mainSpot.type = LightType.Spot;
                    mainSpot.intensity = 7.5f; // ความสว่างกำลังพอดี ดูเป็นธรรมชาติ
                    mainSpot.range = 18f;
                    mainSpot.spotAngle = 75f;
                    mainSpot.color = new Color(1f, 0.90f, 0.74f, 1f); // แสงแดดสีส้มอุ่นธรรมชาติ
                    mainSpot.shadows = LightShadows.None;
                    Undo.RecordObject(mainSpot, "Set Natural Sunlight");
                }

                Debug.Log("☀️🌿 [WindowLight] ปรับแสงแดดให้นุ่มนวลเป็นธรรมชาติ และลบแสงดวงไฟออกเรียบร้อยแล้วครับ! (Intensity: 7.5)");
            }
            else
            {
                Debug.LogWarning("[WindowLight] หา Sunlight ใน Window_Curtain_Set ไม่เจอ");
            }
        }
        else
        {
            Debug.LogWarning("[WindowLight] หา Window_Curtain_Set ในฉากไม่เจอ");
        }
    }

    [MenuItem("Tools/สร้างหน้าต่างคู่ 2 บานพร้อมผ้าม่าน (Build Double Cozy Window Set)")]
    public static void BuildCozyWindowSet()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        GameObject oldWindow = GameObject.Find("Window_Curtain_Set");
        Vector3 spawnPos = new Vector3(2.14f, 1.35f, -13.0f);
        Quaternion spawnRot = Quaternion.identity;
        if (oldWindow != null)
        {
            spawnPos = oldWindow.transform.position;
            spawnRot = oldWindow.transform.rotation;
            Undo.DestroyObjectImmediate(oldWindow);
        }

        GameObject root = new GameObject("Window_Curtain_Set");
        Undo.RegisterCreatedObjectUndo(root, "Create Double Cozy Window Set");

        GameObject windowPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/WindowAndDoorsPack01/Mesh/Prefabs/BoxSashWindowFurnitureWithBarsOak.prefab");
        if (windowPrefab != null)
        {
            GameObject windowLeft = (GameObject)PrefabUtility.InstantiatePrefab(windowPrefab, root.transform);
            windowLeft.name = "Window_Oak_Left";
            windowLeft.transform.localPosition = new Vector3(-0.55f, 0f, 0f);
            windowLeft.transform.localRotation = Quaternion.identity;
            windowLeft.transform.localScale = Vector3.one;

            GameObject windowRight = (GameObject)PrefabUtility.InstantiatePrefab(windowPrefab, root.transform);
            windowRight.name = "Window_Oak_Right";
            windowRight.transform.localPosition = new Vector3(0.55f, 0f, 0f);
            windowRight.transform.localRotation = Quaternion.identity;
            windowRight.transform.localScale = Vector3.one;
        }

        Material blackMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/WindowAndDoorsPack01/Materials/Black.mat");
        Material curtainMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/OuterFile/Object/Outdoor/White_Grid.mat");
        if (curtainMat == null) curtainMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/OuterFile/Object/Outdoor/Cream.mat");
        if (curtainMat == null) curtainMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/BasicBedroomPack-Mavi3D/Materials/URP/Bed/BedSheetMat.mat");

        GameObject rod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rod.name = "Curtain_Rod";
        rod.transform.SetParent(root.transform, false);
        rod.transform.localPosition = new Vector3(0f, 1.15f, 0.12f);
        rod.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        rod.transform.localScale = new Vector3(0.04f, 1.7f, 0.04f);
        if (blackMat != null) rod.GetComponent<MeshRenderer>().sharedMaterial = blackMat;
        DestroyImmediate(rod.GetComponent<Collider>());

        GameObject curtainLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        curtainLeft.name = "Curtain_Left";
        curtainLeft.transform.SetParent(root.transform, false);
        curtainLeft.transform.localPosition = new Vector3(-1.45f, 0.05f, 0.1f);
        curtainLeft.transform.localRotation = Quaternion.Euler(0f, 0f, 2.5f);
        curtainLeft.transform.localScale = new Vector3(0.32f, 2.15f, 0.08f);
        if (curtainMat != null) curtainLeft.GetComponent<MeshRenderer>().sharedMaterial = curtainMat;
        DestroyImmediate(curtainLeft.GetComponent<Collider>());

        GameObject curtainRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        curtainRight.name = "Curtain_Right";
        curtainRight.transform.SetParent(root.transform, false);
        curtainRight.transform.localPosition = new Vector3(1.45f, 0.05f, 0.1f);
        curtainRight.transform.localRotation = Quaternion.Euler(0f, 0f, -2.5f);
        curtainRight.transform.localScale = new Vector3(0.32f, 2.15f, 0.08f);
        if (curtainMat != null) curtainRight.GetComponent<MeshRenderer>().sharedMaterial = curtainMat;
        DestroyImmediate(curtainRight.GetComponent<Collider>());

        // ☀️ ลำแสงแดดธรรมชาติ (Natural Sunlight Beam)
        GameObject lightObj = new GameObject("Sunlight");
        lightObj.transform.SetParent(root.transform, false);
        lightObj.transform.localPosition = new Vector3(0f, 1.2f, 0.25f);
        lightObj.transform.localRotation = Quaternion.Euler(55f, 0f, 0f);

        Light spotLight = lightObj.AddComponent<Light>();
        spotLight.type = LightType.Spot;
        spotLight.color = new Color(1f, 0.90f, 0.74f, 1f); // Warm natural sunlight
        spotLight.intensity = 7.5f; // ความสว่างธรรมชาติ นุ่มนวล
        spotLight.range = 18f;
        spotLight.spotAngle = 75f;
        spotLight.shadows = LightShadows.None;

        lightObj.SetActive(false); // ปิดไว้ก่อนตอนเริ่ม

        BoxCollider boxCol = root.AddComponent<BoxCollider>();
        boxCol.center = new Vector3(0f, 0.05f, 0.05f);
        boxCol.size = new Vector3(3.2f, 2.3f, 0.6f);

        CurtainInteraction curtainInter = root.AddComponent<CurtainInteraction>();
        SerializedObject curtainSO = new SerializedObject(curtainInter);
        
        SerializedProperty windowLightProp = curtainSO.FindProperty("windowLight");
        if (windowLightProp != null) windowLightProp.objectReferenceValue = lightObj;

        AudioClip soundClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Use SFX/curtain-closing-323254.mp3");
        SerializedProperty soundProp = curtainSO.FindProperty("curtainSound");
        if (soundProp != null && soundClip != null) soundProp.objectReferenceValue = soundClip;

        curtainSO.ApplyModifiedProperties();

        BedroomInteractable bedInter = root.AddComponent<BedroomInteractable>();
        bedInter.promptMessage = "เปิดผ้าม่าน";
        if (bedInter.onInteract == null) bedInter.onInteract = new UnityEvent();

        UnityEventTools.AddPersistentListener(bedInter.onInteract, new UnityAction(curtainInter.ToggleCurtain));

        root.transform.position = spawnPos;
        root.transform.rotation = spawnRot;

        string prefabPath = "Assets/Prefabs/Cozy_Window_Set.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);

        Selection.activeGameObject = root;
        Debug.Log($"✨ [CozyWindowSetup] สร้างหน้าต่างพร้อมแสงแดดธรรมชาติเรียบร้อยครับ! {prefabPath}", root);
    }
}
#endif
