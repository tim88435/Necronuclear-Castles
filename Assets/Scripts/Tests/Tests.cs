using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

public class Tests
{
    GameObject managerObject;
    [SetUp]
    public void SetUp()
    {
        managerObject = GameObject.Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Managers.prefab", typeof(GameObject)));
    }
    [TearDown]
    public void TearDown()
    {
        GameObject.Destroy(managerObject);
    }
    [Test]
    public void TestsSimplePasses()
    {
        Assert.IsNotNull(managerObject);
    }
    [UnityTest]
    public IEnumerator TestsWithEnumeratorPasses()
    {
        yield return null;
    }
}
