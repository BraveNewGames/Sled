using NUnit.Framework;
using UnityEngine;

namespace SledDogs.Tests
{
    public class WorkflowSmokeTests
    {
        [Test]
        public void UnityEditorIsAvailableToEditModeTests()
        {
            Assert.That(Application.unityVersion, Is.Not.Empty);
        }
    }
}
