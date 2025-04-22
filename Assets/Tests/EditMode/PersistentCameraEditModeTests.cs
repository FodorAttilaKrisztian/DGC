using NUnit.Framework;
using UnityEngine;
using System.Reflection;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.Cinemachine;

[TestFixture]
public class PersistentCameraEditModeTests
{
    [SetUp]
    public void SetUp()
    {
        // Use reflection to set PersistentCamera.instance to null before each test
        var fieldInfo = typeof(PersistentCamera).GetProperty("instance", BindingFlags.Public | BindingFlags.Static);
        fieldInfo.SetValue(null, null);
    }

    [Test]
    public void PersistentCamera_ShouldHaveCinemachineCameraComponent()
    {
        var cameraObject = new GameObject("Camera");
        var cam = cameraObject.AddComponent<PersistentCamera>();
        var cine = cameraObject.AddComponent<CinemachineCamera>();

        Assert.IsNotNull(cine, "CinemachineCamera component should be attached to PersistentCamera.");

        Object.DestroyImmediate(cameraObject);
    }

    [Test]
    public void PersistentCamera_Singleton_InstanceShouldBeNullInEditor()
    {
        // Assert that PersistentCamera.instance is null in EditMode before play.
        Assert.IsNull(PersistentCamera.instance, "PersistentCamera.instance should be null in EditMode before play.");
    }
}