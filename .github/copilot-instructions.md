# GitHub Copilot Instructions for RimWorld Mod: Combat Shields (Continued)

Welcome to the **Combat Shields (Continued)** mod development guide. This document provides an overview of the mod, outlines key features, coding patterns, harmony patching, and integration techniques. It also includes suggestions for using GitHub Copilot to facilitate mod development.

## Mod Overview and Purpose

The **Combat Shields (Continued)** mod is an update and continuation of N7Huntsman's original mod. It introduces various types of shields from different historical eras, designed for specific combat roles and adaptable with certain melee and ranged weapons. The purpose of the mod is to expand the tactical options available to players in RimWorld, allowing for greater specialization of colonists based on combat roles and tasks around the colony.

## Key Features and Systems

- **Diverse Shield Types**: Introduces shields like the Nguni Shield, Wicker Shield, Aspis, Scutum, Norse Shield, Kite Shield, Pavise Shield, Buckler, Riot Shield, Assault Shield, Tactical Shield, Ballistic Shield, and Carapace Shield. Each with distinct recipes, protection levels, and historical origins.
  
- **Mod Settings**: Added configurations to determine which ranged weapons are compatible with shields.

- **Role Specialization**: Shields designed for different combat scenarios, providing players with strategic depth in equipping their colonists for specific roles and tasks.

## Coding Patterns and Conventions

- **Object-Oriented Design**: The codebase is organized using clear object-oriented principles. For instance, classes like `Apparel_Shield` and `ColorableShield` encapsulate properties and behaviors specific to shields.

- **Modular Code Structure**: Separation of concerns is maintained in the design, with distinct classes handling settings (`CombatShieldsSettings`), harmony patches (`Harmonypatches`), and shield logic (`Apparel_Shield`).

- **Consistent Naming Conventions**: Class names and methods follow PascalCase, while fields and parameters utilize camelCase, maintaining clarity and consistency throughout the codebase.

## XML Integration

- **Definition Management**: The mod uses XML files for defining apparel layers (`AdditionalLayersShield.xml`), shield categories (`ShieldThingCategoryDefs.xml`), pawn kinds (`PawnKinds_Shields.xml`), and base shield properties (`Apparel_ShieldBase.xml`).

- **Modularity and Extensibility**: XML files are structured to allow easy addition and modification of shield types, using `ThingDef` and `ApparelLayerDef` elements to declare shield properties and layering.

## Harmony Patching

- **Patch Techniques**: Harmony is employed to add or modify game behavior without altering the original game files. The `Harmonypatches` class contains methods for checking shield states and interactions (`getPawnShield`, `IsShield`, `PawnHasShieldEquipped`).

- **Non-Destructive Enhancements**: Patches ensure the mod's changes are applied safely, preserving functionality and compatibility with other mods.

## Suggestions for Copilot

- **Code Completion**: Utilize Copilot's code completion capabilities to streamline writing new class or method implementations, particularly when extending shield functionalities or settings.

- **Recommendation Insights**: Leverage Copilot's suggestions for XML structuring when defining new shield types, ensuring adherence to existing patterns and improving readability.

- **Debugging Assistance**: Employ Copilot for resolving common coding issues or identifying potential logic errors, especially in complex harmony patches or multi-layered class interactions.

- **Automated Documentation**: Use Copilot to generate inline documentation for methods and classes, aiding in maintaining clear and informative code documentation.

This guide aims to support your mod development efforts by providing structured guidelines and leveraging Copilot for efficient coding practices and integration.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

