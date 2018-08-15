using UnityEngine;

[CreateAssetMenu(fileName = "BallSettings", menuName = "BallSettings", order = 0)]
public class BallSettings : ScriptableObject {
    [Header("Speed")] public float MinSpeed;
    public float MaxSpeed;
    [Header("Size")] public float MinSize;
    public float MaxSize;
    [Header("Color")] public float MinHue;
    public float MaxHue, MinSaturation, MaxSaturation, MinValue, MaxValue;
}