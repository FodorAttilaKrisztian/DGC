using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools; // Ensure this is included for LogAssert
using System.Collections.Generic;
using System.Linq;

public class SerializableDictionaryTests
{
    [Test]
    public void OnBeforeSerialize_SerializesDictionaryCorrectly()
    {
        var dict = new SerializableDictionary<int, string>();
        dict.Add(1, "One");
        dict.Add(2, "Two");

        dict.OnBeforeSerialize();

        List<int> keys = dict.Keys.ToList();
        List<string> values = dict.Values.ToList();

        Assert.AreEqual(2, keys.Count, "Keys list should contain 2 items.");
        Assert.AreEqual(2, values.Count, "Values list should contain 2 items.");
        Assert.AreEqual(1, keys[0], "First key should be 1.");
        Assert.AreEqual("One", values[0], "First value should be 'One'.");
        Assert.AreEqual(2, keys[1], "Second key should be 2.");
        Assert.AreEqual("Two", values[1], "Second value should be 'Two'.");
    }

    [Test]
    public void OnAfterDeserialize_DeserializesDictionaryCorrectly()
    {
        var dict = new SerializableDictionary<int, string>();

        dict.GetType().GetField("keys", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(dict, new List<int> { 1, 2 });
        dict.GetType().GetField("values", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(dict, new List<string> { "One", "Two" });

        dict.OnAfterDeserialize();

        Assert.AreEqual(2, dict.Count, "Dictionary should contain 2 key-value pairs.");
        Assert.AreEqual("One", dict[1], "Dictionary should contain key 1 with value 'One'.");
        Assert.AreEqual("Two", dict[2], "Dictionary should contain key 2 with value 'Two'.");
    }

    [Test]
    public void OnAfterDeserialize_ThrowsErrorWhenKeysAndValuesDoNotMatch()
    {
        var dict = new SerializableDictionary<int, string>();

        dict.GetType().GetField("keys", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(dict, new List<int> { 1, 2 });
        dict.GetType().GetField("values", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(dict, new List<string> { "One" });

        LogAssert.Expect(LogType.Error, "Tried to deserialize a SerializableDictionary, but the amount of keys (2) does not match the number of values (1) which indicates that something went wrong.");

        dict.OnAfterDeserialize();
    }
}