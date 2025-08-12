using UnityEngine;

public class SniperRifle : Gun
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        _limitAmount = 10;
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
