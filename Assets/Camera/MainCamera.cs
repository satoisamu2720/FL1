using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UIElements;

public class MainCameraScript : MonoBehaviour
{
    public static MainCameraScript Instance { get; private set; }

    private Transform target;
    private Vector3 offset = new Vector3(0f, 0f, -15f);

    [SerializeField, Range(0.01f, 1f)]
    private float smoothSpeed = 0.125f; // �ʏ펞

    public float focusMoveSpeed = 5f;   // �t�H�[�J�X���̈ړ����x�i�傫������j
    private bool isFocusing = false;
    private float focusDuration = 5f;

    private Transform defaultTarget;
    private Vector3 focusPosition; // Destroy����Ă��ǂ�����W

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SetDefaultTarget(player.transform);
        }
    }

    void FixedUpdate()
    {
        if (isFocusing)
        {
            // Destroy����Ă��Ō�̍��W��ǂ�
            Vector3 desiredPosition = focusPosition + offset;
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, focusMoveSpeed * Time.fixedDeltaTime);
        }
        else if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    public void SetDefaultTarget(Transform newTarget)
    {
        defaultTarget = newTarget;
        target = newTarget;
    }

    public void FocusOn(Transform focusTarget, float duration = 3f)
    {
        if (!isFocusing)
        {
            focusDuration = duration;
            StartCoroutine(FocusCoroutine(focusTarget));
        }
    }

    private IEnumerator FocusCoroutine(Transform focusTarget)
    {
        isFocusing = true;
        GameManager.Instance.isPause = true;

        float timer = focusDuration;
        while (timer > 0f)
        {
            if (focusTarget != null)
            {
                focusPosition = focusTarget.position;
            }
            timer -= Time.deltaTime;
            yield return null;
        }

        // �v���C���[�ɖ߂�
        if (defaultTarget != null)
        {
            target = defaultTarget;
        }

        GameManager.Instance.isPause = false;
        isFocusing = false;
    }
    private IEnumerator FocusRoutine(Vector3 position, float duration)
    {
        isFocusing = true;
        focusPosition = position;

        float timer = duration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        isFocusing = false;
    }
    public void FocusOn(Vector3 worldPosition, float duration = 2f)
    {
        StartCoroutine(FocusRoutine(worldPosition, duration));
    }


}