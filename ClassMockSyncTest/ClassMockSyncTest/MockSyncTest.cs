#region License

/*
 * Copyright (C) 2009 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System.Runtime.CompilerServices;
using System.Threading;
using Castle.DynamicProxy;
using Moq;
using NUnit.Framework;
using Rhino.Mocks;

namespace ClassMockSyncTest
{
    [TestFixture] public class MockSyncTest
    {
        [Test]
        public void RhinoMocksSyncTest()
        {
            MockRepository mockery = new MockRepository();
            var sync = mockery.PartialMock<SyncExample>();
            mockery.ReplayAll();
            VerfiySync(sync);
        }

        [Test]
        public void MoqSyncTest()
        {
            var syncMock = new Mock<SyncExample>();
            VerfiySync(syncMock.Object);
        }

        [Test] 
        public void DynamicProxyTest()
        {
            var sync = new ProxyGenerator().CreateClassProxy<SyncExample>();
            VerfiySync(sync);
        }

        [Test]
        public void RealSyncTest()
        {
            var sync = new SyncExample();
            VerfiySync(sync);
        }

        private static void VerfiySync(SyncExample sync)
        {
            Thread t = new Thread(
                () =>
                    {
                        Thread.Sleep(250);
                        sync.Signal(); // signal after 250ms
                    });
            t.Start();
            try
            {
                if (!sync.Wait(500))
                    Assert.Fail("SyncExample.Signal() should have been called by now but that didn't happen.");
            }
            finally
            {
                t.Join(1000);
                if (t.IsAlive) t.Abort();
            }
        }

        public class SyncExample
        {
            private bool _hasSignal;

            [MethodImpl(MethodImplOptions.Synchronized)]
            public virtual bool Wait(int timeout)
            {
                while (!_hasSignal)
                {
                    // If we don't use timeout, the test will hang for mock object.
                    if (!Monitor.Wait(this, timeout)) return false;
                }
                return true;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public virtual void Signal()
            {
                _hasSignal = true;
                Monitor.PulseAll(this);
            }
        }
    }
}