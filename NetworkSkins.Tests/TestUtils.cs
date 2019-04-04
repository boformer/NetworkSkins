using System;

namespace NetworkSkins.Tests
{
    public static class TestUtils
    {
        public static void LogTest(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        public static void AssertTrue(bool test)
        {
            if (!test)
            {
                throw new Exception("Test Failed");
            }
        }
    }
}
