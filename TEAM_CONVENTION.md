# TEAM_CONVENTION.md

## 0. 문서 정보
- 목적: `헬창 키우기` 프로젝트에서 코드/에셋/운영 규칙을 통일해 유지보수 비용을 줄이고, 신규 기능 개발 속도를 높인다.
- 적용 범위: Unity 프로젝트 전체 (`Assets/`, `ProjectSettings/`, `Packages/`, 배포 문서)
- 문서 오너: 개발 리드(초기 제안: 클라이언트 메인 담당)
- 리뷰 주기: 2주(스프린트) 1회


## 1. 필수 규칙 v1.0 (Must / 즉시 적용)
아래 5개는 "권장"이 아니라 **필수(Must)** 입니다. PR에서 위반 시 사유와 만료일이 없으면 머지하지 않습니다.

### MUST-01. 네이밍 규칙
- 클래스/파일: `PascalCase`
- 메서드/프로퍼티: `PascalCase`
- private 필드: `_camelCase`
- 지역 변수/파라미터: `camelCase`
- 상수: `UPPER_SNAKE_CASE`
- 신규 코드에서 언더스코어 기반 클래스명/축약어 혼합(`AL_Item_Get`, `Atonamic_Drop` 스타일) 금지

### MUST-02. 폴더 배치 규칙
- 신규/수정 코드는 아래 기준을 우선 적용:
  - `Assets/Scripts/Core/`
  - `Assets/Scripts/Feature/Shop/`
  - `Assets/Scripts/Feature/Ads/`
  - `Assets/Scripts/Feature/UI/`
  - `Assets/Scripts/Feature/Player/`
- 레거시는 **수정 시점 점진 이동** 원칙(대규모 일괄 이동 금지)

### MUST-03. PR 체크리스트 3종
모든 PR은 아래 3개를 필수 점검:
- [ ] NullReference 가능성 점검
- [ ] 모바일 성능 영향 점검(Update/GC/리소스 로드)
- [ ] 광고/결제/저장 데이터 영향 점검

### MUST-04. 최소 검증 3종
머지 전 아래 3개를 필수 수행:
- [ ] Android 빌드 1회
- [ ] 로딩 → 게임 시작 메인 루프 진입 확인
- [ ] Shop/광고/저장 smoke test

### MUST-05. 예외 허용 시 만료일 기록
- 컨벤션 예외를 허용할 경우 PR 본문에 반드시 기록:
  - 예외 항목
  - 허용 사유
  - 만료일 (`YYYY-MM-DD`)
  - 후속 정리 작업

---
## 2. 현재 프로젝트 기준 권장 운영 방식
- `README.md`에는 "프로젝트 소개 + 빠른 시작 + 문서 링크"만 유지한다.
- 팀 규칙 본문은 별도 파일(`TEAM_CONVENTION.md`)에서 관리한다.
- 이유:
  - README가 길어지면 신규 인원이 핵심 정보 찾기 어렵다.
  - 컨벤션은 자주 바뀌므로 독립 문서가 버전 관리/리뷰에 유리하다.
  - PR에서 "기능 변경 + 컨벤션 변경"을 분리해 리뷰하기 쉽다.

## 3. 코드 구조 규칙 (현 저장소 반영)

### 3-1. 폴더 역할
- `Assets/Scripts/`: 런타임 코드
- `Assets/Editor/`: 에디터 전용 코드
- `Assets/Scripts/ScriptableObjects/`: SO 정의
- `Assets/Resources/`: 런타임 로드 리소스 (필요 최소만 유지)
- `Assets/Scenes/`: 씬 파일

### 3-2. 신규 코드 배치 규칙
현재 `Assets/Scripts`에 파일이 평평하게 배치되어 있으므로, 신규 파일부터 아래로 정리 시작:
- `Assets/Scripts/Core/` : GameManager, DataManager, 공통 시스템
- `Assets/Scripts/Feature/Shop/` : Shop, Item_Get, Sell_Item, Item_Explain 등 상점/아이템 기능
- `Assets/Scripts/Feature/Ads/` : GoogleAds, AdPanel
- `Assets/Scripts/Feature/UI/` : UI 상호작용(DragAbleUI, DroppableUI, Scroll 등)
- `Assets/Scripts/Feature/Player/` : PlayerCtrl, LevelControl, LevelUpScreen

> 레거시 파일은 한 번에 이동하지 말고, 기능 수정 시점에 같이 이동한다.

### 3-3. 네이밍 규칙
- 클래스/파일: `PascalCase` (`PlayerController`)
- 메서드/프로퍼티: `PascalCase`
- private 필드: `_camelCase`
- 지역 변수/파라미터: `camelCase`
- 상수: `UPPER_SNAKE_CASE`
- 축약어/언더스코어가 섞인 이름(`AL_Item_Get`, `Atonamic_Drop`)은 신규 코드에서 금지

## 4. Unity/게임 특화 규칙

### 4-1. MonoBehaviour 규칙
- `Awake/OnEnable/Start/Update`에서 하는 일은 주석으로 역할 명시
- `Update`는 반드시 필요할 때만 사용하고, 가능하면 이벤트/코루틴 기반으로 대체
- 외부에서 주입 가능한 참조는 `SerializeField private` 우선

### 4-2. ScriptableObject 규칙
- 데이터 정의는 `Assets/Scripts/ScriptableObjects/`에 타입별로 유지
- 런타임 변경 데이터와 원본 밸런스 데이터 구분
- 데이터 키(아이템 ID)는 중복 검사 루틴 유지

### 4-3. 리소스 규칙
- `Resources` 폴더 신규 추가는 "필요성"을 PR에 설명
- 경로 문자열 하드코딩 최소화 (상수/매핑 테이블 사용)
- 이미지/사운드 파일 이름은 의미 있는 접두사 사용 (예: `UI_`, `SFX_`, `BGM_`)

## 5. 품질 규칙

### 5-1. PR 규칙
- PR은 가급적 400줄 내외(리뷰 가능한 크기)
- 기능 PR에 문서 영향이 있으면 `README.md` 또는 `TEAM_CONVENTION.md` 동시 수정
- 체크리스트 필수:
  - [ ] NullReference 가능성 점검
  - [ ] 모바일 성능 영향 점검(Update/GC/리소스 로드)
  - [ ] 광고/결제/저장 데이터 영향 점검

### 5-2. 테스트/검증 규칙
- 최소 검증:
  - Android 빌드 1회
  - 메인 루프 진입(로딩→게임 시작) 동작 확인
  - Shop/광고/저장 관련 기능 smoke test
- 버그 수정 시 재현 절차를 PR 본문에 기록

## 6. 문서 운영 규칙
- 컨벤션 위반을 발견하면 "사람 지적"보다 "규칙 문서/자동화" 업데이트를 우선
- 분기마다 1회, 사용하지 않는 규칙 삭제
- 임시 예외는 만료일을 반드시 적는다 (예: `2026-03-31까지`)

## 7. 2주 도입 플랜 (현재 프로젝트용)

### 1주차
1. README에 문서 링크 섹션 추가
2. 이 문서 초안 리뷰(기획/클라)
3. 신규 코드 네이밍 규칙부터 우선 적용

### 2주차
1. Shop/Ads 영역부터 폴더 정리 시범 적용
2. PR 체크리스트 정착
3. 다음 스프린트에서 DataManager 리팩터링 시 규칙 준수 여부 점검

## 8. 운영 템플릿
- PR 템플릿: `.github/PULL_REQUEST_TEMPLATE.md`
- Shop/Ads 정리 체크리스트: `docs/checklists/SHOP_ADS_MIGRATION_CHECKLIST.md`