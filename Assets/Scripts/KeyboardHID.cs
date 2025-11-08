using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KeyboardHID : MonoBehaviour
{
    public SortedList<byte, string> map = new SortedList<byte, string>
        {
            // 字母键
            { 0x04, "A" },
            { 0x05, "B" },
            { 0x06, "C" },
            { 0x07, "D" },
            { 0x08, "E" },
            { 0x09, "F" },
            { 0x0A, "G" },
            { 0x0B, "H" },
            { 0x0C, "I" },
            { 0x0D, "J" },
            { 0x0E, "K" },
            { 0x0F, "L" },
            { 0x10, "M" },
            { 0x11, "N" },
            { 0x12, "O" },
            { 0x13, "P" },
            { 0x14, "Q" },
            { 0x15, "R" },
            { 0x16, "S" },
            { 0x17, "T" },
            { 0x18, "U" },
            { 0x19, "V" },
            { 0x1A, "W" },
            { 0x1B, "X" },
            { 0x1C, "Y" },
            { 0x1D, "Z" },
            
            // 数字键
            { 0x1E, "1" },
            { 0x1F, "2" },
            { 0x20, "3" },
            { 0x21, "4" },
            { 0x22, "5" },
            { 0x23, "6" },
            { 0x24, "7" },
            { 0x25, "8" },
            { 0x26, "9" },
            { 0x27, "0" },
            
            // 特殊键
            { 0x28, "Enter" },
            { 0x29, "Escape" },
            { 0x2A, "Backspace" },
            { 0x2B, "Tab" },
            { 0x2C, "Space" },
            { 0x2D, "-" },
            { 0x2E, "=" },
            { 0x2F, "[" },
            { 0x30, "]" },
            { 0x31, "\\" },
            { 0x33, ";" },
            { 0x34, "'" },
            { 0x35, "`" },
            { 0x36, "," },
            { 0x37, "." },
            { 0x38, "/" },
            { 0x39, "Caps Lock" },

            // 功能键
            { 0x3A, "F1" },
            { 0x3B, "F2" },
            { 0x3C, "F3" },
            { 0x3D, "F4" },
            { 0x3E, "F5" },
            { 0x3F, "F6" },
            { 0x40, "F7" },
            { 0x41, "F8" },
            { 0x42, "F9" },
            { 0x43, "F10" },
            { 0x44, "F11" },
            { 0x45, "F12" },
            
            // 方向键
            { 0x50, "Left Arrow" },
            { 0x51, "Down Arrow" },
            { 0x52, "Up Arrow" },
            { 0x53, "Right Arrow" },
            
            // 控制键
            { 0xE0, "Left Control" },
            { 0xE1, "Left Shift" },
            { 0xE2, "Left Alt" },
            { 0xE3, "Left GUI" },
            { 0xE4, "Right Control" },
            { 0xE5, "Right Shift" },
            { 0xE6, "Right Alt" },
            { 0xE7, "Right GUI" }
        };

    public List<KeyValuePair<byte, string>> select(int start, int end)
    {
        return map.Where(x => x.Key >= start && x.Key <= end).ToList();
    }

    public List<KeyValuePair<byte, string>> multi_select(List<(int start, int end)> ranges)
    {
        var section = new List<KeyValuePair<byte, string>>();
        foreach (var range in ranges)
        {
            section.AddRange(this.select((byte)range.start, (byte)range.end));
        }
        return section;
    }
}
