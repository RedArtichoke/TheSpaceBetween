//libraries
#include <PulseSensorPlayground.h> //includes the PulseSensorPlayground Library.   

///// Pin definitions /////

const int PulseSensorPin = A0; //pulse sensor pin
const int redLEDPin = 13; //heart beat pin

//RGB Pin Definitions
const int redRGBPin = 6; 
const int greenRGBPin = 11;
const int blueRGBPin = 4;

///// Timing and states /////

int pulseSensorSignal; 
int pulseThreshold = 530; //determine which pulseSensorSignal to "count as a beat" and which to ignore.

float batteryLevel;
                    
PulseSensorPlayground pulseSensor; //creates an instance of the PulseSensorPlayground object called "pulseSensor"

void setup() 
{   
  Serial.begin(9600); 

  //configure the PulseSensor object, by assigning our variables to it. 
  pulseSensor.analogInput(PulseSensorPin);   
  pulseSensor.blinkOnPulse(redLEDPin); //blink arduino LED
  pulseSensor.setThreshold(pulseThreshold);   

  pinMode(redRGBPin, OUTPUT);
  pinMode(greenRGBPin, OUTPUT);
  pinMode(blueRGBPin, OUTPUT);
  //pinMode(whiteLEDPin, OUTPUT);
  //pinMode(buttonPin, INPUT_PULLUP);
}


void loop() 
{
  
  unityPowerLevel();
  RGBLedBattery(batteryLevel); 
  calculateBPM();

  delay(20);                    
}

void calculateBPM()
{
  //test to see if "a beat happened".
  if (pulseSensor.sawStartOfBeat()) 
  {            
    int myBPM = pulseSensor.getBeatsPerMinute();  //calls function on our pulseSensor object that returns BPM as an "int".
    Serial.println(myBPM); //Send BPM to Unity

  }
}

//function to set RGB LED color 
void setColor(int red, int green, int blue) 
{
  analogWrite(redRGBPin, red);
  analogWrite(greenRGBPin, green);
  analogWrite(blueRGBPin, blue);
}

//function to map a value from 0-100 to a color gradient (Green to Red)
void RGBLedBattery(int value)
{
  //map value (0-100) to RGB color range (Green to Red)
  int red = map(value, 0, 150, 255, 0);
  int green = map(value, 0, 150, 0, 255);
  int blue = 0; 
  
  setColor(red, green, blue); //update the RGB LED color
}

void unityPowerLevel()
{
  //change battery level light based on what power level is in unity
  switch (Serial.read())
  {
        case 'A':
            batteryLevel = 100;
            break;
        case 'B':
            batteryLevel = 90;
            break;
        case 'C':
            batteryLevel = 80;
            break;
        case 'D':
            batteryLevel = 70;
            break;
        case 'E':
            batteryLevel = 60;
            break;
        case 'F':
            batteryLevel = 50;
            break;
        case 'G':
            batteryLevel = 40;
            break;
        case 'H':
            batteryLevel = 30;
            break;
        case 'I':
            batteryLevel = 20;
            break;
        case 'J':
            batteryLevel = 10;
            break;
        case 'K':
            batteryLevel = 0;
            break;
  }
}

