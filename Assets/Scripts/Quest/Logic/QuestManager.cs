using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManager : Singleton<QuestManager>
{
    [System.Serializable]
    public class QuestTask      //��ȡ����ʱ���������������-���ÿ�¡��ʽ����Ӱ�챾������
    {
        public QuestData_SO questData;  //�����������ݣ���֤�¿���Ϸ������������ݲ����޸ĵ�QuestTask�����޸�,���޸ĵĸ���QuestManager

        public bool IsStarted   //׷�ٽ���
        {
            get
            {
                return questData.isStarted;     //��ȥ��ǰ����
            }
            set
            {
                questData.isStarted = value;    //�޸ĵ�ǰ����
            }
        }
        public bool IsComplete
        {
            get
            {
                return questData.isComplete;
            }
            set
            {
                questData.isComplete = value;
            }
        }
        public bool IsFinished
        {
            get
            {
                return questData.isFinished;
            }
            set
            {
                questData.isFinished = value;
            }
        }
    }

    public List<QuestTask> tasks = new List<QuestTask>();

    private void Start()
    {
        LoadQuestManager();
    }

    public void LoadQuestManager()
    {
        var questCount = PlayerPrefs.GetInt("QuestCount");

        for (int i = 0; i < questCount; i++)    
        {
            //����һ���յ�QuestData_SO
            var newQuest = ScriptableObject.CreateInstance<QuestData_SO>();
            //�Ѷ�ȡ�����ݸ��ǵ��µ�
            SaveManager.Instance.Load(newQuest, "task" + i);
            //����һ���µ�QuestTask�б�
            tasks.Add(new QuestTask { questData = newQuest });
        }
    }

    //�����������
    public void SaveQuestManager()
    {
        PlayerPrefs.SetInt("QuestCount",tasks.Count);       //�ȱ����б������
        for (int i = 0; i < tasks.Count; i++)               //ÿһ��������һ�����ȥ����
        {
            SaveManager.Instance.Save(tasks[i].questData,"task" + i);
        }
    }

    //����������ʰȡ��Ʒʱ����
    public void UpdateQuestProgress( string requireName , int amount )                   //�����������
    {
        foreach (var task in tasks) //����������������-�ҳ��Ƿ��д���ı���
        {
            if (task.IsFinished)    //����Ѿ����꽱��-ֱ������
                continue;
            //������ҷ�ʽ-��task�б��п��ٲ���
            var matchTask = task.questData.questRequires.Find(r =>r.name==requireName);
            if (matchTask != null)
                matchTask.currentAmount += amount;          //��ǰ���ȼ�һ
            
            task.questData.CheckQuestProgress();
        }
    }

    public bool HaveQuest(QuestData_SO data)
    {
        //����Ĳ��ҷ���-linq-Any-�����б����Ƿ��а���������
        if (data != null)
            return tasks.Any(q => q.questData.questName == data.questName);
        else
            return false;        
    }

    //��QuestManager��ȡ��ǰ����������
    public QuestTask GetTask( QuestData_SO data )
    {
        //����QuestData_SO����û��ƥ�����Ŀ
        return tasks.Find(q => q.questData.questName == data.questName);
    }
}
