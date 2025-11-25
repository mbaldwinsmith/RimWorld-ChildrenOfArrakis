# AGENTS.md

This document outlines the AI agents used during development of the **Arrakis-Inspired RimWorld Mod**. Each agent has a clearly defined role to keep the project consistent, maintainable, and reproducible.

---

## Overview

Agents assist with:

- Gameplay + systems design  
- XML Def generation  
- C# code scaffolding  
- Art + sprite concepts  
- Testing + debugging  
- Documentation  
- Packaging releases  

All agent outputs are drafts. **Final commits are always reviewed manually.**

---

# Agents

## 1. WorldBuilder Agent
**Role:** High-level design of biomes, Fremen culture, water-scarcity systems, and survival loops.  
**Outputs:** Gameplay rules, design notes, balancing intentions.  
**Limits:** Does not produce XML or C#.

---

## 2. XMLForge Agent
**Role:** Generate RimWorld Def XML.  
**Outputs:**  
- BiomeDefs (Arrakis desert)  
- ThingDefs (windtraps, thumpers, spice blooms)  
- PawnKindDefs (Fremen, desert fauna)  
- NeedDefs (hydration)  
- IncidentDefs (worm emergence)  
**Rules:**  
- Use RW XML conventions  
- Keep defs lean for balancing  
- No excessive `<li>` nesting

---

## 3. CSharpSmith Agent
**Role:** C# class + Harmony patch generation.  
**Outputs:**  
- `HydrationNeed.cs`  
- `WormAI.cs` (burrowing, vibration detection, emergence logic)  
- `MapComponent_Vibration.cs`  
- `Building_Thumper.cs`  
- Utility classes (sand movement bonuses, stillsuit hydration reduction)  
**Rules:**  
- Minimal reflection  
- Follow RimWorld naming conventions  
- Clear prefix/postfix patch structure

---

## 4. ArtEngine Agent
**Role:** Concept art + sprite drafts.  
**Outputs:**  
- Sandworm silhouettes + animation hints  
- Stillsuit/cloak/cryknfie concepts  
- Windtrap + moisture collector structures  
- Spice bloom tiles  
**Rules:**  
- Drafts only  
- Non-painterly, consistent with RimWorld style  
- No photoreal textures

---

## 5. TestRunner Agent
**Role:** Identify bugs + balance issues through simulated testing.  
**Outputs:**  
- Map gen errors  
- Hydration system issues  
- Worm pathing failures  
- Overpowered/underpowered building behaviours  
**Rules:**  
- Issues must be reproducible  
- No speculative feedback

---

## 6. Documentation Agent
**Role:** Produce project documentation.  
**Outputs:**  
- README.md  
- AGENTS.md  
- Changelogs  
- Feature breakdowns  
**Rules:**  
- Concise, GitHub-friendly  
- No promotional fluff  
- Clear formatting

---

## 7. ReleaseBot Agent
**Role:** Build exports + prepare GitHub releases.  
**Outputs:**  
- Tagged ZIP builds  
- Updated About.xml  
- Release notes  
**Rules:**  
- Only runs after manual approval  
- Never overwrites existing releases

---

# Workflow

1. **Design →** WorldBuilder  
2. **Prototype →** XMLForge + CSharpSmith  
3. **Sprites →** ArtEngine  
4. **Testing →** TestRunner  
5. **Docs →** Documentation Agent  
6. **Packaging →** ReleaseBot  

Each stage loops iteratively with manual review.
- Assume RimWorld 1.5 unless specified.

