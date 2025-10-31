
# IT008 WinForms Game

A small 2D WinForms game project with scenes, audio, and basic player–bullet–enemy gameplay.

## TOC
<!-- TOC start (generated with https://github.com/derlin/bitdowntoc) -->

- [IT008 WinForms Game](#it008-winforms-game)
   * [📥 Clone](#-clone)
   * [🏁 Entry point](#-entry-point)
   * [🧭 Scene switching](#-scene-switching)
   * [🔊 Audio](#-audio)
   * [🧱 Scenes, gameobjects, controls](#-scenes-gameobjects-controls)
   * [🎮 Current gameplay](#-current-gameplay)
   * [🤝 Contributing](#-contributing)
   * [🗂 Important files](#-important-files)

<!-- TOC end -->

## 📥 Clone

```bash
git clone https://github.com/thaiminh2022/IT008_MiniVamp
cd IT008_Game
```

Open the solution in **Visual Studio** and run.

## 🏁 Entry point

- **Entry form/entry point:** `GameForm.cs`
  - creates the main WinForms window
  - sets up input
  - starts the update/render loop
  - forwards update/draw to the current scene

## 🧭 Scene switching

- Managed by **`SceneManager`**.
- Use:
  ```csharp
  SceneManager.ChangeScene(MainMenuScene.Name);
  ```
- On scene change:
  - old scene → `Unload()` → destroy all gameobjects, clear all controls
  - new scene → `Load()` → init all UIs and gameobjects

## 🔊 Audio

- All audio goes through **`AudioManager`**.
- Call:
  ```csharp
  AudioManager.Shoot.Play();
  ```
  or whatever key you registered.
- Centralizing audio makes it easier to change the backend later.

## 🧱 Scenes, gameobjects, controls

Each scene **contains** its own game objects and UI controls.

- **On load** (scene starts):
  - init all UIs
  - create all game objects (player, enemies, bullets list…)
  - add controls to the form/scene
- **On unload** (scene ends):
  - destroy/dispose of all game objects
  - clear / remove all controls
  - unsubscribe from any events

This keeps scene switches clean (no leftover enemies or UI from previous scenes).

## 🎮 Current gameplay

- The project currently contains:
  - **Player** class
  - **Bullet** class
  - **Enemy** class
- **Player controls:**
  - Move: **WASD** or **Arrow keys**
  - Shoot: **Space** (spawns a bullet and adds it to the scene)
  - Destroy player (test): **X**
  - Pause the game: **Esc**
- **Enemy:**
  - Can be hit by a bullet
  - collision between **bullet ↔ enemy** is checked in **`MainGameScene`** (the scene loops over bullets and enemies and removes them on hit)

## 🤝 Contributing

1. **Create a new branch** from `main`:

   ```bash
   git checkout main
   git pull
   git checkout -b feature/<your-feature-name>
   ```

2. **Make your changes**:
   - new scene → register in `SceneManager`
   - new sound → add to `AudioManager`
   - new gameplay object → create it in a scene’s `OnLoad()`

3. **Commit**:

   ```bash
   git add .
   git commit -m "feat: add new enemy type"
   ```

4. **Push and open a Pull Request**:

   ```bash
   git push -u origin feature/<your-feature-name>
   ```

   Then on GitHub:
   - base: `master`
   - compare: `feature/<your-feature-name>`
   - Describe what you added and how to test it

## 🗂 Important files

- `GameForm.cs` — entry point
- `SceneManager.cs` — scene switching
- `AudioManager.cs` — audio playback
- `Scenes/MainGameScene.cs` — main gameplay, bullet–enemy collision
- `Player.cs`, `Bullet.cs`, `Enemy.cs` — core gameplay objects
