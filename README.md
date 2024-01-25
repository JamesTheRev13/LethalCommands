# LethalCommands

This is an early work in progress BepInEx plugin for **Lethal Company (v49)**. Functionality/commands are likely to change as the plugin evolves.

Inspired by 'GameMaster', 'DanceTools', and 'Non-Lethal Company'

## Commands
To enter a [command](https://refactoring.guru/design-patterns/command), open the text chat (default key is `/`) and enter a command listed below:

### Player Commands

`/god`: Toggles God Mode

`/kill`: Kills the player

`/noclip`: Toggles NoClip (Control with `W` `A` `S` `D`, `CTRL` and `SPACE`)

`/noclip <number>`: Set NoClip 'flying' speed - Ex: `/noclip 15`

`/vision`: Toggles Night Vision

`/speed`: Toggles Movement Speed Modifications (Value must be set with the `/set speed <value>` command

`/speed <value>`: Set the movementSpeed value (used if `/speed` is on) - Ex: `/set speed 30`

`/sprint`: Toggle infinite sprint

`/jump`: Toggle Super Jump (Value must be set with the `/set jump <value>` command

`/jump <value>`: Set the jumpForce value (used if `/jump` is toggled on) - Ex: `/set jump 40`

`/jumps`: Toggle infinite jumps (can jump in mid-air, you basically can double/triple/etc jump)

`/ammo`: Toggle infinite ammo (Shotgun)

### Game Commands

`/credits`: Toggle infinite credits (freezes credits at `69420`)

`/deadline`: **(HOST ONLY)** Toggle infinite/no deadline (freezes deadline at 4 days)

`/unlock`: Unlock all doors on the map (doesn't open/unlock large/double doors yet)

### Spawning Commands

`/item <itemName> <?location> <?quantity>`: **(HOST ONLY)** Spawn item(s) at your (or another/all player(s)) feet

Examples: 
- `/item mapper 4`
- `/item jetpack`
- `/item shotgun all`
- `/item comedy BobSaget`
- `/item ammo all 2`

`/enemy <enemyName> <?quantity>`: **(HOST ONLY)** Spawn enemy(or enemies) at your position - Ex: `/enemy nutcracker 3` or `/enemy girl`

### Teleporting (Self only at the moment)

`/teleport <location>`: Teleport to <location> - Currently valid locations: `ship, inside, outside`, or any player username (must be in game)

Examples: 
- `/teleport BobSaget` - Teleports player to position the player in the lobby with the username `BobSaget`
- `/teleport ship` - Teleports player inside ship
- `/teleport inside` - Teleports player at the inside entrance of the facility/dungeon

## Installation (Manual)
1. Navigate to the [Releases](https://github.com/JamesTheRev13/LethalCommands/releases) section, find the latest release, and download the `Releasevx.xx.x.zip`
2. Open your Steam Library, right click Lethal Company, click Manage -> Browse Local Files.
3. Open the `Releasevx.xx.x.zip` download, and drag the `BepInEx` folder into your Lethal Company folder.
4. Don't ruin public lobbies, and have fun!
