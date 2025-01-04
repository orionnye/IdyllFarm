# IdyllFarm

A farming simulation game built with Godot 4.3 and C#, where players can create and manage their own idyllic farm environment.

## Overview

IdyllFarm is a farming simulation game that combines modern game development practices with an immersive farming experience. The game is built using the Godot game engine with C# as the scripting language, providing a robust and performant foundation for gameplay mechanics.

## Installation

### Prerequisites
- Godot 4.3 or later
- .NET SDK 6.0 or later
- Visual Studio Code (recommended) with C# extensions

### Setup Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/IdyllFarm.git
   ```

2. Open Godot Engine and import the project by selecting the `project.godot` file.

3. Install the required .NET dependencies:
   ```bash
   dotnet restore
   ```

4. Open the project in Visual Studio Code:
   ```bash
   code .
   ```

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
