using System;

namespace NetworkSkins.Tests
{
    public static class Assert
    {
        public static void IsTrue(bool test)
        {
            if (!test)
            {
                throw new Exception("Test Failed");
            }
        }
    }
}