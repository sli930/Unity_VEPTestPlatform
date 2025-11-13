using SteelSeries.GameSense;
using SteelSeries.GameSense.DeviceZone;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KeyboardControl : MonoBehaviour
{
    public static Boolean SSVEP_or_CVEP = true;    // In inspector, to switch SSVEP or CVEP modes

    private List<KeyValuePair<byte, string>> hid_codes;
    private List<GameObject> keyButton_Buttons = new List<GameObject>();

    public int refresh_rate = 60;
    public float frames_per_bit = 3.0f;
    public GameObject button_Prefab;
    public RectTransform bottom_Panel;
    public TextMeshProUGUI modeButton_Text;

    /// <summary>
    /// Limit the maximum FPS when start
    /// </summary>
    void Start()
    {
        hid_codes = gameObject.AddComponent<KeyboardHID>().select(0x1E, 0x27); // Number from 1 to 0
        //hid_Codes = gameObject.AddComponent<KeyboardHID>().multi_select(new List<(int start, int end)> { (0x35, 0x35), (0x1E, 0x27), (0x2D, 0x2D) });

        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = refresh_rate;
        this.initalize();
    }

    /// <summary>
    /// UP: Lanuch
    /// Down: Stop
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (SSVEP_or_CVEP)
            {
                // Physical Key Flash is support by Steelseries Offical API.
                Debug.Log("SSVEP Start!");
                foreach (GameObject keyButton_Button in keyButton_Buttons)
                {
                    keyButton_Button.GetComponent<FlashScript>().StartBlinking();
                }
            }
            else
            {
                Debug.Log("CVEP Start!");
                foreach (GameObject keyButton_Button in keyButton_Buttons)
                {
                    keyButton_Button.GetComponent<FlashScript>().StartPlaying();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (SSVEP_or_CVEP)
            {
                Debug.Log("SSVEP Stop!");
                foreach (GameObject keyButton_Button in keyButton_Buttons)
                {
                    keyButton_Button.GetComponent<FlashScript>().StopBlinking();
                }
            }
            else
            {
                Debug.Log("CVEP Stop!");
                foreach (GameObject keyButton_Button in keyButton_Buttons)
                {
                    keyButton_Button.GetComponent<FlashScript>().StopPlaying();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart_CurrentScene();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Toggle_Mode();
        }
    }

    public void initalize()
    {
        if (keyButton_Buttons.Count == 0)
        {
            keyButton_Buttons = Create_Keyboard(hid_codes);
        }
        string gameName = SSVEP_or_CVEP ? "SSVEP" : "CVEP";
        GSClient.Instance.RegisterGame(gameName, gameName, "Shengle");
        modeButton_Text.text = gameName;

        if (SSVEP_or_CVEP)
        {
            int flash_freq = 5;

            Set_Keyboard(flash_freq);
            Debug.Log($"flash_freq:{flash_freq}");
        }
        else
        {
            string m_seq = "1110011010010000101011010001";
            float flash_interval = frames_per_bit / refresh_rate;

            Set_Keyboard(flash_interval, m_seq);
            Debug.Log($"flash_interval:{flash_interval}");
        }
    }

    private List<GameObject> Create_Keyboard(List<KeyValuePair<byte, string>> hid_codes)
    {
        int position_x = 0;
        List<GameObject> temp = new List<GameObject>();

        foreach (KeyValuePair<byte, string> hid_code in hid_codes)
        {
            temp.Add(Create_Key(new Vector2(position_x, 0), hid_code));
            position_x += 100;
        }
        return temp;
    }

    private GameObject Create_Key(Vector2 anchoredPosition, KeyValuePair<byte, string> hid_code)
    {
        GameObject keyButton_Button = Instantiate(button_Prefab);
        keyButton_Button.transform.SetParent(bottom_Panel);

        Text keyButton_Text = keyButton_Button.GetComponentInChildren<Text>();
        keyButton_Text.text = hid_code.Value;

        RectTransform keyButton_RectTransform = keyButton_Button.GetComponent<RectTransform>();
        keyButton_RectTransform.anchorMin = new Vector2(0, 0.5f);
        keyButton_RectTransform.anchorMax = new Vector2(0, 0.5f);
        keyButton_RectTransform.anchoredPosition = anchoredPosition;

        FlashScript keyButton_Script = keyButton_Button.GetComponent<FlashScript>();
        keyButton_Script.hid_code = hid_code.Key;

        return keyButton_Button;
    }

    private void Set_Keyboard(int flash_freq)
    {
        int temp_flash_freq = flash_freq;
        foreach (GameObject keyButton_Button in keyButton_Buttons)
        {
            // Virutal Keyboard
            FlashScript keyButton_Script = keyButton_Button.GetComponent<FlashScript>();
            keyButton_Script.flash_freq = (float)temp_flash_freq;

            // Physical Keyboard
            byte hid_code = keyButton_Script.hid_code;
            GSClient.Instance.BindEvent(
                               $"{hid_code}-EVENT",
                               0,
                               1,
                               EventIconId.Default,
                               new AbstractHandler[] { this.PlainColorHandler(hid_code, (uint)temp_flash_freq, 0) }
                               );
            temp_flash_freq += 1;
        }
    }
    private void Set_Keyboard(float flash_interval, string m_seq)
    {
        int shift = (int)m_seq.Length / hid_codes.Count;
        foreach (GameObject keyButton_Button in keyButton_Buttons)
        {
            // Virutal Keyboard
            FlashScript keyButton_Script = keyButton_Button.GetComponent<FlashScript>();
            keyButton_Script.flash_interval = flash_interval;
            keyButton_Script.m_seq = m_seq;

            // Physical Keyboard
            byte hid_code = keyButton_Script.hid_code;
            GSClient.Instance.BindEvent(
                               $"{hid_code}-EVENT",
                               0,
                               1,
                               EventIconId.Default,
                               new AbstractHandler[] { this.PlainColorHandler(hid_code) }
                               );
            m_seq = mSeqShift(m_seq, shift);
        }
    }

    private ColorHandler PlainColorHandler(byte hidcode)
    {
        RGBPerkeyZoneCustom zone = UnityEngine.ScriptableObject.CreateInstance<RGBPerkeyZoneCustom>();
        zone.zone = new byte[1] { hidcode };

        return ColorHandler.Create(zone, IlluminationMode.Color, ColorStatic.Create(255, 255, 255));
    }

    /// <summary>
    /// Colorhandler is used for setting up the Key Event.
    /// </summary>
    /// <param name="hidcode">HID Code</param>
    /// <param name="flash_freq">Flash Frequency</param>
    /// <param name="flash_limit">Flash Total Times</param>
    /// <returns></returns>
    private ColorHandler PlainColorHandler(byte hidcode, uint flash_freq, uint flash_limit)
    {
        RGBPerkeyZoneCustom zone = UnityEngine.ScriptableObject.CreateInstance<RGBPerkeyZoneCustom>();
        zone.zone = new byte[1] { hidcode };

        RateStatic rate = ScriptableObject.CreateInstance<RateStatic>();
        rate.frequency = flash_freq;
        rate.repeatLimit = flash_limit;

        return ColorHandler.Create(zone, IlluminationMode.Color, ColorStatic.Create(255, 255, 255), rate);
    }

    public void Toggle_Mode()
    {
        foreach (GameObject keyButton_Button in keyButton_Buttons)
        {
            var flashScript = keyButton_Button.GetComponent<FlashScript>();
            flashScript.StopBlinking();
            flashScript.StopPlaying();
            flashScript.RESET();
        }

        SSVEP_or_CVEP = !SSVEP_or_CVEP;
        Restart_CurrentScene();
    }

    public void Restart_CurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene(); // Get the current scene
        SceneManager.LoadScene(currentScene.name); // Reload the scene by its name
    }
    private static string mSeqShift(string m_seq, int shift)
    {
        return m_seq.Substring(shift, m_seq.Length - shift) + m_seq.Substring(0, shift);
    }
}