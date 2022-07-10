# include <SoftwareSerial.h>    // 소프트웨어 시리얼 통신 라이브러리 호출

/*  
1)PIN 13 : 심장 박동에 따른 LED 깜빡임 - 심박수
2)PIN 05 : 실시간 심장 박동에 따른 LED 페이드 - 심박 간격( 심각 간격이 빠르면 LED가 밝게, 심박 간격이 느리면 LED가 느리게)
3)BPM 측정
4)위의 내용을 직렬로 출력
*/

// 변수
SoftwareSerial BTSerial(7, 8);  // 소프트웨어 시리얼포트 (TX, RX)
int pulsePin = 0;                 // 아날로그 핀 0번에 연결된 심장박동센서
int blinkPin = 13;                // 13번 핀에 연결된 LED : 심장 박동에 따라 깜빡임
int fadePin = 5;                  // 5번 핀에 연결된 LED : 실시간 심장 박동에 따른 페이드
int fadeRate = 0;                 // 5번 핀 LED에 페이드를 줄 때 사용할 비율

// 휘발성 변수, Interrupt 루틴(심장 박동 센서 읽기)에 사용
volatile int BPM;                   // 아날로그 0번핀에서 들어오는 int. 매 2 밀리초마다 업데이트됨.
volatile int Signal;                // 들어오는 raw data
volatile int IBI = 600;             // 맥박 사이의 시간 간격을 유지하는 int
volatile boolean Pulse = false;     // 실시간 맥박이 감지될 경우 true, 실시간 맥박이 아닌 경우 false
volatile boolean QS = false;        // 맥박이 감지될 때 true가 됨

void setup()
{
  pinMode(blinkPin,OUTPUT);         // 심장박동에 따라 깜빡일 13번핀 LED, OUTPUT으로 pinMode 설정
  pinMode(fadePin,OUTPUT);          // 심장박동에 따라 페이드 할 5번핀 LED, OUTPUT으로 pinMode 설정
  Serial.begin(115200);             // 시리얼통신 속도 설정
  BTSerial.begin(38400);
  interruptSetup();                 // 2밀리초마다 심장 박동 센서 신호를 읽기
}

void loop()
{
  if (QS == true)
  {     
        // 심장 박동이 감지됨. BPM 과 IBI이 측정됨. 
        fadeRate = 255;                   // LED Fade 효과 발생
                                          // 맥박으로 LED를 페이드하려면 'fadeRate' 변수를 255로 설정해야 함.
        QS = false;                       // QS 리셋
  }

  ledFadeToBeat();                        // LED Fade 효과 발생
  delay(20);                      
}

void ledFadeToBeat()
{
    fadeRate -= 15;                         //  LED fade 값 설정
    fadeRate = constrain(fadeRate,0,255);   //  LED fade 값이 음수가 되지 않도록 유지
    analogWrite(fadePin,fadeRate);          //  fade LED
  }
