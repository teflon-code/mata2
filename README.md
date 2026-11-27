# Nairobi Matatu

A compact native Unity 3D arcade-driving game set in a stylized Nairobi CBD.

## Play

1. Install Unity Hub and Unity Editor 2022.3 LTS.
2. Open this folder as a Unity project.
3. Open `Assets/Scenes/NairobiMatatu.unity`.
4. Press Play.
5. Build a desktop executable with **Nairobi Matatu > Build Windows**.

## Controls

- **W / Up**: accelerate
- **S / Down**: reverse
- **A/D or arrows**: steer
- **Space**: brake
- **R**: reset matatu
- **H**: horn

## Gameplay

Drive the green matatu through five checkpoint rings around central Nairobi. The HUD tracks speed, route progress, and elapsed time. Roads are generated from a compact local subset of OpenStreetMap centerline geometry; see `DATA_ATTRIBUTION.md`.

The game is native and desktop-focused. It uses no browser runtime, online API, or third-party map SDK.

## Browser version

A self-contained browser mirror is served from `docs/` for GitHub Pages. It retains the same compact OSM road geometry and checkpoint route. Open the public GitHub Pages URL after Pages deployment is enabled.
