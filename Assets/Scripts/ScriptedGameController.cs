using SteelSeries.GameSense;
using SteelSeries.GameSense.DeviceZone;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class ScriptedGameController : MonoBehaviour {

    public Texture2D imagetex;
    public UnityEngine.UI.Text rangeText;
    private int score;

    void Awake() {
        // register our game
        GSClient.Instance.RegisterGame("UNITY-GAMESENSE-APP-SCRIPTED", "Scripted Gamesense App", "LegitDevInc");

        ScreenHandler screenHandler = OLEDHandlerExamples();
        ColorHandler rangedColorHandler = RangedColorHandler();
        ColorHandler staticColorHandler = StaticColorHandler();
        TactileHandler tactileHandler = StaticTactileHandler();

        // bind our event with the handler
        GSClient.Instance.BindEvent(
            "RANGED-EVENT",                             // event name
            0,                                          // min value
            100,                                        // max value
            EventIconId.Default,                        // icon id
            new AbstractHandler[] { rangedColorHandler, screenHandler }   // array of handlers
            );

        GSClient.Instance.BindEvent(
            "BINARY-EVENT",                             // event name
            0,                                          // min value
            1,                                          // max value
            EventIconId.Default,                        // icon id
            new AbstractHandler[] { tactileHandler, staticColorHandler }   // array of handlers
            );
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            EventRangeUp(5);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            EventRangeDown(5);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void EventRangeUp( int increment = 1 ) {
        if (score + increment > 100) { return; }
        score += increment;
        rangeText.text = score.ToString();
        SteelSeries.GameSense.GSClient.Instance.SendEvent("RANGED-EVENT", score);
    }

    public void EventRangeDown( int increment = 1 ) {
        if (score - increment < 0) { return; }
        score -= increment;
        rangeText.text = score.ToString();
        SteelSeries.GameSense.GSClient.Instance.SendEvent("RANGED-EVENT", score);
    }

    public void EventToggle( bool value ) {
        Console.WriteLine(value);
        SteelSeries.GameSense.GSClient.Instance.SendEvent("BINARY-EVENT", Convert.ToInt32(value));
    }

    private ColorHandler RangedColorHandler() {
        KeyboardZoneMainKeyboard zone = UnityEngine.ScriptableObject.CreateInstance<KeyboardZoneMainKeyboard>();

        ColorStatic cyan = ColorStatic.Create(0, 194, 255);
        ColorStatic green = ColorStatic.Create(0, 255, 0);
        ColorStatic red = ColorStatic.Create(255, 0, 0);

        ColorRange cyanRange = new ColorRange(0, 10, cyan);
        ColorRange redRange = new ColorRange(10, 20, red);
        ColorRange greenRange = new ColorRange(20, 100, green);

        ColorRange[] ranges = { cyanRange, redRange, greenRange };

        RateStatic rate = ScriptableObject.CreateInstance<RateStatic>();
        rate.frequency = 1;
        rate.repeatLimit = 3;

        return ColorHandler.Create(zone, IlluminationMode.Color, ColorRanges.Create(ranges), rate);
    }

    private ColorHandler StaticColorHandler() {
        MouseZoneLogo zone = UnityEngine.ScriptableObject.CreateInstance<MouseZoneLogo>();
        RateStatic rate = ScriptableObject.CreateInstance<RateStatic>();
        rate.frequency = 5;
        rate.repeatLimit = 0;

        ColorStatic cyan = ColorStatic.Create(0, 194, 255);
        return ColorHandler.Create(zone, IlluminationMode.Color, cyan, rate);
    }

    private TactileHandler StaticTactileHandler() {

        TactileEffectSimple effect = new TactileEffectSimple(TactileEffectType.ti_predefined_shortdoubleclickmedium2_80);

        RateStatic rate = RateStatic.Create(0);

        return TactileHandler.Create(UnityEngine.ScriptableObject.CreateInstance <TactileZoneOne>(), TactileMode.vibrate, new TactileEffectSimple[] { effect }, rate);
    }

    // Extra OLED examples that covers multiple handler data types
    private ScreenHandler OLEDHandlerExamples() {
        ScreenedZoneOne zone = UnityEngine.ScriptableObject.CreateInstance<ScreenedZoneOne>();

        // base level line info
        LineDataProgressBar bar = LineDataProgressBar.Create();
        LineDataText text = LineDataText.Create("prefix", "suffix", true, 0);

        // base level image
        ImageDataTexture2D image = ImageDataTexture2D.Create(imagetex);

        // create lines from linedata
        LineData bar_line = LineData.Create(bar);
        LineData text_line = LineData.Create(text);

        // modifiers for static and timed frames
        FrameModifiers mod0 = new FrameModifiers(0, false);
        FrameModifiers mod1 = new FrameModifiers(500, 1);

        // single line frame and single line with timed event frame
        FrameDataSingleLine[] single_line = { FrameDataSingleLine.Create(bar_line, mod0) };
        FrameDataSingleLine[] multi_single_line = { FrameDataSingleLine.Create(text_line, mod1), FrameDataSingleLine.Create(bar_line, mod1) };



        // multi line frame and multi line with timed event frame
        FrameDataMultiLine[] multi_line = { FrameDataMultiLine.Create(new LineData[] { text_line, bar_line }, mod0) };

        // image frame - change 'Screened128x36ZoneOne' based on which screen you want it displayed on
        FrameDataImage[] image_frame = { FrameDataImage.Create(UnityEngine.ScriptableObject.CreateInstance<Screened128x36ZoneOne>(), image, mod1) };

        // mixing multi-line + image
        AbstractFrameData[] mixed_frames = { FrameDataImage.Create(UnityEngine.ScriptableObject.CreateInstance<Screened128x36ZoneOne>(), image, mod1), FrameDataMultiLine.Create(new LineData[] { text_line, bar_line }, mod0) };

        // range that from event values 0-10 will display multi-line, then from 10-100 displays image
        FrameDataRange[] ranged_frames = { new FrameDataRange(0, 50, multi_line), new FrameDataRange(50, 100, single_line) };



        // Choose from AbstractFrameDataRange[]'s above to pass into handler
        return ScreenHandler.Create(zone, ScreenMode.screen, ranged_frames); // can also use multi-line, image_frame, or mixed_frames
    }

    public void SwitchScenes() {
        SceneManager.LoadScene("GSPrefabScene");
    }
}
