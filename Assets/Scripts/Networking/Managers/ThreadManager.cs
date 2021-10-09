using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager: MonoBehaviour
{
    private static readonly List<Action> ExecuteOnMainThreadValue = new List<Action>();

    private static readonly List<Action> ExecuteCopiedOnMainThread = new List<Action>();

    private static bool actionToExecuteOnMainThread = false;

    /// <summary>Sets an action to be executed on the main thread.</summary>
    /// <param name="action">The action to be executed on the main thread.</param>
    public static void ExecuteOnMainThread(Action action)
    {
        if (action == null)
        {
            Debug.Log("No action to execute on main thread!");
            return;
        }

        lock (ExecuteOnMainThreadValue)
        {
            ExecuteOnMainThreadValue.Add(action);
            actionToExecuteOnMainThread = true;
        }
    }

    /// <summary>Executes all code meant to run on the main thread. NOTE: Call this ONLY from the main thread.</summary>
    public static void UpdateMain()
    {
        if (actionToExecuteOnMainThread)
        {
            ExecuteCopiedOnMainThread.Clear();

            lock (ExecuteOnMainThreadValue)
            {
                ExecuteCopiedOnMainThread.AddRange(ExecuteOnMainThreadValue);
                ExecuteOnMainThreadValue.Clear();
                actionToExecuteOnMainThread = false;
            }

            for (int i = 0; i < ExecuteCopiedOnMainThread.Count; i++)
            {
                ExecuteCopiedOnMainThread[i]();
            }
        }
    }

    private void Update()
    {
        UpdateMain();
    }
}