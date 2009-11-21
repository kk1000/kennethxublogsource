using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace NotInherited
{
    public interface IMainPresenter
    {
    }

    public interface IMainView
    {
        IFirstView FirstView { get; }
        ISecondView SecondView { get; }
        IMainPresenter Presenter { set; }
    }

    public interface IFirstPresenter
    {
    }

    public interface IFirstView
    {
        IFirstPresenter Presenter { set; }
    }

    public interface ISecondPresenter
    {
    }

    public interface ISecondView
    {
        ISecondPresenter Presenter { set; }
    }

    public class FirstPresenter : IFirstPresenter
    {
        public void Init()
        {
            if (View == null) throw new InvalidOperationException();
            View.Presenter = this;
        }

        public IFirstView View { internal get; set; }
    }

    public class SecondPresenter : ISecondPresenter
    {
        public void Init()
        {
            if (View == null) throw new InvalidOperationException();
            View.Presenter = this;
        }

        public ISecondView View { internal get; set; }
    }

    public class MainPresenter : IMainPresenter
    {
        public FirstPresenter FirstPresenter { private get; set; }
        public SecondPresenter SecondPresenter { private get; set; }

        public void Init()
        {
            if (View == null) throw new InvalidOperationException();
            View.Presenter = this;
            if (FirstPresenter == null) throw new InvalidOperationException();
            if (SecondPresenter == null) throw new InvalidOperationException();
            FirstPresenter.View = View.FirstView;
            FirstPresenter.Init();
            SecondPresenter.View = View.SecondView;
            SecondPresenter.Init();
        }

        public IMainView View { internal get; set; }
    }

    public class PresenterViewTests
    {
        [Test]
        public void SunnyDay()
        {
            //Arrange
            var firstPresenter = new FirstPresenter();
            var secondPresenter = new SecondPresenter();
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

            mockFirstView.AssertWasCalled(x => x.Presenter = firstPresenter);
            mockSecondView.AssertWasCalled(x => x.Presenter = secondPresenter);
            mockMainView.AssertWasCalled(x => x.Presenter = sut);
        }
    }
}
