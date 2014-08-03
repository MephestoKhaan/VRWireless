package com.MephestoKhaan.gyroscopestreamer;

import android.os.Bundle;
import android.app.Activity;
import android.content.Intent;
import android.view.View;
import android.widget.EditText;


public class MainActivity extends Activity
{
	public static enum PORTFORWARDING
	{
		STEP("0"),
		MOV_ORIENTATION("1"),
		GUN_ORIENTATION("2"),
		FIRE("3"),
		TOGGLE_VIEW("4"),
		SYNC("5");
		
		private String value;
	    private PORTFORWARDING(String value)
	    {
	       this.value = value;
	    }
	    public String getValue() {
	       return value;
	    }
	}
	
	private EditText mIPAddress;
	private EditText mPortPAddress;
    
	@Override
	protected void onCreate(Bundle savedInstanceState)
	{
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);
		
        initializeVariables();
	}
	
	private void initializeVariables()
	{
		mIPAddress = (EditText) findViewById(R.id.editText1);
		mPortPAddress = (EditText) findViewById(R.id.editText2);
		updateAddress(null);
	}
	

	public void updateAddress(View v)
	{
		SendMessage.SetAddress(mIPAddress.getText().toString(), mPortPAddress.getText().toString(),this);
	}
	
	public void goToMovementActivity(View v)
	{
    	startActivity(new Intent(this, MovementActivity.class));
	}
	
	public void goToGunActivity(View v)
	{
    	startActivity(new Intent(this, GunActivity.class));
	}
}

