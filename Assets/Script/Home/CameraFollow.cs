using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // ช่องสำหรับลากตัวละครหลักที่เราต้องการให้กล้องวิ่งตาม
    public Transform target; 
    
    // ระยะห่างของกล้องจากตัวละคร (X, Y, Z) ปรับแต่งได้ตามใจชอบใน Inspector
    public Vector3 offset = new Vector3(0f, 5f, -7f); 
    
    // ความนุ่มนวลในการเลื่อนตาม (ยิ่งค่าน้อย กล้องจะยิ่งนุ่มและหน่วงตามเบาๆ)
    public float smoothSpeed = 0.125f; 

    void Start()
    {
        FindTargetIfMissing();
    }

    private float lastSearchTime = -1f;
    private void FindTargetIfMissing()
    {
        if (target == null && (lastSearchTime < 0f || Time.time - lastSearchTime > 0.5f))
        {
            lastSearchTime = Time.time;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) player = GameObject.Find("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindTargetIfMissing();
        }

        if (target != null)
        {
            // คำนวณตำแหน่งที่กล้องควรจะไปอยู่
            Vector3 desiredPosition = target.position + offset;
            
            // ทำให้กล้องเลื่อนตำแหน่งอย่างนุ่มนวลจากจุดปัจจุบันไปจุดใหม่
            float t = 1f - Mathf.Exp(-smoothSpeed * 25f * Time.deltaTime);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, t);
            transform.position = smoothedPosition;

            // สั่งให้กล้องหันหน้าโฟกัสไปที่ตัวละครตลอดเวลา
            transform.LookAt(target.position + Vector3.up); 
        }
    }
}