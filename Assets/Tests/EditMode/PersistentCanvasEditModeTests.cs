using NUnit.Framework;
using UnityEngine;
using System.Reflection;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[TestFixture]
public class PersistentCanvasEditModeTests
{
    private GameObject canvasObject;
    private PersistentCanvas persistentCanvas;
    private Canvas canvas;

    [SetUp]
    public void SetUp()
    {
        canvasObject = new GameObject("PersistentCanvas");
        canvas = canvasObject.AddComponent<Canvas>();
        persistentCanvas = canvasObject.AddComponent<PersistentCanvas>();


        PersistentCanvas.SetSingleton(persistentCanvas);

        if (!Application.isPlaying)
        {
            persistentCanvas.enabled = false;
        }
    }


    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(canvasObject);
    }

    [Test]
    public void Singleton_ShouldOnlyHaveOneInstance()
    {
        Assert.NotNull(PersistentCanvas.instance, "PersistentCanvas instance should not be null.");
        Assert.AreEqual(persistentCanvas, PersistentCanvas.instance, "There should only be one instance of PersistentCanvas.");
    }

    [Test]
    public void Canvas_ShouldBeAttached()
    {
        Assert.NotNull(canvas, "PersistentCanvas should have a Canvas component attached.");
    }

    [Test]
    public void Canvas_ShouldNotHaveChildrenInitially()
    {
        Assert.AreEqual(0, persistentCanvas.transform.childCount, "PersistentCanvas should not have any child objects at the start.");
    }
}