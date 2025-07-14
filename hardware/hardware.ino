/**************
 * (핀 정의는 이전과 동일)
 ***************/

// Dial 구조체는 이전과 동일
typedef struct sDial {
  int A_pin, B_pin;
  int value;
  int oldEncoderA, oldEncoderB;

  sDial(int pinClk, int pinDt) : A_pin(pinClk), B_pin(pinDt), value(0) {
    pinMode(A_pin, INPUT_PULLUP);
    pinMode(B_pin, INPUT_PULLUP);
    oldEncoderA = digitalRead(A_pin);
    oldEncoderB = digitalRead(B_pin);
  }

  bool readAndProcess() {
    int currentA = digitalRead(A_pin);
    int currentB = digitalRead(B_pin);
    bool changed = false;

    if (currentA != oldEncoderA) {
      if (currentA == LOW) {
        if (currentB != currentA) value++;
        else value--;
        changed = true;
      }
    }
    oldEncoderA = currentA;
    oldEncoderB = currentB;
    return changed;
  }
} Dial;

Dial elevationDial(3, 4);
Dial windageDial(5, 6);
Dial parallaxDial(7, 8);

const int zoomPotPin = A1;
const int reloadSliderPin = A0;

// 각 센서의 이전 값을 저장하여 변경 감지
int prevElevationValue = 0;
int prevWindageValue = 0;
int prevParallaxValue = 0;
int prevZoomValue = 0;
int prevReloadValue = 0;

const int analogThreshold = 5;

unsigned long lastCheckTime = 0;
const unsigned long checkInterval = 20; // 20ms마다 센서 값 변화 확인

void setup() {
  Serial.begin(9600);
}

void loop() {
  if (millis() - lastCheckTime >= checkInterval) {
    lastCheckTime = millis();

    // 각 엔코더 다이얼 처리 및 값 변경 감지
    bool elevationChanged = elevationDial.readAndProcess();
    bool windageChanged = windageDial.readAndProcess();
    bool parallaxChanged = parallaxDial.readAndProcess();

    // 가변저항 값 읽기 및 변경 감지
    int currentZoom = analogRead(zoomPotPin);
    bool zoomChanged = abs(currentZoom - prevZoomValue) > analogThreshold;
    if (zoomChanged) prevZoomValue = currentZoom;

    int currentReload = analogRead(reloadSliderPin);
    bool reloadChanged = abs(currentReload - prevReloadValue) > analogThreshold;
    if (reloadChanged) prevReloadValue = currentReload;

    // 변경된 데이터만 담을 String 객체
    String dataToSend = "";

    if (elevationChanged) {
      if (dataToSend.length() > 0) dataToSend += ";";
      dataToSend += "E:";
      dataToSend += elevationDial.value;
    }
    if (windageChanged) {
      if (dataToSend.length() > 0) dataToSend += ";";
      dataToSend += "W:";
      dataToSend += windageDial.value;
    }
    if (parallaxChanged) {
      if (dataToSend.length() > 0) dataToSend += ";";
      dataToSend += "P:";
      dataToSend += parallaxDial.value;
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

    // 변경된 데이터가 하나라도 있다면 전송
    if (dataToSend.length() > 0) {
      Serial.println(dataToSend); // 한 줄로 전송 후 줄바꿈
    }
  }

  delay(1);
}
