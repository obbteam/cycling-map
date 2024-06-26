# Cycling-map - README

## Table of Contents
1. [Introduction](#introduction)
2. [Prerequisites](#prerequisites)
3. [Setup and Installation](#setup-and-installation)
    - [Rider](#rider)
    - [Visual Studio](#visual-studio)
4. [Running the Application](#running-the-application)
5. [Usage](#usage)
6. [Troubleshooting](#troubleshooting)
7. [Contributing](#contributing)
8. [License](#license)

## Introduction
**Cycling-map** is a C# WPF (Windows Presentation Foundation) application designed to find the shortest path from A to B (designed as the map for bicycles but now its just a map). This README provides instructions on how to set up, run, and contribute to the project using Rider or Visual Studio.

## Prerequisites
Before you begin, ensure you have met the following requirements:
- **Windows Operating System** (required for WPF applications)
- **.NET SDK** (version 6.0 or later)
- **Rider** or **Visual Studio** (latest version recommended)

## Setup and Installation

### Rider
1. **Install Rider**: Download and install [Rider](https://www.jetbrains.com/rider/download/).
2. **Clone the Repository**: Open a terminal and run the following command:
    ```bash
    git clone https://github.com/obbteam/cycling-map.git
    ```
3. **Open the Project**: Launch Rider and open the cloned repository folder.

### Visual Studio
1. **Install Visual Studio**: Download and install [Visual Studio](https://visualstudio.microsoft.com/downloads/). Ensure you include the ".NET Desktop Development" workload during installation.
2. **Clone the Repository**: Open a terminal or command prompt and run the following command:
    ```bash
    git clone https://github.com/obbteam/cycling-map.git
    ```
3. **Open the Project**: Launch Visual Studio and open the cloned repository folder.

## Running the Application

### Rider
1. **Build the Project**: In the Solution Explorer, right-click on the solution and select `Build`.
2. **Run the Application**: Click on the "Run" button or press `Shift + F10`.

### Visual Studio
1. **Build the Project**: In the Solution Explorer, right-click on the solution and select `Build Solution`.
2. **Run the Application**: Click on the "Start" button or press `F5`.

## Usage
Once the application is running, you can type in two addresses, choose the travel mode and press on button to find locations and then calculate route.

## Troubleshooting
- **Build Errors**: Ensure all dependencies are installed and the .NET SDK is correctly configured.
- **Running Issues**: Check the output window for any runtime errors and consult the logs for more details.

## Reference

- TOM TOM API was used for all the functionality - https://developer.tomtom.com/documentation

---

For any questions or issues, please contact [obidkhonakhmadkhonov@gmail.com].

Enjoy using **cycling-map**!
