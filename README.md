# Unity Util 스크립트 모음

재사용하기 좋은 Unity 유틸리티 코드 모음입니다.

---

## 📋 목차

- [EventBus](#eventbus)
- [Logger](#logger)
- [ObjectPoolManager](#objectpoolmanager)
- [Singleton](#singleton)
- [TMPDoTweenExtensions](#tmpdotweenextensions)
- [UIManager](#uimanager)
- [ShowIfAttribute](#showifattribute)

---

## EventBus

**타입 기반 및 문자열 기반 이벤트 버스 시스템**

### 개요
컴포넌트 간 느슨한 결합을 위한 이벤트 시스템입니다. 타입 기반과 문자열 기반 두 가지 방식을 지원합니다.

### 주요 기능
- 타입 기반 이벤트 구독/발행
- 문자열 기반 이벤트 구독/발행
- 이벤트 테이블 초기화

### 사용법

#### 타입 기반 이벤트

```csharp
// 이벤트 클래스 정의
public class PlayerDeathEvent { }
public class ScoreChangedEvent { public int Score; }

// 구독
EventBus.Subscribe<PlayerDeathEvent>(OnPlayerDeath);
EventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);

// 발행
EventBus.Publish(new PlayerDeathEvent());
EventBus.Publish(new ScoreChangedEvent { Score = 100 });

// 구독 해제
EventBus.Unsubscribe<PlayerDeathEvent>(OnPlayerDeath);
```

#### 문자열 기반 이벤트

```csharp
// 구독
EventBus.Subscribe("OnPlayerJump", (param) => {
    Debug.Log("Player Jumped!");
});

// 발행
EventBus.Publish("OnPlayerJump");
EventBus.Publish("OnScoreUpdate", scoreValue);

// 구독 해제
Action<object> callback = (param) => { /* ... */ };
EventBus.Subscribe("EventName", callback);
EventBus.Unsubscribe("EventName", callback);
```

### API 명세

#### 타입 기반 메서드

| 메서드 | 설명 | 매개변수 | 반환값 |
|--------|------|----------|--------|
| `Subscribe<T>(Action<T> callback)` | 타입 T 이벤트 구독 | `callback`: 이벤트 발생 시 호출될 콜백 | `void` |
| `Unsubscribe<T>(Action<T> callback)` | 타입 T 이벤트 구독 해제 | `callback`: 해제할 콜백 | `void` |
| `Publish<T>(T evt)` | 타입 T 이벤트 발행 | `evt`: 발행할 이벤트 객체 | `void` |

#### 문자열 기반 메서드

| 메서드 | 설명 | 매개변수 | 반환값 |
|--------|------|----------|--------|
| `Subscribe(string eventName, Action<object> callback)` | 문자열 이벤트 구독 | `eventName`: 이벤트 이름<br>`callback`: 콜백 함수 | `void` |
| `Unsubscribe(string eventName, Action<object> callback)` | 문자열 이벤트 구독 해제 | `eventName`: 이벤트 이름<br>`callback`: 해제할 콜백 | `void` |
| `Publish(string eventName, object param = null)` | 문자열 이벤트 발행 | `eventName`: 이벤트 이름<br>`param`: 전달할 파라미터 | `void` |

#### 유틸리티 메서드

| 메서드 | 설명 | 매개변수 | 반환값 |
|--------|------|----------|--------|
| `Clear()` | 모든 이벤트 테이블 초기화 | 없음 | `void` |

### 주의사항
- 타입 기반 이벤트는 컴파일 타임 타입 안정성을 제공합니다.
- 문자열 기반 이벤트는 런타임 오류 가능성이 있으므로 주의가 필요합니다.
- 씬 전환 시 `Clear()`를 호출하여 메모리 누수를 방지하세요.

---

## Logger

**Unity 에디터 전용 로깅 유틸리티**

### 개요
Unity 에디터에서만 동작하는 색상이 적용된 로깅 시스템입니다.

### 주요 기능
- 에디터 전용 로그 (빌드에서 제외)
- 에러 로그 (항상 동작)
- 색상 지원 (노란색/빨간색)

### 사용법

```csharp
// 에디터에서만 동작하는 로그 (빌드에서는 제거됨)
Logger.Log("디버그 메시지");

// 항상 동작하는 에러 로그
Logger.LogError("에러 메시지");
```

### API 명세

| 메서드 | 설명 | 매개변수 | 반환값 | 빌드 포함 여부 |
|--------|------|----------|--------|----------------|
| `Log(string message)` | 디버그 로그 출력 (노란색) | `message`: 로그 메시지 | `void` | ❌ 에디터 전용 |
| `LogError(string message)` | 에러 로그 출력 (빨간색) | `message`: 에러 메시지 | `void` | ✅ 항상 포함 |

### 주의사항
- `Log()` 메서드는 `[Conditional("UNITY_EDITOR")]` 속성으로 인해 빌드에서는 완전히 제거됩니다.
- `LogError()`는 빌드에서도 동작합니다.

---

## ObjectPoolManager

**게임 오브젝트 풀링 관리 시스템**

### 개요
자주 생성/파괴되는 게임 오브젝트를 재사용하여 성능을 최적화하는 오브젝트 풀링 시스템입니다.

### 주요 기능
- 풀 생성 및 관리
- 오브젝트 가져오기/반환
- 씬 전환 시 유지 (DontDestroyOnLoad)
- IPoolObject 인터페이스 지원

### 사용법

```csharp
// 초기화 (선택사항)
ObjectPoolManager.Init(poolParentTransform);

// 풀 생성
GameObject bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
ObjectPoolManager.CreatePool("Bullet", bulletPrefab, 10);

// 오브젝트 가져오기
GameObject bullet = ObjectPoolManager.Get("Bullet");
if (bullet != null)
{
    bullet.transform.position = spawnPosition;
}

// 오브젝트 반환
ObjectPoolManager.Return(bullet, "Bullet");
```

### IPoolObject 인터페이스

풀 오브젝트에 이름을 초기화하기 위한 인터페이스입니다.

```csharp
public class Bullet : MonoBehaviour, IPoolObject
{
    public void InitName(string name)
    {
        // 풀 이름 초기화 로직
    }
}
```

### API 명세

| 메서드 | 설명 | 매개변수 | 반환값 |
|--------|------|----------|--------|
| `Init(Transform poolParent = null)` | 풀 매니저 초기화 | `poolParent`: 풀 오브젝트의 부모 Transform | `void` |
| `CreatePool(string name, GameObject prefab, int initialCount)` | 풀 생성 (전역 부모 사용) | `name`: 풀 이름<br>`prefab`: 프리팹<br>`initialCount`: 초기 생성 개수 | `void` |
| `CreatePool(string name, GameObject prefab, int initialCount, Transform parent)` | 풀 생성 (지정 부모 사용) | `name`: 풀 이름<br>`prefab`: 프리팹<br>`initialCount`: 초기 생성 개수<br>`parent`: 부모 Transform | `void` |
| `Get(string name)` | 풀에서 오브젝트 가져오기 | `name`: 풀 이름 | `GameObject` (없으면 null) |
| `Return(GameObject obj, string name)` | 오브젝트를 풀에 반환 | `obj`: 반환할 오브젝트<br>`name`: 풀 이름 | `void` |

### ObjectPool 클래스

내부적으로 사용되는 풀 클래스입니다.

| 메서드 | 설명 | 매개변수 | 반환값 |
|--------|------|----------|--------|
| `Get()` | 풀에서 오브젝트 가져오기 (없으면 새로 생성) | 없음 | `GameObject` |
| `Return(GameObject obj)` | 오브젝트를 풀에 반환 | `obj`: 반환할 오브젝트 | `void` |

### 주의사항
- 풀에 반환된 오브젝트는 자동으로 비활성화됩니다.
- 풀이 없을 때 `Return()`을 호출하면 오브젝트가 파괴됩니다.
- 모든 풀 오브젝트는 `DontDestroyOnLoad`로 설정되어 씬 전환 시에도 유지됩니다.

---

## Singleton

**제네릭 싱글톤 패턴 구현**

### 개요
MonoBehaviour를 상속받는 클래스를 싱글톤으로 만들어주는 제네릭 베이스 클래스입니다.

### 주요 기능
- 자동 인스턴스 생성
- 씬 전환 시 유지 (DontDestroyOnLoad)
- 중복 인스턴스 방지

### 사용법

```csharp
public class GameManager : Singleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        // 초기화 로직
    }

    public void DoSomething()
    {
        // ...
    }
}

// 사용
GameManager.Instance.DoSomething();
```

### API 명세

| 속성/메서드 | 설명 | 반환값 |
|------------|------|--------|
| `Instance` | 싱글톤 인스턴스 접근 (없으면 자동 생성) | `T` |
| `Awake()` | 가상 메서드, 오버라이드 가능 | `void` |

### 동작 방식
1. `Instance` 접근 시 씬에 해당 타입의 오브젝트가 없으면 자동으로 생성합니다.
2. `Awake()`에서 중복 인스턴스를 감지하고 파괴합니다.
3. 싱글톤 오브젝트는 `DontDestroyOnLoad`로 설정되어 씬 전환 시에도 유지됩니다.

### 주의사항
- `Awake()`를 오버라이드할 때 반드시 `base.Awake()`를 호출해야 합니다.
- 씬에 이미 오브젝트가 있으면 해당 오브젝트를 인스턴스로 사용합니다.

---

## TMPDoTweenExtensions

**TextMeshPro와 DOTween 연동 확장 메서드**

### 개요
TextMeshPro 텍스트에 DOTween 애니메이션을 쉽게 적용할 수 있는 확장 메서드 모음입니다.

### 의존성
- **DOTween** (필수)
- **TextMeshPro** (필수)

### 주요 기능
- 텍스트 숫자 카운팅 애니메이션
- 타이핑 효과 애니메이션
- Float 값 트윈 유틸리티

### 사용법

```csharp
using TMPro;

TMP_Text scoreText;

// 숫자 카운팅 애니메이션
scoreText.DoText("1000", 2f);

// 타이핑 효과
scoreText.DoTextClean("Hello World!", 1f);

// Float 트윈
DoTweenExtensions.TweenFloat(0f, 100f, 2f, 
    onUpdate: (value) => { 
        scoreText.text = value.ToString("F0"); 
    },
    onComplete: () => { 
        Debug.Log("완료!"); 
    },
    easeType: Ease.OutQuad
);
```

### API 명세

| 메서드 | 설명 | 매개변수 | 반환값 |
|--------|------|----------|--------|
| `DoText(this TMP_Text text, string targetText, float duration)` | 텍스트를 숫자 카운팅처럼 변경 | `text`: 대상 텍스트<br>`targetText`: 목표 텍스트<br>`duration`: 애니메이션 시간 | `Tweener` |
| `DoTextClean(this TMP_Text text, string targetText, float duration)` | 타이핑 효과로 텍스트 표시 | `text`: 대상 텍스트<br>`targetText`: 목표 텍스트<br>`duration`: 애니메이션 시간 | `Tweener` |
| `TweenFloat(float start, float end, float duration, Action<float> onUpdate = null, Action onComplete = null, Ease easeType = Ease.Linear)` | Float 값 트윈 | `start`: 시작 값<br>`end`: 종료 값<br>`duration`: 시간<br>`onUpdate`: 업데이트 콜백<br>`onComplete`: 완료 콜백<br>`easeType`: 이징 타입 | `Tween` |

### 주의사항
- `DoText()`는 숫자 문자열에 최적화되어 있습니다.
- `DoTextClean()`은 문자열을 한 글자씩 표시합니다.
- 모든 애니메이션은 기본적으로 `Ease.Linear`를 사용합니다.

---

## UIManager

**UI 관리 싱글톤 시스템**

### 개요
UI 프리팹을 자동으로 로드하고 관리하는 싱글톤 매니저입니다. Resources 폴더에서 UI를 로드하며 스택 기반으로 UI 순서를 관리합니다.

### 주요 기능
- UI 자동 로드 및 관리
- UI 스택 기반 순서 관리
- Canvas sortingOrder 자동 설정
- 씬 전환 시 UI 스택 초기화

### 사용법

```csharp
// UI 클래스 정의
public class MainMenuUI : MonoBehaviour { }
public class SettingsUI : MonoBehaviour { }

// UI 열기
UIManager.Instance.OpenUI<MainMenuUI>();

// UI 닫기
UIManager.Instance.CloseUI<MainMenuUI>();

// UI 전환
UIManager.Instance.SwitchUI<MainMenuUI, SettingsUI>();
```

### 필수 설정
1. **Resources 폴더 구조**: `Resources/UI/{UI클래스명}.prefab`
   - 예: `Resources/UI/MainMenuUI.prefab`
2. **인스펙터 설정**: UIManager의 `parentObj` 필드에 UI 부모 Transform 지정

### API 명세

| 메서드 | 설명 | 매개변수 | 반환값 |
|--------|------|----------|--------|
| `OpenUI<T>()` | 제네릭 타입으로 UI 열기 | 없음 | `void` |
| `OpenUI(Type type)` | 타입으로 UI 열기 | `type`: UI 컴포넌트 타입 | `void` |
| `CloseUI<T>()` | 제네릭 타입으로 UI 닫기 | 없음 | `void` |
| `CloseUI(Type type)` | 타입으로 UI 닫기 | `type`: UI 컴포넌트 타입 | `void` |
| `SwitchUI<TFrom, TTo>()` | UI 전환 (닫고 열기) | 없음 | `void` |
| `SwitchUI(Type from, Type to)` | UI 전환 (닫고 열기) | `from`: 닫을 UI 타입<br>`to`: 열 UI 타입 | `void` |

### 동작 방식
1. UI를 처음 열 때 `Resources/UI/{타입명}.prefab`에서 로드합니다.
2. 로드된 UI는 딕셔너리에 캐싱되어 재사용됩니다.
3. UI를 열 때마다 스택에 추가되고, Canvas의 `sortingOrder`가 자동으로 증가합니다.
4. 씬 전환 시 UI 스택이 자동으로 초기화됩니다.

### 주의사항
- **반드시 인스펙터에서 `parentObj`를 지정해야 합니다.**
- UI 프리팹은 `Resources/UI/` 폴더에 있어야 합니다.
- UI 프리팹 이름은 클래스 이름과 정확히 일치해야 합니다.
- UI는 처음 로드 시 비활성화 상태로 생성됩니다.

---

## ShowIfAttribute

**조건부 인스펙터 필드 표시 속성**

### 개요
Unity 인스펙터에서 특정 조건에 따라 필드를 표시/숨김 처리하는 커스텀 속성입니다.

### 주요 기능
- Boolean 필드 값에 따른 조건부 표시
- 여러 조건 동시 지원 (AllowMultiple)
- 리스트/배열 필드 지원

### 사용법

```csharp
public class Example : MonoBehaviour
{
    public bool showAdvancedSettings;
    
    [ShowIf("showAdvancedSettings")]
    public float advancedValue;
    
    [ShowIf("showAdvancedSettings")]
    public string advancedText;
    
    public bool enableFeature;
    
    [ShowIf("enableFeature")]
    public int featureLevel;
}
```

### API 명세

#### ShowIfAttribute

| 속성 | 설명 | 타입 |
|------|------|------|
| `ConditionFieldName` | 조건이 되는 필드 이름 | `string` |

#### 생성자

| 생성자 | 설명 | 매개변수 |
|--------|------|----------|
| `ShowIfAttribute(string conditionFieldName)` | 속성 생성 | `conditionFieldName`: 조건 필드 이름 |

### 동작 방식
1. `ShowIfAttribute`가 적용된 필드는 지정된 Boolean 필드가 `true`일 때만 표시됩니다.
2. `ShowIfEditor`가 모든 MonoBehaviour에 적용되어 인스펙터를 커스터마이징합니다.
3. 여러 `ShowIfAttribute`를 동시에 사용할 수 있으며, 모든 조건이 `true`여야 표시됩니다.

### 주의사항
- 조건 필드는 반드시 `bool` 타입이어야 합니다.
- 조건 필드가 없거나 타입이 맞지 않으면 필드가 숨겨집니다.
- 에디터 전용 기능이므로 빌드에는 영향을 주지 않습니다.

---

## 📦 의존성

### 필수 패키지
- **Unity Engine** (2020.3 이상 권장)
- **TextMeshPro** (TMPDoTweenExtensions 사용 시)
- **DOTween** (TMPDoTweenExtensions 사용 시)

### 선택 패키지
- 없음

---

## 📝 라이선스

이 코드는 자유롭게 사용 및 수정 가능합니다.

---

## 🤝 기여

버그 리포트나 개선 제안은 언제든 환영합니다!
