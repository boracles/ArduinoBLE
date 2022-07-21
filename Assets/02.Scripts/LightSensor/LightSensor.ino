#include <SoftwareSerial.h>     // 소프트웨어 시리얼 통신 라이브러리 호출

SoftwareSerial BTSerial(7, 8);  // 소프트웨어 시리얼포트 (TX, RX)

int cds = 0;

void setup()
{
  Serial.begin(9600);
  BTSerial.begin(38400);    //블루투스 모듈과의 시리얼 통신 초기화
}

void loop()
{
  cds = analogRead(A0);     // 아날로그 0번핀

  Serial.println(cds);
  BTSerial.println(cds);    // 아날로그 0번핀으로 들어오는 값 유니티로 전송
  delay(200);               // 0.2초 대기
}
