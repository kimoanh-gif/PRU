using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelButton_Tung : MonoBehaviour
{
    // Hàm nhận vào tên màn chơi để chuyển cảnh
    public void LoadSceneByName(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Lỗi: Bạn chưa nhập tên Scene vào nút bấm rồi!");
        }
    }
}