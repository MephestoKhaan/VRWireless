#include "Arduino.h"

int CODE_TRIGGER = 't';
int CODE_TRIGGER_ON = 't';
int CODE_TRIGGER_OFF = 'y';
int CODE_CLIP = 'x';
int CODE_CLIP_ON = 'c';
int CODE_CLIP_OFF = 'v';
int CODE_PUMP = 'p';

int keyCodes[NUM_INPUTS] =
{
  CODE_TRIGGER,      
  CODE_CLIP,    
  CODE_PUMP    
};

#define SWITCH_THRESHOLD_OFFSET_PERC  5 
#define SWITCH_THRESHOLD_CENTER_BIAS 55
