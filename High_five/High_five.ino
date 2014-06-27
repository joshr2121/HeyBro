//  *****************************************************************************************************************
//  *                                                                                                               *
//  *                                         Nick Kornek & SpikenzieLabs.com                                                     *
//  *                                                                                                               *
//  *                                           High-5000 (Based on Drum Kit - Kit)                                                      *
//  *                                                                                                               *
//  *                                                                                                               *
//  *****************************************************************************************************************
//
//  ORIGINAL CODE BY: MARK DEMERS Copywrite 20009
//  April. 2009
//  VERSION: 1.b
//
//  MODIFIED BY: NICK KORNEK 2014
//
//  DESCRIPTION:
//  Arduino analog input for use with High-5000 controller
//  
//
//  
// LEGAL:
// This code is provided as is. No guaranties or warranties are given in any form. It is your responsibilty to 
// determine this codes suitability for your application.




//*******************************************************************************************************************
// User settable variables			
//*******************************************************************************************************************

unsigned char PadNote[6] = {52,16,66,63,40,65};         // MIDI notes from 0 to 127 (Mid C = 60)

// JOSH'S LAPTOP
//int PadCutOff[6] = {25,10,100,200,40,100};           // Minimum Analog value to cause a drum hit

// ACB'S LAPTOP
int PadCutOff[6] = {100,100,500,400,200,500};           // Minimum Analog value to cause a drum hit

int MaxPlayTime[6] = {90,90,90,90,90,90};               // Cycles before a 2nd hit is allowed

#define  midichannel	0;                              // MIDI channel from 0 to 15 (+1 in "real world")

boolean VelocityFlag  = true;                           // Velocity ON (true) or OFF (false)





//*******************************************************************************************************************
// Internal Use Variables			
//*******************************************************************************************************************

boolean activePad[6] = {0,0,0,0,0,0};                   // Array of flags of pad currently playing
int PinPlayTime[6] = {0,0,0,0,0,0};                     // Counter since pad started to play

unsigned char status;

int pin = 0;     
int hitavg = 0;

//*******************************************************************************************************************
// Setup			
//*******************************************************************************************************************

void setup() 
{
  Serial.begin(9600);                                  // connect to the serial port 115200
}

//*******************************************************************************************************************
// Main Program			
//*******************************************************************************************************************

void loop() 
{
  for(int pin=0; pin < 6; pin++)
  {
    hitavg = analogRead(pin);                              // read the input pin

    if((hitavg > PadCutOff[pin]))
    {
      if (analogRead(0) > analogRead(1) & analogRead(0) > analogRead(2) & analogRead(0) > PadCutOff[0])
        {
          Serial.println(1);
          Serial.flush();
        }
      else if (analogRead(1) > analogRead(0) & analogRead(1) > analogRead(2) & analogRead(1) > PadCutOff[1])
        {
          Serial.println(2);
          Serial.flush();
        }
      else if (analogRead(2) > analogRead(0) & analogRead(2) > analogRead(1) & analogRead(2) > PadCutOff[2])
        {
          Serial.println(3);
          Serial.flush();
        }
      if (analogRead(3) > analogRead(4) & analogRead(3) > analogRead(5) & analogRead(3) > PadCutOff[3])
        {
          Serial.println(4);
          Serial.flush();
        }
      else if (analogRead(4) > analogRead(3) & analogRead(4) > analogRead(5) & analogRead(4) > PadCutOff[4])
        {
          Serial.println(5);
          Serial.flush();
        }
      else if (analogRead(5) > analogRead(3) & analogRead(5) > analogRead(4) & analogRead(5) > PadCutOff[5])
        {
          Serial.println(6);
          Serial.flush();
        }
     // Serial.println(pin+1);
     // Serial.flush();
    }
  } 
}
