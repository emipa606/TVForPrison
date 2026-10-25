# GitHub Copilot Instructions for TVForPrison (Continued)

## Mod Overview and Purpose
TVForPrison is a mod for the game RimWorld. It is an update of the original TVForPrison mod created by pelador. The mod adds functionality to vanilla TVs and TVs from other mods (such as Vanilla Furniture Expanded and WallStuff) to allow them to be used as propaganda tools for prisoners.

The main purpose of the mod is to reduce the resistance of prisoners and increase their mood over time when they are within a certain range of a vanilla TV. The effectiveness of the propaganda tool depends on various factors such as the prisoner's state, the type of TV used, and the distance from the TV.

## Key Features and Systems
- **Resistance Reduction**: Vanilla TVs and TVs from other mods can be used to reduce the resistance of prisoners.
- **Mood Increase**: Prisoners within range of a vanilla TV will experience a small flat mood increase.
- **Compatibility**: The mod is compatible with other modded TV versions when loaded.
- **Range and Effectiveness**: The range and relative effectiveness of the propaganda tool can be controlled.

## Coding Patterns and Conventions
The mod follows standard coding patterns and conventions for RimWorld modding. The main components of the mod are defined in C# files and XML files.

### C# Files
- `CompProperties_TVForPrison.cs`: Contains the properties for the TV component.
- `CompTVForPrison.cs`: Contains the logic for the TV component, including the ticking mechanism, giving TV thoughts, and checking if the component is functional or valid.

### XML Files
- `Patch_TV_Vanilla.xml`: Patches for vanilla TVs.
- `Patch_VFE.xml`: Patches for TVs from Vanilla Furniture Expanded.
- `Patch_VFESpacer.xml`: Patches for TVs from Vanilla Furniture Expanded - Spacer Module.
- `Patch_WallStuff.xml`: Patches for TVs from WallStuff.
- `Defs_TVThought.xml`: Definitions for TV thoughts.

## XML Integration
The mod uses XML files to define and patch various aspects of TVs. These XML files are located in the `Patches` directory and the `Source\TranslationTemplate\ThoughtDef` directory.

### Example XML Patch
xml
<!-- Patch_TV_Vanilla.xml -->
<Patch>
    <Operation Class="Harmony.PatchOperation.Add">
        <Target Class="Verse.ThingDefOf">
            <Field Name="TV">
                <Value Class="Verse.CompProperties_TVForPrison"/>
            </Field>
        </Target>
    </Operation>
</Patch>


## Harmony Patching
The mod uses Harmony patching to modify the behavior of vanilla TVs and TVs from other mods. Harmony is a popular library for patching code in RimWorld and other games.

### Example Harmony Patch
csharp
// CompTVForPrison.cs
public class CompTVForPrison : ThingComp
{
    public override void CompTick()
    {
        base.CompTick();
        // Custom logic for TV propaganda
    }

    public void giveTVThought(Pawn prisoner)
    {
        // Logic for giving TV thoughts to prisoners
    }

    public bool isFunctional()
    {
        // Logic to check if the TV is functional
    }

    public bool isValidCell()
    {
        // Logic to check if the cell is valid
    }

    public bool isValidPawn(Pawn prisoner)
    {
        // Logic to check if the pawn is valid
    }

    public CompProperties_TVForPrison Props
    {
        get { return (CompProperties_TVForPrison)props; }
    }

    public Thing TV
    {
        get { return parent; }
    }
}


## Suggestions for Copilot
Here are some suggestions for using GitHub Copilot with the TVForPrison mod:

1. **Creating New Components**: When adding support for new TV types from other mods, Copilot can be used to create new component classes similar to `CompTVForPrison`.
2. **Modifying Existing Code**: Copilot can help in modifying existing code to add new features or fix bugs.
3. **Generating XML Patches**: Copilot can generate XML patches for new TV types or to modify existing patches.
4. **Writing Unit Tests**: Copilot can help in writing unit tests for the mod to ensure its functionality.

### Example Copilot Usage
csharp
// Example of using Copilot to create a new component for a new TV type
public class CompNewTVType : ThingComp
{
    public override void CompTick()
    {
        base.CompTick();
        // Custom logic for new TV type propaganda
    }

    // Other methods and properties similar to CompTVForPrison
}


## Conclusion
TVForPrison is a mod that adds functionality to vanilla TVs and TVs from other mods to reduce prisoner resistance and increase mood. The mod follows standard coding patterns and conventions for RimWorld modding. GitHub Copilot can be a valuable tool for developers working on the mod, helping with tasks such as creating new components, modifying existing code, generating XML patches, and writing unit tests.

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

