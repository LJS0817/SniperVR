<div align="center">
  <h1>🎯 SniperVR - 하드웨어/소프트웨어 연동 테스트(PoC) 프로젝트</h1>
  <p><strong>Unity와 Arduino 간의 시리얼 통신 성능을 검증하고, 커스텀 하드웨어를 활용한 실감형 조작계를 테스트하기 위해 제작한 하드웨어 연동 프로젝트</strong></p>

  <!-- 방패 뱃지들 -->
  <img src="https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white" alt="Unity">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#">
  <img src="https://img.shields.io/badge/C++-00599C?style=for-the-badge&logo=c%2B%2B&logoColor=white" alt="C++">
  <img src="https://img.shields.io/badge/Arduino-00979D?style=for-the-badge&logo=arduino&logoColor=white" alt="Arduino">
  <img src="https://img.shields.io/badge/VR-0450A5?style=for-the-badge" alt="VR">
  <img src="https://img.shields.io/badge/AI_Used-10B981?style=for-the-badge" alt="AI 사용">
  <br><br>
</div>

## 📌 Project Overview
- **개발 기간:** 2025.07 ~ 2025.08
- **개발 인원:** 1인 개발 (클라이언트 프로그래밍, 하드웨어 설계 및 통신 시스템 구축 전담)
- **장르:** 하드웨어 연동 통신 테스트 (Proof of Concept)
- **AI 활용 내역:** Antigravity를 적극 활용하여, 아두이노 시리얼 통신 데이터 파싱 및 하드웨어 핀 인터럽트 최적화 로직 설계 등 C++ / C# 간 브릿지 시스템 구현에 활용.

## 🎮 Project Concept (Connection Test)
이 프로젝트는 Unity와 Arduino 간의 빠르고 안정적인 시리얼 통신을 검증하기 위한 **연결 테스트(Proof of Concept)** 목적으로 기획되었습니다. VR 컨트롤러의 공간 인식과 직접 제작한 하드웨어 조작계(로터리 엔코더, 가변 저항)를 융합하여 입력 지연(Latency)과 데이터 손실 없는 하드웨어-소프트웨어 통신 파이프라인을 구축하는 것에 중점을 두었습니다.

### 💡 주요 특징 (Key Highlights)
1. **이원화된 융합 컨트롤 시스템 (VR + Custom Hardware):** 
   총기의 기본 위치 이동 및 회전(조준)은 VR 컨트롤러의 6DOF 트래킹을 활용해 직관적으로 처리하고, 세밀한 기계적 조작은 3D 총기 모델에 부착된 아두이노 기반 커스텀 하드웨어로 제어하도록 설계하여 현실감을 극대화했습니다.
2. **리얼 체감형 스코프(Scope) 제어:** 
   3개의 로터리 엔코더(Rotary Encoder)를 스코프의 Elevation(영점 조절), Windage(편향 조절), Parallax/Zoom(시차 및 배율) 다이얼로 구성. 다이얼을 물리적으로 조작하면 Unity 내부 카메라의 FOV(시야각) 및 Transform 값이 즉각적으로 보간(`Mathf.Lerp`)되어 실제 스코프 렌즈를 조작하는 듯한 경험을 제공합니다.
3. **물리 기반 슬라이드 장전(Bolt Action) 메커니즘:** 
   총기에 부착된 슬라이드 포텐셔미터(Slide Potentiometer)의 아날로그 값을 읽어 Unity 내 약실(`Cylinder`)의 Z축 위치와 실시간으로 동기화합니다. 슬라이드를 당기고 미는 물리적 거리에 따라 탄피 배출(Pop Out)과 새 탄환 장전(Push In) 로직이 트리거되는 정교한 사격 시퀀스를 구현했습니다.
4. **최적화된 시리얼 데이터 파싱 아키텍처:** 
   아두이노에서 전달되는 센서 문자열(예: `E:10;W:-2;R:80`)을 유니티에서 `SerializableDictionary` 및 `SensorDataUpdateEvent`를 통해 파싱. 데이터의 변경이 감지될 때만 이벤트를 발생시키는 구조를 채택하여 프레임 드랍을 방지했습니다.

---

## 🛠 Tech Stack

### **핵심 환경 및 라이브러리**
- **Engine:** Unity 3D (XR Interaction Toolkit)
- **Language:** C# (Unity), C++ (Arduino)
- **Hardware / Device:** Arduino, VR Headset & Controllers
- **Electronic Parts:** 3× Rotary Encoder (with Push-Button), 1× Slide Potentiometer (슬라이드 가변저항)
- **Communication:** USB Serial Communication (이벤트 주도형 비동기 처리)

---

## 🔥 Challenge & Solution (Troubleshooting)

### 1. 기계적 바운싱(Bouncing) 및 다중 오입력 현상 극복
- **Problem:** 개발 초기, 아두이노의 `loop()`문 내부에서 다수의 로터리 엔코더 값을 폴링(Polling) 방식으로 읽어오도록 구현했습니다. 하지만 기계적 접점의 한계(Bouncing 현상)와 루프 딜레이로 인해 다이얼 조작 시 입력이 누락되거나 한 번의 클릭이 다중 오입력으로 처리되는 문제가 발생했습니다.
- **Solution:** 외부 인터럽트(`PinChangeInterrupt`) 기반 아키텍처로 전면 개편하고, ISR 내부에 상태 머신 테이블(`enc_table`)을 구현하여 비정상 바운싱 노이즈는 연산에서 무시(`delta = 0`)하도록 필터링했습니다. 이를 통해 물리 다이얼의 1클릭이 유니티 화면의 1스텝 변화와 1:1로 정확히 동기화되도록 보정했습니다.

---

### 2. 배율 · 사거리 연동 탄착 마커 스케일 보정
- **Problem:** 스코프 배율 줌인/줌아웃 시 탄착 예측 마커(`Tagger`)가 스코프 렌즈 화면을 벗어나거나, 타깃 사거리에 따른 원근 투영 왜곡으로 인해 실제 조준점과 히트 지점 간의 오차가 확대되는 현상이 있었습니다.
- **Solution:** `Scope.cs`의 배율 상태와 `Tagger.cs`의 탄착 거리값을 `Mathf.InverseLerp`로 정규화하고 렌즈 시야각(FOV)에 비례하는 역스케일 가중치를 동적 적용하여, 1배율부터 12배율까지 전 구간에서 마커가 일정한 상대 시각 크기와 정확한 탄착점을 유지하도록 보정했습니다.
