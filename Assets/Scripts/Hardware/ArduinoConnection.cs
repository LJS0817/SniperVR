using UnityEngine;
using System.IO.Ports;
using System;
using System.Collections.Generic; // Dictionary를 사용하기 위함

public class ArduinoConnection : MonoBehaviour
{
    public string portName = "COM4";
    public int baudRate = 9600;
    private string buffer = "";

    // 현재 센서 값 (UI/게임 로직에서 사용)
    public int elevationValue;
    public int windageValue;
    public int parallaxValue;
    public int zoomValue;
    public int reloadValue;

    private SerialPort serialPort;
    private bool isPortOpen = false;

    void Start()
    {
        OpenSerialPort();
        // 초기값 설정
        elevationValue = 0;
        windageValue = 0;
        parallaxValue = 0;
        zoomValue = 0;
        reloadValue = 0;
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

    void ParseData(string data)
    {
        Debug.Log($"[RAW] {data}");

        string[] keyValuePairs = data.Split(';');
        Dictionary<string, int> dataMap = new Dictionary<string, int>();

        foreach (string pair in keyValuePairs)
        {
            string[] parts = pair.Split(':');
            if (parts.Length == 2 && int.TryParse(parts[1], out int val))
            {
                dataMap[parts[0]] = val;
            }
        }

        if (dataMap.ContainsKey("Z")) zoomValue = dataMap["Z"];
        if (dataMap.ContainsKey("R")) reloadValue = dataMap["R"];
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