using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManager : Singleton<QuestManager>
{
    [System.Serializable]
    public class QuestTask      //读取进度时加载所有任务进度-利用克隆方式不会影响本身数据
    {
        public QuestData_SO questData;  //保存任务数据，保证新开游戏本身的任务数据不会修改但QuestTask可以修改,将修改的给到QuestManager

        public bool IsStarted   //追踪进度
        {
            get
            {
                return questData.isStarted;     //拿去当前数据
            }
            set
            {
                questData.isStarted = value;    //修改当前数据
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
            //生成一个空的QuestData_SO
            var newQuest = ScriptableObject.CreateInstance<QuestData_SO>();
            //把读取的数据覆盖到新的
            SaveManager.Instance.Load(newQuest, "task" + i);
            //生成一个新的QuestTask列表
            tasks.Add(new QuestTask { questData = newQuest });
        }
    }

    //保存任务进度
    public void SaveQuestManager()
    {
        PlayerPrefs.SetInt("QuestCount",tasks.Count);       //先保存列表的数量
        for (int i = 0; i < tasks.Count; i++)               //每一个任务有一个序号去保存
        {
            SaveManager.Instance.Save(tasks[i].questData,"task" + i);
        }
    }

    //敌人死亡、拾取物品时调用
    public void UpdateQuestProgress( string requireName , int amount )                   //更新任务进度
    {
        foreach (var task in tasks) //遍历所有任务内容-找出是否有传入的变量
        {
            if (task.IsFinished)    //如果已经领完奖励-直接跳过
                continue;
            //方便查找方式-在task列表中快速查找
            var matchTask = task.questData.questRequires.Find(r =>r.name==requireName);
            if (matchTask != null)
                matchTask.currentAmount += amount;          //当前进度加一
            
            task.questData.CheckQuestProgress();
        }
    }

    public bool HaveQuest(QuestData_SO data)
    {
        //方便的查找方法-linq-Any-查找列表中是否有包含的条件
        if (data != null)
            return tasks.Any(q => q.questData.questName == data.questName);
        else
            return false;        
    }

    //在QuestManager获取当前的任务数据
    public QuestTask GetTask( QuestData_SO data )
    {
        //查找QuestData_SO里有没有匹配的项目
        return tasks.Find(q => q.questData.questName == data.questName);
    }
}
