volatile int rate[10];                        // 마지막 10개의 IBI 값을 저장할 배열
volatile unsigned long sampleCounter = 0;     // 맥박 타이밍을 결정하는 데 사용
volatile unsigned long lastBeatTime = 0;      // IBI를 찾는 데 사용
volatile int P =512;                          // 맥파에서 최고점를 찾는데 사용
volatile int T = 512;                         // 맥파에서 최저점을 찾는데 사용
volatile int thresh = 530;                    // 심장 박동의 즉각적인 순간을 찾는데 사용
volatile int amp = 0;                         // 맥파의 진폭을 유지하는 데 사용
volatile boolean firstBeat = true;     
volatile boolean secondBeat = false;         


void interruptSetup()
{  
  TCCR2A = 0x02;     // 디지털 핀 3 및 11에서 PWM을 비활성화하고 CTC 모드로 이동
  TCCR2B = 0x06;     // 256 PRESCALER
  OCR2A = 0X7C;      // 500Hz 샘플 속도의 경우 카운트의 상단을 124로 설정
  TIMSK2 = 0x02;     // TIMER2와 OCR2A 사이의 매치에서 인터럽트 활성화
  sei();             // 전역 interrupt가 활성화되어 있는지 확인
}


// TIMER 2 INTERRUPT SERVICE ROUTINE : 2ms 마다 판독
ISR(TIMER2_COMPA_vect)
{                                             // Timer2가 124로 계산될 때 트리거됨
  cli();                                      // 이 작업을 수행하는 동안 interrupt 비활성화
  Signal = analogRead(pulsePin);              // 맥박 센서 읽기
  sampleCounter += 2;                         // ms 단위로 추적
  int N = sampleCounter - lastBeatTime;       // 마지막 맥박 이후의 시간을 모니터링하여 노이즈 방지

    // 맥파의 최고점과 최저점 찾기
  if(Signal < thresh && N > (IBI/5)*3)
  {                                           // 마지막 IBI의 3/5를 대기하여 노이즈 방지 
    if (Signal < T)
    {                                         // T : 최저점
      T = Signal;                             // 맥파의 가장 낮은 지점 추적
    }
  }

  if(Signal > thresh && Signal > P)
  {                                           // 노이즈 방지
    P = Signal;                               // P : 최고치
  }                                           // 맥파의 가장 높은 지점을 추적

  // 심장 박동 찾기
  // 맥박이 감지될 때마다 신호 증가
  if (N > 250)
  {                                   // 고주파 노이즈 피하기
    if ( (Signal > thresh) && (Pulse == false) && (N > (IBI/5)*3) )
    {
      Pulse = true;                               // 맥박이 감지됨
      digitalWrite(blinkPin,HIGH);                // pin 13 LED 켜기
      BTSerial.println("1");
      Serial.println("1");
      IBI = sampleCounter - lastBeatTime;         // 맥박을 ms 단위로 측정
      lastBeatTime = sampleCounter;               // 다음 맥박에 대한 시간 추적

      if(secondBeat)
      {                                           // 두번째 맥박일 경우
        secondBeat = false;                 
        for(int i=0; i<=9; i++)
        {                                         // 시작시 실제 BPT값을 얻기 위함
          rate[i] = IBI;
        }
      }

      if(firstBeat)
      {                                      // 첫번째 맥박일 경후
        firstBeat = false;                   
        secondBeat = true;                  
        sei();                               // interrupts 재활성화
        return;                           
      }


      // 마지막 10개의 IBI 값 유지
      word runningTotal = 0;                  // runningTotal 변수 지우기

      for(int i=0; i<=8; i++)
      {                                       // rate 배열의 데이터 이동
        rate[i] = rate[i+1];                  // 가장 오래된 IBI 값 삭제
        runningTotal += rate[i];              // 9개의 가장 오래된 IBI 값 더하기
      }

      rate[9] = IBI;                          // rate 배열에 최신 IBI 추가
      runningTotal += rate[9];                // runningTotal에 최신 IBI 추가
      runningTotal /= 10;                     // 마지막 10개의 IBI 값 평균
      BPM = 60000/runningTotal;               // BPM
      Serial.println(BPM);
      BTSerial.println(BPM);
      QS = true;                           
    }
  }

  if (Signal < thresh && Pulse == true)
  {                                        // 값이 내려가는 시점이 맥박이 끝나는 시점
    digitalWrite(blinkPin,LOW);            // pin 13 LED 끄기
    BTSerial.println("0");
    Serial.println("0");
    Pulse = false;                         // 맥박 리셋
    amp = P - T;                           // 맥파의 진폭 얻기
    thresh = amp/2 + T;                    // thresh 임계값 설정 : 진폭의 50%
    P = thresh;                   
    T = thresh;
  }

  if (N > 2500)
  {                                        // 맥박없이 2.5초가 지나가면
    thresh = 530;                          // thresh 기본값 설정
    P = 512;                               // P 기본값 설정
    T = 512;                               // T 기본값 설정
    lastBeatTime = sampleCounter;          // lastBeatTime을 최신 상태로 유지
    firstBeat = true;                     
    secondBeat = false;                   
  }

  sei();                                   // 완료되면 interrupt 활성화
}

// isr 끝
