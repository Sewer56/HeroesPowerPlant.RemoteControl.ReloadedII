<div align="center">
	<h1>HPP: Remote Control</h1>
	<img src="https://i.imgur.com/BjPn7rU.png" width="150" align="center" />
	<br/> <br/>
	<strong>Just making life a bit more convenient.<br/></strong>
<b>Id: sonicheroes.hpp.remotecontrol</b>
</div>

# Prerequisites
This mod uses the [Hooks Shared Library](https://github.com/Sewer56/Reloaded.SharedLib.Hooks).
Please download and extract that mod first.

# About This Project

The following project is a [Reloaded II](https://github.com/Reloaded-Project/Reloaded-II) Mod Loader mod that hosts a local server, allowing clients to perform the following set of functions in real time.

- Load New Collision
- ??? (Coming in the future (TM))

It is designed and intended to be consumed by the [Heroes Power Plant](https://github.com/igorseabra4/HeroesPowerPlant) level editor.

# Usage

- Add this repository to your submodules.
- Add `HeroesPowerPlant.RemoteControl.Shared.csproj` to your project.
- Create an instance of `Client` class to connect to the mod.

```csharp
var heroes = Process.GetProcessesByName("tsonic_win")[0];
var client = new Client(heroes);
```

**Example Usage**
```csharp
await client.LoadCollision("s03"); // Loads s03.cl, s03_xx.cl, s03_wt.cl
```
