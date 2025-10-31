using UnityEngine;

public class MapTransitionTrigger : MonoBehaviour
{
    public Vector3 cameraTargetPosition;  // ���̃}�b�v���S
    public Vector3 playerTargetPosition;  // �v���C���[�̐V�����ʒu
    public float transitionDelay = 0.5f;  // �X�N���[�����o�̊�

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Transition(other.transform));
        }
    }

    private System.Collections.IEnumerator Transition(Transform player)
    {
        isTransitioning = true;

        // �v���C���[�̑�����ꎞ��~�i��FPlayerMovement�X�N���v�g�������j
        var move = player.GetComponent<Player>();
        if (move != null) move.enabled = false;

        // �J�������ړ�
        Camera.main.GetComponent<RoomCamera>().MoveTo(cameraTargetPosition);

        // �����҂��Ă���v���C���[�ړ�
        yield return new WaitForSeconds(transitionDelay);
        player.position = playerTargetPosition;

        // �v���C���[�����߂�
        if (move != null) move.enabled = true;

        isTransitioning = false;
    }
}
