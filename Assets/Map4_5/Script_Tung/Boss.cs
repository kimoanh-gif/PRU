using UnityEngine;

public class Boss : StateMachineBehaviour
{
    [Tooltip("Gõ chính xác tên Object con của Boss muốn bật lên (DiChuyenBoss, TanCongBoss, DanBoss, ChetBoss)")]
    [SerializeField] private string tenSpriteMuonBat;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (Transform child in animator.transform)
        {
            if (child.name == tenSpriteMuonBat)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                // Chỉ tắt các object trạng thái hình ảnh của Boss
                if (child.name == "DiChuyenBoss" || child.name == "TanCongBoss" || child.name == "DanBoss" || child.name == "ChetBoss")
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
}