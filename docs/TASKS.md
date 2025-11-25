# TASKS.md  
*Arrakis Mod – Master Task Breakdown with Embedded USER STORIES (For AI Agent Use)*  

This file contains **all development tasks**, each accompanied by **User Stories** to guide the AI agent toward the intended *player experience*, *narrative tone*, and *mechanical purpose* behind every feature.

Agents:  
- **WorldBuilder** – design, balance  
- **XMLForge** – XML defs  
- **CSharpSmith** – C# systems, comps, Harmony  
- **ArtEngine** – sprites  
- **TestRunner** – gameplay tests  
- **Documentation** – docs  
- **ReleaseBot** – packaging  

---

# 0. GLOBAL SYSTEMS

---

## 0.1 Patch Dubs Hygiene Thirst Need  
**Goal:** Replace custom hydration need with a patched version of Dubs’ Thirst need.

### User Stories  
- **As a player**, I want colonists’ survival to revolve around water scarcity, so that Arrakis feels truly unforgiving.  
- **As a player**, I want stillsuits and stilltents to *meaningfully* slow thirst, so that desert survival tools feel essential.  
- **As a player**, I want drinking jobs to use Literjons and Catchbasins, not Dubs’ water network, so that hydration is fully Arrakis-driven.  

### Tasks  
**Agent:** CSharpSmith  
**Outputs:**  
- Harmony patches to Thirst need tick-rate (biome-aware).  
- Patch drinking job to prefer Arrakis containers and basins.  
- Modify pawn dehydration thresholds to better match extreme desert gameplay.

---

## 0.2 Patch Out All Dubs Hygiene Buildings Except Plumbing  
**Goal:** Disable/remove all water-creating/using DH buildings except toilets/pipes.

### User Stories  
- **As a player**, I want all DH water sources removed so water scarcity stays harsh.  
- **As a player**, I want toilets/plumbing retained but consuming no water when wearing stillsuits, so that desert living feels culturally accurate.  
- **As a colony designer**, I want my sanitation to remain intact without trivialising water scarcity.  

### Tasks  
**Agent:** CSharpSmith / XMLForge  
**Outputs:**  
- Patch DH build menu to hide/remove wells, tanks, pumps, sinks, etc.  
- Maintain toilets and sewage pipes.  
- Add “No water consumption while wearing stillsuit” patch.

---

## 0.3 Vibration Heat System  
**Goal:** A global system tracking vibration from footsteps, mining, machinery, etc., triggering sandworm emergence.

### User Stories  
- **As a player**, I want worm attacks to be predictable and fair, based on vibration I create.  
- **As a base builder**, I want ways to reduce vibration near my settlement.  
- **As a storyteller**, I want worm eruptions to feel dramatic and earned.  

### Tasks  
**Agent:** CSharpSmith  
**Outputs:**  
- `MapComponent_Vibration.cs`  
- API for vibration-producing actions and buildings.  
- Hooks for Thumpers and worm AI.

---

# 1. MATERIALS & RESOURCES

---

## 1.1 Spice Fibre  
**Goal:** New textile harvested from spice plants. Required for stillcloth.

### User Stories  
- **As a harvester**, I want to gather spice fibre as a rare material, so that stillcloth production has meaningful prerequisites.  
- **As a crafter**, I want spice fibre to feel unique, so that Arrakis textiles are distinct from vanilla cloth.  

### Tasks  
**Agent:** XMLForge / ArtEngine  
**Outputs:**  
- `ThingDef_SpiceFibre.xml`  
- Icon / textile pattern  
- Harvest product on spice plants

---

## 1.2 Stillcloth (Updated Recipe)  
**Goal:** Stillcloth requires Spice Fibre + a secondary textile.

### User Stories  
- **As a tailor**, I want stillcloth to require spice fibre, so advanced desert gear feels earned.  
- **As a progression player**, I want the fibre requirement to gate stillsuits, so early-game is challenging.  

### Tasks  
**Agent:** XMLForge  
**Outputs:**  
- Update `Stillcloth.xml`  
- Recipe: spice fibre + cloth/leather/devilstrand  
- Appropriate heat insulation stats  

---

## 1.3 Literjons  
**Goal:** Implement 1-litre water containers consumed by the thirst job.

### User Stories  
- **As a player**, I want colonists to drink directly from rationed vessels, so water management feels tactile.  
- **As a planner**, I want Literjons to be tradable and stackable, so they integrate into all routes of play.  

### Tasks  
**Agent:** XMLForge / CSharpSmith  
**Outputs:**  
- `ThingDef_Literjon.xml`  
- `Comp_HydrationDrink`  
- JobDriver support

---

## 1.4 Spice Melange  
**Goal:** Core spice item with buffs, addiction, eyes, lifespan, etc.

### User Stories  
- **As a player**, I want spice to feel mythic and powerful.  
- **As a psycaster**, I want spice to enhance psyfocus and psychic sensitivity like Dune.  
- **As a long-term user**, I want visible blue eyes and lifespan extension.  

### Tasks  
**Agent:** XMLForge / CSharpSmith  
**Outputs:**  
- `ThingDef_SpiceMelange.xml`  
- Hediffs: SpiceHigh, SpiceAddiction, SpiceAdaptation  
- Eye tint comp  
- PsyFocus & consciousness buffs  

---

## 1.5 Spice → Neutroamine Synthesis  
**Goal:** Arrakis chemical bench converts spice to neutroamine.

### User Stories  
- **As a chemist**, I want spice to refine into neutroamine, so I’m not trader-dependent.  
- **As a strategist**, I want this process to be slow and expensive, so spice remains valuable.  

### Tasks  
**Agent:** XMLForge / CSharpSmith  
**Outputs:**  
- Refinery building  
- Conversion recipe  
- WorkGiver + JobDriver  
- Balancing passes  

---

## 1.6 Spice as Universal High-Value Currency  
**Goal:** Spice accepted by all traders, worth enormous amounts.

### User Stories  
- **As a trader**, I want spice to be exchangeable for nearly anything, so the spice economy feels lore-accurate.  
- **As a min-maxer**, I want spice to unlock powerful trade strategies.  
- **As a storyteller**, I want factions to value spice highly.  

### Tasks  
**Agent:** CSharpSmith / XMLForge  
**Outputs:**  
- Patch `Tradeability` and `MarketValue`  
- Add acceptance patches for all trader classes  
- Optional: faction goodwill bonuses for spice trade  

---

# 2. APPAREL & SHELTER

---

## 2.1 Stillsuit  
**Goal:** Apparel with 80% thirst reduction, waste recycling, heat insulation.

### User Stories  
- **As a desert explorer**, I want stillsuits to dramatically slow thirst, so travel is possible.  
- **As a hygiene-aware colonist**, I want toilets to be unnecessary while wearing them, so immersion feels accurate.  

### Tasks  
**Agent:** XMLForge / CSharpSmith / ArtEngine  
**Outputs:**  
- `Arrakis_Stillsuit.xml`  
- `Comp_Stillsuit.cs`  
- Art assets  
- Patches to Dubs toilet behaviour  

---

## 2.2 Stilltent  
**Goal:** Portable bed that reduces thirst while sleeping.

### User Stories  
- **As a traveller**, I want to sleep safely in the desert with reduced thirst.  
- **As a tactician**, I want stilltents for expeditions far from base.  

### Tasks  
**Agent:** XMLForge / CSharpSmith / ArtEngine  
**Outputs:**  
- Building def  
- `Comp_StillShelter.cs`  
- Bed behaviour integration  

---

# 3. WATER INFRASTRUCTURE

---

## 3.1 Catchbasin  
**Goal:** Lossless underground water reservoir.

### User Stories  
- **As a player**, I want to store water without losing any to evaporation.  
- **As a planner**, I want multi-basin networks feeding windtraps and deathstills.  

### Tasks  
**Agent:** XMLForge / CSharpSmith  
**Outputs:**  
- `Arrakis_Catchbasin.xml`  
- `Comp_WaterTank.cs`  

---

## 3.2 Windtrap  
**Goal:** Gathers moisture (especially at night) into catchbasins.

### User Stories  
- **As a colonist**, I want windtraps to feel iconic and lore-accurate.  
- **As a survivalist**, I want windtraps to produce only modest water, preserving scarcity.  

### Tasks  
**Agent:** XMLForge / CSharpSmith / ArtEngine  
**Outputs:**  
- `Arrakis_Windtrap.xml`  
- `Comp_Windtrap.cs`  
- Night multiplier + basin linkage  

---

## 3.3 Deathstill  
**Goal:** Convert corpses into water.

### User Stories  
- **As a Fremen role-player**, I want to reclaim a person’s water for the tribe.  
- **As an ethicist**, I want this to be a cultural or ideological choice.  

### Tasks  
**Agent:** XMLForge / CSharpSmith / ArtEngine  
**Outputs:**  
- `Arrakis_Deathstill.xml`  
- `Comp_Deathstill.cs`  
- Corpse → water job logic  

---

# 4. IDEOLOGY

---

## 4.1 Deathstill Funeral  
**Goal:** Ritual centred around the deathstill.

### User Stories  
- **As a Fremen faithful**, I want funerals that honour tradition.  
- **As a storyteller**, I want the ritual to give unique mood buffs.  

### Tasks  
**Agent:** XMLForge  
**Outputs:**  
- RitualPattern  
- Precept  
- ThoughtDefs  

---

# 5. WEAPONS

---

## 5.1 Maula Pistol  
**User Stories:**  
- Stealthy poison weapon for assassinations.  

**Tasks:**  
- Weapon + dart defs  
- Poison hediff  
- Art  

---

## 5.2 Maker Hooks  
**User Stories:**  
- Essential for worm-riding fantasy.  

**Tasks:**  
- Tool def  
- `Comp_MakerHook`  
- Integration with worm AI  

---

## 5.3 Thumper  
**User Stories:**  
- Used to summon and control worms.  

**Tasks:**  
- Building def  
- `Comp_Thumper`  
- Vibration integration  

---

## 5.4 Crysknife  
**User Stories:**  
- Sacred melee blade made from worm teeth.  

**Tasks:**  
- Weapon def  
- Art  

---

## 5.5 Las Weapons  
**User Stories:**  
- Mid-future energy weapons fitting RimWorld.  

**Tasks:**  
- Laspistol, Lasrifle, Lascannon  
- Projectiles  
- Art  

---

# 6. SPICE ECOSYSTEM

---

## 6.1 Spice Plant & Bloom Incident  
**User Stories:**  
- Rare, hazardous, rewarding blooms.  
- Worm-attracting resource nodes.  

**Tasks:**  
- Plant def  
- Spice yield  
- Bloom incident  
- Textures  

---

# 7. SANDWORM SYSTEM

---

## 7.1 Sandworm Pawn & AI  
**User Stories:**  
- Worm eruptions triggered by vibration.  
- Fearsome, iconic megafauna.  

**Tasks:**  
- PawnKindDef  
- Worm AI  
- Burrowing movement  
- Emergence logic  

---

## 7.2 Worm-Riding  
**User Stories:**  
- Achieve full Fremen fantasy.  
- Dangerous, high-reward mechanic.  

**Tasks:**  
- Mounting JobDriver  
- Saddle/anchor point logic  
- Control behaviour  

---

# 8. ARRAKIS BIOME

### User Stories  
- Live in a harsh, deadly, waterless environment.  
- A true tribute to Dune.  

### Tasks  
**Agent:** XMLForge  
**Outputs:**  
- BiomeDef  
- Limited flora/fauna  
- Heat & sandstorm settings  

---

# 9. ART TASKS

### User Stories  
- All assets should look consistent with RimWorld.  

### Tasks  
**Agent:** ArtEngine  
**Outputs:**  
- All item, apparel, building sprites  
- Worm silhouettes + crest  
- Terrain textures  

---

# 10. TESTING

**User Stories:**  
- Everything operates stably.  
- All systems respect scarcity, danger, and the Dune theme.  

### Tasks  
**Agent:** TestRunner  
- Thirst loop testing  
- Stillsuit effectiveness  
- Deathstill correctness  
- Spice bloom behaviour  
- Worm behaviour tests  

---

# 11. DOCUMENTATION & RELEASE

### Tasks  
**Agent:** Documentation / ReleaseBot  
- Update README.md  
- Update AGENTS.md  
- Generate changelogs  
- Package releases  

---

# Completion Criteria

- Thirst system patched and integrated  
- All Dubs water sources removed  
- Arrakis biome playable  
- Spice loop fully implemented  
- Worms behave correctly  
- Stillsuits fully override thirst & waste  
- Windtraps, deathstills, catchbasins operational  
- All assets load without errors  
- Documentation complete  
