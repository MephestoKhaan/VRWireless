package com.MephestoKhaan.Movemment;

public interface VRSensorListener
{
    public void onStep(double strenght);
    public void onOrientation(double[] orientation);
}
