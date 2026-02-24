# Async File Downloader

## 1. 목표
async/await 동작 원리, Delegate/Event, Value Type vs Reference Type을 실제 다운로드 앱을 만들며 체득.

## 2. 요건
### 2-1. 화면 구성
- URL 입력 TextBox 3개 (아래 테스트용 URL을 기본값으로 세팅)

- 각 항목마다 ProgressBar + 퍼센트 텍스트 + 파일 크기 표시

- "전체 다운로드" 버튼, "취소" 버튼

- 하단 StatusBar에 상태 메시지 표시

- 테스트용 URL
	- > [*] https://speed.cloudflare.com/100mb.test (작동안함 : 404)
	- > [1] https://ash-speed.hetzner.com/100MB.bin
	- > [2] http://xcal1.vodafone.co.uk/100MB.zip (대체 링크 구함)
	- > [3] https://proof.ovh.net/files/100Mb.dat

### 2-2. 기능 요건

1. "전체 다운로드" 클릭 시 Microsoft.Win32.SaveFileDialog로 저장 폴더/파일명 선택

2. HttpClient + Stream으로 청크 단위 다운로드하며 실시간 진행률 갱신

3. Task.WhenAll로 3개 동시 다운로드

4. "취소" 버튼 클릭 시 CancellationTokenSource로 다운로드 중단

5. 다운로드 중에도 UI가 멈추지 않아야 함 (다른 TextBox에 타이핑이 가능해야 함)

6. 다운로드 완료 시 StatusBar에 "다운로드 완료 - 총 소요시간: X초" 표시

7. 네트워크 오류 발생 시 해당 항목에 "실패: {예외 메시지}" 표시, 나머지는 계속 진행

### 2-3. 진행률 콜백 3가지 방식 구현

**방식 A**: `event Action<int> ProgressChanged` 선언 후 구독

**방식 B**: `IProgress<int>` 인터페이스 사용

**방식 C**: `Func<int, Task>` 람다로 전달

세 방식의 차이점과 각각 어떤 상황에서 적합한지 문서에 정리합니다.

## 3. 이론 정리 (실습과 연결)
아래 항목을 실습 코드와 결과를 근거로 정리합니다.

- async/await 동작 원리  
다운로드 버튼 클릭 → await HttpClient.GetAsync() 호출 시점에 실행 흐름이 어떻게 되는지 단계별 서술  
```
[UI Thread] Button Click
    → OnDownloadAll()         ← UI Thread에서 호출
        → SaveFile(null)      ← UI Thread에서 호출
            → await Task.WhenAll(downloadTasks)
                              ↑ 여기서 SynchronizationContext 캡쳐
            → 이후 UI Thread에서 프로퍼티 업데이트하기 때문에 Blocking 없음
```
- Value Type vs Reference Type  
다운로드 진행률을 int percent (Value Type)으로 전달할 때와 DownloadProgress 클래스 (Reference Type)로 전달할 때 차이를 코드로 비교  
=> 본 프로젝트에서는 진행률 콜백을 구현하기 위해 해당 방법을 사용하지 않고 MVVM 패턴을 통해 Binding으로 구현

- 청크 읽기 루프에서 byte[] buffer가 힙에 할당되는 이유  
=> C#에서는 `new Array[]`를 참조형식으로 힙에 저장함.

- Delegate / Event / Lambda  
=> 본 프로젝트에서는 진행률 콜백을 구현하기 위해 해당 방법을 사용하지 않고, Binding Property를 통하여 즉시 반영한다.

- CancellationToken 동작 원리  
취소 버튼 클릭 → `CancellationTokenSource.Cancel()` → 다운로드 루프 중단까지의 과정 추적  
=> 1. 취소버튼 클릭 -> 2. Command로 바인딩 된 `CancelCommand.Invoke` -> 3. `CancelCommand`에 등록된 콜백함수(`OnCancel()`) 실행 -> 4. 토큰값을 캔슬로 변경, 다운로드 상태 초기화(클린업) -> 5. 이후 `contentStream.ReadAsync` 루프에서 cts 취소 토큰과 함께 리퀘스트시 루프 탈출


### 확인 포인트
- [ ] 일부러 Task.Run 안에서 ProgressBar.Value를 직접 건드려보고 예외 확인 → 왜 터지는지 정리

- [ ] await를 빼고 .Result로 바꿔보고 UI가 멈추는 것 확인 → 왜 멈추는지 데드락 원리와 함께 설명

### TODO
- [ ] Linting (Life 4 참조)
- [ ] Task.Run()
- [ ] RelayCommand 수정
- [ ] HttpClient 따로 파일 분리 (static 함수로 만들었으면 좋겠다. Download 함수)
- [ ] View Items로 바꿔서 중복 제거
- [ ] ViewModel Base [] <- ?
- [ ] 에러 어디서 터지는지 try-catch 확인 <- Debug - Windows - Immediate ...
- [ ] CanExecute null check
- [ ] Why CommandManager?
- [ ] RaiseCanExecuteChanged
- [ ] 