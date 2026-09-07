using UnityEngine;
// ใส่บรรทัดนี้เพิ่มเพื่อให้ระบบรู้จัก Input System แบบใหม่
using UnityEngine.InputSystem; 

public class PlayerMovement : MonoBehaviour
{
    [Header("ความเร็วในการเคลื่อนที่")]
    public float speed = 5f;
    [Tooltip("ตัวคูณความเร็วเมื่อกดปุ่ม Shift ค้าง (เช่น 2 = เดินเร็วขึ้น 2 เท่า)")]
    public float runMultiplier = 2f;

    [Header("อ้างอิงภาพและแอนิเมชัน")]
    [Tooltip("ใส่ Transform ของรูปภาพ (เช่น PlayerVisual) สำหรับสั่ง Mirror Scale X")]
    public Transform visualTransform;

    [Tooltip("ใส่ SpriteRenderer ของตัวละคร")]
    public SpriteRenderer spriteRenderer;

    [Tooltip("ใส่ Animator ของตัวละคร")]
    public Animator animator;

    [Tooltip("ภาพท่ายืนนิ่ง (Idle) ถ้าปล่อยว่างระบบจะจำภาพแรกเริ่มให้อัตโนมัติ")]
    public Sprite idleSprite;

    [Tooltip("ติ๊กช่องนี้เพื่อสลับด้านการ Mirror ซ้าย-ขวา")]
    public bool invertMirror = false;

    [Tooltip("แนะนำเปิดไว้: ใช้ FlipX แสงจาก Directional Light จะไม่เพี้ยนและสีไม่ตก")]
    public bool useFlipX = true;

    private CharacterController controller;
    private Vector3 velocity;
    private float gravity = -9.81f;
    private float baseScaleX = 1f;
    private bool isFacingRight = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (visualTransform == null && spriteRenderer != null)
        {
            visualTransform = spriteRenderer.transform;
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (visualTransform != null)
        {
            baseScaleX = Mathf.Abs(visualTransform.localScale.x);
            if (baseScaleX == 0f) baseScaleX = 1f;
        }

        // จำภาพ Idle เริ่มต้นไว้เพื่อใช้แสดงตอนหยุดเดิน
        if (idleSprite == null && spriteRenderer != null)
        {
            idleSprite = spriteRenderer.sprite;
        }
    }

    void Update()
    {
        float horizontal = 0f;
        float vertical = 0f;
        bool isRunning = false;

        // ระบบอ่านปุ่มกดแบบใหม่ (Input System)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal = 1f;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical = -1f;

            if (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed)
            {
                isRunning = true;
            }
        }

        Vector3 move = new Vector3(horizontal, 0f, vertical).normalized;
        bool isMoving = move.magnitude >= 0.1f;

        // กำหนดทิศทางการหันหน้า
        if (horizontal > 0f)
        {
            isFacingRight = true;
        }
        else if (horizontal < 0f)
        {
            isFacingRight = false;
        }
        else if (vertical > 0f) // เดินขึ้น (ให้หันขวาเหมือนตอนเดินขวา)
        {
            isFacingRight = true;
        }
        else if (vertical < 0f) // เดินลง (ให้หันซ้ายเหมือนตอนเดินซ้าย)
        {
            isFacingRight = false;
        }

        // 🔥 ระบบ Mirror Horizontally (กลับภาพแนวนอนจริงผ่าน Scale X)
        bool shouldMirror = invertMirror ? isFacingRight : !isFacingRight;
        
        if (visualTransform != null)
        {
            Vector3 s = visualTransform.localScale;
            s.x = (shouldMirror ? -1f : 1f) * baseScaleX;
            visualTransform.localScale = s;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = false; // ปิดไว้เพื่อไม่ให้ตีกับ Scale
        }

        // 🔥 ควบคุมแอนิเมชัน และสลับกลับภาพ Idle ทันทีที่หยุดเดิน
        if (isMoving)
        {
            if (animator != null)
            {
                animator.enabled = true;
                animator.speed = isRunning ? 1.5f : 1f;
            }
        }
        else
        {
            // หยุดเดิน: ปิด Animator ชั่วคราว แล้วแสดงภาพ Idle ทันที
            if (animator != null)
            {
                animator.enabled = false;
            }
            if (spriteRenderer != null && idleSprite != null)
            {
                spriteRenderer.sprite = idleSprite;
            }
        }

        // คำนวณความเร็วและการเคลื่อนที่
        float currentSpeed = isRunning ? (speed * runMultiplier) : speed;

        if (isMoving)
        {
            controller.Move(move * currentSpeed * Time.deltaTime);
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