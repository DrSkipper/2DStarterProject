﻿using UnityEngine;
using UnityEditor;
using System;

//Credit: https://gist.github.com/paraself/0d75962a2c509759a6c9
public enum PlayModeState
{
    Stopped,
    Playing,
    Paused,
    AboutToStop,
    AboutToPlay
}

[InitializeOnLoad]
public class EditorPlayMode
{
    private static PlayModeState _currentState = PlayModeState.Stopped;

    static EditorPlayMode()
    {
        EditorApplication.playmodeStateChanged = OnUnityPlayModeChanged;
        if (EditorApplication.isPaused)
            _currentState = PlayModeState.Paused;
    }

    static int Bool2Int(bool b) { if (b) return 1; else return 2; }

    static int GetEditorAppStateBoolComb()
    {
        int b1 = Bool2Int(EditorApplication.isUpdating);
        int b2 = Bool2Int(EditorApplication.isPlayingOrWillChangePlaymode);
        int b3 = Bool2Int(EditorApplication.isPlaying);
        int b4 = Bool2Int(EditorApplication.isPaused);
        int b5 = Bool2Int(EditorApplication.isCompiling);
        return b1 + b2 * 10 + b3 * 100 + b4 * 1000 + b5 * 10000;
    }
    public static event Action<PlayModeState, PlayModeState> PlayModeChanged;

    private static void OnPlayModeChanged(PlayModeState currentState, PlayModeState changedState)
    {
        if (PlayModeChanged != null)
            PlayModeChanged(currentState, changedState);
    }

    private static void OnUnityPlayModeChanged()
    {
        var changedState = _currentState;

        //Stoped -> Playing : 22112
        //playing -> abouttostop : 22122
        //about2stop -> stopped : 22222
        //playing -> paused : 21112
        //paused -> playing : 22112
        //paused -> abouttostop : 21122
        //stoped -> paused in editor (stoped) : 21222
        //editor paused - > play(paused) : 21112
        //stoped -> abouttoplay in editor: 22212
        //editor paused -> play : 21212 //paused anyway

        int state = GetEditorAppStateBoolComb();
        switch (state)
        {
            case (22112):
                changedState = PlayModeState.Playing;
                break;
            case (21112):
                changedState = PlayModeState.Paused;
                break;
            case (22222):
                changedState = PlayModeState.Stopped;
                break;
            case (22122):
                changedState = PlayModeState.AboutToStop;
                break;
            case (21122):
                changedState = PlayModeState.AboutToStop;
                break;
            case (21222):
                changedState = PlayModeState.Stopped;
                break;
            case 22212:
                changedState = PlayModeState.AboutToPlay;
                break;
            case 21212:
                changedState = PlayModeState.Paused;
                break;
            default:
                Debug.LogWarning("No such state combination defined: " + state);
                break;
        }

        // Fire PlayModeChanged event.
        if (_currentState != changedState)
            OnPlayModeChanged(_currentState, changedState);

        // Set current state.
        _currentState = changedState;
    }
}
