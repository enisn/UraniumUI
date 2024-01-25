namespace UraniumUI.Material.Controls;

public enum TabViewCachingStrategy
{
    /// <summary>
    /// The view is removed from the visual tree when a tab is deselected. But instance is kept in memory. And same instance is used when tab is selected again.
    /// </summary>
    CacheOnCodeBehind,

    /// <summary>
    /// The view is kept in the visual tree and visibility is toggled when a tab is selected.
    /// </summary>
    CacheOnLayout,

    /// <summary>
    /// No caching. View is removed from the visual tree and a new instance is created when a tab is selected.
    /// </summary>
    RecreateAlways
}