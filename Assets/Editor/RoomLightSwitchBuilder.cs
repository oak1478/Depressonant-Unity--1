#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine.Events;

public class RoomLightSwitchBuilder : MonoBehaviour
{
    [MenuItem("Tools/สร้างหลอดไฟเพดานพร้อมสวิตช์เปิดปิด (Build Room Light & Wall Switch)")]
    public static void BuildRoomLightAndSwitch()
    {
        // 1. ตรวจสอบโฟลเดอร์สำหรับเซฟ Prefab
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // ลบชุดไฟเพดานเดิม (ถ้ามี)
        GameObject oldLightSet = GameObject.Find("Room_Ceiling_Light_Set");
        if (oldLightSet != null) Undo.DestroyObjectImmediate(oldLightSet);

        GameObject oldSwitch = GameObject.Find("Wall_Light_Switch");
        if (oldSwitch != null) Undo.DestroyObjectImmediate(oldSwitch);

        // 2. สร้าง หลอดไฟเพดานหลักกลางห้อง (Ceiling Light)
        GameObject ceilingLightGroup = new GameObject("Room_Ceiling_Light_Set");
        Undo.RegisterCreatedObjectUndo(ceilingLightGroup, "Create Room Ceiling Light");
        ceilingLightGroup.transform.position = new Vector3(1.5f, 3.2f, -9.8f);

        // โคมไฟเพดาน (Fixture Visual)
        GameObject lampMesh = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        lampMesh.name = "Ceiling_Lamp_Fixture";
        lampMesh.transform.SetParent(ceilingLightGroup.transform, false);
        lampMesh.transform.localPosition = Vector3.zero;
        lampMesh.transform.localScale = new Vector3(0.5f, 0.05f, 0.5f);
        DestroyImmediate(lampMesh.GetComponent<Collider>());

        Material whiteMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/WindowAndDoorsPack01/Materials/PureWhite.mat");
        if (whiteMat == null) whiteMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/OuterFile/Object/Outdoor/White.mat");
        if (whiteMat != null) lampMesh.GetComponent<MeshRenderer>().sharedMaterial = whiteMat;

        // ดวงไฟสว่างทั่วห้อง (Point Light)
        GameObject lightObj = new GameObject("Main_Ceiling_Light");
        lightObj.transform.SetParent(ceilingLightGroup.transform, false);
        lightObj.transform.localPosition = new Vector3(0f, -0.2f, 0f);

        Light mainLight = lightObj.AddComponent<Light>();
        mainLight.type = LightType.Point;
        mainLight.color = new Color(1f, 0.95f, 0.88f, 1f); // Warm White / Cozy Bright
        mainLight.intensity = 3.8f; // สว่างชัดเจนเห็นทั่วทั้งห้อง
        mainLight.range = 22f;      // ครอบคลุมรัศมีทั้งห้องนอน
        mainLight.shadows = LightShadows.Soft;

        // 3. สร้าง สวิตช์ไฟติดผนังข้างประตู (Wall Light Switch)
        GameObject switchObj = new GameObject("Wall_Light_Switch");
        Undo.RegisterCreatedObjectUndo(switchObj, "Create Wall Light Switch");
        // วางข้างประตูห้องนอนที่ระดับสายตา/มือ
        switchObj.transform.position = new Vector3(0.1f, 1.25f, -6.85f);
        switchObj.transform.rotation = Quaternion.identity;

        // แป้นสวิตช์สีขาว (Base Plate)
        GameObject basePlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        basePlate.name = "Switch_BasePlate";
        basePlate.transform.SetParent(switchObj.transform, false);
        basePlate.transform.localPosition = Vector3.zero;
        basePlate.transform.localScale = new Vector3(0.12f, 0.16f, 0.02f);
        if (whiteMat != null) basePlate.GetComponent<MeshRenderer>().sharedMaterial = whiteMat;
        DestroyImmediate(basePlate.GetComponent<Collider>());

        // ปุ่มกดสวิตช์ (Rocker Button)
        GameObject rockerBtn = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rockerBtn.name = "Switch_RockerButton";
        rockerBtn.transform.SetParent(switchObj.transform, false);
        rockerBtn.transform.localPosition = new Vector3(0f, 0f, 0.015f);
        rockerBtn.transform.localScale = new Vector3(0.05f, 0.08f, 0.02f);
        DestroyImmediate(rockerBtn.GetComponent<Collider>());

        Material blackMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/WindowAndDoorsPack01/Materials/Black.mat");
        if (blackMat != null) rockerBtn.GetComponent<MeshRenderer>().sharedMaterial = blackMat;

        // 4. ใส่ BoxCollider บนสวิตช์เพื่อให้เล็งกด F ได้ง่าย
        BoxCollider switchCol = switchObj.AddComponent<BoxCollider>();
        switchCol.center = Vector3.zero;
        switchCol.size = new Vector3(0.35f, 0.35f, 0.2f);

        // 5. ใส่ RoomLightSwitch บนสวิตช์
        RoomLightSwitch lightSwitch = switchObj.AddComponent<RoomLightSwitch>();
        SerializedObject switchSO = new SerializedObject(lightSwitch);

        SerializedProperty lightsArray = switchSO.FindProperty("ceilingLights");
        if (lightsArray != null)
        {
            lightsArray.arraySize = 1;
            lightsArray.GetArrayElementAtIndex(0).objectReferenceValue = mainLight;
        }

        AudioClip switchSFX = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/select_1.wav");
        if (switchSFX == null) switchSFX = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Use SFX/select_1.wav");
        SerializedProperty soundProp = switchSO.FindProperty("switchSound");
        if (soundProp != null && switchSFX != null) soundProp.objectReferenceValue = switchSFX;

        SerializedProperty rockerProp = switchSO.FindProperty("switchTogglePart");
        if (rockerProp != null) rockerProp.objectReferenceValue = rockerBtn.transform;

        switchSO.ApplyModifiedProperties();

        // 6. ใส่ BedroomInteractable บนสวิตช์
        BedroomInteractable switchInteractable = switchObj.AddComponent<BedroomInteractable>();
        switchInteractable.promptMessage = "ปิดไฟห้อง";
        if (switchInteractable.onInteract == null) switchInteractable.onInteract = new UnityEvent();

        // ผูก Event OnInteract -> RoomLightSwitch.ToggleLight
        UnityEventTools.AddPersistentListener(switchInteractable.onInteract, new UnityAction(lightSwitch.ToggleLight));

        Selection.activeGameObject = switchObj;
        Debug.Log("💡✨ [RoomLightSwitch] สร้างหลอดไฟเพดานสว่างทั่วห้องพร้อมสวิตช์เปิดปิดข้างประตูสำเร็จเรียบร้อยครับ!");
    }
}
#endif
