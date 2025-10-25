# Thesis Battle System
This project is the practical implementation of my university thesis, titled "Development of a Video Game with an Integrated Move Selection System". It's a 2D turn-based tactical RPG built from the ground up in the **Godot Engine** using **C#**.

## Gameplace Showcase 
`![Gameplay Screenshot 1](Gameplay Screenshots/Gameplay 01.png)`
`![Gameplay Screenshot 2](Gameplay Screenshots/Gameplay 02.png)`

---

## Key Features

### Game Content
*  **Three Core Screens:** Main Menu, Enemy Selection, and the Battle Screen.
*  **Four Unique Opponents:**
    * Custom 2D sprites designed specifically for the project.
    * Each with a unique set of moves and stats.
    * Designed with different RPG archetypes, requiring distinct strategies to defeat.

## Battle System
*  **Deep Tactical Combat:** A turn-based system featuring **9 damage types** and **2 damafe categories** (Physical & Special).
*  **Strategic Design:** The system is based on the principles of orthogonal unit differentiation and non-transistive relationships.

### Artificial Intelligence
*  **AI Companion:** The player is assisted by an AI-controlled helper character.
*  **Expectiminimax Algorith:** The AI's decision-making is powered by the Expectiminimax algorithm, allowing it to "look ahead" into the future.
*  **Two-Ply Search Depth:** The AI can calculate all possible game states for the next **two turns** (one helper move + one enemy response).
*  **Custom Evaluation Function:** A sophisticated evaluation function scores the outcomes of simulated battles, enabling the AI to choose the move that leads to the most beneficial state for the player.

## Technologies Used
*  **Game Engine:** Godot Engine v4.x 
*  **Developed and Tested on:** v4.4.1 (with .NET)
*  **Programming Language:** C#
*  **Data Management:** Game data is managed via external JSON files for easy modification and scalability.

---

## Thesis Paper
You can read the full academic paper on which this implementation was based **[here]()**

