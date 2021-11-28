using NUnit.Framework;
using Packing.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Packing.Tests.Shared
{
    [TestFixture]
    class ResultTests
    {
        enum Err { A, B }

        Result<int, Err> sut;

        [Test]
        public void ResultShouldReturnValueIfSupplied()
        {
            sut = new Result<int, Err>(1);

            Assert.AreEqual(sut.Get, 1);
            Assert.IsTrue(sut);
            Assert.IsTrue(sut.IsOk);
            Assert.IsFalse(sut.IsErr);
            Assert.Throws<InvalidOperationException>(() => { _ = sut.Err; });
        }

        [Test]
        public void ResultShouldReturnErrIfSupplied()
        {
            sut = new Result<int, Err>(Err.A);

            Assert.AreEqual(sut.Err, Err.A);
            Assert.IsFalse(sut);
            Assert.IsTrue(sut.IsErr);
            Assert.IsFalse(sut.IsOk);
            Assert.Throws<InvalidOperationException>(() => { _ = sut.Get; });
        }

        [Test]
        public void ResultShouldPassItsOkToNextOne()
        {
            const int value = 5;
            sut = new Result<int, Err>(value)
                .Then(b =>
                {
                    Assert.AreEqual(b, value);
                    return new Result<int, Err>(value);
                });

            Assert.AreEqual(sut.Get, value);
        }

        [Test]
        public void ResultShouldRefuseToExecuteThenIfErr()
        {
            int a = 3;
            Func<int, Result<int, Err>> neverCalled = _ =>
            {
                a++;
                throw new Exception();
            };
            sut = new Result<int, Err>(1)
                .Then(b => new Result<int, Err>(b + 1))
                .Then(b => new Result<int, Err>(Err.A))
                .Then(neverCalled);

            Assert.AreEqual(sut.Err, Err.A);
            Assert.AreEqual(3, a);
        }
    }
}
