using UnityEngine;
using System.IO.Ports;
using System;
using System.Collections.Generic; // Dictionary를 사용하기 위함

// Key-Value 쌍을 직렬화하기 위한 Serializable 클래스
[Serializable]
public class MyDictionaryEntry<TKey, TValue>
{
    public TKey Key;
    public TValue Value;

    public MyDictionaryEntry(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}

// Dictionary를 Inspector에서 보이게 하기 위한 Wrapper 클래스
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<MyDictionaryEntry<TKey, TValue>> _entries = new List<MyDictionaryEntry<TKey, TValue>>();

    // 직렬화 시 호출됨: Dictionary 내용을 List로 변환
    public void OnBeforeSerialize()
    {
        _entries.Clear();
        foreach (var pair in this)
        {
            _entries.Add(new MyDictionaryEntry<TKey, TValue>(pair.Key, pair.Value));
        }
    }

    // 역직렬화 시 호출됨: List 내용을 Dictionary로 변환
    public void OnAfterDeserialize()
    {
        this.Clear();
        foreach (var entry in _entries)
        {
            if (entry.Key != null && !this.ContainsKey(entry.Key)) // 중복 키 방지
            {
                this.Add(entry.Key, entry.Value);
            }
        }
    }
}

public class ArduinoConnection : MonoBehaviour
{
    public string portName = "COM4";
    public int baudRate = 9600;
    private string buffer = "";

    // 현재 센서 값 (UI/게임 로직에서 사용)
    //public int elevationValue;
    //public int windageValue;
    //public int parallaxValue;
    //public int zoomValue;
    //public int reloadValue;
    public SerializableDictionary<string, int> data;


    private SerialPort serialPort;
    private bool isPortOpen = false;

    void Start()
    {
        OpenSerialPort();
        data = new SerializableDictionary<string, int>();
    }

    void OpenSerialPort()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 1;
            serialPort.Open();
            isPortOpen = true;
            Debug.Log($"Serial Port {portName} Opened successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to open serial port {portName}: {e.Message}");
            isPortOpen = false;
        }
    }

    void Update()
    {
        if (isPortOpen && serialPort.IsOpen)
        {
            try
            {
                while (serialPort.BytesToRead > 0)
                {
                    char ch = (char)serialPort.ReadChar();
                    if (ch == '\n') // 줄 끝 (LF)
                    {
                        ParseData(buffer.Trim());
                        buffer = "";
                    }
                    else
                    {
                        buffer += ch;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Serial read error: {e.Message}");
            }
        }
    }

    void ParseData(string str)
    {

        Debug.Log($"[RAW] {str}");
        string[] keyValuePairs = str.Split(';');

        foreach (string pair in keyValuePairs)
        {
            string[] parts = pair.Split(':');
            if (parts.Length == 2 && int.TryParse(parts[1], out int val))
            {
                data[parts[0]] = val;
            }
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log($"Serial Port {portName} closed.");
        }
    }
}