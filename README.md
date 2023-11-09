# 저어러어언 어

## 특징

- 인터프리터 언어로 한 줄이 한 명령을 뜻한다.
- 메모리에는 8192개의 4바이트 변수가 있다. 각 변수는 0~8191번의 이름을 가진다. 맨 처음 시작될 때 이 모든 변수에는 0이 저장된다.
- 각 변수는 기본적으로 int 형태로 저장된다. 오버플로우가 발생한 경우 예외를 발생시킨다.
- 포인터 1개가 변수의 번호를 가르킨다. 이 포인터는 번호가 커서 일반 명령으로 접근하기에는 너무 코드가 길어지는 경우에 사용할 수 있다. 만약 포인터가 메모리 주소를 벗어나게 된다면 예외를 발생시킨다.

## 기본적 구조

> `저어어어러어어어어언....` => 4번 변수에 40을 더한다.
>
> - '저'로 한 문장을 시작
> - '러'앞의 '어'의 개수는 명령어 코드를 뜻한다.(op)
> - '러'뒤의 '어'의 개수는 명령의 영향을 받을 변수 번호를 뜻한다.(rs)
> - '언'뒤의  마침표(.)의 개수는 넣어줄 값을 뜻한다.(imm)

## 명령어의 타입

- I : op, rs, imm를 필요로 하는 명령어
- NI : op, rs만을 필요로 하는 명령어, 뒤의 점은 무시한다.
- USERIN : op, rs를 필요로 하고, 콘솔에서 사용자의 입력을 받는다.

## 명령어 표

### 기초 명령

| 명령어 코드| 이름     |   설명 | 타입 |
| ----------|--------| --------------------------------         | ----- |
| 0        | mul3x1 | rs = rs * 3 + 1를 imm번 반복한다. | I |
| 1        | div2   | rs = r2 / 2 를 imm번 반복한다.(소수점 버림)  | I |
| 2         | in     | 사용자로부터 값을 입력받는다.  | USERIN |
| 3         | inchr  | 사용자로부터 문자 1개를 입력받는다.  | USERIN |
| 4         | out    | rs에 들어 있는 값을 수 그대로 출력한다.  | NI |
| 5         | outchr | rs에 들어 있는 값을 UTF-8 문자로 출력한다.  |  NI |

### 산술 연산

| 명령어 코드| 이름     |   설명 |  타입 |
| ----------|--------| --------------------------------         | ----- |
| 6         | addi   | rs번 변수 값을 imm[3x+1] 값만큼 증가시킨다.         |  I |
| 7         | addi1  | rs번 변수 값을 imm만큼 증가시킨다.          |  I |
| 8         | addv   | rs번 변수 값을 imm번 변수의 값만큼 증가시킨다.  |  I |
| 9         | subi   | rs번 변수 값을 imm[3x+1] 값만큼 감소시킨다.          |  I |
| 10         | subi1  | rs번 변수 값을 imm만큼 감소시킨다.  |  I |
| 11         | subv   | rs번 변수 값을 imm번 변수의 값만큼 감소시킨다.  |  I |

### 대입

| 명령어 코드| 이름     |   설명 |  타입 |
| ----------|--------| --------------------------------         | ----- |
| 12        | ahhee  | rs번 변수에 44,032('가'의 UTF-8 값)를 대입한다.  |   NI |
| 13        | putv  | rs번 변수에 imm번 변수의 값을 대입한다.  |   I |
| 14        | put0  | rs번 변수에 0을 대입한다.  |   NI |

### 분기

| 명령어 코드| 이름     |   설명 |  타입 |
| ----------|--------| --------------------------------         | ----- |
| 15        | jzero  | rs에 들어 있는 값이 0이면 가장 가까운 imm번의 레이블으로 점프한다.  |   I |
| 16        | jv     | rs에 들어 있는 값이 imm번의 변수의 값과 같으면 가장 가까운 imm번의 레이블으로 점프한다.  |   I |

## 점프 레이블

- '저런'을 사용하면 점프 레이블을 만들 수 있다. 점프 레이블의 번호는 '런' 뒤의 '.'의 개수에 의해 결정된다.
- '저런'은 0번, '저런.'은 1번, '저런..'은 2번 레이블이 된다.
- 분기 명령에서 해당 번호를 가진 레이블이 없는 경우 예외를 발생시킨다.

## imm과 imm[3x+1]

- imm의 값은 점의 개수와 같다.
- imm[3x+1]은 점의 개수에 따라 0, 1, 4, 13, 40, 121, 364, 1093, 3280, 9841, 29524, 88573, 265720, 797161, 2391484, 7174453, 21523360 64570081, 193710244, 581130733, 1743392200의 값을 가진다. 점은 20개까지 올 수 있으며 그 이상의 점은 무시된다.

## 주석

- '저'로 시작하지 않은 모든 문장은 주석 취급받는다.
- // 를 사용해서 주석을 표시하는 것을 추천한다.

## 예제

### 1. "Hello, World!" 출력

```
// H 출력 72
저러언.....
저어러언.
저어어어어어어러언...
저어어어어어어어어어어러언.
저어어어어어러언
// e 출력 101
저어어어어어어러언...
저어어어어어어러언...
저어어어어어어어러언...
저어어어어어러언
// l 출력(2개) 108
저어어어어어어어러언.......
저어어어어어러언
저어어어어어러언
// o 출력 111
저어어어어어어어러언...
저어어어어어러언
// , 출력 44
저러어언....
저어어어어어어어러어언....
저어어어어어러어언
// 공백 출력 32
저어어어어어어어어어러어언...
저어어어어어어어러어언.
저어어어어어러어언
// W 출력 87
저러어언.
저어어어어어어어어어어러어언..........
저어어어어어러어언
// o 출력 111
저어어어어어러언
// r 출력 114
저어어어어어어어러언...
저어어어어어러언
// l 출력 108
저어어어어어어어어어어러언......
저어어어어어러언
// d 출력 100
저어어어어어어어어어어러언........
저어어어어어러언
// ! 출력 33
저어러어언..
저어어어어어어어러어언............
저어어어어어러어언
```

## History

- 2023-11-09 저어러어언... 언어 아이디어 고안
