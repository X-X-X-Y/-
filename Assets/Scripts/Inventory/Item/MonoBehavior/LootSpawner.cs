using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [System.Serializable]                   //窗口可编辑
    public class LootItem
    {
        public GameObject item;

        [Range(0,1)]
        public float weight;                //掉落概率
    }

    public LootItem[] lootItems;            //掉落物品的列表

    public void SpawnLoot()                  //生成战利品
    {
        float currentValue = Random.value;   //当前数值随机生成

        for (int i = 0; i < lootItems.Length; i++)
        {
            if (currentValue <= lootItems[i].weight)    //物品掉落概率与当前生成的随机数值相比较
            {
                GameObject obj = Instantiate(lootItems[i].item);
                obj.transform.position = transform.position + Vector3.up * 2;
                break;                                  //一次掉落一个物品
            }
        }
    }


}
