export enum GameStateEngine {
  InitialGameState = 'InitialGameState',
  ArrowEvent = 'ArrowEvent',
  ArrowDelayedEvent = 'ArrowDelayedEvent',
  PlayerEvent = 'PlayerEvent',
  TurnComplete = 'TurnComplete',
  GetNextPlayer = 'GetNextPlayer',
  PlayerSpecialMoveTransportMoveEvent = 'PlayerSpecialMoveTransportMoveEvent',
  WinnerDeclared = 'WinnerDeclared'
}
