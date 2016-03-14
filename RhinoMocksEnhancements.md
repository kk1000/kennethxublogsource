# Introduction #

Some enhancements to RhinoMocks

# Details #

  * Create multi mock for AAA: Mockery.GenerateMultiMock
  * Create partial mock for AAA: Mockery.GeneratePartialMock, Mocker.GeneratePartialMultiMock
  * Ordered Expectations: http://kennethxu.blogspot.com/2009/06/rhinomocks-ordered-expectations.html
  * Expect calls to any one of methods and use operator to check "or" condition: mock.ActivityOf(x=>x.Foo()) | mock2.ActivityOf(x=>x.Bar()): http://kennethxu.blogspot.com/2009/07/introduce-powerful-aaa-syntax-for.html
  * Ability to use test framework's assert: Assert.IsTrue(mock.ActivityOf(x=>x.Foo()));