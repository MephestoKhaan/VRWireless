package com.MephestoKhaan.Movemment;

import java.util.ArrayList;

import org.joda.time.Interval;

import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;


public class StepDetector implements SensorEventListener
{
	private boolean check = false;
	
	private float gravity = 9.81f;
    private float   upperLimit = 1.5f;
    private float   lowerLimit = -1.5f;
    private int newRithm_ = 3;
    private int steps_ = newRithm_;
    
    private MeanQueue interval_ = new MeanQueue();
    private MeanQueue outlier_ = new MeanQueue();
    private Interval AcceptedSpan = new Interval(0,0);
    private Interval LastSpan = new Interval(0,0);
    
    private ArrayList<VRSensorListener> mStepListeners = new ArrayList<VRSensorListener>();
    
    public StepDetector() 
    {
    }
    
    public void addStepListener(VRSensorListener sl)
    {
        mStepListeners.add(sl);
    }
    
    public void removeMovementListeners()
    {
    	mStepListeners.clear();
    }
    
    public void onSensorChanged(SensorEvent event)
    {
	  if (event.sensor.getType() == Sensor.TYPE_ACCELEROMETER)
	  {
		  detectStep(event);
	  }
    }
    
    private void detectStep(SensorEvent acceleration)
    {
    	 float x = 0.0f;
         float y = 0.0f;
         float z = 0.0f;
         double mod = 0.0f;
         
         synchronized (this) 
         {
          	x = acceleration.values[0];
          	y = acceleration.values[1];
          	z = acceleration.values[2];
	         mod = Math.sqrt(x*x + y*y + z*z) - gravity;

	        if(CheckAccOnUp(mod))
	        {
	        	sendStep();
	        }
         }
    }
    
    private void sendStep()
    {
    	float stepsCount = CheckStep();
    	for(VRSensorListener listener : mStepListeners)
    	{
    		listener.onStep(stepsCount);
    	}
    }
    
    private boolean CheckAccOnUp(double p)
    {
    	if(p >= upperLimit && !check)
    	{
	    	check = true;
	    	return true;
	    }
    	
    	if(p <= lowerLimit)
    	{
	    	check = false;
    	}
    	return false;
    }
   
    
    float CheckStep()
    {
    	Interval  span  = new Interval(0,android.os.SystemClock.uptimeMillis());
    	
    	float stepAccuracy = 1.0f;
    	int steps = 0;
    	long ticks;
    	
    	boolean accepted = false;
    	if(LastSpan.toDurationMillis()!= 0)//La primera medida sirve solo de referencia, no como paso.
    	{
    		ticks = (span.toDurationMillis()-AcceptedSpan.toDurationMillis());
    		
    		
    		if(interval_.Similar(ticks))//Lecturas validas que continúan el ritmo (y la primera)
    		{
    			stepAccuracy = interval_.AccuracyOf(ticks);
    			interval_.Add(ticks);
    			AcceptedSpan = span;
    			accepted = true;
    			steps = 1;
    		}
    		if(!accepted || outlier_.GetSize() > 0)
    		{
    			ticks = (span.toDurationMillis()-LastSpan.toDurationMillis());
    			if(outlier_.Similar(ticks))//Si se produce un cambio de ritmo
    			{

        			stepAccuracy = outlier_.AccuracyOf(ticks);
    				outlier_.Add(ticks);
    				if(outlier_.GetSize()>= newRithm_)//Si el ritmo nuevo se mantiene
    				{
    					interval_.Set(outlier_);
    					AcceptedSpan = span;
    					steps = steps_;
    					steps_ = newRithm_;
    				}else if(accepted && outlier_.GetSize() == 2)
    				{
    					steps_ = newRithm_-1;
    				}

    			}
    			else
    			{//Si el ritmo es totalmente nuevo
    				outlier_.Clear();
    				outlier_.Add(ticks);
    				steps_ = newRithm_;
    			}
    		}
    	}
    	else
    	{
    		AcceptedSpan = span;
    	}
    	LastSpan = span;
    	
    	return stepAccuracy;
    	//return steps;
    }
    
	@Override
	public void onAccuracyChanged(Sensor sensor, int accuracy)
	{
	}
    
   

}