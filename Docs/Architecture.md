# WindOS Architecture Guide

## Overview

WindOS is a C# operating system based on the Cosmos Dev Kit. It moves away from the traditional Cosmos "Ring 0" monolithic loop by implementing a modular application architecture.

## Core Components

### 1. Kernel (`Kernel.cs`)
The entry point of the OS.
-   **Initialization**: Sets up VFS, Graphics, and Resources.
-   **Main Loop**:
    -   Handles Global Input (e.g., Start Menu toggle, Wallpapers).
    -   Manages the Global Lock Screen.
    -   Delegates execution to the `ProcessManager`.
    -   Draws global UI elements (Background, Taskbar, Cursor).

### 2. Process Manager (`System/ProcessManager.cs`)
Responsible for managing the lifecycle of applications.
-   **Registration**: Apps are registered at boot.
-   **Switching**: `StartApp(name)` pauses the current app and starts the new one.
-   **Execution**: Calls `Update()` and `Draw()` on the active app.

### 3. Application Base (`Apps/App.cs`)
The abstract base class for all applications.
-   `Start()` / `Stop()`: Lifecycle hooks.
-   `Update()`: Handle input and logic.
-   `Draw(Canvas)`: Render UI to the screen.

### 4. Configuration (`System/ConfigManager.cs`)
Persists system settings to `0:\settings.cfg`.
-   Handles PIN, Colors, Wallpaper index, etc.

## Adding a New App

1.  Create a new class in `Apps/` inheriting from `App`.
2.  Implement `Update()` for logic (Input) and `Draw()` for rendering.
3.  Register the app in `Kernel.cs` inside `BeforeRun()`:
    ```csharp
    processManager.RegisterApp(new MyApp());
    ```
4.  Add the app to the `Menu` class constructor to make it launchable.

## Graphics

WindOS uses the Cosmos `Canvas` API (System.Drawing-like). All drawing is immediate mode inside the `Draw()` loop.

## File System

Uses standard `System.IO` classes mapped to Cosmos VFS. Root is `0:\`.
