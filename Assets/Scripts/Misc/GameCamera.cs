using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameCamera
{
    /// <summary>
    /// The first enabled camera tagged "UICamera"
    /// </summary>
    public static Camera UI
    {
        get { return GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>(); }
    }

    //Functions For Track

    public static bool ImmediateTrack(this Camera camera)
    {
        TrackedCamera tracker = camera.GetComponent<TrackedCamera>();
        if (tracker == null) return false;
        tracker.ImmediateSet();
        return true;
    }

    public static bool SetTrackOffset(this Camera camera, Vector3 offset)
    {
        TrackedCamera tracker = camera.GetComponent<TrackedCamera>();
        if (tracker == null) return false;
        tracker.SetOffset(offset);
        return true;
    }

    public static bool SetTrackOffset(this Camera camera, float x, float y, float z)
    {
        return camera.SetTrackOffset(new Vector3(x, y, z));
    }

    //Functions For Post Effects

    public static bool SetChannel(this Camera camera, float value, float duration = 0f)
    {
        RGBChannel channel = camera.GetComponent<RGBChannel>();
        if (channel == null) return false;
        channel.SetEffectScale(value, duration);
        return true;
    }

    public static bool SetBlur(this Camera camera, float value, float duration = 0f)
    {
        GaussBlur blur = camera.GetComponent<GaussBlur>();
        if (blur == null) return false;
        blur.SetEffectScale(value, duration);
        return true;
    }

    public static bool SetBloom(this Camera camera, float value, float duration = 0f)
    {
        BloomEffect bloom = camera.GetComponent<BloomEffect>();
        if (bloom == null) return false;
        bloom.SetEffectScale(value, duration);
        return true;
    }

    public static bool SetSnowflake(this Camera camera, float duration)
    {
        Snowflake snowflake = camera.GetComponent<Snowflake>();
        if (snowflake == null) return false;
        snowflake.TurnOn(duration);
        return true;
    }

    public static bool SetGrid(this Camera camera, float value, float duration = 0f)
    {
        ScreenGrid grid = camera.GetComponent<ScreenGrid>();
        if (grid == null) return false;
        grid.SetEffectScale(value, duration);
        return true;
    }

    public static bool SetBorder(this Camera camera, float value, float duration = 0f)
    {
        FadeBorder border = camera.GetComponent<FadeBorder>();
        if (border == null) return false;
        border.SetEffectScale(value, duration);
        return true;
    }
}
