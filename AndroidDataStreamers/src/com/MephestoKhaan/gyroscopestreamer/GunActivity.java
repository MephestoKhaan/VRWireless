package com.MephestoKhaan.gyroscopestreamer;

import android.app.Activity;
import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.view.KeyEvent;
import android.view.View;
import android.widget.TextView;
import android.os.Vibrator;

import com.MephestoKhaan.Movemment.OrientationDetector;
import com.MephestoKhaan.Movemment.VRSensorListener;
import com.MephestoKhaan.gyroscopestreamer.MainActivity.PORTFORWARDING;

public class GunActivity extends Activity implements VRSensorListener
{
	private TextView orientationText;

    private OrientationDetector mOrientationDetector;
    private SensorManager mSensorManager;
    
    private Vibrator vibrator;
    
	public static enum GUNEVENTS
	{
		CLIP_ON(KeyEvent.KEYCODE_C),
		CLIP_OFF(KeyEvent.KEYCODE_V),
		TRIGGER_ON(KeyEvent.KEYCODE_T),
		TRIGGER_OFF(KeyEvent.KEYCODE_Y),
		PUMP(KeyEvent.KEYCODE_P);
		
		private int value;
	    private GUNEVENTS(int value)
	    {
	       this.value = value;
	    }
	    public int getValue() {
	       return value;
	    }
	}
    
	@Override
	protected void onCreate(Bundle savedInstanceState)
	{
		super.onCreate(savedInstanceState);
		setContentView(R.layout.gun_layout);
		
        initializeVariables();
        startDetection();
	}
	
	@Override
	public void onDestroy()
	{
		super.onDestroy();
		stopDetection();
	}
	

	private void initializeVariables()
	{
		orientationText  = (TextView) findViewById(R.id.gunorientationtext);
		vibrator = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
	}
	
	@Override
	public void onOrientation(double[] orientation)
	{
		String msg = ""+orientation[1]+";"+orientation[0]+";"+(90.0-orientation[2]);
		orientationText.setText(msg.replace(';', '\n').replace(':', '\n'));
		new SendMessage().execute(PORTFORWARDING.GUN_ORIENTATION.getValue(),"UDP",msg);
	}
	
	public void onGunEvent(String gunEvent)
	{
		if(gunEvent == "fire" || gunEvent == "stopfire")
		{
			vibrate(gunEvent == "fire");
		}
		
		new SendMessage().execute(PORTFORWARDING.FIRE.getValue(),"UDP",gunEvent);
	}

	public void vibrate(boolean on)
	{
		long[] pattern = {200,40,60,50,50,60,40,70,40,70,20,90,10};
		
		if(vibrator != null)
		{
			if(on)
			{
				vibrator.vibrate(pattern,10);
			}
			else
			{
				vibrator.cancel();
			}
		}
	}
	
	
	
	public void syncronizeOrientations(View v)
	{
		String msg = "sync";
		new SendMessage().execute(PORTFORWARDING.SYNC.getValue(),"UDP",msg);
	}

	@Override
	public void onStep(double strenght) {}
	
	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) 
	{
		if(keyCode == GUNEVENTS.TRIGGER_ON.getValue())
		{
			onGunEvent("fire");
		}
		if(keyCode == GUNEVENTS.TRIGGER_OFF.getValue())
		{
			onGunEvent("stopfire");	
		}
		if(keyCode == GUNEVENTS.PUMP.getValue())
		{
			onGunEvent("pump");	
		}
		if(keyCode == GUNEVENTS.CLIP_ON.getValue())
		{
			onGunEvent("clipon");	
		}
		if(keyCode == GUNEVENTS.CLIP_OFF.getValue())
		{
			onGunEvent("clipoff");	
		}
		
		
	    return super.onKeyDown(keyCode, event);
	} 
	
	private void startDetection()
	{
        mOrientationDetector = new OrientationDetector();
        registerDetector();
	}
	
	private void stopDetection()
	{
		unregisterDetector();
	}
	
	private void registerDetector()
	{
        mSensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);
        mSensorManager.registerListener(mOrientationDetector, mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER), SensorManager.SENSOR_DELAY_FASTEST);
        mSensorManager.registerListener(mOrientationDetector, mSensorManager.getDefaultSensor(Sensor.TYPE_GYROSCOPE), SensorManager.SENSOR_DELAY_FASTEST);
        mSensorManager.registerListener(mOrientationDetector, mSensorManager.getDefaultSensor(Sensor.TYPE_MAGNETIC_FIELD), SensorManager.SENSOR_DELAY_FASTEST);
        mOrientationDetector.addOrientationListener(this);
        mOrientationDetector.startDetection();
    }

    private void unregisterDetector()
    {
    	mOrientationDetector.stopDetection();
    	mOrientationDetector.removeOrientationListeners();
        mSensorManager.unregisterListener(mOrientationDetector);
    }

}
