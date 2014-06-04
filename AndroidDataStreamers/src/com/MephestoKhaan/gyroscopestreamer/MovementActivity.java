package com.MephestoKhaan.gyroscopestreamer;

import java.util.TooManyListenersException;

import android.app.Activity;
import android.hardware.Sensor;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.view.View;
import android.widget.TextView;
import android.widget.ToggleButton;

import com.MephestoKhaan.Movemment.VRSensorListener;
import com.MephestoKhaan.Movemment.OrientationDetector;
import com.MephestoKhaan.Movemment.StepDetector;
import com.MephestoKhaan.gyroscopestreamer.MainActivity.PORTFORWARDING;

public class MovementActivity extends Activity implements VRSensorListener
{
	private TextView stepText, orientationText;
	private ToggleButton viewModeButton;
	
    private StepDetector mStepDetector;
    private OrientationDetector mOrientationDetector;
    private SensorManager mSensorManager;
    
	@Override
	protected void onCreate(Bundle savedInstanceState)
	{
		super.onCreate(savedInstanceState);
		setContentView(R.layout.walk_layout);
		
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
		orientationText  = (TextView) findViewById(R.id.movementorientationtext);
		stepText  = (TextView) findViewById(R.id.steptext);
		viewModeButton = (ToggleButton)findViewById(R.id.toggleButton1);
	}
	
	public void toggleHeading(View v)
	{
		ToggleButton button = (ToggleButton) v;
		String msg = button.isChecked()? "eyes" : "hip";
		new SendMessage().execute(PORTFORWARDING.TOGGLE_VIEW.getValue(),"UDP",msg);
	}
	
	@Override
	public void onOrientation(double[] orientation) 
	{
		if(!viewModeButton.isChecked())
		{
			String msg = ""+orientation[2]+";"+orientation[0];
			orientationText.setText(msg.replace(';', '\n').replace(':', '\n'));
			new SendMessage().execute(PORTFORWARDING.MOV_ORIENTATION.getValue(),"UDP",msg);
		}
	}
	
	@Override
	public void onStep(double strenght)
	{
		String msg = ""+strenght;
		stepText.setText(msg);
		new SendMessage().execute(PORTFORWARDING.STEP.getValue(),"UDP",msg);
	}
	
	
	private void startDetection()
	{
        mStepDetector = new StepDetector();
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
        
        mSensorManager.registerListener(mStepDetector, mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER), SensorManager.SENSOR_DELAY_UI);
        mStepDetector.addStepListener(this);
        
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
    	mStepDetector.removeMovementListeners();
    	
        mSensorManager.unregisterListener(mStepDetector);
        mSensorManager.unregisterListener(mOrientationDetector);
    }

}