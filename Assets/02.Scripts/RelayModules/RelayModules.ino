#include <SoftwareSerial.h> 

SoftwareSerial BTSerial(7,8);

int relayH = 3;
int relayF = 5;

char data;

void setup() 
{
    Serial.begin(9600);
    BTSerial.begin(38400);

    pinMode(relayH, OUTPUT);
    pinMode(relayF, OUTPUT);
    
    data = 0;
}

void loop() 
{
  if(BTSerial.available())
  {
    data = BTSerial.read();

    switch(data)
    {
      case '0' :
        digitalWrite(relayH, LOW); // 전부 Off
        digitalWrite(relayF, LOW);
        BTSerial.println(0);
        break;
      case '1' :
        digitalWrite(relayH, HIGH);  // 가습기 모듈 On, 팬 모터 Off
        digitalWrite(relayF, LOW);
        BTSerial.println(1);
        break; 
      case '2' :
        digitalWrite(relayF, HIGH); // 팬 모터 On, 가습기 모듈 Off
        digitalWrite(relayH, LOW);
        break;
    }    
  }
}
