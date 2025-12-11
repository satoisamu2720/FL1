using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivisionBossEnemy : MonoBehaviour
{
    [Header("召集・全体攻撃")]
    public float gatherDuration = 2.0f;      // 集合時間
    public float waveAttackRadius = 8f;      // 全体攻撃の範囲
    public int waveAttackDamage = 1;         // ボスの全体攻撃ダメージ

    [Header("ギミック")]
    public bool gimmickReleased = false;     // ギミック解除されたら true
    public Transform safeZone;               // 安置場所の Transform（表示/透明化でOK）

    [Header("ミニ敵")]
    public List<SplitEnemy> minis = new List<SplitEnemy>();

    private bool isGathering = false;

    void Start()
    {
        // 安置は最初は無効化
        if (safeZone != null)
            safeZone.gameObject.SetActive(false);
    }

    void Update()
    {
        // テスト用：G キーで召集開始（あとで外してOK）
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(GatherAndAttack());
        }
    }

    // ================================================
    // 召集 → 全体攻撃 → 解散
    // ================================================
    IEnumerator GatherAndAttack()
    {
        if (isGathering) yield break;
        isGathering = true;

        RemoveDeadMinis();

        // ミニ敵に集合開始を通知
        foreach (var m in minis)
        {
            if (m != null)
                m.StartGathering(transform);
        }

        yield return new WaitForSeconds(gatherDuration);

        // 全体攻撃
        PerformWaveAttack();

        // ミニ敵解除
        foreach (var m in minis)
        {
            if (m != null)
                m.StopGathering();
        }

        isGathering = false;
    }

    // ================================================
    // 全体攻撃（ギミック解除で安置が出現）
    // ================================================
    void PerformWaveAttack()
    {
        Debug.Log("Boss 全体攻撃！");

        if (!gimmickReleased)
        {
            // ギミック未解除 → プレイヤーに大ダメージ
            var hit = Physics2D.OverlapCircleAll(transform.position, waveAttackRadius);
            foreach (var h in hit)
            {
                if (h.CompareTag("Player"))
                {
                    Player hp = h.GetComponent<Player>();
                    if (hp != null)
                        hp.TakeDamage(waveAttackDamage);
                }
            }
        }
        else
        {
            // ギミック解除 → 安置をオンにする
            if (safeZone != null)
                safeZone.gameObject.SetActive(true);
        }
    }

    // ================================================
    // 死んだミニ敵をリストから削除
    // ================================================
    void RemoveDeadMinis()
    {
        minis.RemoveAll(m => m == null);
    }

    // ギミック解除
    public void ReleaseGimmick()
    {
        gimmickReleased = true;
    }
}
