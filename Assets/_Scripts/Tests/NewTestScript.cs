using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class NewTestScript {

    [Test]
    public void MergeArrayFunctionTest() {
        // Use the Assert class to test conditions.
        int[] a = { 1, 2, 3 };
        int[] b = { 3, 6, 8 };
        int[] c = MergeArray(a, b);

        foreach (int i in c)
        {
            Debug.Log(i);
        }
    }

    //// A UnityTest behaves like a coroutine in PlayMode
    //// and allows you to yield null to skip a frame in EditMode
    //[UnityTest]
    //public IEnumerator NewTestScriptWithEnumeratorPasses() {
    //    // Use the Assert class to test conditions.
    //    // yield to skip a frame
    //    yield return null;
    //}

    public static T[] MergeArray<T>(T[] a1, T[] a2)
    {
        T[] dst = new T[a1.Length + a2.Length];
        System.Array.Copy(a1, 0, dst, 0, a1.Length);
        System.Array.Copy(a2, 0, dst, a1.Length, a2.Length);
        return dst;
    }
}
