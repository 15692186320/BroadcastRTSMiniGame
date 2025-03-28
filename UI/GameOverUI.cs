using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    private void Start()
    {
        DOTSEventManager.Instance.onHQDead += DOTSEventManager_onHQDead;
        Hide();
    }

    private void DOTSEventManager_onHQDead(object sender, System.EventArgs e)
    {
       Show();
        Time.timeScale = 0f;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
