using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [System.Serializable]                   //���ڿɱ༭
    public class LootItem
    {
        public GameObject item;

        [Range(0,1)]
        public float weight;                //�������
    }

    public LootItem[] lootItems;            //������Ʒ���б�

    public void SpawnLoot()                  //����ս��Ʒ
    {
        float currentValue = Random.value;   //��ǰ��ֵ�������

        for (int i = 0; i < lootItems.Length; i++)
        {
            if (currentValue <= lootItems[i].weight)    //��Ʒ��������뵱ǰ���ɵ������ֵ��Ƚ�
            {
                GameObject obj = Instantiate(lootItems[i].item);
                obj.transform.position = transform.position + Vector3.up * 2;
                break;                                  //һ�ε���һ����Ʒ
            }
        }
    }


}
