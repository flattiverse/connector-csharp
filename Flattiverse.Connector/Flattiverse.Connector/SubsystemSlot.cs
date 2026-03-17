namespace Flattiverse.Connector;

/// <summary>
/// Identifies the concrete subsystem slot within a controllable.
/// </summary>
public enum SubsystemSlot : byte
{
    /// <summary>
    /// Primary energy battery slot.
    /// </summary>
    EnergyBattery = 0x00,

    /// <summary>
    /// Primary ion battery slot.
    /// </summary>
    IonBattery = 0x01,

    /// <summary>
    /// Primary neutrino battery slot.
    /// </summary>
    NeutrinoBattery = 0x02,

    /// <summary>
    /// Primary energy-cell slot.
    /// </summary>
    EnergyCell = 0x10,

    /// <summary>
    /// Primary ion-cell slot.
    /// </summary>
    IonCell = 0x11,

    /// <summary>
    /// Primary neutrino-cell slot.
    /// </summary>
    NeutrinoCell = 0x12,

    /// <summary>
    /// Primary scanner slot.
    /// </summary>
    PrimaryScanner = 0x20,

    /// <summary>
    /// Secondary scanner slot.
    /// </summary>
    SecondaryScanner = 0x21,

    /// <summary>
    /// Tertiary scanner slot.
    /// </summary>
    TertiaryScanner = 0x22,

    /// <summary>
    /// Primary engine slot.
    /// </summary>
    PrimaryEngine = 0x30,

    /// <summary>
    /// Secondary engine slot.
    /// </summary>
    SecondaryEngine = 0x31,

    /// <summary>
    /// Tertiary engine slot.
    /// </summary>
    TertiaryEngine = 0x32,

    /// <summary>
    /// Front shot-launcher slot.
    /// </summary>
    FrontShotLauncher = 0x40,
}
