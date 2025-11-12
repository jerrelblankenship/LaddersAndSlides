# Ladders & Slides

A modern TypeScript/Bun implementation of the classic Chutes and Ladders board game.

## Overview

This is a modernized version of a Windows 8 game, completely rewritten from C#/XAML to TypeScript/Bun with a modern web-based interface. The game maintains all the original gameplay mechanics (17 special moves, 2-4 players, spinner-based movement) while leveraging cutting-edge web technologies and the blazing-fast Bun runtime.

## Features

- **Classic Gameplay**: 100-square serpentine board with 17 special moves (8 ladders, 9 chutes)
- **2-4 Players**: Support for 2-4 players with colorful monster tokens (Blue, Green, Orange, Purple)
- **Animated Spinner**: Physics-based spinner wheel with deceleration animation (spins 2500-3300 degrees)
- **Smooth Animations**: Visual feedback for player movement and special move transitions
- **Responsive Design**: Works seamlessly on desktop and mobile devices
- **Original Assets**: All game graphics from the Windows 8 version preserved and reused
- **Fast Runtime**: Powered by Bun for instant startup and fast execution
- **Type-Safe**: Written in TypeScript for robust, maintainable code

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

## Quick Start (TL;DR)

```bash
# Install Bun (if needed)
curl -fsSL https://bun.sh/install | bash

# Clone, install, build, and run
bun install && bun run build && bun start

# Open http://localhost:3000 in your browser
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

### Available Scripts

- `bun install` - Install all dependencies
- `bun run build` - Compile TypeScript and copy assets to dist/
- `bun run copy-assets` - Copy assets to dist/ folder
- `bun start` - Run the compiled server from dist/
- `bun run dev` - Build and start in one command
- `bun test` - Run all unit tests
- `bun run watch` - Watch TypeScript files for changes

## Game Rules

1. **Setup**: Choose 2-4 players to start
2. **Objective**: Be the first to reach square 100
3. **Turns**: Click "Spin!" to spin the wheel and move 1-6 spaces
4. **Ladders**: Landing on a ladder takes you UP to a higher square
5. **Chutes**: Landing on a chute slides you DOWN to a lower square
6. **Winning**: First player to reach square 100 wins!

### Game Mechanics (Preserved from Original)

- **Spinner Physics**: Arrow spins 2500-3300 degrees with 98% deceleration per tick
- **Spinner Zones**: 6 zones (60° each) map to moves 1-6
  - 0-60°: 2 spaces
  - 61-120°: 1 space
  - 121-180°: 6 spaces
  - 181-240°: 5 spaces
  - 241-300°: 4 spaces
  - 301-360°: 3 spaces
- **Board Layout**: 10x10 serpentine grid (alternating row directions)
- **Movement Speed**: 100ms delay between each square moved
- **Special Move Animation**: 500ms delay before ladder/chute transport
- **Turn Rotation**: Automatic progression through players in order
- **Overshoot Protection**: Cannot move past square 100 (stays in place)

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

## Modernization from Windows 8

This application represents a complete modernization from the original Windows 8 Metro app to a modern web application. Every component was carefully converted to leverage current technologies while preserving the original game experience.

### What Was Converted

**Code Migration:**
- **C# → TypeScript**:
  - `GameEngine.cs` (394 lines) → `GameEngine.ts` with identical game logic
  - `Player.cs`, `PlayerColor.cs`, `SpecialMove.cs` → TypeScript domain models
  - All game state management preserved
  - Special moves hard-coded grid coordinates maintained

- **XAML → HTML/CSS**:
  - `Game.xaml` → Modern HTML5 with CSS Grid layout
  - `GameSetup.xaml` → Interactive player selection interface
  - `EntryPage.xaml` → Splash screen with CSS animations
  - Windows 8 Metro design → Modern responsive web design

- **Windows Runtime → Web APIs**:
  - `DispatcherTimer` → `setInterval` for game loop
  - `Storyboard` animations → CSS transitions and transforms
  - `PlaneProjection` → CSS `transform` for spinner rotation
  - XAML Grid system → CSS Grid with 10x10 layout

**Testing:**
- **MSTest → Bun Test**:
  - 14 comprehensive unit tests migrated
  - `GameEngine_GetNextPlayerTests.cs` → `GameEngine.test.ts`
  - `GameEngine_IsSpecialMoveTests.cs` → Test suites for special moves and spinner
  - Should.dll assertions → Bun's built-in expect API

**Runtime & Tooling:**
- **npm/Node.js → Bun**:
  - Fast package management (installs in seconds)
  - Native TypeScript support (no transpilation for running)
  - Built-in test runner (no Jest configuration needed)
  - All-in-one toolchain for better developer experience

**Assets Preserved:**
- Game board image: `chutesAndLadders3.png` (152KB)
- Spinner wheel graphics with multiple DPI versions
- All 4 monster player tokens (Blue, Green, Orange, Purple)
- Arrow pointer for spinner

### Architecture Improvements

- **Separation of Concerns**: Clean separation between game engine (business logic) and UI
- **Platform Independence**: Game engine has zero web/UI dependencies
- **Modern JavaScript**: ES2020 target with async/await and modern syntax
- **Type Safety**: Strict TypeScript configuration with no implicit any
- **Web Standards**: Uses standard DOM APIs, CSS Grid, and HTML5 semantics

## Architecture

The application follows a clean separation of concerns:

- **Domain Layer**: Pure TypeScript classes representing game entities
- **Engine Layer**: Platform-independent game logic
- **UI Layer**: Web-based controller connecting the engine to the DOM
- **Server**: Simple Express server for static file serving

## Testing

The project includes comprehensive unit tests covering:

- **Player turn rotation logic**: Ensures correct player sequencing (14 tests total)
- **Special move detection**: Validates all 17 ladders and chutes
- **Spinner angle to moves calculation**: Tests all 6 possible move outcomes (1-6 spaces)
- **Game state transitions**: Verifies state machine flows correctly

Run tests with:
```bash
bun test
```

All 14 tests pass successfully, providing confidence in the game logic accuracy.

## Performance Benefits

Switching to Bun provides significant performance improvements:

- **Faster Installs**: Dependencies install 20-100x faster than npm
- **Instant Startup**: Server starts in milliseconds vs seconds
- **Native TypeScript**: Run `.ts` files directly without compilation step
- **Smaller Bundle**: No need for heavy test frameworks or transpilers
- **Better DX**: Unified toolchain eliminates configuration complexity

## Why This Tech Stack?

**TypeScript**: Provides type safety and excellent IDE support, preventing entire classes of runtime errors.

**Bun**: Modern JavaScript runtime that's significantly faster than Node.js while maintaining compatibility. Bundles package manager, runtime, test runner, and bundler into one tool.

**Express**: Battle-tested web framework with minimal overhead for static file serving.

**CSS Grid**: Modern layout system perfect for the 10x10 game board, eliminating complex positioning logic.

**No Framework**: Vanilla TypeScript/HTML/CSS keeps the bundle small and eliminates unnecessary dependencies for this simple application.

## Project Stats

- **Lines of Code**: ~2,300 TypeScript lines (converted from ~2,300 C# lines)
- **Test Coverage**: 14 unit tests, all passing
- **Assets**: 15 image files totaling ~2.5MB
- **Dependencies**: 3 production dependencies (minimal footprint)
- **Special Moves**: 17 hard-coded ladder/chute configurations
- **Game States**: 8 distinct game state transitions
- **Player Tokens**: 4 unique monster designs
- **Build Time**: ~2 seconds
- **Package Install Time**: ~3 seconds with Bun

## Troubleshooting

**Bun not found:**
```bash
# Install Bun
curl -fsSL https://bun.sh/install | bash

# Restart your terminal or reload shell
source ~/.bashrc  # or ~/.zshrc
```

**TypeScript compilation errors:**
```bash
# Ensure you have the latest TypeScript
bun add -d typescript@latest

# Clean and rebuild
rm -rf dist && bun run build
```

**Assets not loading:**
```bash
# Ensure assets are copied
bun run copy-assets

# Check dist/assets directory exists
ls -la dist/assets
```

**Port 3000 already in use:**
```bash
# Change port in src/server.ts or use environment variable
PORT=3001 bun start
```

## Future Enhancements

Potential improvements for future versions:

- **Multiplayer Online**: WebSocket-based online multiplayer
- **Game Replay**: Record and replay game sessions
- **Leaderboard**: Track wins and fastest completion times
- **Sound Effects**: Audio feedback for spins, moves, and special events
- **Themes**: Additional board designs and player token options
- **Accessibility**: Screen reader support and keyboard navigation
- **Mobile App**: PWA or native mobile app version
- **AI Players**: Computer opponents with difficulty levels

## License

MIT

## Credits

- **Original Game**: Windows 8 Metro app by Jerrel Blankenship
- **Modernization**: Complete rewrite to TypeScript/Bun (2025)
- **Game Design**: Based on the classic Chutes and Ladders board game
- **Assets**: Original monster artwork and board design preserved from Windows 8 version
