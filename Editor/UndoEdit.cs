using UnityEditor;
using UnityEngine;

namespace TsiYuki.Core.Editor
{
    /// <summary>
    /// Undo-correct component editing: record before mutating, then mark dirty
    /// and register prefab-instance overrides after, so an edit made through a
    /// tool window survives a save and shows up as an override on a prefab.
    /// </summary>
    public static class UndoEdit
    {
        public static void Begin(Object target, string action) => Undo.RecordObject(target, action);

        public static void End(Object target)
        {
            EditorUtility.SetDirty(target);
            if (target is Component component)
                PrefabUtility.RecordPrefabInstancePropertyModifications(component);
        }
    }
}
