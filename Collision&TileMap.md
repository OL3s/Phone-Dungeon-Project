# Godot 4 – TileMap & Collision Node Overview

## 🧱 TileMap
- Renders a grid of tiles from a **TileSet**.
- Each tile can include:
  - Sprite (visual)
  - Collision shape(s)
  - Navigation polygons
  - Custom data (metadata, autotile rules)
- Efficient for large static maps.
- Often split into layers: floor, walls, decor, collision.

### Common Uses
- Background/terrain drawing.
- Static world collision (per-tile shapes).
- Visual layer separation (e.g., `wall_top`, `floor`, `shadow`).

---

## ⚙️ Collision Nodes

| Node | Purpose | Moves? | Detects? | Typical Use |
|------|----------|--------|-----------|--------------|
| **CollisionShape2D** | Defines a shape (rect, circle, capsule, polygon). Must be child of a body or Area2D. | — | — | Shape for physics/area detection |
| **StaticBody2D** | Immovable collider; blocks others. | ❌ | ✅ | Walls, props |
| **CharacterBody2D** | Kinematic body moved via `MoveAndSlide()`. | ✅ | ✅ | Player, enemies |
| **RigidBody2D** | Simulated by physics engine. | ⚙️ | ✅ | Physics objects (rocks, crates) |
| **Area2D** | Detects overlaps (signals). | ❌ | ✅ (via `area_entered` / `body_entered`) | Hitboxes, triggers, pickups |

---

## 💡 Recommended Setup (Top-Down Game)

| Type | Node | Description |
|------|------|--------------|
| Player/Enemies | `CharacterBody2D` + `CollisionShape2D` | Controlled movement + collision |
| Walls/Terrain | `TileMap` with tile collision | Static world |
| Props/Objects | `StaticBody2D` + `CollisionShape2D` | Solid decor |
| Attacks/Triggers | `Area2D` + `CollisionShape2D` | Detect hits/overlaps |
