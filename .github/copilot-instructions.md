# GitHub Copilot Instructions for RimWorld Mod: Combat Shields

## Mod Overview and Purpose

The Combat Shields mod for RimWorld enhances gameplay by introducing personal shield mechanics tailored for combat scenarios. This mod aims to provide an immersive experience by equipping pawns with various shield types that protect against incoming projectiles and melee attacks, offering new tactical defensive strategies for players.

## Key Features and Systems

- **Shield Apparel**: Introduces shield items that can be equipped in the apparel slot. These shields absorb damage and can be customized with different colors.
- **Dynamic Shield Status**: Displays a gizmo on shielded pawns, providing real-time status updates on the shield’s integrity.
- **Color Customization**: Players can change the color of shields to suit their preferences, offering visual diversity and personalization.
- **Integration with Weapons**: The mod manages interactions between shields and ranged/melee weapons, enhancing strategic adaptability.
- **Settings Management**: Offers players options to customize shield behavior directly from the mod settings interface.

## Coding Patterns and Conventions

- **C# Naming Conventions**:
  - Class names use PascalCase (e.g., `Apparel_Shield`, `ColorableShield`).
  - Methods use camelCase (e.g., `updateWeapon`, `Reset`, `DrawIcon`).
  - Internal logic structs and classes, such as within a method, are encapsulated for clarity.

- **Class Inheritance**:
  - `Apparel_Shield` inherits from `Apparel`, adding shield-specific functionality.
  - `ColorableShield` extends `Apparel_Shield`, providing additional features for color customization.

- **Structure of Methods**:
  - Methods are defined for specific roles, such as updating weapon visuals or resetting configuration in `CombatShieldsMod`.

## XML Integration

The mod does not currently outline specific uses of XML; however, typical integration in RimWorld mods involves XML files defining new items, pawn traits, and textures. Developers using Copilot should consider:

- Defining shield items in XML for item properties and stats.
- Using XML patching to modify existing item behaviors or pawn interactions.

## Harmony Patching

- The `Harmonypatches` class serves as the central hub for all Harmony patches. This class is crucial for modifying base game behavior non-destructively:
  - Ensure `prefix` and `postfix` are used to enhance or replace game methods.
  - Copilot-generated patches should align with the class’s existing method decoration patterns.

## Suggestions for Copilot

- **Code Completion**: Copilot can assist in method stubs for new functionalities like additional shield effects or interactions.
- **Harmony Support**: Generate informative patches by suggesting `prefix`, `postfix`, and including detailed comments for clarity.
- **XML Template Suggestions**: Recommend XML structure for easily adding new shield types or modifications.
- **Debugging Assistance**: Highlight potential pitfalls in shield-apparel interactions or Harmony patch conflicts.
- **Integrate Mod Helpers**: Suggest code for using modding libraries or helpers available to RimWorld C# developers.

Following these guidelines will help maintain consistency and enhance the mod’s functionality, contributing to the development of a richer RimWorld experience.
