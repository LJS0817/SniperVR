using UnityEngine;

public class AssaultRifle : Gun
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        _limitAmount = 30;
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
