import { GameEngine } from '../engine/GameEngine.js';
import { Player } from '../domain/Player.js';
import { PlayerColor } from '../domain/PlayerColor.js';
import { GameStateEngine } from '../engine/GameStateEngine.js';

export class GameController {
  private gameEngine: GameEngine;
  private gameLoopInterval: number | null = null;
  private selectedPlayers: Set<PlayerColor> = new Set();

  constructor() {
    this.gameEngine = new GameEngine();
    this.init();
  }

  private init(): void {
    // Entry screen
    const startGameBtn = document.getElementById('start-game-btn');
    startGameBtn?.addEventListener('click', () => this.showSetupScreen());

    // Setup screen
    const backToEntryBtn = document.getElementById('back-to-entry-btn');
    backToEntryBtn?.addEventListener('click', () => this.showEntryScreen());

    const startPlayingBtn = document.getElementById('start-playing-btn');
    startPlayingBtn?.addEventListener('click', () => this.startGame());

    // Setup player selection
    const playerCards = document.querySelectorAll('.player-card');
    playerCards.forEach(card => {
      card.addEventListener('click', () => {
        const checkbox = card.querySelector('.player-checkbox') as HTMLInputElement;
        checkbox.checked = !checkbox.checked;
        this.updatePlayerSelection();
      });
    });

    const checkboxes = document.querySelectorAll('.player-checkbox');
    checkboxes.forEach(checkbox => {
      checkbox.addEventListener('change', () => this.updatePlayerSelection());
    });

    // Game screen
    const spinBtn = document.getElementById('spin-btn');
    spinBtn?.addEventListener('click', () => this.handleSpin());

    // Winner screen
    const playAgainBtn = document.getElementById('play-again-btn');
    playAgainBtn?.addEventListener('click', () => this.resetGame());
  }

  private showEntryScreen(): void {
    this.switchScreen('entry-screen');
  }

  private showSetupScreen(): void {
    this.switchScreen('setup-screen');
  }

  private showGameScreen(): void {
    this.switchScreen('game-screen');
  }

  private showWinnerScreen(): void {
    this.switchScreen('winner-screen');
  }

  private switchScreen(screenId: string): void {
    const screens = document.querySelectorAll('.screen');
    screens.forEach(screen => screen.classList.remove('active'));

    const targetScreen = document.getElementById(screenId);
    targetScreen?.classList.add('active');
  }

  private updatePlayerSelection(): void {
    this.selectedPlayers.clear();

    const checkboxes = document.querySelectorAll('.player-checkbox') as NodeListOf<HTMLInputElement>;
    checkboxes.forEach(checkbox => {
      const card = checkbox.closest('.player-card');
      const color = card?.getAttribute('data-color') as PlayerColor;

      if (checkbox.checked && color) {
        this.selectedPlayers.add(color);
        card?.classList.add('selected');
      } else {
        card?.classList.remove('selected');
      }
    });

    const startPlayingBtn = document.getElementById('start-playing-btn') as HTMLButtonElement;
    if (startPlayingBtn) {
      startPlayingBtn.disabled = this.selectedPlayers.size < 2 || this.selectedPlayers.size > 4;
    }
  }

  private startGame(): void {
    // Initialize players
    this.gameEngine.players = [];
    let turnOrder = 1;

    this.selectedPlayers.forEach(color => {
      const player = new Player({
        name: `${color} Monster`,
        imageUri: `../assets/Players/${color}-Monster.png`,
        gameTokenColor: color,
        turnOrder: turnOrder++,
        currentTileNumber: 0,
        currentGridRow: 9,
        currentGridColumn: 0,
        currentMovesRemaining: 0,
        turnInProcess: false,
        currentlyOnAlternateRow: false,
        specialMoveTransportDestination: 0
      });

      this.gameEngine.players.push(player);
    });

    this.showGameScreen();
    this.initGameBoard();
    this.gameEngine.currentState = GameStateEngine.InitialGameState;
    this.gameEngine.getNextPlayer();
    this.updateCurrentPlayerIndicator();
    this.startGameLoop();
  }

  private initGameBoard(): void {
    const gameBoard = document.getElementById('game-board');
    const gutter = document.getElementById('gutter');

    if (!gameBoard || !gutter) return;

    // Clear previous game
    gameBoard.innerHTML = '';
    gutter.innerHTML = '';

    // Create grid cells
    for (let row = 0; row < 10; row++) {
      for (let col = 0; col < 10; col++) {
        const cell = document.createElement('div');
        cell.className = 'grid-cell';
        cell.dataset.row = row.toString();
        cell.dataset.col = col.toString();
        gameBoard.appendChild(cell);
      }
    }

    // Add player tokens to gutter
    this.gameEngine.players.forEach(player => {
      const token = this.createPlayerToken(player);
      gutter.appendChild(token);
    });
  }

  private createPlayerToken(player: Player): HTMLImageElement {
    const token = document.createElement('img');
    token.src = player.imageUri;
    token.alt = player.name;
    token.className = 'player-token';
    token.id = `token-${player.gameTokenColor}`;
    token.style.width = '55px';
    token.style.height = '55px';
    return token;
  }

  private handleSpin(): void {
    if (this.gameEngine.currentState !== GameStateEngine.TurnComplete) return;

    const spinBtn = document.getElementById('spin-btn') as HTMLButtonElement;
    spinBtn.disabled = true;

    this.gameEngine.calculateArrowSpin();
    this.gameEngine.currentState = GameStateEngine.ArrowEvent;
    this.animateArrow();
  }

  private animateArrow(): void {
    const arrow = document.getElementById('arrow');
    if (!arrow) return;

    const duration = this.gameEngine['arrowSpinDuration'];
    arrow.style.transition = 'transform 3s cubic-bezier(0.17, 0.67, 0.12, 0.99)';
    arrow.style.transform = `translate(-50%, -50%) rotate(${duration}deg)`;

    setTimeout(() => {
      arrow.style.transition = '';
    }, 3000);
  }

  private startGameLoop(): void {
    this.gameLoopInterval = window.setInterval(() => {
      this.gameLoop();
    }, 100);
  }

  private gameLoop(): void {
    switch (this.gameEngine.currentState) {
      case GameStateEngine.ArrowEvent:
        const currentRotation = this.gameEngine.processArrowEvent();
        break;

      case GameStateEngine.ArrowDelayedEvent:
        this.gameEngine.processArrowDelayEvent();
        // Check if state changed to PlayerEvent after processing
        if ((this.gameEngine.currentState as GameStateEngine) === GameStateEngine.PlayerEvent) {
          const moves = this.gameEngine['numberOfMovesForCurrentPlayer'];
          this.updateMovesDisplay(moves);
        }
        break;

      case GameStateEngine.PlayerEvent:
        const moved = this.gameEngine.processPlayerEvent();
        if (moved) {
          this.updatePlayerTokenPosition();
        }
        break;

      case GameStateEngine.GetNextPlayer:
        this.gameEngine.getNextPlayer();
        this.updateCurrentPlayerIndicator();
        const spinBtn = document.getElementById('spin-btn') as HTMLButtonElement;
        if (spinBtn) spinBtn.disabled = false;
        this.updateMovesDisplay(0);
        break;

      case GameStateEngine.PlayerSpecialMoveTransportMoveEvent:
        this.handleSpecialMove();
        break;

      case GameStateEngine.WinnerDeclared:
        this.handleWinner();
        break;
    }
  }

  private updatePlayerTokenPosition(): void {
    if (!this.gameEngine.currentPlayer) return;

    const player = this.gameEngine.currentPlayer;
    const token = document.getElementById(`token-${player.gameTokenColor}`);
    const gameBoard = document.getElementById('game-board');
    const gutter = document.getElementById('gutter');

    if (!token || !gameBoard || !gutter) return;

    // Move token from gutter to board if needed
    if (player.currentTileNumber === 1 && token.parentElement === gutter) {
      gutter.removeChild(token);
      gameBoard.appendChild(token);
    }

    // Position token on board
    if (token.parentElement === gameBoard) {
      this.positionToken(token, player.currentGridRow, player.currentGridColumn);
    }

    this.updateMovesDisplay(player.currentMovesRemaining);
  }

  private positionToken(token: HTMLElement, row: number, col: number): void {
    token.style.gridRow = (row + 1).toString();
    token.style.gridColumn = (col + 1).toString();
  }

  private handleSpecialMove(): void {
    if (!this.gameEngine.currentPlayer || !this.gameEngine.currentSpecialMove) return;

    setTimeout(() => {
      this.gameEngine.makeSpecialMove();
      this.updatePlayerTokenPosition();
    }, 500);
  }

  private updateCurrentPlayerIndicator(): void {
    const playerNameSpan = document.getElementById('current-player-name');
    if (playerNameSpan && this.gameEngine.currentPlayer) {
      playerNameSpan.textContent = this.gameEngine.currentPlayer.name;
      playerNameSpan.style.color = this.getPlayerColorHex(this.gameEngine.currentPlayer.gameTokenColor);
    }
  }

  private updateMovesDisplay(moves: number): void {
    const movesCount = document.getElementById('moves-count');
    if (movesCount) {
      movesCount.textContent = moves.toString();
    }
  }

  private getPlayerColorHex(color: PlayerColor): string {
    const colorMap: { [key in PlayerColor]: string } = {
      [PlayerColor.Blue]: '#3b82f6',
      [PlayerColor.Green]: '#10b981',
      [PlayerColor.Orange]: '#f97316',
      [PlayerColor.Purple]: '#a855f7'
    };
    return colorMap[color];
  }

  private handleWinner(): void {
    if (this.gameLoopInterval) {
      clearInterval(this.gameLoopInterval);
      this.gameLoopInterval = null;
    }

    if (this.gameEngine.currentPlayer) {
      const winnerImage = document.getElementById('winner-image') as HTMLImageElement;
      const winnerName = document.getElementById('winner-name');

      if (winnerImage) {
        winnerImage.src = this.gameEngine.currentPlayer.imageUri;
        winnerImage.alt = this.gameEngine.currentPlayer.name;
      }

      if (winnerName) {
        winnerName.textContent = this.gameEngine.currentPlayer.name;
        winnerName.style.color = this.getPlayerColorHex(this.gameEngine.currentPlayer.gameTokenColor);
      }
    }

    setTimeout(() => {
      this.showWinnerScreen();
    }, 1000);
  }

  private resetGame(): void {
    // Reset game state
    this.gameEngine = new GameEngine();
    this.selectedPlayers.clear();

    // Reset UI
    const checkboxes = document.querySelectorAll('.player-checkbox') as NodeListOf<HTMLInputElement>;
    checkboxes.forEach(checkbox => {
      checkbox.checked = false;
    });

    const playerCards = document.querySelectorAll('.player-card');
    playerCards.forEach(card => card.classList.remove('selected'));

    // Reset arrow rotation
    const arrow = document.getElementById('arrow');
    if (arrow) {
      arrow.style.transform = 'translate(-50%, -50%) rotate(0deg)';
    }

    this.showEntryScreen();
  }
}

// Initialize the game when the DOM is loaded
if (typeof document !== 'undefined') {
  document.addEventListener('DOMContentLoaded', () => {
    new GameController();
  });
}
