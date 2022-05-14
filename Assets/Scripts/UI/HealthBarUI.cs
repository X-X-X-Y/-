using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;

    public Transform barPoint;                  //血条位置
    public bool alwaysVisible;

    public float visibleTime;
    private float timeLeft;

    //UI控件
    Image healthSlider;
    Transform UIbar;
    Transform cam;                              //保持正对摄像机

    CharacterStats currentStats;                //当前角色血量

    private void Awake()
    {
        currentStats = GetComponent<CharacterStats>();

        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar ;       //订阅Stats中的事件
    }

    private void OnEnable()
    {
        cam = Camera.main.transform;                                    //相机正对

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())          //遍历所有UI控件
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                UIbar = Instantiate(healthUIPrefab,canvas.transform).transform;
                healthSlider = UIbar.GetChild(0).GetComponent<Image>(); //拿到第一个子物体的image组件
                UIbar.gameObject.SetActive(alwaysVisible);

            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
            Destroy(UIbar.gameObject);

        UIbar.gameObject.SetActive(true);

        timeLeft = visibleTime;                                 //更新显示时间

        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    //血条跟随人物移动 LateUpdate-渲染后一帧执行
    private void LateUpdate()
    {
        if (UIbar != null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward = -cam.forward;

            if (timeLeft <= 0 && !alwaysVisible)
            {
                UIbar.gameObject.SetActive(false);
            }
            else
                timeLeft -= Time.deltaTime;
        }
    }
}
