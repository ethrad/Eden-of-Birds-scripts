using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCropController : CropController
{
    public Sprite afterSprite;

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "player" && canHarvest == true)
        {
            DungeonManager.instance.OnHarvestButton(this.gameObject);
        }
    }

    protected override void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "player" && canHarvest == true)
        {
            DungeonManager.instance.OffHarvestButton();
        }
    }

    // 스프라이트 변화
    public override void Harvest()
    {
        int random = Random.Range(0, 100);

        int itemIndex = -1;
        int tempProb = 0;

        for (int i = 0; i < itemProb.Length; i++)
        {
            if (random <= itemProb[i] + tempProb)
            {
                itemIndex = i;
            }
            else break;

            tempProb += itemProb[i];
        }

        canHarvest = false;
        DungeonManager.instance.GetItem(itemNames[itemIndex]);
        GetComponent<SpriteRenderer>().sprite = afterSprite;
    }

}
