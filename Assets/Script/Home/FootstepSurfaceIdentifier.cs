using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ประเภทพื้นผิวสำหรับระบบเสียงฝีเท้า
/// </summary>
public enum FootstepSurfaceType
{
    Concrete, // คอนกรีต / ถนน / ทางเท้า / พื้นแข็งทั่วไป
    Grass,    // พื้นหญ้า
    Dirt,     // พื้นดิน / ทางดินในสวน
    Wood      // พื้นไม้
}

/// <summary>
/// คอมโพเนนต์เสริมสำหรับติดที่ GameObject พื้นผิว เพื่อระบุประเภทเสียงฝีเท้าโดยตรง (Override)
/// </summary>
[DisallowMultipleComponent]
public class GroundSurface : MonoBehaviour
{
    [Tooltip("ระบุประเภทพื้นผิวของวัตถุนี้โดยตรง")]
    public FootstepSurfaceType surfaceType = FootstepSurfaceType.Concrete;
}

/// <summary>
/// ระบบวิเคราะห์และจำแนกประเภทพื้นผิวจาก Collider, ชื่อวัตถุ, ลำดับชั้น Hierarchy และ Material
/// </summary>
public static class FootstepSurfaceIdentifier
{
    public static FootstepSurfaceType DetectSurface(Collider col, Transform playerTransform = null)
    {
        if (col == null)
        {
            return GetSceneDefaultSurface();
        }

        // 1. ตรวจสอบคอมโพเนนต์ GroundSurface โดยตรง
        GroundSurface explicitSurface = col.GetComponentInParent<GroundSurface>();
        if (explicitSurface != null)
        {
            return explicitSurface.surfaceType;
        }

        // 2. ตรวจสอบ Tag ของวัตถุ (เปรียบเทียบสตริงโดยตรง ไม่เรียก CompareTag เพื่อเลี่ยง error ในกรณีที่ Tag ยังไม่ได้ลงทะเบียน)
        string objTag = col.gameObject.tag;
        if (objTag == "Grass") return FootstepSurfaceType.Grass;
        if (objTag == "Dirt") return FootstepSurfaceType.Dirt;
        if (objTag == "Wood") return FootstepSurfaceType.Wood;
        if (objTag == "Concrete") return FootstepSurfaceType.Concrete;

        // 3. ตรวจสอบ PhysicMaterial หากมี
        if (col.sharedMaterial != null)
        {
            string matName = col.sharedMaterial.name.ToLower();
            if (matName.Contains("grass") || matName.Contains("turf")) return FootstepSurfaceType.Grass;
            if (matName.Contains("dirt") || matName.Contains("mud") || matName.Contains("sand") || matName.Contains("gravel")) return FootstepSurfaceType.Dirt;
            if (matName.Contains("wood")) return FootstepSurfaceType.Wood;
            if (matName.Contains("concrete") || matName.Contains("rock") || matName.Contains("stone") || matName.Contains("tile")) return FootstepSurfaceType.Concrete;
        }

        // 4. ตรวจสอบชื่อวัตถุ และชื่อของ Parent ทุกระดับชั้นขึ้นไปจนถึง Root
        Transform current = col.transform;
        string combinedHierarchyNames = "";
        int depth = 0;
        while (current != null && depth < 6)
        {
            combinedHierarchyNames += " " + current.name.ToLower();
            current = current.parent;
            depth++;
        }

        // ตรวจสอบหญ้า (Grass)
        if (combinedHierarchyNames.Contains("grass") ||
            combinedHierarchyNames.Contains("lawn") ||
            combinedHierarchyNames.Contains("turf") ||
            combinedHierarchyNames.Contains("schoolgrass") ||
            combinedHierarchyNames.Contains("flower") ||
            combinedHierarchyNames.Contains("hedge"))
        {
            return FootstepSurfaceType.Grass;
        }

        // ตรวจสอบดิน (Dirt)
        if (combinedHierarchyNames.Contains("dirt") ||
            combinedHierarchyNames.Contains("mud") ||
            combinedHierarchyNames.Contains("earth") ||
            combinedHierarchyNames.Contains("sand") ||
            combinedHierarchyNames.Contains("gravel") ||
            combinedHierarchyNames.Contains("foliage_path") ||
            combinedHierarchyNames.Contains("path_center") ||
            combinedHierarchyNames.Contains("path_edge") ||
            combinedHierarchyNames.Contains("path_corner"))
        {
            return FootstepSurfaceType.Dirt;
        }

        // ตรวจสอบไม้ (Wood)
        if (combinedHierarchyNames.Contains("wood") ||
            combinedHierarchyNames.Contains("timber") ||
            combinedHierarchyNames.Contains("parquet"))
        {
            return FootstepSurfaceType.Wood;
        }

        // ตรวจสอบคอนกรีต / ถนน / พื้นแข็ง (Concrete)
        if (combinedHierarchyNames.Contains("road") ||
            combinedHierarchyNames.Contains("street") ||
            combinedHierarchyNames.Contains("concrete") ||
            combinedHierarchyNames.Contains("stationfloor") ||
            combinedHierarchyNames.Contains("schoolfloor") ||
            combinedHierarchyNames.Contains("school_misc_floor") ||
            combinedHierarchyNames.Contains("asphalt") ||
            combinedHierarchyNames.Contains("stone") ||
            combinedHierarchyNames.Contains("rock") ||
            combinedHierarchyNames.Contains("tile") ||
            combinedHierarchyNames.Contains("pavement") ||
            combinedHierarchyNames.Contains("sidewalk") ||
            combinedHierarchyNames.Contains("home_misc_floor") ||
            combinedHierarchyNames.Contains("floor"))
        {
            return FootstepSurfaceType.Concrete;
        }

        // 5. ตรวจสอบ Material จาก Renderer
        Renderer rend = col.GetComponent<Renderer>();
        if (rend != null && rend.sharedMaterial != null)
        {
            string rendMatName = rend.sharedMaterial.name.ToLower();
            if (rendMatName.Contains("grass") || rendMatName.Contains("lawn")) return FootstepSurfaceType.Grass;
            if (rendMatName.Contains("dirt") || rendMatName.Contains("earth") || rendMatName.Contains("mud")) return FootstepSurfaceType.Dirt;
            if (rendMatName.Contains("wood")) return FootstepSurfaceType.Wood;
            if (rendMatName.Contains("concrete") || rendMatName.Contains("road") || rendMatName.Contains("rock") || rendMatName.Contains("stone") || rendMatName.Contains("tile")) return FootstepSurfaceType.Concrete;
        }

        return GetSceneDefaultSurface();
    }

    private static FootstepSurfaceType GetSceneDefaultSurface()
    {
        string scene = SceneManager.GetActiveScene().name;
        if (string.IsNullOrEmpty(scene)) return FootstepSurfaceType.Concrete;

        if (scene.Equals("OutSide", System.StringComparison.OrdinalIgnoreCase))
        {
            return FootstepSurfaceType.Concrete;
        }
        if (scene.Equals("School", System.StringComparison.OrdinalIgnoreCase))
        {
            return FootstepSurfaceType.Concrete;
        }
        if (scene.Equals("Home", System.StringComparison.OrdinalIgnoreCase))
        {
            return FootstepSurfaceType.Concrete;
        }

        return FootstepSurfaceType.Concrete;
    }
}
