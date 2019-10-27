﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpHash.Base;

namespace SharpHash.Hash32.Tests
{
    [TestClass]
    public class RSTests : HashAdapter1BaseTests
    {
        static RSTests()
        {
            hash = HashFactory.Hash32.CreateRS();

            ExpectedHashOfEmptyData = "00000000";
            ExpectedHashOfDefaultData = "9EF98E63";
            ExpectedHashOfOnetoNine = "704952E9";
            ExpectedHashOfabcde = "A4A13F5D";
        }
    }
}