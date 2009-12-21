#region License

/*
 * Copyright (C) 2009-2010 the original author or authors.
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

using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace GenericInherited
{
    public interface IView<TP>
    {
        TP Presenter { get; set; }
    }

    public interface IPresenter<TV>
    {
        TV View { get; set; }
    }

    public interface IMainPresenter : IPresenter<IMainView>
    {
    }

    public interface IMainView : IView<IMainPresenter>
    {
        IFirstView FirstView { get; }
        ISecondView SecondView { get; }
    }

    public interface IFirstPresenter : IPresenter<IFirstView>
    {
    }

    public interface IFirstView : IView<IFirstPresenter>
    {
    }

    public interface ISecondPresenter : IPresenter<ISecondView>
    {
    }

    public interface ISecondView : IView<ISecondPresenter>
    {
    }

    public abstract class AbstractPresenter<TV, TP> : IPresenter<TV>
        where TV : class, IView<TP>
        where TP : class, IPresenter<TV>
    {
        public TV View { get; set; }
        public void Init()
        {
            if (View == null) throw new InvalidOperationException();
            View.Presenter = (TP)(object)this;
            InitPresenter();
        }

        protected virtual void InitPresenter()
        {
        }
    }

    public class FirstPresenter : AbstractPresenter<IFirstView, IFirstPresenter>, IFirstPresenter
    {
    }

    public class SecondPresenter : AbstractPresenter<ISecondView, ISecondPresenter>, ISecondPresenter
    {
    }

    public class MainPresenter : AbstractPresenter<IMainView, IMainPresenter>, IMainPresenter
    {
        public FirstPresenter FirstPresenter { private get; set; }
        public SecondPresenter SecondPresenter { private get; set; }

        protected override void InitPresenter()
        {
            if (FirstPresenter == null) throw new InvalidOperationException();
            if (SecondPresenter == null) throw new InvalidOperationException();
            FirstPresenter.View = View.FirstView;
            FirstPresenter.Init();
            SecondPresenter.View = View.SecondView;
            SecondPresenter.Init();
        }
    }

    public class PresenterViewTests
    {
        [Test]
        public void SunnyDay()
        {
            //Arrange
            FirstPresenter firstPresenter = new FirstPresenter();
            SecondPresenter secondPresenter = new SecondPresenter();
            var mockFirstView = MockRepository.GenerateStub<IFirstView>();
            var mockSecondView = MockRepository.GenerateStub<ISecondView>();
            var mockMainView = MockRepository.GenerateStub<IMainView>();
            mockMainView.Stub(m => m.FirstView).Return(mockFirstView);
            mockMainView.Stub(m => m.SecondView).Return(mockSecondView);

            //Act
            var sut = new MainPresenter
            {
                FirstPresenter = firstPresenter,
                SecondPresenter = secondPresenter,
                View = mockMainView,
            };
            sut.Init();

            //Assert
            Assert.That(firstPresenter.View, Is.SameAs(mockFirstView));
            Assert.That(secondPresenter.View, Is.SameAs(mockSecondView));
            Assert.That(sut.View, Is.SameAs(mockMainView));

            Assert.That(mockFirstView.Presenter, Is.SameAs(firstPresenter));
            Assert.That(mockSecondView.Presenter, Is.SameAs(secondPresenter));
            Assert.That(mockMainView.Presenter, Is.SameAs(sut));
        }
    }
}