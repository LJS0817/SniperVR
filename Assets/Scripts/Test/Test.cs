using UnityEngine;

public class Test : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] public static float scale;
    [SerializeField] private float intSerializationHelper;                  //don't use for anything else

    public static float a;
    public static float b;
    public static float c;
    [SerializeField] private float aH;                  //don't use for anything else
    [SerializeField] private float bH;                  //don't use for anything else
    [SerializeField] private float cH;                  //don't use for anything else


    public void OnAfterDeserialize()
    {
        scale = intSerializationHelper;
        a = aH;

        b = bH;
        
        c = cH;
    }

    public void OnBeforeSerialize()
    {
        //throw new System.NotImplementedException();
    }
}
