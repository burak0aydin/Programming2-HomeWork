# Programming2-HomeWork

https://github.com/user-attachments/assets/b8fa74c8-6a3a-4050-98ea-ee81aece0c86

## General Overview

Programming2-HomeWork is a cross-platform desktop application developed in **C#** using the **Avalonia UI** framework. The main purpose of the project is to demonstrate dynamic UI element creation and interactive behavior on a canvas. When the application starts, it automatically creates 5 Labels and 5 TextBoxes positioned randomly within the window. Users can interact with these elements by dragging and dropping them around the interface. As elements are moved, the application continuously checks for collisions (overlapping elements) and visually indicates these conflicts by changing the elements’ border colors to red. Additionally, a dedicated button allows users to perform a batch collision check, which lists all overlapping element pairs in a ListBox.

## Technologies Used

- **C#**: The core programming language used for writing the application’s logic.
- **.NET 6**: The application is built on .NET 6, leveraging modern language features and performance optimizations.
- **Avalonia UI**: A cross-platform UI framework that enables the application to run on Windows, Linux, and macOS. It provides XAML-based UI definitions and supports rich styling and animations.
- **XAML**: Used for designing the UI layouts (in `App.axaml` and `MainWindow.axaml`), enabling a clean separation between the interface and the business logic.
- **NuGet**: Manages the Avalonia and other package dependencies required by the project.

## Project Structure

```plaintext
Programming2-HomeWork/
├── homeWork.csproj        # Project file with dependencies and build configuration
├── App.axaml              # Defines global resources, styles, and themes
├── App.axaml.cs           # Initializes the application and loads the main window
├── MainWindow.axaml       # Defines the UI layout of the main window using XAML
├── MainWindow.axaml.cs    # Contains the application logic: element creation, drag-and-drop, and collision detection
├── Program.cs             # Application entry point, bootstrapping the Avalonia app
└── README.md              # Project documentation (this file)
