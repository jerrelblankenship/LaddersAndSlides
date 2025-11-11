export class SpecialMove {
  startingTileNumber: number = 0;
  startingGridColumn: number = 0;
  startingGridRow: number = 0;
  endingTileNumber: number = 0;
  endingGridColumn: number = 0;
  endingGridRow: number = 0;

  constructor(init?: Partial<SpecialMove>) {
    if (init) {
      Object.assign(this, init);
    }
  }
}
