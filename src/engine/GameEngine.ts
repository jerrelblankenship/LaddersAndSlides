import { Player } from '../domain/Player.js';
import { SpecialMove } from './SpecialMove.js';
import { GameStateEngine } from './GameStateEngine.js';

export class GameEngine {
  players: Player[] = [];
  specialMoves: SpecialMove[] = [];
  currentPlayer: Player | null = null;
  currentState: GameStateEngine = GameStateEngine.InitialGameState;
  currentSpecialMove: SpecialMove | null = null;

  protected randomNumberGenerator: Random;
  protected arrowSpinDuration: number = 0;
  protected numberOfMovesForCurrentPlayer: number = 0;
  protected waitStart: number = 0;

  constructor() {
    this.randomNumberGenerator = new Random();
    this.createListSpecialMoves();
  }

  getNextPlayer(): void {
    if (this.currentPlayer === null) {
      this.currentPlayer = this.players.find(x => x.turnOrder === 1) || null;
    } else {
      const nextTurnOrder = this.currentPlayer.turnOrder + 1;

      this.currentPlayer = nextTurnOrder > this.players.length
        ? this.players.find(x => x.turnOrder === 1) || null
        : this.players.find(x => x.turnOrder === nextTurnOrder) || null;
    }

    this.currentState = GameStateEngine.TurnComplete;
  }

  processArrowEvent(): number {
    this.arrowSpinDuration *= 0.98;

    if (this.randomNumberGenerator.nextDouble() < 0.05 && this.arrowSpinDuration < 360) {
      this.currentState = GameStateEngine.ArrowDelayedEvent;
      this.numberOfMovesForCurrentPlayer = this.calculateNumberOfPlayerMoves(this.arrowSpinDuration);
      this.waitStart = Date.now();
    }

    return this.arrowSpinDuration;
  }

  calculateNumberOfPlayerMoves(arrowSpinAngle: number): number {
    if (arrowSpinAngle >= 0 && arrowSpinAngle <= 60) return 2;
    if (arrowSpinAngle > 60 && arrowSpinAngle <= 120) return 1;
    if (arrowSpinAngle > 120 && arrowSpinAngle <= 180) return 6;
    if (arrowSpinAngle > 180 && arrowSpinAngle <= 240) return 5;
    if (arrowSpinAngle > 240 && arrowSpinAngle <= 300) return 4;
    return 3;
  }

  calculateArrowSpin(): void {
    this.randomNumberGenerator = new Random();
    this.arrowSpinDuration = this.randomNumberGenerator.next(2500) + this.randomNumberGenerator.next(200, 800);
  }

  processArrowDelayEvent(): void {
    if ((Date.now() - this.waitStart) / 1000 > 0.45) {
      this.currentState = GameStateEngine.PlayerEvent;
    }
  }

  processPlayerEvent(): boolean {
    if (!this.currentPlayer) return false;

    if ((Date.now() - this.waitStart) > 100) {
      this.waitStart = Date.now();

      if (!this.currentPlayer.turnInProcess) {
        this.currentPlayer.currentMovesRemaining = this.numberOfMovesForCurrentPlayer;
        this.currentPlayer.turnInProcess = true;
      }

      if (this.currentPlayer.currentTileNumber === 0) {
        this.currentPlayer.currentMovesRemaining--;
        this.currentPlayer.currentTileNumber = 1;
        this.currentPlayer.currentGridColumn = 0;
        this.currentPlayer.currentGridRow = 9;
      } else {
        if (this.currentPlayer.currentTileNumber + this.currentPlayer.currentMovesRemaining > 100) {
          this.currentPlayer.currentMovesRemaining = 0;
        } else {
          this.makePlayerMove();
        }
      }

      if (this.currentPlayer.currentMovesRemaining <= 0) {
        this.currentState = GameStateEngine.GetNextPlayer;
        this.currentPlayer.turnInProcess = false;

        const specialMove = this.isSpecialMove(this.currentPlayer.currentTileNumber);

        if (specialMove) {
          this.currentState = GameStateEngine.PlayerSpecialMoveTransportMoveEvent;
        } else if (this.currentPlayer.currentTileNumber === 100) {
          this.currentState = GameStateEngine.WinnerDeclared;
        }
      }

      return true;
    }

    return false;
  }

  makePlayerMove(): void {
    if (!this.currentPlayer) return;

    if (this.currentPlayer.currentTileNumber % 10 === 0) {
      this.currentPlayer.currentGridRow -= 1;
      this.currentPlayer.currentlyOnAlternateRow = this.currentPlayer.currentGridRow % 2 !== 0;
    } else {
      if (this.currentPlayer.currentlyOnAlternateRow) {
        this.currentPlayer.currentGridColumn -= 1;
      } else {
        this.currentPlayer.currentGridColumn += 1;
      }
    }

    this.currentPlayer.currentTileNumber++;
    this.currentPlayer.currentMovesRemaining--;
  }

  makeSpecialMove(): void {
    if (!this.currentPlayer || !this.currentSpecialMove) return;

    this.currentPlayer.currentGridColumn = this.currentSpecialMove.endingGridColumn;
    this.currentPlayer.currentGridRow = this.currentSpecialMove.endingGridRow;
    this.currentPlayer.currentTileNumber = this.currentSpecialMove.endingTileNumber;
    this.currentPlayer.currentlyOnAlternateRow = this.currentSpecialMove.endingGridRow % 2 === 0;
    this.currentState = GameStateEngine.GetNextPlayer;
    this.currentPlayer.turnInProcess = false;
  }

  isSpecialMove(playerTile: number): boolean {
    const result = this.specialMoves.find(x => x.startingTileNumber === playerTile);

    if (result) {
      this.currentSpecialMove = result;
      if (this.currentPlayer) {
        this.currentPlayer.specialMoveTransportDestination = result.endingTileNumber;
      }
    }

    return result !== undefined;
  }

  createListSpecialMoves(): void {
    this.specialMoves = [];

    const specialMoves: { [key: number]: number } = {
      1: 38, 4: 14, 9: 31, 21: 42,
      28: 84, 51: 67, 71: 91, 80: 100,
      16: 2, 47: 26, 49: 11, 56: 53,
      64: 60, 87: 24, 93: 73, 95: 75,
      98: 78
    };

    for (const [startTile, endTile] of Object.entries(specialMoves)) {
      const startingGridLocation = this.getGridColumnRow(parseInt(startTile));
      const endingGridLocation = this.getGridColumnRow(endTile);

      const sMove = new SpecialMove({
        startingTileNumber: parseInt(startTile),
        startingGridColumn: startingGridLocation.column,
        startingGridRow: startingGridLocation.row,
        endingTileNumber: endTile,
        endingGridColumn: endingGridLocation.column,
        endingGridRow: endingGridLocation.row
      });

      this.specialMoves.push(sMove);
    }
  }

  private getGridColumnRow(tileNumber: number): { column: number; row: number } {
    const gridLocations: { [key: number]: { column: number; row: number } } = {
      1: { column: 0, row: 9 },
      2: { column: 1, row: 9 },
      4: { column: 3, row: 9 },
      9: { column: 8, row: 9 },
      11: { column: 9, row: 8 },
      14: { column: 6, row: 8 },
      16: { column: 4, row: 8 },
      21: { column: 0, row: 7 },
      24: { column: 3, row: 7 },
      26: { column: 5, row: 7 },
      28: { column: 7, row: 7 },
      31: { column: 9, row: 6 },
      38: { column: 2, row: 6 },
      42: { column: 1, row: 5 },
      47: { column: 6, row: 5 },
      49: { column: 8, row: 5 },
      51: { column: 9, row: 4 },
      53: { column: 7, row: 4 },
      56: { column: 4, row: 4 },
      60: { column: 0, row: 4 },
      64: { column: 3, row: 3 },
      67: { column: 6, row: 3 },
      71: { column: 9, row: 2 },
      73: { column: 7, row: 2 },
      75: { column: 5, row: 2 },
      78: { column: 2, row: 2 },
      80: { column: 0, row: 2 },
      84: { column: 3, row: 1 },
      87: { column: 6, row: 1 },
      91: { column: 9, row: 0 },
      93: { column: 7, row: 0 },
      95: { column: 5, row: 0 },
      98: { column: 2, row: 0 },
      100: { column: 0, row: 0 }
    };

    return gridLocations[tileNumber] || { column: 0, row: 0 };
  }
}

// Simple Random class to replace C# Random
class Random {
  next(min: number = 0, max?: number): number {
    if (max === undefined) {
      max = min;
      min = 0;
    }
    return Math.floor(Math.random() * (max - min)) + min;
  }

  nextDouble(): number {
    return Math.random();
  }
}
