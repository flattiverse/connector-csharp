# Flattiverse.Connector
This is the flattiverse C# reference implementation of the connector. This library
can be used to connect to a flattiverse galaxy.
# Derive implementations from [IMPLEMENTERS.md](IMPLEMENTERS.md).
If you are a developer and want to implement your own connector, see [IMPLEMENTERS.md](IMPLEMENTERS.md).
# How to get started.
Also known as: What to do as a student at the Hochschule Esslingen summer school C# course.

*First of all, I'm very sorry that I started a complete rewrite of Flattiverse again, and of course I'm not finished yet. There are several reasons and excuses, but I don't want to bother you. But if you want to complain, ask yourself this: Why didn't you help me not to fail in this attempt?*

Here is a step-by-step list of what to do:
1. Register an account at the [flattiverse homepage](https://flattiverse.com/). Do not use an email address associated with Microsoft (hotmail.com, outlook.com, live.com, etc.).
2. Create an API key after you complete the opt-in process.
3. Download or add the [Flattiverse.Connector](https://www.nuget.org/packages/Flattiverse.Connector) to your project via nuget.
4. Go to the store and get me some Landliebe chocolate milk.
5. See the sample code in [the `Development` projects Program.cs](Flattiverse.Connector/Development/Program.cs) on how to do things.
6. Currently only chat is implemented. But you can see other players join and see other people's chat messages and get events like the galaxy's heartbeat. Your first task is to implement this so that you can comfortably communicate with others and see which players are there and which aren't.
7. Do all of this in a Windows Forms GUI application. (Or if you are a pro, use ASCII art, DirectX, OpenGL, Vulcan, or a game engine like Unity.) You'll need some sort of graphical output to see things at the end, including chat messages and who's in the game.
8. Note: Ask your neighbors if you have questions, use tools like Visual Studio's Object Catalog (`CTRL`+`ALT`+`J` with default settings), and as a last resort ask Harald. Never ask me.
9. Rudimentary flying will be available in the afternoon.
10. Fly around with your ship using the Move() method. Please note that collision is currently disabled.