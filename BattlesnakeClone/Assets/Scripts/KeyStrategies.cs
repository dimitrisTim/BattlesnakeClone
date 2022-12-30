using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKeyStrategy
{
    bool CheckUp();
    bool CheckDown();
    bool CheckLeft();
    bool CheckRight();    
    bool CheckAbilityKey();
}

public class ArrowsStrategy : IKeyStrategy
{
    public bool CheckUp()
    {
        return Input.GetKeyDown(KeyCode.UpArrow);
    }

    public bool CheckDown()
    {
        return Input.GetKeyDown(KeyCode.DownArrow);
    }

    public bool CheckLeft()
    {
        return Input.GetKeyDown(KeyCode.LeftArrow);
    }

    public bool CheckRight()
    {
        return Input.GetKeyDown(KeyCode.RightArrow);
    }

    public bool CheckAbilityKey()
    {
        return Input.GetKeyDown(KeyCode.RightShift);
    }
}

public class WASDStrategy : IKeyStrategy
{
    public bool CheckUp()
    {
        return Input.GetKeyDown(KeyCode.W);
    }

    public bool CheckDown()
    {
        return Input.GetKeyDown(KeyCode.S);
    }

    public bool CheckLeft()
    {
        return Input.GetKeyDown(KeyCode.A);
    }

    public bool CheckRight()
    {
        return Input.GetKeyDown(KeyCode.D);
    }

    public bool CheckAbilityKey()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
}
