# IdyllFarm

A farming simulation game built with Godot 4.3 and C#, where players can create and manage their own idyllic farm environment.

## Overview

IdyllFarm is a farming simulation game that combines modern game development practices with an immersive farming experience. The game is built using the Godot game engine with C# as the scripting language, providing a robust and performant foundation for gameplay mechanics.

## Installation

### Prerequisites
- Godot 4.3 or later
- .NET SDK 6.0 or later (9.0+ recommended)
- Visual Studio Code (recommended) with C# extensions
- Homebrew (for macOS users)

### macOS Setup
1. Install Homebrew if not already installed:
   ```bash
   /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
   ```

2. Install .NET SDK:
   ```bash
   brew install dotnet
   ```

3. Set up .NET environment (add to ~/.zshrc):
   ```bash
   export DOTNET_ROOT="/opt/homebrew/opt/dotnet/libexec"
   ```

4. Install Godot:
   ```bash
   brew install --cask godot
   ```

5. Install Visual Studio Code:
   ```bash
   brew install --cask visual-studio-code
   ```

### Project Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/IdyllFarm.git
   cd IdyllFarm
   ```

2. Install the required .NET dependencies:
   ```bash
   dotnet restore
   ```

3. Install Visual Studio Code C# Extension:
   - Open VS Code
   - Press `Cmd+Shift+X` (macOS) or `Ctrl+Shift+X` (Windows/Linux)
   - Search for "C#"
   - Install the official Microsoft C# extension

4. Open Godot Engine and import the project by selecting the `project.godot` file.

5. Open the project in Visual Studio Code:
   ```bash
   code .
   ```

### Running the Project
1. For the best experience, run the game directly in debug mode with Full HD resolution:
   ```bash
   /Applications/Godot_mono.app/Contents/MacOS/Godot --verbose
   --resolution 1920x1080 --debug Environment/Farm/PlantTest.tscn
   ```

2. Alternatively, launch Godot:
   ```bash
   godot
   ```
   Or open Godot from your Applications folder

2. In the Project Manager window:
   - Click "Import"
   - Navigate to your IdyllFarm project folder
   - Select the `project.godot` file
   - Click "Import & Edit"

3. Once the project is open:
   - Click the "Build" button (hammer icon) or press F7 to build the C# project
   - Click the "Play" button (triangle icon) or press F5 to run the game

Note: The first time you run the project, Godot will need to import assets and compile shaders. This may take a few minutes.

## Architecture

The project follows a modular architecture with clear separation of concerns:

### Core Components

1. **Actors/**
   - `Plants/`: Plant-related game objects and behaviors
   - `Items/`: Inventory and item system implementations
   - `Controllers/`: Game logic and input handling

2. **Environment/**
   - `Farm/`: Main farming environment and mechanics
   - `Interior/`: Indoor environments and structures
   - `Movement/`: Movement and physics systems
   - `Particles/`: Visual effects and particle systems
   - `Rigid/`: Rigid body interactions
   - `Static/`: Static environment elements

3. **UI/**
   - User interface elements and HUD components

4. **Assets/**
   - Game assets including textures, models, and sounds

### Technical Stack
- **Engine**: Godot 4.3
- **Language**: C# (.NET)
- **Rendering**: Forward Plus rendering pipeline
- **Version Control**: Git

### Design Principles
- Modular component architecture
- Data-driven design
- Clear separation between game logic and presentation
- Scalable and maintainable codebase structure

## License

This project is licensed under the terms included in the LICENSE file.
