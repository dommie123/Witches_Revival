using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;

    [SerializeField] private TMP_Text survivorsText;
    [SerializeField] private Transform doorIcon;

    private Transform survivorSlot;
    private Transform informationPanel;

    private void Awake() 
    {
        instance = this;
        survivorSlot = transform.Find("Info Panel").Find("Survivor Slot");
        informationPanel = transform.Find("Info Panel");
        GameManager.instance.HUDManagerIsAwake();
    }

    public void UpdateSurvivorText(int survivorsEscaped, int survivorsAlive)
    {
        survivorsText.text = $"{survivorsEscaped} / {survivorsAlive}";
    }

    public void UpdateSelectedSurvivors(List<Survivor> selectedSurvivors, List<HidingSpot> selectedHidingSpots)
    {
        foreach (Transform child in informationPanel)
        {
            if (child == survivorSlot || child == survivorsText.transform || child == doorIcon) continue;
            Destroy(child.gameObject);
        }

        float x = 0.5f;
        float y = -2.4f;
        float survivorSlotCellSize = 11f;

        foreach (Survivor survivor in selectedSurvivors)
        {
            RectTransform survivorSlotRectTransform = Instantiate(survivorSlot, informationPanel).GetComponent<RectTransform>();
            survivorSlotRectTransform.gameObject.SetActive(true);

            survivorSlotRectTransform.anchoredPosition = new Vector2(x * survivorSlotCellSize, y * survivorSlotCellSize);
            Image img = survivorSlotRectTransform.GetComponent<Image>();
            img.sprite = survivor.GetPortrait();

            x++;
            if (x >= 6)
            {
                x = 0;
                y--;
            }
        }

        foreach (HidingSpot hidingSpot in selectedHidingSpots)
        {
            RectTransform hidingSpotSlotRectTransform = Instantiate(survivorSlot, informationPanel).GetComponent<RectTransform>();
            hidingSpotSlotRectTransform.gameObject.SetActive(true);

            hidingSpotSlotRectTransform.anchoredPosition = new Vector2(x * survivorSlotCellSize, y * survivorSlotCellSize);
            Image img = hidingSpotSlotRectTransform.GetComponent<Image>();
            img.sprite = hidingSpot.GetPortrait();

            x++;
            if (x >= 6)
            {
                x = 0;
                y--;
            }
        }
    }
}