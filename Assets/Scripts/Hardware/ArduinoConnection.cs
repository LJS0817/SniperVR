using UnityEngine;
using System.IO.Ports;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class SensorDataUpdateEvent : UnityEvent<Dictionary<string, int>> { }

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

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<MyDictionaryEntry<TKey, TValue>> _entries = new List<MyDictionaryEntry<TKey, TValue>>();

    public void OnBeforeSerialize()
    {
        _entries.Clear();
        foreach (var pair in this)
        {
            _entries.Add(new MyDictionaryEntry<TKey, TValue>(pair.Key, pair.Value));
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();
        foreach (var entry in _entries)
        {
            if (entry.Key != null && !this.ContainsKey(entry.Key))
            {
                this.Add(entry.Key, entry.Value);
            }
        }
    }
}


public class ArduinoConnection : MonoBehaviour
{
    // === UnityEvent 정의 
    [SerializeField]
    private SensorDataUpdateEvent _onSensorDataUpdated = new SensorDataUpdateEvent();
    public SensorDataUpdateEvent OnSensorDataUpdated => _onSensorDataUpdated;

    public string portName = "COM4";
    public int baudRate = 9600;
    private string buffer = "";

    // 기존의 SerializableDictionary<string, int> data는 그대로 유지
    public SerializableDictionary<string, int> data;

    private SerialPort serialPort;
    private bool isPortOpen = false;

    void Start()
    {
        data = new SerializableDictionary<string, int>();
        OpenSerialPort();
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
        bool dataChanged = false; // 데이터 변경 여부 플래그

        foreach (string pair in keyValuePairs)
        {
            string[] parts = pair.Split(':');
            if (parts.Length == 2 && int.TryParse(parts[1], out int val))
            {
                // 데이터가 실제로 변경되었는지 확인
                if (!data.ContainsKey(parts[0]) || data[parts[0]] != val)
                {
                    data[parts[0]] = val;
                    dataChanged = true; // 변경되었으면 플래그 설정
                }
            }
        }

        if (dataChanged)
        {
            _onSensorDataUpdated?.Invoke(new Dictionary<string, int>(data));
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