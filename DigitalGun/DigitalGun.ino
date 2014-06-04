6wxdsd#define BUFFER_LENGTH    3     // 3 bytes gives us 24 samples
#define NUM_INPUTS       3    // 6 on the front + 12 on the back
//#define TARGET_LOOP_TIME 694   // (1/60 seconds) / 24 samples = 694 microseconds per sample 
//#define TARGET_LOOP_TIME 758  // (1/55 seconds) / 24 samples = 758 microseconds per sample 
#define TARGET_LOOP_TIME 744  // (1/56 seconds) / 24 samples = 744 microseconds per sample 

#include "settings.h"

typedef struct {
  byte pinNumber;
  int keyCode;
  byte measurementBuffer[BUFFER_LENGTH]; 
  boolean oldestMeasurement;
  byte bufferSum;
  boolean pressed;
  boolean prevPressed;
} 
MakeyMakeyInput;

MakeyMakeyInput inputs[NUM_INPUTS];

int bufferIndex = 0;
byte byteCounter = 0;
byte bitCounter = 0;

int pressThreshold;
int releaseThreshold;

// Pin Numbers
// input pin numbers for kickstarter production board
int pinNumbers[NUM_INPUTS] = {
  18,19,20
};


// timing
int loopTime = 0;
int prevTime = 0;
int loopCounter = 0;

void initializeArduino();
void initializeInputs();
void updateMeasurementBuffers();
void updateBufferSums();
void updateBufferIndex();
void updateInputStates();
void addDelay();


void setup() 
{
  initializeArduino();
  initializeInputs();
}

void loop() 
{
  updateMeasurementBuffers();
  updateBufferSums();
  updateBufferIndex();
  updateInputStates();
  addDelay();
}

void initializeArduino()
{
  /* Set up input pins 
   DEactivate the internal pull-ups, since we're using external resistors */
  for (int i=0; i<NUM_INPUTS; i++)
  {
    pinMode(pinNumbers[i], INPUT);
    digitalWrite(pinNumbers[i], LOW);
  }

  Keyboard.begin();
  Mouse.begin();
}

void initializeInputs()
{

  float thresholdPerc = SWITCH_THRESHOLD_OFFSET_PERC;
  float thresholdCenterBias = SWITCH_THRESHOLD_CENTER_BIAS/50.0;
  float pressThresholdAmount = (BUFFER_LENGTH * 8) * (thresholdPerc / 100.0);
  float thresholdCenter = ( (BUFFER_LENGTH * 8) / 2.0 ) * (thresholdCenterBias);
  pressThreshold = int(thresholdCenter + pressThresholdAmount);
  releaseThreshold = int(thresholdCenter - pressThresholdAmount);


  for (int i=0; i<NUM_INPUTS; i++)
  {
    inputs[i].pinNumber = pinNumbers[i];
    inputs[i].keyCode = keyCodes[i];

    for (int j=0; j<BUFFER_LENGTH; j++)
    {
      inputs[i].measurementBuffer[j] = 0;
    }
    inputs[i].oldestMeasurement = 0;
    inputs[i].bufferSum = 0;

    inputs[i].pressed = false;
    inputs[i].prevPressed = false;
  }
}

void updateMeasurementBuffers()
{

  for (int i=0; i<NUM_INPUTS; i++) {

    // store the oldest measurement, which is the one at the current index,
    // before we update it to the new one 
    // we use oldest measurement in updateBufferSums
    byte currentByte = inputs[i].measurementBuffer[byteCounter];
    inputs[i].oldestMeasurement = (currentByte >> bitCounter) & 0x01; 

    // make the new measurement
    boolean newMeasurement = digitalRead(inputs[i].pinNumber);

    // invert so that true means the switch is closed
    newMeasurement = !newMeasurement; 

    // store it    
    if (newMeasurement)
    {
      currentByte |= (1<<bitCounter);
    } 
    else
    {
      currentByte &= ~(1<<bitCounter);
    }
    inputs[i].measurementBuffer[byteCounter] = currentByte;
  }
}

void updateBufferSums()
{

  // the bufferSum is a running tally of the entire measurementBuffer
  // add the new measurement and subtract the old one
  for (int i=0; i<NUM_INPUTS; i++)
  {
    byte currentByte = inputs[i].measurementBuffer[byteCounter];
    boolean currentMeasurement = (currentByte >> bitCounter) & 0x01; 
    if (currentMeasurement)
    {
      inputs[i].bufferSum++;
    }
    if (inputs[i].oldestMeasurement)
    {
      inputs[i].bufferSum--;
    }
  }  
}

void updateBufferIndex()
{
  bitCounter++;
  if (bitCounter == 8)
  {
    bitCounter = 0;
    byteCounter++;
    if (byteCounter == BUFFER_LENGTH)
    {
      byteCounter = 0;
    }
  }
}

void updateInputStates()
{
  for (int i=0; i<NUM_INPUTS; i++)
  {
    inputs[i].prevPressed = inputs[i].pressed; // store previous pressed state
    if (inputs[i].pressed)
    {
      if (inputs[i].bufferSum < releaseThreshold)
      {  
        inputs[i].pressed = false;
        if(inputs[i].keyCode == CODE_CLIP)
        {
          Keyboard.write(CODE_CLIP_OFF);
        }
        else if(inputs[i].keyCode == CODE_TRIGGER)
        {
          Keyboard.write(CODE_TRIGGER_OFF);
        }
        else
        {
          Keyboard.release(inputs[i].keyCode); 
        }
      }
    } 
    else if(!inputs[i].pressed)
    {
      if (inputs[i].bufferSum > pressThreshold)
      {  
        inputs[i].pressed = true; 
        
        if(inputs[i].keyCode == CODE_CLIP)
        {
          Keyboard.write(CODE_CLIP_ON);
        }
        else if(inputs[i].keyCode == CODE_TRIGGER)
        {
          Keyboard.write(CODE_TRIGGER_ON);
        }
        else
        {
          Keyboard.press(inputs[i].keyCode); 
        }
      }
    }
  }
}

void addDelay()
{
  loopTime = micros() - prevTime;
  if (loopTime < TARGET_LOOP_TIME)
  {
    int wait = TARGET_LOOP_TIME - loopTime;
    delayMicroseconds(wait);
  }

  prevTime = micros();
}
