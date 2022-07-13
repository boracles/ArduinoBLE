#include <SoftwareSerial.h>     // 소프트웨어 시리얼 통신 라이브러리 호출

SoftwareSerial BTSerial(7, 8);  // 소프트웨어 시리얼포트 (TX, RX)

void setup()
{
  Serial.begin(9600);   // 컴퓨터와의 시리얼 통신 초기화
  Serial.println("아두이노가 준비되었습니다.");
  Serial.println("시리얼 모니터에서 Both NL & CR 을 선택하세요.");
  
  BTSerial.begin(9600); // 블루투스 모듈과의 시리얼 통신 초기화
}

void loop() 
{
  if(BTSerial.available())      // 블루투스에 신호가 들어오면
  {
    char ch = BTSerial.read();  // 블루투스 모듈 > 아두이노 > 시리얼 모니터
    Serial.write(ch);
  }
  if(Serial.available())        // 시리얼에 신호가 들어오면
  {
    char ch = Serial.read();    // 시리얼 모니터 > 아두이노 > 블루투스 모듈
    BTSerial.write(ch);
  }
}
