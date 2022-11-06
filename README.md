# **LightningStrikes** <sub>*by [Hydriuk](https://github.com/Hydriuk)*</sub>

This plugin allows admins to send lightning strikes, the ones from the game itself.

## Commands

`strike`: 
- **Syntax**: `[<player>] [-damage | -d]`
- **Description**: Spawn a lightning strike at the position you are looking at.
- **Parameters**: 
  - `[<player>]` (*Player name*): **Optional**. Will spawn the lightning strike on this player instead of where you are looking at.
  - `[-damage | -d]` (*Flag*): **Optional**. If you add this flag in the command, the lightning strike will deal damage.

`strikering`:
- **Alias**: `striker`
- **Syntax**: `[<player>] <amount> <radius> [<minDelay> <maxDelay>] [-damage | -d] [-random | -r | -circle | -c] [-ground | -g]`
- **Description**: Spawn multiple lightning strikes around a ring.
- **Parameters**:
  - `[<player>]` (*Player name*): **Optional**. Name of the player on which to execute the command.
  - `<amount>` (*Number*): **Required**. Number of lightning strikes to spawn.
  - `<radius>` (*Number*): **Required**. Radius of the circle over which to spawn lightning strikes.
  - `[<minDelay> <maxDelay>]` (*Number Number*): **Optional**. Set the minimum and maximum delay bewteen two consecutive strikes. A random value between these two will be chosen.
  - `[-damage | -d]` (*Flag*): **Optional**. If you add this flag in the command, the lightning strike will deal damage.
  - `[-random | -r ]` (*Flag*): **Optional**. If you set this flag, the lightning strikes will have random positions.
  - `[-circle | -c]` (*Flag*): **Optional**. If you set this flag, the lightning strikes will be randomly spawned inside the given radius instead of at its bounds.
  - `[-ground | -g]` (*Flag*): **Optional**. if you set this flag, the lightning strikes will be spawned at ground level instead of at the highest structure point.

## Notes

<font color="ff1021">**Warning**</font>

> If you send too many lightning strikes at once, players will kicked.  
> This can happen at 4000+ lightning strikes with a millisecond delay or with 1000+ lightning strikes with no delay for example.  
> These values depend on your server, it could happen at different values.

If you send enough lightning strikes with the `-damage` flag, you can kill players and destroy structures, barricades and resources.

The following command will spawn a single lightning strike randomly in a 200m range and deal damage : `/striker 1 200 -d -c`

### Attributions

Icon:
- [Thunder icons created by prettycons - Flaticon](https://www.flaticon.com/free-icons/thunder)
- [Icon generator](https://romannurik.github.io/AndroidAssetStudio/icons-launcher.html)