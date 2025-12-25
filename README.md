# WindOS

WindOS is a C# operating system built using the COSMOS dev kit. It features a GUI with a window-like manager (App system), file system support, and several built-in applications.

## Features

*   **Process Manager**: Switch between multiple running apps (simulated).
*   **File System**: Browse `0:\`, create folders, and open files.
*   **Apps**:
    *   **Calculator**: Basic arithmetic operations.
    *   **Notepad**: Create, edit, and save text files.
    *   **Clock**: Real-time clock with Stopwatch and Timer.
    *   **File Explorer**: Navigate directories and manage files.
    *   **Console**: Command-line interface with basic commands.
*   **System**:
    *   **Lock Screen**: Secure pin-based login (Default PIN: 1234).
    *   **Start Menu**: Launch applications and power options.
    *   **Taskbar**: View running apps and current time.
    *   **Wallpapers**: Switch between backgrounds (top-right corner click).

## How to Use

1.  **Boot**: Enter PIN `1234` at the lock screen.
2.  **Navigation**: Use the Start Menu (bottom left) to launch apps.
3.  **Interaction**:
    *   **Menu**: Click the 'Start' button.
    *   **Wallpaper**: Click the top-right corner of the screen to cycle wallpapers.
    *   **File Explorer**: Use `Arrow Keys` to navigate, `Enter` to open, `Backspace` to go up. `F2` to create a folder.
    *   **Notepad**: Type normally. `F1` to save.

## For Developers

1.  **Framework**: COSMOS Dev Kit 2022.
2.  **IDE**: Visual Studio 2022.
3.  **Structure**:
    *   `Apps/`: Application logic (inherited from `App` class).
    *   `System/`: Core system components (ProcessManager, Graphics, Input).
    *   `Kernel.cs`: Entry point and main loop.

## Credits

*   Made by Sanskar using COSMOS.
*   Refactored by Jules.
