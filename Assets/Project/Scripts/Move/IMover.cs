using UnityEngine;

public interface IMover 
{
    public void Move(Vector2 _dir);
    public void Look(Vector2 _rot);

    public void AfterMove();

    public void Gravity();
}
