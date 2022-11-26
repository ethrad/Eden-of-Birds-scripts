using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FoodController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    Vector3 DefaultPos;
    public string foodName;
    public int plateIndex;

    public void UpdateFood(int plateIndex, string recipeName)
    {
        foodName = recipeName;
        this.plateIndex = plateIndex;
        GetComponent<Image>().sprite = Resources.Load<Sprite>("Dots/Tycoon/" + foodName);
    }

    public void ResetPlate()
    {
        BarPlateManager.instance.ResetPlate(plateIndex);
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        DefaultPos = this.transform.position;
        GetComponent<Image>().raycastTarget = false;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector3 currentPos = Camera.main.ScreenToWorldPoint(eventData.position);
        currentPos.z = 90f;
        currentPos.y -= 160f;
        this.transform.position = currentPos;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        this.transform.position = DefaultPos;
        GetComponent<Image>().raycastTarget = true;
    }
}
