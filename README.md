# BlackSpire_VotA  
A next‑generation voxel survival engine built for harsh worlds, procedural exploration, and long‑term progression.

BlackSpire_VotA is the core engine powering a fully dynamic survival sandbox world.  
It combines smooth terrain, cubic structures, layered biomes, and a simulation‑driven environment into a scalable, studio‑grade framework.

This repository contains the engine code, world generation systems, voxel logic, and gameplay foundations for the BlackSpire project.

---

## 🌑 Overview

BlackSpire_VotA is designed around:

- **Procedural terrain** with layered noise, biomes, and underground networks  
- **Chunk‑based voxel streaming** for massive seamless worlds  
- **Handcrafted and procedural structures** integrated into terrain  
- **Survival systems** built around scarcity, exploration, and environmental danger  
- **A modular engine architecture** that supports expansion and future content  
- **A dark, atmospheric world tone** with environmental storytelling  

This is a commercial project intended for future release.

---

## ⚙️ Core Features

### **Terrain & World Generation**
- Smooth terrain with cliffs, ridges, and badlands  
- Multi‑pass noise system (height, moisture, temperature, biome blending)  
- Underground caverns, tunnels, and void pockets  
- Surface features: debris, ruins, foliage, rock formations  
- Structure injection system for POIs, shelters, and world landmarks  

### **Voxel Engine**
- Infinite chunk streaming  
- Optimized greedy meshing  
- Mesh colliders generated per chunk  
- Block registry with metadata  
- Runtime material assignment  
- Modular block definitions  

### **Survival Systems**
- Player stats (health, stamina, hunger, hydration, temperature)  
- Weight‑based inventory  
- Crafting + resource processing (framework in place)  
- Day/night cycle hooks  
- Environmental hazards (heat, cold, radiation, corruption zones)

### **Runtime Asset Pipeline**
- JSON‑driven block definitions  
- Runtime texture atlas loading  
- Hot‑reload friendly  
- Supports modded textures, blocks, and materials  

---

## 📁 Project Structure

/Assets
  /Engine
    /Blocks
    /Chunks
    /Core
    /Player
    /Rendering
    /Terrain
      /Chunks
      /Density
      /Generation
      /Hermite
      /Meshing
      /World
/Prefabs
/Scenes

This structure is intentionally modular to support expansion, refactoring, and future systems.

---

## 🚀 Getting Started

1. Clone the repository  
2. Open the project in **Unity 2022 LTS**    
3. Press Play to generate a new world  

The engine will automatically:
- Load block definitions    
- Initialize world generation  
- Stream chunks as the player moves  

---

## 🧭 Roadmap

### **Short‑Term**
- Expanded biome suite  
- Additional structure types (ruins, shelters, industrial remnants)  
- Improved foliage + debris generation  
- Crafting & workstation system  
- Loot tables + container system  

### **Mid‑Term**
- Wildlife + hostile entities  
- Weather simulation  
- Temperature + clothing system  
- Farming + food production  
- Vehicle framework  

### **Long‑Term**
- Multiplayer support  
- Modding API  
- World events + dynamic threats  
- Full survival progression loop  

---

## 🛡️ License

**All Rights Reserved**

This project is proprietary and may not be copied, modified, distributed, or used commercially without explicit written permission from the owner.

If you are viewing this repository, you may:
- Read the code  
- Clone it locally for review  

You may **not**:
- Redistribute it  
- Modify and publish it  
- Use it in your own commercial or non‑commercial projects  
- Repackage or resell any part of it  

All rights belong to **CyberTrace Studios**.

---

## 🖤 Credits

Developed by **CyberTrace Studios**  
Lead Developer: **Matthew Hammel**
