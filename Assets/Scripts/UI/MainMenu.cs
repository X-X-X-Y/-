using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    Button newGameBtn;

    Button contiuneBtn;

    Button quitBtn;

    PlayableDirector director ;


    private void Awake()
    {
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        contiuneBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();

        newGameBtn.onClick.AddListener(PlayTimeline);
        contiuneBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);

        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;
    }

    void PlayTimeline()
    {
        director.Play();
    }


    void NewGame(PlayableDirector obj)
    {
        PlayerPrefs.DeleteAll();
        //转换场景
        SceneConroller.Instance.TransitionToFirstLevel();

    }
    void ContinueGame()
    {
        //转换场景，读取数据
        SceneConroller.Instance.TransitionToLoadGame();
    }


    void QuitGame()
    {
        Application.Quit();
        Debug.Log("溜了溜了");
    }
}
