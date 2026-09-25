using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace TsiYuki.Core.Editor.Tests
{
    // The test object is created and removed through Undo: destroying it directly would let the
    // test runner's own undo cleanup bring it back into the open scene.
    public class UndoEditTests
    {
        GameObject _go;
        BoxCollider _collider;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("UndoEditTests", typeof(BoxCollider));
            Undo.RegisterCreatedObjectUndo(_go, "Create UndoEditTests");
            _collider = _go.GetComponent<BoxCollider>();
            Undo.IncrementCurrentGroup();
        }

        [TearDown]
        public void TearDown() => Undo.DestroyObjectImmediate(_go);

        [Test]
        public void AnEditBetweenBeginAndEndIsUndone()
        {
            var before = _collider.size;
            UndoEdit.Begin(_collider, "Resize");
            _collider.size = before * 2;
            UndoEdit.End(_collider);

            Undo.PerformUndo();
            Assert.That(_collider.size, Is.EqualTo(before));
        }

        [Test]
        public void EndMarksTheTargetDirty()
        {
            UndoEdit.Begin(_collider, "Resize");
            _collider.size *= 2;
            UndoEdit.End(_collider);
            Assert.IsTrue(EditorUtility.IsDirty(_collider));
        }
    }
}
