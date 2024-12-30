using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton to access globally

    public Image airSlider;
    public GameObject respawnMessage;

    private void Awake()
    {
        // Configure singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateAirSlider(float currentAir)
    {
        if (this.airSlider != null)
        {
            this.airSlider.fillAmount = currentAir/100;
        }
    }

    public void ShowRespawnMessage(bool show)
    {
        if (respawnMessage != null)
        {
            respawnMessage.SetActive(show);
        }
    }
}
