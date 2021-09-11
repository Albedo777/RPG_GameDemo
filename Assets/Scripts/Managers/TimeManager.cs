using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * 时间管理者
 * 游戏暂停
 * 延时调用方法
 * @author ligzh
 * @date 2021/9/5
 */

public class TimeManager
{
    private struct DelayAction
    {
        public int id;
        public int startTime;
        public Action action;
    }

    public static float GlobalDeltaTime;
    public static float GameDeltaTime;
    public static int deltaTime { get; private set; }
    public static float fDeltaTime { get; private set; }
    public static int time { get; private set; }
    private static int delayActionId;

    public static float GlobalTimeScale
    {
        get { return globalTimeScale; }
        set
        {
            globalTimeScale = value;
            Time.timeScale = globalTimeScale * gameTimeScale;
        }
    }

    public static float GameTimeScale
    {
        get { return gameTimeScale; }
        set
        {
            gameTimeScale = value;
            Time.timeScale = globalTimeScale * gameTimeScale;
        }
    }

    private static float globalTimeScale; //全局时间，用于控制暂停游戏
    private static float gameTimeScale; //游戏内时间，用于技能控制
    private List<DelayAction> delayActionList = new List<DelayAction>();
    private static TimeManager instance;

    public static TimeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TimeManager();
            }

            return instance;
        }
    }

    public void Update()
    {
        GlobalDeltaTime = globalTimeScale * Time.unscaledDeltaTime;
        GameDeltaTime = globalTimeScale * gameTimeScale * Time.unscaledDeltaTime;
        time += (int) (GlobalDeltaTime * 1000);

        for (int i = 0; i < delayActionList.Count; i++)
        {
            if (delayActionList[i].startTime <= time)
            {
                try
                {
                    delayActionList[i].action.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.StackTrace);
                    throw;
                }

                delayActionList.RemoveAt(i);
                i--;
            }
        }
    }

    public void Dispose()
    {
        time = 0;
        deltaTime = 0;
        instance = null;
    }


    public int DelayCall(int delay, Action action)
    {
        delayActionList.Add(new DelayAction
        {
            startTime = time + delay,
            action = action,
            id = delayActionId,
        });
        int id = delayActionId;
        delayActionId++;
        return id;
    }

    public void CancelDelayCall(int id)
    {
        for (int i = 0; i < delayActionList.Count; i++)
        {
            if (delayActionList[i].id == id)
            {
                delayActionList.RemoveAt(i);
                break;
            }
        }
    }
}