﻿using UnityEngine;
using System.Collections;

public class CPUManager : ICPUManager 
{
    private IGrid _grid;
    private ISetting _setting;
    private ICPUBehaviour _behaviour;
    public CPUManager(IGrid grid, ISetting setting)
    {
        _grid = grid;
        _setting = setting;
        _behaviour = new NullBehaviour();
    }

    public void ChangeCPUMode(CPUMode mode)
    {
        switch (mode)
        {
            case CPUMode.None:
                _behaviour = new NullBehaviour();
                break;
            case CPUMode.Easy:
                _behaviour = new RandomMovementBehaviour(_grid);
                break;
            case CPUMode.Normal:
                _behaviour = new SmartCPUBehaviour(_grid, _setting);
                break;
        }
    }

    public void OnUpdate()
    {
        _behaviour.DoAction();
    }
}
