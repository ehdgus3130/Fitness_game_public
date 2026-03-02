# DataManager 분리 계획 (현행 반영)

## 목표
`DataManager`를 데이터 파사드로 유지하고, 리소스/저장소/런타임 UI 결합을 포트 기반으로 분리해 유지보수성과 테스트 가능성을 확보한다.

## 현재 상태 (완료)
1. 모델/저장소/서비스 분리 완료
- `Core/Data/Models`: `ItemData`, `PlayerData`, `PlayerSaveData`
- `Core/Data/Persistence`: `JsonFileStore`, `ItemRepository`, `PlayerRepository`
- `Core/Data/Services`: `ItemDataService`, `PlayerDataService`

2. 런타임 결합 분리 완료
- `PlayerRuntimeDataSync`는 `PlayerRuntimeUiBridge` 경유로 UI 접근
- `PlayerChangeOrchestrator`는 런타임 포트(`IPlayerChangeRuntimePort`) 경유

3. `DataManager` API 캡슐화 완료
- 직접 필드 접근 제거(`Player_Info/AllPlayer_Info/Items/CurPlayer` 등)
- 읽기 전용 컬렉션/조회 메서드 중심으로 사용
- 저장 API 통합: `SaveCurrentPlayer`, `SaveAllPlayers`, `SavePlayerItem(int)`

4. 의존 역전 1차 완료
- 리소스 접근 포트: `IDataResourceProvider`
- 저장소 포트: `IItemStore`, `IPlayerStore`
- `DataManager` 초기화 단계 분리 + 초기화 경고 로깅 단일 지점화

## 구조 요약
- `DataManager`: 파사드/초기화 오케스트레이션
- `DataResourceProvider`: Unity `Resources` 접근 어댑터
- `ItemStore/PlayerStore`: 영속성 포트
- `RuntimeUiBridge/ChangeRuntimePort`: 씬 런타임 연결 지점

## 호환성 정책
- 저장 파일명/JSON 구조 유지
- 플레이어 레거시 포맷 로드 폴백 유지
- 기존 데이터(플레이어/아이템) 마이그레이션 없이 읽기 가능 상태 유지

## 운영 규칙
1. 신규 코드에서 `DataManager` 내부 컬렉션 직접 수정 금지
2. 저장은 통합 API만 사용
- `SaveCurrentPlayer()`
- `SaveAllPlayers()`
- `SavePlayerItem(slot)`
3. 리소스 접근은 `DataManager` 내부에서 포트 경유로만 수행

## 남은 개선 과제 (선택)
1. `DataManager` 내부 초기화 경로에 대한 PlayMode 자동화 테스트 추가
2. `PlayerRuntimeUiBridge`/`PlayerChangeOrchestrator` 포트 테스트 더블 구축
3. 초기화 `Result` 타입을 공통 유틸로 승격

## 참고
- 실행 점검은 `docs/checklists/DATAMANAGER_SMOKE_TEST_CHECKLIST.md` 기준으로 수행한다.
