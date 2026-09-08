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

    [Header("ระบบเสียงฝีเท้า (Footstep Audio)")]
    [Tooltip("เปิด/ปิดระบบเสียงฝีเท้า")]
    public bool enableFootstepSounds = true;

    [Tooltip("ระยะห่างเวลาการก้าวเดินปกติ (วินาที)")]
    public float walkStepInterval = 0.42f;

    [Tooltip("ระยะห่างเวลาการก้าววิ่ง (วินาที)")]
    public float runStepInterval = 0.28f;

    [Range(0f, 1f)]
    [Tooltip("ระดับความดังเสียงตอนเดินปกติ")]
    public float walkStepVolume = 0.6f;

    [Range(0f, 1f)]
    [Tooltip("ระดับความดังเสียงตอนวิ่ง")]
    public float runStepVolume = 0.85f;

    [Tooltip("ความถี่เสียงสุ่มต่ำสุด (Pitch Min)")]
    public float pitchMin = 0.92f;

    [Tooltip("ความถี่เสียงสุ่มสูงสุด (Pitch Max)")]
    public float pitchMax = 1.08f;

    [Tooltip("AudioSource สำหรับเสียงฝีเท้า (หากว่างไว้ระบบจะค้นหาหรือสร้างให้อัตโนมัติ)")]
    public AudioSource footstepAudioSource;

    [Header("ชุดเสียงเดิน (Walk Clips - หากว่างจะโหลดจาก Resources ให้อัตโนมัติ)")]
    public AudioClip[] dirtWalkClips;
    public AudioClip[] grassWalkClips;
    public AudioClip[] concreteWalkClips;
    public AudioClip[] woodWalkClips;

    [Header("ชุดเสียงวิ่ง (Run Clips - หากว่างจะโหลดจาก Resources ให้อัตโนมัติ)")]
    public AudioClip[] dirtRunClips;
    public AudioClip[] grassRunClips;
    public AudioClip[] concreteRunClips;
    public AudioClip[] woodRunClips;

    private CharacterController controller;
    private Vector3 velocity;
    private float gravity = -9.81f;
    private float baseScaleX = 1f;
    private bool isFacingRight = false;

    private float stepTimer = 0f;
    private int lastPlayedIndex = -1;
    private Collider lastGroundCollider = null;
    private DialogueManager cachedDialogueManager = null;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        PlayerSpawner.ApplySpawning(gameObject);

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

        // ตั้งค่า AudioSource สำหรับเสียงฝีเท้า
        if (footstepAudioSource == null)
        {
            footstepAudioSource = GetComponent<AudioSource>();
            if (footstepAudioSource == null)
            {
                footstepAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        if (footstepAudioSource != null)
        {
            footstepAudioSource.playOnAwake = false;
            footstepAudioSource.loop = false;
            footstepAudioSource.spatialBlend = 0f;
        }

        cachedDialogueManager = Object.FindAnyObjectByType<DialogueManager>();

        LoadFootstepClipsIfNeeded();
    }

    private void LoadFootstepClipsIfNeeded()
    {
        if (dirtWalkClips == null || dirtWalkClips.Length == 0) dirtWalkClips = Resources.LoadAll<AudioClip>("Audio/Footsteps/Dirt/Walk");
        if (dirtRunClips == null || dirtRunClips.Length == 0) dirtRunClips = Resources.LoadAll<AudioClip>("Audio/Footsteps/Dirt/Run");
        if (grassWalkClips == null || grassWalkClips.Length == 0) grassWalkClips = Resources.LoadAll<AudioClip>("Audio/Footsteps/Grass/Walk");
        if (grassRunClips == null || grassRunClips.Length == 0) grassRunClips = Resources.LoadAll<AudioClip>("Audio/Footsteps/Grass/Run");
        if (concreteWalkClips == null || concreteWalkClips.Length == 0) concreteWalkClips = Resources.LoadAll<AudioClip>("Audio/Footsteps/Concrete/Walk");
        if (concreteRunClips == null || concreteRunClips.Length == 0) concreteRunClips = Resources.LoadAll<AudioClip>("Audio/Footsteps/Concrete/Run");
        if (woodWalkClips == null || woodWalkClips.Length == 0) woodWalkClips = Resources.LoadAll<AudioClip>("Audio/Footsteps/Wood/Walk");
        if (woodRunClips == null || woodRunClips.Length == 0) woodRunClips = Resources.LoadAll<AudioClip>("Audio/Footsteps/Wood/Run");
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y > 0.4f)
        {
            lastGroundCollider = hit.collider;
        }
    }

    void Update()
    {
        // ตรวจสอบว่าหน้าต่าง Console เปิดอยู่ หรือกำลังสนทนาอยู่หรือไม่
        bool isLocked = (DevConsole.Instance != null && DevConsole.Instance.IsOpen);
        if (!isLocked)
        {
            if (cachedDialogueManager == null)
            {
                cachedDialogueManager = Object.FindAnyObjectByType<DialogueManager>();
            }
            if (cachedDialogueManager != null && cachedDialogueManager.IsDialogueActive())
            {
                isLocked = true;
            }
        }

        if (isLocked)
        {
            if (animator != null && animator.enabled)
            {
                animator.enabled = false;
            }
            if (spriteRenderer != null && idleSprite != null)
            {
                spriteRenderer.sprite = idleSprite;
            }

            stepTimer = 0f;

            // รักษาแรงโน้มถ่วงให้ตัวละครยืนติดพื้น ไม่ลอยเคว้ง
            if (controller != null)
            {
                if (controller.isGrounded && velocity.y < 0)
                {
                    velocity.y = -2f;
                }
                velocity.y += gravity * Time.deltaTime;
                controller.Move(velocity * Time.deltaTime);
            }
            return;
        }

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

        // ระบบ Mirror Horizontally
        bool shouldMirror = invertMirror ? isFacingRight : !isFacingRight;

        if (useFlipX && spriteRenderer != null)
        {
            spriteRenderer.flipX = shouldMirror;
            if (visualTransform != null)
            {
                Vector3 s = visualTransform.localScale;
                s.x = baseScaleX;
                visualTransform.localScale = s;
            }
        }
        else
        {
            if (visualTransform != null)
            {
                Vector3 s = visualTransform.localScale;
                s.x = (shouldMirror ? -1f : 1f) * baseScaleX;
                visualTransform.localScale = s;
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = false;
            }
        }

        // ควบคุมแอนิเมชัน และสลับกลับภาพ Idle ทันทีที่หยุดเดิน
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

        // ระบบเล่นเสียงฝีเท้าตามประเภทของพื้นผิว
        if (enableFootstepSounds && controller != null && controller.isGrounded && isMoving)
        {
            float interval = isRunning ? runStepInterval : walkStepInterval;
            stepTimer += Time.deltaTime;
            if (stepTimer >= interval)
            {
                PlayFootstepSound(isRunning);
                stepTimer = 0f;
            }
        }
        else
        {
            // เมื่อหยุดเดิน ให้ตั้งเวลาเตรียมพร้อมสำหรับก้าวแรกเมื่อเริ่มเดินใหม่
            if (!isMoving)
            {
                stepTimer = (isRunning ? runStepInterval : walkStepInterval) * 0.85f;
            }
        }
    }

    private void PlayFootstepSound(bool running)
    {
        if (footstepAudioSource == null) return;

        // ตรวจสอบ Collider ของพื้นใต้เท้าด้วย Raycast ลงล่าง
        Collider groundCol = null;
        Ray ray = new Ray(transform.position + Vector3.up * 0.2f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1.5f, ~0, QueryTriggerInteraction.Ignore))
        {
            groundCol = hit.collider;
        }
        else if (lastGroundCollider != null)
        {
            groundCol = lastGroundCollider;
        }

        FootstepSurfaceType surface = FootstepSurfaceIdentifier.DetectSurface(groundCol, transform);

        AudioClip[] clipsToUse = GetClipsForSurface(surface, running);
        if (clipsToUse == null || clipsToUse.Length == 0)
        {
            clipsToUse = running ? concreteRunClips : concreteWalkClips;
        }

        if (clipsToUse != null && clipsToUse.Length > 0)
        {
            int index = 0;
            if (clipsToUse.Length > 1)
            {
                do
                {
                    index = Random.Range(0, clipsToUse.Length);
                } while (index == lastPlayedIndex && clipsToUse.Length > 2);
            }
            lastPlayedIndex = index;

            AudioClip clip = clipsToUse[index];
            if (clip != null)
            {
                footstepAudioSource.pitch = Random.Range(pitchMin, pitchMax);
                float vol = running ? runStepVolume : walkStepVolume;
                footstepAudioSource.PlayOneShot(clip, vol);
            }
        }
    }

    private AudioClip[] GetClipsForSurface(FootstepSurfaceType surface, bool running)
    {
        switch (surface)
        {
            case FootstepSurfaceType.Grass:
                return running ? grassRunClips : grassWalkClips;
            case FootstepSurfaceType.Dirt:
                return running ? dirtRunClips : dirtWalkClips;
            case FootstepSurfaceType.Wood:
                return running ? woodRunClips : woodWalkClips;
            case FootstepSurfaceType.Concrete:
            default:
                return running ? concreteRunClips : concreteWalkClips;
        }
    }
}
