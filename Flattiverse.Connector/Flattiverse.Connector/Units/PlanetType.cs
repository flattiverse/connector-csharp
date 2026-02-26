namespace Flattiverse.Connector.Units;

/// <summary>
/// Describes the visual archetype of a planet.
/// </summary>
public enum PlanetType : byte
{
    /// <summary>
    /// A heavy, dark world that looks forged rather than formed: broad iron-black plains cut by pale seams and circular scars.
    /// In certain light it gleams like polished armor, then turns matte again as it rolls under its star.
    /// It has the confident, unromantic beauty of an anvil-magnificent, intimidating, and absolutely not a place for picnics.
    /// </summary>
    /// <remarks>
    /// Expected resources: Metal.
    /// </remarks>
    MetalRichForgeworld,

    /// <summary>
    /// A classic rocky sphere in muted grays and rusts, with crater chains and long fault lines like pencil marks across stone.
    /// From orbit it feels honest and quiet-no theatrics, just hard surfaces and sharp shadows.
    /// The kind of planet that makes you think "I could hike that ridge," and then remember the Flattiverse is strictly a sightseeing tour.
    /// </summary>
    /// <remarks>
    /// Expected resources: Silicon.
    /// </remarks>
    RockyFrontier,

    /// <summary>
    /// A sun-baked marble of gold and amber, wrapped in sweeping dune fields that draw fluid patterns across entire hemispheres.
    /// Its terminator line is dramatic: daylight blinding, night a deep bruise-purple, with dust veils that blur the edge.
    /// It looks inviting in postcards-endless deserts, endless horizons-right up until you remember you cannot set foot on any of it.
    /// </summary>
    DesertDuneWorld,

    /// <summary>
    /// A deep-blue globe with white spiral storms and occasional darker patches that hint at hidden currents and vast depth.
    /// Sunlight glitters off the surface in broken ribbons, making the whole planet look alive and restless.
    /// It is the universe's cruel joke: the most soothing view imaginable, and the one you are least able to touch-no beaches, no dives, only orbit.
    /// </summary>
    OceanWorld,

    /// <summary>
    /// A pearl-white planet with pale cyan shadows, scored by long cracks that lace the surface like shattered porcelain.
    /// In grazing light the fissures glow faintly, as if something warmer is sighing beneath the shell.
    /// It looks clean and peaceful from afar, but the beauty has teeth: stark, brittle, and absolute-an ice cathedral you can admire only through a viewport.
    /// </summary>
    /// <remarks>
    /// Expected resources: Hydrogen.
    /// </remarks>
    IceWorld,

    /// <summary>
    /// An emerald planet wrapped in thick cloud decks, where darker green continents press against bright coastal swirls.
    /// Lightning storms sketch fleeting white lines through the atmosphere, and the day side feels almost luminous with life.
    /// It is the kind of world that makes crews fall quiet for a moment-then laugh at the irony: "Nature is gorgeous. Too bad we are tourists forever."
    /// </summary>
    /// <remarks>
    /// Expected resources: Carbon.
    /// </remarks>
    JungleWorld,

    /// <summary>
    /// A world behind a stained curtain: sickly pastel clouds, bruised shadows, and drifting bands that never quite settle.
    /// The surface is only glimpsed in rare breaks-like seeing a city through frosted glass-then swallowed again by haze.
    /// It has a moody, cinematic look that photographers love and superstitious captains distrust, as if the planet itself prefers privacy and resents being watched.
    /// </summary>
    ToxicVeilWorld,

    /// <summary>
    /// A planet that wears its geology on the outside: glowing rifts, dark ash fields, and fresh scars that redraw coastlines between one calendar and the next.
    /// The night side is never truly dark; faint ember-lines trace fault networks like a living map.
    /// It is spectacular in the way a bonfire is spectacular-beautiful, loud, and entirely indifferent to your comfort, which is convenient because you only get to look.
    /// </summary>
    VolcanicShatterworld,

    /// <summary>
    /// A banded giant with creamy stripes, curling storms, and a sense of depth that makes it feel less like a planet and more like a moving sky.
    /// Its cloud tops shift in slow, deliberate choreography, while occasional dark ovals hint at storms older than institutions.
    /// It is the definition of untouchable grandeur-an endless atmosphere you can circle for years and still never "arrive," like orbiting a living painting.
    /// </summary>
    GasGiant,

    /// <summary>
    /// A cold, pale giant in soft blues and greens, with thin haze layers that turn sunrise into watercolor.
    /// Its storms look gentler than they are, drifting like lazy brushstrokes across a globe that seems half-asleep.
    /// A faint ring or two may ghost around it in the right angle, giving the impression of a planet wearing jewelry it forgot to mention-quiet elegance with enormous mass.
    /// </summary>
    IceGiant,

    /// <summary>
    /// A scarred world of geometric patches and straight-edged shadows where nature's curves have been interrupted by long-dead design.
    /// From orbit you catch hints of gridlines, circles too perfect, and occasional reflective plates that look like buried plazas.
    /// It feels haunted without being spooky-more like an abandoned station the size of a planet, still posing for portraits long after the inhabitants stopped caring.
    /// </summary>
    RuinWorld,

    /// <summary>
    /// A strange globe of mismatched brightness: regions that shimmer like glass beside plains that swallow light, as if two planets were stitched together.
    /// It never photographs the same way twice, and the eye keeps trying to "solve" its shape and failing.
    /// Crews give it nicknames-Mirror-Skin, Patchwork, The Prank-because humor is easier than admitting some worlds look like they were assembled from leftovers.
    /// </summary>
    AnomalyWorld,

    /// <summary>
    /// A lone, starless drifter: mostly darkness with thin frost arcs that catch faint starlight like chalk on black glass.
    /// Its day side is a rumor, its warmth hidden deep, and its silhouette feels heavier than it should.
    /// It invites conspiracies-secret yards, missing fleets, long sleeps-because anything so far from witnesses becomes a blank page for stories, and orbit is the only way to read it.
    /// </summary>
    RogueWorld,

    /// <summary>
    /// A bleached, airless sphere with almost no contrast-just faint tonal bands and an uncomfortable sense of emptiness.
    /// It looks like someone erased a planet with a soft brush, leaving only gentle shading and a few shallow scars.
    /// Crew jokes call it "the loading screen" because it feels unfinished, but the longer you stare, the more the subtle textures start to feel intentional.
    /// Beautiful in a minimalist way, and stubbornly uninterested in giving you anything back.
    /// </summary>
    BarrenChalkWorld,

    /// <summary>
    /// A planet wrapped in an impossibly smooth, reflective cloud layer that catches the star like a pearl and throws it back at you.
    /// There are hints of color underneath-pale violets, faint greens-but never enough to confirm what is really down there.
    /// It is the kind of world that inspires bold theories and bad poetry, because ambiguity is its whole aesthetic.
    /// Everyone wants to "just take a closer look," and everyone remembers you cannot.
    /// </summary>
    MirrorCloudWorld,

    /// <summary>
    /// A planet with bright auroral curtains pinned around its poles like stage drapes, visible even in washed-out daylight.
    /// The colors look impossibly saturated-greens, violets, occasional razor-thin reds-making it feel more like a festival than a world.
    /// Of course, it is all happening far below, unreachable, while you and your crew orbit like disappointed VIPs who bought the wrong tickets.
    /// The planet does not care; it keeps performing anyway, flawlessly.
    /// </summary>
    AuroraHaloWorld,

    /// <summary>
    /// A canyon planet where the surface seems carved into enormous trenches and branching gashes that cast ink-dark shadows.
    /// From orbit, the network looks like a cracked ceramic bowl or a map of dry riverbeds drawn by a giant hand.
    /// The day side is all sharp contrast and drama; the night side is a quiet void with occasional glittering edges.
    /// Everyone asks what it would sound like down there, and everyone answers the same way: "We will never know."
    /// </summary>
    CanyonLabyrinthWorld,

    /// <summary>
    /// An obsidian-dark planet with a thin, broken ring system that looks like scattered glass around a black coin.
    /// The rings catch starlight in brief flashes, then vanish, making the whole scene feel like a magic trick that keeps resetting.
    /// Surface detail is subtle-faint ridges, dim craters-because the planet seems determined to absorb attention rather than reflect it.
    /// It is a perfect visual metaphor for the Flattiverse: spectacular composition, no access, maximum temptation.
    /// </summary>
    RingedObsidianWorld
}
