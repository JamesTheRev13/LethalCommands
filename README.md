# LethalCommands

This is an early work in progress BepInEx plugin for **Lethal Company (v49)**. Functionality/commands are likely to change as the plugin evolves.

Inspired by 'GameMaster' and 'Non-Lethal Company'

## Commands
To enter a command, open the text chat (default key is `/`) and enter a command listed below:

`/god`: Toggles God Mode

`/noclip`: Toggles NoClip (Control with `W` `A` `S` `D`, `CTRL` and `SPACE`)

`/set noclip <value>`: Set the noclip speed

`/vision`: Toggles Night Vision

`/speed`: Toggles Movement Speed Modifications (Value must be set with the `/set speed <value>` command

`/set speed <value>`: Set the movementSpeed value (used if `/speed` is on) - Ex: `/set speed 30`

`/sprint`: Toggle infinite sprint

`/jump`: Toggle Super Jump (Value must be set with the `/set jump <value>` command

`/set jump <value>`: Set the jumpForce value (used if `/jump` is toggled on) - Ex: `/set jump 40`

`/jumps`: Toggle infinite jumps (can jump in mid-air, you basically can double/triple/etc jump)

`/credits`: Toggle infinite credits (freezes credits at `69420`)

`/deadline`: Toggle infinite/no deadline (freezes deadline at 4 days)

`/unlock`: Unlock all doors on the map (doesn't open/unlock large/double doors yet)

`/shotgun`: (HOST ONLY) Spawn a shotgun at your feet (spawns with "infinite" ammo)

`/ammo`: Toggle infinite ammo (Shotgun)

`/nutcracker`: (HOST ONLY) Spawn a Nutcracker at your position

`/teleport <location>`: Teleport to <location> - Currently valid locations: `ship, inside, outside`, or any player username (must be in game)

Examples: 
- `/teleport BobSaget` - Teleports player to position the player in the lobby with the username `BobSaget`
- `/teleport ship` - Teleports player inside ship
- `/teleport inside` - Teleports player at the inside entrance of the facility/dungeon
