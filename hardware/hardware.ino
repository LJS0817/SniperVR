#include <PinChangeInterrupt.h>
#include <PinChangeInterruptBoards.h> // 이 헤더도 포함하는 것이 좋습니다.

/**************
 * (핀 정의는 이전과 동일)
 *
 * Elevation Dial: CLK to D3, DT to D4 (D3은 외부 인터럽트 가능, D4는 PCINT 필요)
 * Windage Dial: CLK to D5, DT to D6 (D5, D6 모두 PCINT 필요)
 * Parallax Dial: CLK to D7, DT to D8 (D7, D8 모두 PCINT 필요)
 *
 * (주의: 이 예시는 A_pin에만 인터럽트를 걸지만, 더 정확한 디코딩을 위해선 B_pin에도 거는 것이 좋음.
 * 모든 엔코더의 A/B 핀에 PCINT를 걸고 ISR 내부에서 모두 처리해야 함.)
 ***************/

typedef struct sDial {
  int A_pin, B_pin;
  volatile int value;
  uint8_t oldState;

  const int8_t enc_table[16] = {
    0, -1,  1,  0,
    1,  0,  0, -1,
   -1,  0,  0,  1,
    0,  1, -1,  0
  };

  sDial(int pinA, int pinB) : A_pin(pinA), B_pin(pinB), value(0) {
    pinMode(A_pin, INPUT_PULLUP);
    pinMode(B_pin, INPUT_PULLUP);
    oldState = (digitalRead(A_pin) << 1) | digitalRead(B_pin);
  }

  // 이 함수는 이제 PinChangeInterrupt ISR에서 호출됩니다.
  bool updateFromISR() {
    uint8_t currentState = (digitalRead(A_pin) << 1) | digitalRead(B_pin);
    uint8_t index = (oldState << 2) | currentState;
    int8_t delta = enc_table[index];
    bool changed = false;

    if (delta != 0) {
      value += delta; // 이 줄은 엔코더의 '한 스텝' 변화당 1씩 증가/감소하게 합니다.
      changed = true;
    }
    oldState = currentState; // 항상 업데이트
    return changed;
  }
} Dial;

// 모든 다이얼 인스턴스 전역 선언 (ISR에서 접근 위함)
Dial elevationDial(2, 3); // D2, D3
Dial windageDial(4, 5);   // D4, D5
Dial parallaxDial(6, 7);  // D6, D7

const int zoomPotPin = A1;
const int reloadSliderPin = A0;

// 이전 값 변수들은 메인 루프에서 전송 여부를 결정하기 위해 필요
int prevElevationValue = 0;
int prevWindageValue = 0;
int prevParallaxValue = 0;
int prevZoomValue = 0;
int prevReloadValue = 0;

const int analogThreshold = 5;

unsigned long lastSendTime = 0;
const unsigned long sendInterval = 20; // 20ms마다 유니티로 데이터 전송

// Serial.print()나 delay()는 여기에 사용하면 안 됩니다.
void elevationA_ISR_Wrapper() { elevationDial.updateFromISR(); }
void elevationB_ISR_Wrapper() { elevationDial.updateFromISR(); }

void windageA_ISR_Wrapper() { windageDial.updateFromISR(); }
void windageB_ISR_Wrapper() { windageDial.updateFromISR(); }

void parallaxA_ISR_Wrapper() { parallaxDial.updateFromISR(); }
void parallaxB_ISR_Wrapper() { parallaxDial.updateFromISR(); }


void setup() {
  Serial.begin(9600);

  // === PinChangeInterrupt 설정 ===
  // 각 엔코더의 A, B 핀 모두에 인터럽트를 겁니다.
  // digitalPinToPinChangeInterrupt() 함수는 해당 디지털 핀이 속한
  // PinChangeInterrupt 객체를 반환하며, 이 객체에 콜백 함수를 연결합니다.
  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(elevationDial.A_pin), elevationA_ISR_Wrapper, CHANGE);
  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(elevationDial.B_pin), elevationB_ISR_Wrapper, CHANGE);

  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(windageDial.A_pin), windageA_ISR_Wrapper, CHANGE);
  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(windageDial.B_pin), windageB_ISR_Wrapper, CHANGE);

  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(parallaxDial.A_pin), parallaxA_ISR_Wrapper, CHANGE);
  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(parallaxDial.B_pin), parallaxB_ISR_Wrapper, CHANGE);

  // 참고: CHANGE 모드는 핀이 HIGH에서 LOW로 또는 LOW에서 HIGH로 변경될 때 모두 인터럽트를 발생시킵니다.
  // 엔코더 디코딩을 위해선 CHANGE 모드가 적합합니다.
}

void loop() {
  if (millis() - lastSendTime >= sendInterval) {
    lastSendTime = millis();

    // 인터럽트로 업데이트된 엔코더 값은 직접 확인합니다.
    // 이전 값과 비교하여 변경 여부를 결정
    // 여기서 핵심: 엔코더는 한 클릭에 4개의 스텝을 만듭니다.
    // 따라서 value는 한 클릭에 4씩 변합니다.
    // 만약 한 클릭에 1씩 변하기를 원한다면, value를 4로 나누어서 사용해야 합니다.
    bool elevationChanged = (elevationDial.value / 4 != prevElevationValue);
    bool windageChanged = (windageDial.value / 4 != prevWindageValue);
    bool parallaxChanged = (parallaxDial.value / 4 != prevParallaxValue);

    // 가변저항 값 읽기 및 변경 감지 (여전히 폴링)
    int currentZoom = analogRead(zoomPotPin);
    currentZoom = map(currentZoom, 0, 1023, 0, 100);
    bool zoomChanged = abs(currentZoom - prevZoomValue) > analogThreshold;
    if (zoomChanged) prevZoomValue = currentZoom;
    
    int currentReload = analogRead(reloadSliderPin);
    currentReload = map(currentReload, 1023, 0, 0, 100);
    bool reloadChanged = abs(currentReload - prevReloadValue) > 3;
    if (reloadChanged) prevReloadValue = currentReload;
    

    String dataToSend = "";

    if (elevationChanged) {
      if (dataToSend.length() > 0) dataToSend += ";";
      dataToSend += "E:";
      dataToSend += (elevationDial.value / 4); // 4로 나누어 전송
      prevElevationValue = (elevationDial.value / 4);
    }
    if (windageChanged) {
      if (dataToSend.length() > 0) dataToSend += ";";
      dataToSend += "W:";
      dataToSend += (windageDial.value / 4); // 4로 나누어 전송
      prevWindageValue = (windageDial.value / 4);
    }
    if (parallaxChanged) {
      if (dataToSend.length() > 0) dataToSend += ";";
      dataToSend += "P:";
      dataToSend += (parallaxDial.value / 4); // 4로 나누어 전송
      prevParallaxValue = (parallaxDial.value / 4);
    }
    if (zoomChanged) {
      if (dataToSend.length() > 0) dataToSend += ";";
      dataToSend += "Z:";
      dataToSend += currentZoom;
    }
    if (reloadChanged) {
      if (dataToSend.length() > 0) dataToSend += ";";
      dataToSend += "R:";
      dataToSend += currentReload;
    }

    if (dataToSend.length() > 0) {
      Serial.println(dataToSend);
    }
  }
}
