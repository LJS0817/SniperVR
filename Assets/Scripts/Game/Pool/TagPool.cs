using System.Collections.Generic;
using UnityEngine;

public class TagPool : MonoBehaviour
{
    [SerializeField] GameObject _tagPrefab;
    [SerializeField] Queue<Transform> _tagPool;
    [SerializeField] Queue<Transform> _activateTagPool;

    [SerializeField] Transform _tagParent;

    const int MAX_TAG_COUNT = 25;

    public static TagPool Instance { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        InitPool();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            clearActivateTag();
        }
    }

    public Transform GetTag()
    {
        Transform tag = null;

        if (_tagPool.Count > 0) tag = _tagPool.Dequeue();
        else tag = _activateTagPool.Dequeue();

        _activateTagPool.Enqueue(tag);
        tag.gameObject.SetActive(true);
        return tag;
    }

    void clearActivateTag()
    {
        for (int i = 0; i < _activateTagPool.Count; i++)
        {
            _tagPool.Enqueue(_activateTagPool.Dequeue());
        }
    }

    void InitPool()
    {
        _tagPool = new Queue<Transform>();
        _activateTagPool = new Queue<Transform>();

        for(int i = 0; i < MAX_TAG_COUNT; i++)
        {
            GameObject obj = Instantiate(_tagPrefab, _tagParent);
            obj.gameObject.SetActive(false);
            _tagPool.Enqueue(obj.transform);
        }
    }
}
