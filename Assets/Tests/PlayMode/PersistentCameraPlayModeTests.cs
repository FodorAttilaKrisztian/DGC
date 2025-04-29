using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;

public class PersistentCameraPlayModeTests
{
    private GameObject cameraObject;
    private PersistentCamera persistentCamera;
    private CinemachineCamera cinemachineCamera;
    private GameObject playerObject;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        cameraObject = new GameObject("PersistentCamera");

        cinemachineCamera = cameraObject.AddComponent<CinemachineCamera>();
        persistentCamera = cameraObject.AddComponent<PersistentCamera>();

        playerObject = new GameObject("Player");
        var controller = playerObject.AddComponent<PlayerController>();
        PlayerController.instance = controller;

        yield return null;

        Assert.NotNull(PersistentCamera.instance, "PersistentCamera.instance should be assigned after Awake.");
        Assert.AreEqual(persistentCamera, PersistentCamera.instance, "Singleton instance should match the camera in the scene.");
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(cameraObject);
        Object.Destroy(playerObject);
        PlayerController.instance = null;

        yield return null;
    }

    [UnityTest]
    public IEnumerator PersistentCamera_ShouldNotBeDestroyed_AfterSceneLoad()
    {
        var testScene = SceneManager.CreateScene("TestScene");

        yield return null;

        SceneManager.SetActiveScene(testScene);
        yield return null;

        Assert.IsNotNull(PersistentCamera.instance, "PersistentCamera should persist across scene loads.");
    }

    [UnityTest]
    public IEnumerator Camera_ShouldFollowPlayer()
    {
        yield return null;

        Assert.AreEqual(playerObject.transform, cinemachineCamera.Follow, "Camera should follow the PlayerController's transform.");
    }

    [UnityTest]
    public IEnumerator Camera_ShouldNotFollow_WhenNoPlayer()
    {
        PlayerController.instance = null;

        persistentCamera.GetComponent<CinemachineCamera>().Follow = null;

        yield return null;

        Assert.IsNull(cinemachineCamera.Follow, "Camera should not follow anything if PlayerController is null.");
    }
}