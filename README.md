# 메이플스토리 오목 
<img src="https://github.com/21jae/PlayGomoku/assets/90013449/b8bc10fa-2b4c-48bf-a09f-5a150a99d1b8" width="50%" height="auto">

## 개요
제목 | MapleStory Omok 
------------ | ------------- 
장르 | 캐쥬얼 보드 게임
개발 기간 | 1주
사용 툴 | Unity, Photon Networking, Photoshop
개발자 | 21jae

## 게임 소개
![omok](https://github.com/21jae/PlayGomoku/assets/90013449/3f3f7091-1833-41d7-8f26-4b6cb13b8f6a) </br>
오래된 브라운 색상의 오목 게임이 아닌, 누구나 즐길 수 있는 멀티 오목 게임을 개발하였습니다.
</br>


## 주요 기능
* 멀티플레이 게임플레이 : 포톤 네트워킹을 사용하여 실시간으로 다른 플레이어와 오목이 가능합니다.
* 캐주얼한 디자인: 사용자 친화적인 인터페이스와 사운드로 누구나 쉽게 접근하고 즐길수있습니다.
* 그래픽스 최적화:  UI 최적화에 신경 썼습니다.
</br>

## 주요 클래스 설명
* [GameManager](https://github.com/21jae/PlayGomoku/blob/main/Assets/Scripts/Managers/GameManager.cs)
  * 싱글톤 패턴
  * 게임의 상태를 관리합니다. (준비, 진행중, 게임오버)
  * 바둑판 생성 및 초기화
  * 멀티플레이 설정
  * 플레이어 턴 준비 상태 및 관리
  * 게임 승리 조건 체크
* [UIManager](https://github.com/21jae/PlayGomoku/blob/main/Assets/Scripts/Managers/UIManager.cs)
  * 싱글톤 패턴
  * 게임 UI 상태 업데이트 (타이머, 턴 가이드)
  * 버튼 핸들링 (준비, 스타트, 나가기, 방 생성 등)
  * 게임 결과 표시
* [LobbyManager](https://github.com/21jae/PlayGomoku/blob/main/Assets/Scripts/Managers/LobbyManager.cs)
  * 로비 입장 및 방 생성 기능
  * 방 입장 및 플레이어 번호 설정
  * 방 업데이트
* [ChatManager](https://github.com/21jae/PlayGomoku/blob/main/Assets/Scripts/Managers/ChatManager.cs)
  *  실시간 채팅 시스템 구현
</br>


## 배운점
* UI 리소스 관리하는 방법에 대하여 배웠습니다. (네이밍 규칙, 최적화 기법 등)
* 포톤 네트워크에 대해 공부하였습니다.
* 오목 로직 구현하는게 생각보다 많이 어려워서, 효율적인 게임 로직을 위해 (C#, C++)들을 이용하여 알고리즘과 데이터 구조에 대해
더 많은 지식이 필요하다는 것을 깨닫게 되었습니다.
</br>


## 기술서
[PDF자료](https://file.notion.so/f/f/60d85208-d2f5-4b65-bcee-71940fa52b65/964e31db-849a-4ce2-b91f-c9abf52e0c08/%EB%A9%94%EC%9D%B4%ED%94%8C%EC%8A%A4%ED%86%A0%EB%A6%AC_%EC%98%A4%EB%AA%A9.pdf?id=a982583b-3aa4-453f-87f3-435c0435cdf7&table=block&spaceId=60d85208-d2f5-4b65-bcee-71940fa52b65&expirationTimestamp=1702540800000&signature=ujeFI8-IGcM-rQ9kU4YoRRRTslC3iLvBXclEs8X3lyw&downloadName=%EB%A9%94%EC%9D%B4%ED%94%8C%EC%8A%A4%ED%86%A0%EB%A6%AC+%EC%98%A4%EB%AA%A9.pdf)


## 플레이 영상
[플레이](https://youtu.be/YOqCOfoNdhY)

