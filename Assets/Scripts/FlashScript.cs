using SteelSeries.GameSense;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashScript : MonoBehaviour
{
    [Header("闪烁设置")]
    public float flash_freq;
    public string m_seq;
    public byte hid_code;
    public float flash_interval;

    private Button keyButton_Button;
    private Image keyButton_Image;
    private Color keyButton_Original_ImageColor;

    private Text keyButton_Text;
    private Color keyButton_Original_TextColor;

    private bool isBlinking = false;
    private bool isPlaying = false;
    private Coroutine blink_Coroutine;
    private Coroutine play_Coroutine;
    private List<Func<IEnumerator>> coroutines = null; // 存储创建协程的方法

    /// <summary>
    /// Calcuate the interval when start
    /// </summary>
    void Start()
    {
        keyButton_Button = GetComponent<Button>();

        keyButton_Image = keyButton_Button.GetComponent<Image>();
        keyButton_Original_ImageColor = keyButton_Image.color;

        keyButton_Text = keyButton_Button.GetComponentInChildren<Text>();
        keyButton_Original_TextColor = keyButton_Text.color;
    }

    public void StartBlinking()
    {
        if (isBlinking) return;
        isBlinking = true;

        blink_Coroutine = StartCoroutine(BlinkRoutine());
    }
    public void StopBlinking()
    {
        if (!isBlinking) return;
        isBlinking = false;

        if (blink_Coroutine != null)
        {
            StopCoroutine(blink_Coroutine);
            blink_Coroutine = null;
        }
        RESET();
    }
    private IEnumerator BlinkRoutine()
    {
        flash_interval = 1.0f / (2 * flash_freq);
        PhysicalKeyboard_ON();
        while (isBlinking)
        {
            // Switch to ON
            VirtualKeyboard_ON();
            yield return new WaitForSeconds(flash_interval);

            // Switch to OFF
            VirtualKeyboard_OFF();
            yield return new WaitForSeconds(flash_interval);
        }
    }

    public void StartPlaying()
    {
        if (isPlaying) return;
        isPlaying = true;

        if (coroutines == null) Coroutines();
        play_Coroutine = StartCoroutine(LoopCoroutines());
    }
    public void StopPlaying()
    {
        if (!isPlaying) return;
        isPlaying = false;

        if (play_Coroutine != null)
        {
            StopCoroutine(play_Coroutine);
            play_Coroutine = null;
        }
        this.RESET();
    }
    private IEnumerator LoopCoroutines()
    {
        while (isPlaying)
        {
            foreach (var coroutine in coroutines)
            {
                yield return StartCoroutine(coroutine());
            }
        }
    }

    private void Coroutines()
    {
        coroutines = new List<Func<IEnumerator>>();

        int interval_times = 1;
        int last_bit = -1;

        foreach (char bit in m_seq)
        {
            int current_bit = bit - '0';
            if (current_bit == last_bit)
            {
                interval_times++;
            }
            else
            {
                if (last_bit != -1)
                {
                    float delay = flash_interval * interval_times;
                    
                    if (last_bit == 1)
                    {
                        coroutines.Add(() => OnDelay(delay));
                    }
                    else if (last_bit == 0)
                    {
                        coroutines.Add(() => OffDelay(delay));
                    }
                }

                interval_times = 1;  // Reset the sum when current bit is different
                last_bit = current_bit;
            }
        }

        // Calculate the last bit
        if (last_bit != -1)
        {
            float delay = flash_interval * interval_times;

            // Add coroutine function, not the coroutine itself
            if (last_bit == 1)
            {
                coroutines.Add(() => OnDelay(delay));
            }
            else if (last_bit == 0)
            {
                coroutines.Add(() => OffDelay(delay));
            }
        }
    }

    /// <summary>
    /// Control Visual key and Physical Key as the same time. In fact, there are some lag which is unavoidable because of limited bandwidth.
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator OnDelay(float delay)
    {
        PhysicalKeyboard_ON();
        VirtualKeyboard_ON();
        yield return new WaitForSeconds(delay);
    }
    private IEnumerator OffDelay(float delay)
    {
        PhysicalKeyboard_OFF();
        VirtualKeyboard_OFF();
        yield return new WaitForSeconds(delay);
    }

    public void VirtualKeyboard_ON()
    {
        keyButton_Image.color = Color.white;
        keyButton_Text.color = Color.black;
    }
    public void VirtualKeyboard_OFF()
    {
        keyButton_Image.color = keyButton_Original_ImageColor;
        keyButton_Text.color = keyButton_Original_TextColor;
    }

    public void PhysicalKeyboard_ON()
    {
        GSClient.Instance.SendEvent($"{hid_code}-EVENT", 1);
    }
    public void PhysicalKeyboard_OFF()
    {
        GSClient.Instance.SendEvent($"{hid_code}-EVENT", 0);
    }

    /// <summary>
    /// Reset all the key no matter what.
    /// </summary>
    public void RESET()
    {
        PhysicalKeyboard_OFF();
        VirtualKeyboard_OFF();
    }
    void OnDestroy()
    {
        // 确保协程被正确停止
        StopBlinking();
        StopPlaying();
    }
}
