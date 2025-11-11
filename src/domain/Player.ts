import { PlayerColor } from './PlayerColor';

export class Player {
  name: string = '';
  imageUri: string = '';
  currentTileNumber: number = 0;
  currentGridRow: number = 0;
  currentGridColumn: number = 0;
  currentMovesRemaining: number = 0;
  turnOrder: number = 0;
  turnInProcess: boolean = false;
  currentlyOnAlternateRow: boolean = false;
  specialMoveTransportDestination: number = 0;
  gameTokenColor: PlayerColor = PlayerColor.Blue;

  constructor(init?: Partial<Player>) {
    if (init) {
      Object.assign(this, init);
    }
  }

  equals(other: Player | null): boolean {
    if (!other) return false;
    if (this === other) return true;
    return this.name === other.name && this.turnOrder === other.turnOrder;
  }

  static equals(left: Player | null, right: Player | null): boolean {
    if (left === null && right === null) return true;
    if (left === null || right === null) return false;
    return left.equals(right);
  }
}
