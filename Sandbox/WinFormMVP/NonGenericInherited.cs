using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace NonGenericInherited
{
    public interface IView
    {
        IPresenter Presenter { get; set; }
    }

    public interface IPresenter
    {
        IView View { get; set; }
    }

    public interface IMainPresenter : IPresenter
    {
        IMainView MainView { get; set; }
    }

    public interface IMainView : IView
    {
        IMainPresenter MainPresenter { get; set; }
        IFirstView FirstView { get; }
        ISecondView SecondView { get; }
    }

    public interface IFirstPresenter : IPresenter
    {
        IFirstView FirstView { get; set; }
    }

    public interface IFirstView : IView
    {
        IFirstPresenter FirstPresenter { get; set; }
    }

    public interface ISecondPresenter : IPresenter
    {
        ISecondView SecondView { get; set; }
    }

    public interface ISecondView : IView
    {
        ISecondPresenter SecondPresenter { get; set; }
    }

    public abstract class AbstractPresenter : IPresenter
    {
        public IView View { get; set; }
        public void Init()
        {
            if (View == null) throw new InvalidOperationException();
            View.Presenter = this;
            InitPresenter();
        }

        protected abstract void InitPresenter();
    }

    public class FirstPresenter : AbstractPresenter, IFirstPresenter
    {
        public IFirstView FirstView { get; set; }
        protected override void InitPresenter()
        {
            FirstView = (IFirstView) View;
            FirstView.FirstPresenter = this;
        }
    }

    public class SecondPresenter : AbstractPresenter, ISecondPresenter
    {
        public ISecondView SecondView { get; set; }
        protected override void InitPresenter()
        {
            SecondView = (ISecondView) View;
            SecondView.SecondPresenter = this;
        }
    }

    public class MainPresenter : AbstractPresenter, IMainPresenter
    {
        public IMainView MainView { get; set; }
        public FirstPresenter FirstPresenter { private get; set; }
        public SecondPresenter SecondPresenter { private get; set; }

        protected override void InitPresenter()
        {
            MainView = (IMainView) View;
            MainView.MainPresenter = this;
            if (FirstPresenter == null) throw new InvalidOperationException();
            if (SecondPresenter == null) throw new InvalidOperationException();
            FirstPresenter.View = MainView.FirstView;
            FirstPresenter.Init();
            SecondPresenter.View = MainView.SecondView;
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
            Assert.That(firstPresenter.FirstView, Is.SameAs(mockFirstView));
            Assert.That(secondPresenter.View, Is.SameAs(mockSecondView));
            Assert.That(secondPresenter.SecondView, Is.SameAs(mockSecondView));
            Assert.That(sut.View, Is.SameAs(mockMainView));
            Assert.That(sut.MainView, Is.SameAs(mockMainView));

            Assert.That(mockFirstView.Presenter, Is.SameAs(firstPresenter));
            Assert.That(mockFirstView.FirstPresenter, Is.SameAs(firstPresenter));
            Assert.That(mockSecondView.Presenter, Is.SameAs(secondPresenter));
            Assert.That(mockSecondView.SecondPresenter, Is.SameAs(secondPresenter));
            Assert.That(mockMainView.Presenter, Is.SameAs(sut));
            Assert.That(mockMainView.MainPresenter, Is.SameAs(sut));
        }
    }
}
