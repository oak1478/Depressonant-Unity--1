using UnityEngine;
// ใส่บรรทัดนี้เพิ่มเพื่อให้ระบบรู้จัก Input System แบบใหม่
using UnityEngine.InputSystem; 

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private CharacterController controller;
    private Vector3 velocity;
    private float gravity = -9.81f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        PlayerSpawner.ApplySpawning(gameObject);
    }

    void Update()
    {
        float horizontal = 0f;
        float vertical = 0f;

        // ระบบอ่านปุ่มกดแบบใหม่ (Input System)
        if (Keyboard.current != null)
        {
            // ตรวจสอบการกดปุ่ม A / D หรือ ลูกศร ซ้าย / ขวา
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal = 1f;

            // ตรวจสอบการกดปุ่ม W / S หรือ ลูกศร ขึ้น / ลง
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical = -1f;
        }

        // คำนวณทิศทางการเดิน
        Vector3 move = new Vector3(horizontal, 0f, vertical).normalized;

        if (move.magnitude >= 0.1f)
        {
            controller.Move(move * speed * Time.deltaTime);
        }

        // คำนวณแรงโน้มถ่วง
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}