import { GameEngine } from '../GameEngine';
import { Player } from '../../domain/Player';
import { GameStateEngine } from '../GameStateEngine';

describe('GameEngine - GetNextPlayer', () => {
  let gameEngine: GameEngine;
  let player1: Player;
  let player2: Player;
  let player3: Player;

  const PLAYER1_NAME = 'Player1';
  const PLAYER2_NAME = 'Player2';
  const PLAYER3_NAME = 'Player3';

  beforeEach(() => {
    player1 = new Player({
      currentMovesRemaining: 0,
      currentTileNumber: 0,
      name: PLAYER1_NAME,
      turnOrder: 1
    });

    player2 = new Player({
      currentMovesRemaining: 0,
      currentTileNumber: 0,
      name: PLAYER2_NAME,
      turnOrder: 2
    });

    player3 = new Player({
      currentMovesRemaining: 0,
      currentTileNumber: 0,
      name: PLAYER3_NAME,
      turnOrder: 3
    });

    gameEngine = new GameEngine();
    gameEngine.players = [player1, player2, player3];
  });

  test('GetNextPlayer returns player1 when starting game', () => {
    // Method under test
    gameEngine.getNextPlayer();

    expect(gameEngine.currentPlayer).not.toBeNull();
    expect(gameEngine.currentPlayer?.name).toBe(PLAYER1_NAME);
    expect(gameEngine.currentState).toBe(GameStateEngine.TurnComplete);
  });

  test('GetNextPlayer returns second player when player1 is previous player', () => {
    gameEngine.currentPlayer = player1;

    // Method under test
    gameEngine.getNextPlayer();

    expect(gameEngine.currentPlayer).not.toBeNull();
    expect(gameEngine.currentPlayer).toEqual(player2);
    expect(gameEngine.currentState).toBe(GameStateEngine.TurnComplete);
  });

  test('GetNextPlayer returns player1 when everyone has had a turn', () => {
    gameEngine.currentPlayer = player3;

    // Method under test
    gameEngine.getNextPlayer();

    expect(gameEngine.currentPlayer).not.toBeNull();
    expect(gameEngine.currentPlayer).toEqual(player1);
    expect(gameEngine.currentPlayer?.name).toBe(PLAYER1_NAME);
    expect(gameEngine.currentState).toBe(GameStateEngine.TurnComplete);
  });

  test('GetNextPlayer returns last player in list when her turn', () => {
    gameEngine.currentPlayer = player2;

    // Method under test
    gameEngine.getNextPlayer();

    expect(gameEngine.currentPlayer).not.toBeNull();
    expect(gameEngine.currentPlayer).toEqual(player3);
    expect(gameEngine.currentPlayer?.name).toBe(PLAYER3_NAME);
    expect(gameEngine.currentState).toBe(GameStateEngine.TurnComplete);
  });
});

describe('GameEngine - IsSpecialMove', () => {
  let engine: GameEngine;

  beforeEach(() => {
    engine = new GameEngine();
  });

  test('IsSpecialMove returns true when tile is start of special move', () => {
    const result = engine.isSpecialMove(9);
    expect(result).toBe(true);
  });

  test('IsSpecialMove returns false when tile is not start of special move', () => {
    const result = engine.isSpecialMove(10);
    expect(result).toBe(false);
  });

  test('IsSpecialMove correctly identifies all ladder starting positions', () => {
    const ladderStarts = [1, 4, 9, 21, 28, 51, 71, 80];

    ladderStarts.forEach(tile => {
      expect(engine.isSpecialMove(tile)).toBe(true);
    });
  });

  test('IsSpecialMove correctly identifies all chute starting positions', () => {
    const chuteStarts = [16, 47, 49, 56, 64, 87, 93, 95, 98];

    chuteStarts.forEach(tile => {
      expect(engine.isSpecialMove(tile)).toBe(true);
    });
  });
});

describe('GameEngine - CalculateNumberOfPlayerMoves', () => {
  let engine: GameEngine;

  beforeEach(() => {
    engine = new GameEngine();
  });

  test('CalculateNumberOfPlayerMoves returns 2 for angle 0-60', () => {
    expect(engine.calculateNumberOfPlayerMoves(0)).toBe(2);
    expect(engine.calculateNumberOfPlayerMoves(30)).toBe(2);
    expect(engine.calculateNumberOfPlayerMoves(60)).toBe(2);
  });

  test('CalculateNumberOfPlayerMoves returns 1 for angle 61-120', () => {
    expect(engine.calculateNumberOfPlayerMoves(61)).toBe(1);
    expect(engine.calculateNumberOfPlayerMoves(90)).toBe(1);
    expect(engine.calculateNumberOfPlayerMoves(120)).toBe(1);
  });

  test('CalculateNumberOfPlayerMoves returns 6 for angle 121-180', () => {
    expect(engine.calculateNumberOfPlayerMoves(121)).toBe(6);
    expect(engine.calculateNumberOfPlayerMoves(150)).toBe(6);
    expect(engine.calculateNumberOfPlayerMoves(180)).toBe(6);
  });

  test('CalculateNumberOfPlayerMoves returns 5 for angle 181-240', () => {
    expect(engine.calculateNumberOfPlayerMoves(181)).toBe(5);
    expect(engine.calculateNumberOfPlayerMoves(210)).toBe(5);
    expect(engine.calculateNumberOfPlayerMoves(240)).toBe(5);
  });

  test('CalculateNumberOfPlayerMoves returns 4 for angle 241-300', () => {
    expect(engine.calculateNumberOfPlayerMoves(241)).toBe(4);
    expect(engine.calculateNumberOfPlayerMoves(270)).toBe(4);
    expect(engine.calculateNumberOfPlayerMoves(300)).toBe(4);
  });

  test('CalculateNumberOfPlayerMoves returns 3 for angle 301-360', () => {
    expect(engine.calculateNumberOfPlayerMoves(301)).toBe(3);
    expect(engine.calculateNumberOfPlayerMoves(330)).toBe(3);
    expect(engine.calculateNumberOfPlayerMoves(359)).toBe(3);
  });
});
