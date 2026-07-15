using UnityEngine;

public class Nhen : StateMachineBehaviour
{
    [Tooltip("Gõ chính xác tên Object con của Nhện muốn bật lên (DiChuyen, TanCong, Dan, Chet)")]
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
                // Chỉ tắt các object trạng thái hình ảnh của Nhện
                if (child.name == "DiChuyen" || child.name == "TanCong" || child.name == "Dan" || child.name == "Chet")
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
}