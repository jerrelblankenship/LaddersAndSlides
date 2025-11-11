# Ladders & Slides

A modern TypeScript/Node.js implementation of the classic Chutes and Ladders board game.

## Overview

This is a modernized version of the classic Windows 8 game, converted from C#/XAML to TypeScript/Node.js with a web-based interface. The game maintains all the original gameplay mechanics while leveraging modern web technologies.

## Features

- **Classic Gameplay**: 100-square board with 17 ladders and chutes
- **2-4 Players**: Support for 2-4 players with colorful monster tokens
- **Animated Spinner**: Interactive spinner wheel to determine moves
- **Smooth Animations**: Visual feedback for all game actions
- **Responsive Design**: Works on desktop and mobile devices

## Technology Stack

- **TypeScript**: Type-safe game logic and UI code
- **Bun**: Fast all-in-one JavaScript runtime and package manager
- **Express**: Web server for serving static files
- **Bun Test**: Built-in testing framework
- **HTML/CSS**: Modern web UI (replacing XAML)

## Project Structure

```
LaddersAndSlides/
├── src/
│   ├── domain/           # Domain models
│   │   ├── Player.ts
│   │   └── PlayerColor.ts
│   ├── engine/           # Game logic
│   │   ├── GameEngine.ts
│   │   ├── GameStateEngine.ts
│   │   ├── SpecialMove.ts
│   │   └── __tests__/    # Unit tests
│   ├── ui/               # UI controller
│   │   └── GameController.ts
│   ├── public/           # Web assets
│   │   ├── index.html
│   │   └── styles.css
│   ├── assets/           # Game images
│   │   ├── GameArtifacts/
│   │   └── Players/
│   └── server.ts         # Express server
├── dist/                 # Compiled output
├── package.json
├── tsconfig.json
└── bunfig.toml           # Bun configuration
```

## Getting Started

### Prerequisites

- [Bun](https://bun.sh) 1.0+

**Install Bun:**
```bash
# macOS, Linux, and WSL
curl -fsSL https://bun.sh/install | bash

# Or use npm if you have Node.js
npm install -g bun
```

### Installation

1. Install dependencies:
```bash
bun install
```

2. Build the project:
```bash
bun run build
```

3. Start the server:
```bash
bun start
```

4. Open your browser and navigate to:
```
http://localhost:3000
```

## Development

### Build and Watch

To build with automatic recompilation on changes:
```bash
bun run watch
```

### Run Tests

```bash
bun test
```

### Development Mode

Build and start the server:
```bash
bun run dev
```

## Game Rules

1. **Setup**: Choose 2-4 players to start
2. **Objective**: Be the first to reach square 100
3. **Turns**: Click "Spin!" to spin the wheel and move 1-6 spaces
4. **Ladders**: Landing on a ladder takes you UP to a higher square
5. **Chutes**: Landing on a chute slides you DOWN to a lower square
6. **Winning**: First player to reach square 100 wins!

## Special Moves

### Ladders (Climb Up)
- 1 → 38
- 4 → 14
- 9 → 31
- 21 → 42
- 28 → 84
- 51 → 67
- 71 → 91
- 80 → 100

### Chutes (Slide Down)
- 16 → 2
- 47 → 26
- 49 → 11
- 56 → 53
- 64 → 60
- 87 → 24
- 93 → 73
- 95 → 75
- 98 → 78

## Conversion from Windows 8

This application was converted from the original Windows 8 C#/XAML version to modern web technologies:

- **C# → TypeScript**: All game logic converted to TypeScript
- **XAML → HTML/CSS**: UI rebuilt with modern web standards
- **Windows Runtime → Web APIs**: Platform-specific code removed
- **MSTest → Bun Test**: Unit tests converted to Bun's built-in test runner
- **Runtime → Bun**: Using Bun for fast execution and package management
- **Assets Reused**: All original game graphics preserved

## Architecture

The application follows a clean separation of concerns:

- **Domain Layer**: Pure TypeScript classes representing game entities
- **Engine Layer**: Platform-independent game logic
- **UI Layer**: Web-based controller connecting the engine to the DOM
- **Server**: Simple Express server for static file serving

## Testing

The project includes comprehensive unit tests covering:

- Player turn rotation logic
- Special move detection (ladders and chutes)
- Spinner angle to moves calculation
- Game state transitions

Run tests with:
```bash
bun test
```

## License

MIT

## Credits

Original Windows 8 game by Jerrel Blankenship
Modernized to TypeScript/Node.js in 2025
