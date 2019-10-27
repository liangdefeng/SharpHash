﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpHash.Base;

namespace SharpHash.Hash32.Tests
{
    [TestClass]
    public class DJBTests : HashAdapter1BaseTests
    {
        static DJBTests()
        {
            hash = HashFactory.Hash32.CreateDJB();

            ExpectedHashOfEmptyData = "00001505";
            ExpectedHashOfDefaultData = "C4635F48";
            ExpectedHashOfOnetoNine = "35CDBB82";
            ExpectedHashOfabcde = "0F11B894";
        }
    }
}