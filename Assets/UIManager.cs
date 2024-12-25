using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton to access globally

    public Slider airSlider;
    public GameObject respawnMessage;

    private void Awake()
    {
        // Configure singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateAirSlider(float currentAir, float maxAir)
    {
        if (this.airSlider != null)
        {
            this.airSlider.maxValue = maxAir;
            this.airSlider.value = currentAir;
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
