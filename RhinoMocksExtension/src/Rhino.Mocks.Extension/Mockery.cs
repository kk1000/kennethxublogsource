using System;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Extension
{
    public static class Mockery {
        public static T GeneratePartialMock<T>(params object[] argumentsForConstructor)
            where T : class
        {
            return MakeAaaMock(m => m.PartialMock<T>(argumentsForConstructor));
        }

        public static T GeneratePartialMultiMock<T>(params Type[] extraTypes)
            where T : class
        {
            return MakeAaaMock(m => m.PartialMultiMock<T>(extraTypes));
        }

        public static T GeneratePartialMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor)
            where T : class
        {
            return MakeAaaMock(m => m.PartialMultiMock<T>(extraTypes, argumentsForConstructor));
        }

        public static T GenerateMultiMock<T>(Type[] extraTypes, params object[] argumentForConstructor)
        {
            return MakeAaaMock(m => m.DynamicMultiMock<T>(extraTypes, argumentForConstructor));
        }

        public static T GenerateMultiMock<T>(params Type[] extraTypes)
        {
            return MakeAaaMock(m => m.DynamicMultiMock<T>(extraTypes, new object[0]));
        }

        public static T GenerateStrickMock<T>(params object[] argumentForConstructor)
        {
            return MakeAaaMock(m => m.StrictMock<T>(argumentForConstructor));
        }

        public static T GenerateStricMultikMock<T>(Type[] extraTypes, params object[] argumentForConstructor)
        {
            return MakeAaaMock(m => m.StrictMultiMock<T>(extraTypes, argumentForConstructor));
        }

        private static T MakeAaaMock<T>(Converter<MockRepository, T> creator)
        {
            var mockery = new MockRepository();
            var mock = creator(mockery);
            mockery.Replay(mock);
            return mock;
        }

        public static Activities ActivityOf<T>(this T mock, Action<T> action,
            Action<IMethodOptions<object>> setupConstraints) 
        {
            try
            {
                mock.AssertWasCalled(action, setupConstraints);
                return new Activities(null);
            }
            catch (ExpectationViolationException e)
            {
                return new Activities(e);
            }
        }
        public static Activities ActivityOf<T>(this T mock, Action<T> action) {
            return ActivityOf(mock, action, DefaultConstraintSetup);
        }

        public static Activities ActivityOf<T>(this T mock, Function<T, object> func,
            Action<IMethodOptions<object>> setupConstraints) 
        {
            return ActivityOf(mock, new Action<T>(t => func(t)), setupConstraints);
        }

        public static Activities ActivityOf<T>(this T mock, Function<T, object> func) {
            return ActivityOf(mock, func, DefaultConstraintSetup);
        }

        public static void Assert(Activities activities) {
            activities.AssertOccured();
        }

        public static Activities ExactOneOf(params Activities[] activitiesList) {
            return Activities.ExactOneOf(activitiesList);
        }

        private static void DefaultConstraintSetup(IMethodOptions<object> options) {}
    }
}
