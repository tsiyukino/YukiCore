using nadena.dev.ndmf;

[assembly: ExportsPlugin(typeof(TsiYuki.Core.Editor.YukiCorePlugin))]

namespace TsiYuki.Core.Editor
{
    /// <summary>
    /// Settles the menu placements every TsiYuki tool asked for.
    ///
    /// Each tool's pass declares BeforePlugin("moe.tsiyuki.core"), so by the time
    /// this runs they have all generated their menus and a request can be
    /// pointed at any of them regardless of which ran first. It still lands
    /// before Modular Avatar, which is what installs the result.
    /// </summary>
    public class YukiCorePlugin : Plugin<YukiCorePlugin>
    {
        public override string QualifiedName => "moe.tsiyuki.core";
        public override string DisplayName => "TsiYuki Core";

        protected override void Configure()
        {
            InPhase(BuildPhase.Generating)
                .BeforePlugin("nadena.dev.modular-avatar")
                .Run("Place TsiYuki menus", _ => YukiMenuRegistry.Resolve());
        }
    }
}
