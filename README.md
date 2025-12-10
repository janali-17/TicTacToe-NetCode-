# Tic Tac Toe Multiplayer (Unity Netcode)

This is a multiplayer Tic Tac Toe game created by Jan Ali Hassan using Unity Netcode for GameObjects.
The project includes a fully synchronized turn-based system, win/tie detection, and simple UI for both players.

## Features

* Host and Client buttons for multiplayer connection

* Turn-based gameplay (Cross vs Tick)

* Each player gets a unique symbol automatically

* Grid click detection synced over the network

* Win detection (displays “You Win” / “You Lose”)

* Tie detection when the board is full

* Rematch system to restart the game without reconnecting

# How It Works

The game uses a server-authoritative approach.
Only the server decides:

* Whether a move is valid

* Whose turn it is

* When a player wins or the game ties

All game states are synchronized between players through RPC calls.

# Scripts Overview

# GameManager
Handles all gameplay logic, networking, turn system, win/tie detection, and rematch functionality.

# GameVisualManager
Updates the board visuals and places the Tick/Cross symbols.

# GridPos
Detects which grid cell the player clicks on.

# NetworkUIManager
Controls Host and Client buttons.

# PlayerUI
Shows whose turn it is and what symbol the local player has.

# GameOverUI
Displays Win / Lose / Tie and contains the Rematch button.

# Technologies Used

* Unity Engine

* Unity Netcode for GameObjects

* C#
