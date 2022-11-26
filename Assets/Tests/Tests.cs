using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Riptide;

public class Tests
{
    GameObject managerObject;
    GameObject userInterface;//interface is used in attack script on player prefab, so this is here to not mae it error out
    [SetUp]
    public void SetUp()
    {
        managerObject = GameObject.Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Managers.prefab", typeof(GameObject)));
        userInterface = GameObject.Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/UI/Player UI.prefab", typeof(GameObject)));
    }
    [TearDown]
    public void TearDown()
    {
        GameObject.Destroy(managerObject);
        GameObject.Destroy(userInterface);
    }
    [Test]
    public void PlayerDamage()
    {
        //setup server
        NetworkManager.Singleton.StartServer();
        Player.Spawn(0, "Test", "#000000");
        //hold onto info on damage
        float ExpectedFinalHealth = Player.listOfPlayers[0].health - 1;
        //test
        Message damageMessage = Message.Create();
        damageMessage.AddUShort(0);
        damageMessage.AddUShort(0);
        Player.GetDamage(damageMessage);
        //assert
        Assert.AreEqual(ExpectedFinalHealth, Player.listOfPlayers[0].health);
        //close the server
        NetworkManager.Singleton.CloseServer();
    }
    [Test]
    public void ServerHosting()
    {
        //Start Server
        NetworkManager.Singleton.StartServer();
        //Check if Server is active
        Assert.IsTrue(NetworkManager.IsHost && NetworkManager.Singleton.Server.IsRunning);
        //stop Server
        NetworkManager.Singleton.CloseServer();
    }
}
